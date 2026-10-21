using System;
using System.IO;
using System.Linq;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace OpenWaters
{
    // Explicit development CLI fixture. Never runs during an ordinary player session.
    public sealed class PackagedSmokeRunner : MonoBehaviour
    {
        DemoController owner;
        bool fast,finished;
        int frames,tail,errors,timingSamples;
        double cpu,gpu;
        long peakMemory,peakDraws;
        string routeId,inspection,reportPath;
        readonly FrameTiming[] timings=new FrameTiming[1];
        ProfilerRecorder draws;
        float started;
        bool checkDisplay,borderlessVerified,fullscreenVerified;
        string startupDisplay;
        DisplayMode originalMode;
        int displayStage;
        float displayDeadline,nextDisplayStep;
#if UNITY_STANDALONE || UNITY_EDITOR
        public static bool Requested {get;}=Environment.GetCommandLineArgs().Contains("--ow-smoke");
#else
        public static bool Requested=>false;
#endif
        string Arg(string key,string fallback)=>Environment.GetCommandLineArgs().FirstOrDefault(x=>x.StartsWith(key+"=",StringComparison.Ordinal))?.Substring(key.Length+1)??fallback;
        void Start()
        {
            owner=GetComponent<DemoController>();fast=Environment.GetCommandLineArgs().Contains("--ow-smoke-fast");
            checkDisplay=Environment.GetCommandLineArgs().Contains("--ow-settings-check");originalMode=owner.Settings.displayMode;
            routeId=Arg("--ow-route","copper_sheltered");inspection=Arg("--ow-inspection","clean");reportPath=Arg("--ow-report",Path.Combine(Application.temporaryCachePath,"ow-smoke.json"));
            Application.runInBackground=true;Application.logMessageReceived+=Log;
            draws=ProfilerRecorder.StartNew(ProfilerCategory.Render,"Draw Calls Count");
            started=Time.realtimeSinceStartup;owner.BeginTestRun();
        }
        void Log(string message,string stack,LogType type){if(type==LogType.Error || type==LogType.Exception || type==LogType.Assert)errors++;}
        void Update()
        {
            if(finished)return;
            frames++;peakMemory=Math.Max(peakMemory,Profiler.GetTotalAllocatedMemoryLong());
            if(draws.Valid)peakDraws=Math.Max(peakDraws,draws.LastValue);
            FrameTimingManager.CaptureFrameTimings();
            if(FrameTimingManager.GetLatestTimings(1,timings)>0 && timings[0].cpuFrameTime>0){cpu+=timings[0].cpuFrameTime;gpu+=timings[0].gpuFrameTime;timingSamples++;}
            try
            {
                var game=owner.Session;var s=game.State;
                switch(s.phase)
                {
                    case RunPhase.Port:
                        if(s.deliveries==0)Check(game.Accept("grain_beacon"));
                        else if(!s.upgraded)Check(game.Upgrade());
                        else Check(game.Accept(owner.definitions.Route(routeId).contractId));break;
                    case RunPhase.Preparing:Check(game.Dispatch(s.deliveries==0?"beacon_coastal":routeId));break;
                    case RunPhase.Traveling:game.Tick(fast?2:Time.unscaledDeltaTime);break;
                    case RunPhase.EventPending:
                        var e=owner.definitions.Event(s.voyage.pendingEventId);Check(game.Resolve(e.id,e.id=="inspection"?inspection:e.choices[0].id));break;
                    case RunPhase.Delivery:Check(game.AcknowledgeDelivery());break;
                    case RunPhase.Review:Check(game.Review());break;
                    case RunPhase.Complete:
                        tail++;
                        if(checkDisplay)CheckDisplay();
                        else{if(tail==3){startupDisplay=Screen.fullScreenMode.ToString();Capture();}if(tail>30)Finish(true,"");}break;
                }
                if(Time.realtimeSinceStartup-started>240)Finish(false,"Smoke timeout.");
            }
            catch(Exception e){Finish(false,e.Message);}
        }
        static void Check(CommandReceipt result){if(!result.success)throw new InvalidOperationException(result.message);}
        void CheckDisplay()
        {
            float now=Time.realtimeSinceStartup;
            if(displayDeadline==0)displayDeadline=now+2;
            if(now<nextDisplayStep)return;
            if(displayStage==0)
            {
                if(Screen.fullScreenMode!=DisplaySettingsController.ScreenMode(originalMode)&&now<displayDeadline)return;
                startupDisplay=Screen.fullScreenMode.ToString();Capture();owner.Escape();displayStage=1;nextDisplayStep=now+.2f;
            }
            else if(displayStage==1){Click("Settings");displayStage=2;nextDisplayStep=now+.2f;}
            else if(displayStage==2)
            {
                Capture(".settings");Click(owner.Settings.displayMode==DisplayMode.Borderless?"✓ Windowed fullscreen":"Windowed fullscreen");displayStage=3;displayDeadline=now+2;nextDisplayStep=now+.2f;
            }
            else if(displayStage==3)
            {
                if(Screen.fullScreenMode!=FullScreenMode.FullScreenWindow&&now<displayDeadline)return;
                borderlessVerified=Screen.fullScreenMode==FullScreenMode.FullScreenWindow;Capture(".borderless");Click(owner.Settings.displayMode==DisplayMode.Fullscreen?"✓ Fullscreen":"Fullscreen");displayStage=4;displayDeadline=now+2;nextDisplayStep=now+.2f;
            }
            else if(displayStage==4)
            {
                if(Screen.fullScreenMode!=FullScreenMode.ExclusiveFullScreen&&now<displayDeadline)return;
                fullscreenVerified=Screen.fullScreenMode==FullScreenMode.ExclusiveFullScreen;Capture(".fullscreen");displayStage=5;nextDisplayStep=now+.2f;
            }
            else Finish(borderlessVerified&&fullscreenVerified,borderlessVerified&&fullscreenVerified?"":"Display mode transition check failed.");
        }
        static void Click(string name){UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None).Single(x=>x.name==name&&x.IsInteractable()).onClick.Invoke();}
        void Capture(string suffix="")
        {
            Directory.CreateDirectory(Path.GetDirectoryName(reportPath));
            var camera=owner.Harbor.View;var canvas=UnityEngine.Object.FindFirstObjectByType<Canvas>();
            canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;
            var target=new RenderTexture(1280,720,24);camera.targetTexture=target;Canvas.ForceUpdateCanvases();camera.Render();
            RenderTexture.active=target;var image=new Texture2D(1280,720,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1280,720),0,0);image.Apply();
            File.WriteAllBytes(reportPath+suffix+".png",image.EncodeToPNG());RenderTexture.active=null;camera.targetTexture=null;canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            Destroy(image);target.Release();Destroy(target);
        }
        void Finish(bool complete,string failure)
        {
            if(finished)return;finished=true;var s=owner.Session.State;
            var report=new Report{complete=complete && errors==0,error=failure,loggedErrors=errors,fastClock=fast,route=routeId,inspectionChoice=inspection,credits=s.credits,reputation=s.reputation,suspicion=s.suspicion,standing=s.standing,frames=frames,seconds=Time.realtimeSinceStartup-started,peakAllocatedBytes=peakMemory,peakDrawCalls=peakDraws,cpuFrameMs=timingSamples>0?cpu/timingSamples:0,gpuFrameMs=timingSamples>0?gpu/timingSamples:0,timingSamples=timingSamples,unity=Application.unityVersion,cpu=SystemInfo.processorType,gpu=SystemInfo.graphicsDeviceName,width=Screen.width,height=Screen.height};
            report.displayMode=Screen.fullScreenMode.ToString();report.nativeWidth=Screen.currentResolution.width;report.nativeHeight=Screen.currentResolution.height;report.tipsEnabled=owner.Settings.tutorial;report.logoLoaded=Resources.Load<Sprite>("Branding/Logo")!=null;
            report.startupDisplayMode=startupDisplay;report.borderlessVerified=borderlessVerified;report.fullscreenVerified=fullscreenVerified;
            if(checkDisplay){owner.Settings.displayMode=originalMode;owner.Settings.Save();}
            Directory.CreateDirectory(Path.GetDirectoryName(reportPath));File.WriteAllText(reportPath,JsonUtility.ToJson(report,true));Debug.Log("OW_PACKAGED_SMOKE "+report.complete);Application.Quit(report.complete?0:1);
        }
        void OnDestroy(){draws.Dispose();Application.logMessageReceived-=Log;}
        [Serializable] sealed class Report
        {
            public bool complete,fastClock,tipsEnabled,logoLoaded,borderlessVerified,fullscreenVerified;
            public string error,route,inspectionChoice,standing,unity,cpu,gpu,displayMode,startupDisplayMode;
            public int loggedErrors,credits,reputation,suspicion,frames,timingSamples,width,height;
            public int nativeWidth,nativeHeight;
            public float seconds;
            public long peakAllocatedBytes,peakDrawCalls;
            public double cpuFrameMs,gpuFrameMs;
        }
    }
}
