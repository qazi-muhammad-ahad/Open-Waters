using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace OpenWaters
{
    public sealed partial class DemoController : MonoBehaviour
    {
        public static bool UseValidationStorage;
        public DemoDefinitions definitions;
        public GameSession Session {get;private set;}
        public DemoSettings Settings {get;private set;}
        public HarborPresenter Harbor {get;private set;}
        public bool Paused {get;private set;}
        public bool BlockWorld=>menu || Paused || !string.IsNullOrEmpty(dialog) || Session.State.phase==RunPhase.EventPending || Session.State.phase==RunPhase.Delivery || Session.State.phase==RunPhase.Review || (Session.State.phase==RunPhase.Complete && !inspect);
        SaveService saves;
        RunState saved;
        Canvas canvas;
        RectTransform side,modal;
        Text hud,objective,travel;
        Button pauseControl;
        Font font;
        bool menu=true,inspect,details,dirty=true,focused,needsFocus,expanded;
        string dialog="",notice="";
        string routeId;
        float checkpoint;
        AudioSource sfx,ambience;
        AudioClip click,reward,warning,upgradeCue;
        RunPhase audioPhase;
        int audioTier=1;
        static readonly Color Ink=new Color(.88f,.94f,.95f),Navy=new Color(.035f,.09f,.13f,.97f),Gold=new Color(.95f,.7f,.27f);
        public void Awake()
        {
            if(PackagedSmokeRunner.Requested)UseValidationStorage=true;
            definitions.Validate();Settings=DemoSettings.Load();
            DisplaySettingsController.Apply(Settings);
            IRunStorage storage=UseValidationStorage?new FileRunStorage(Path.Combine(Application.temporaryCachePath,"OpenWaters-validation","run-v1.json")):Application.platform==RuntimePlatform.WebGLPlayer?(IRunStorage)new BrowserRunStorage():new FileRunStorage(Path.Combine(Application.persistentDataPath,"run-v1.json"));
            saves=new SaveService(storage,definitions);saved=saves.Load();notice=saves.Message;
            Bind(new GameSession(definitions));
            BuildShell();Harbor=new GameObject("Harbor presentation").AddComponent<HarborPresenter>();Harbor.Initialize(this);
            CreateAudio();Render();
            if(PackagedSmokeRunner.Requested)gameObject.AddComponent<PackagedSmokeRunner>();
        }
        void Bind(GameSession session)
        {
            if(Session!=null)Session.Changed-=OnChanged;
            Session=session;Session.Changed+=OnChanged;
        }
        void OnChanged()
        {
            saves.Save(Session.State);dirty=true;Harbor?.Refresh();
            if(sfx)
            {
                var s=Session.State;
                if(audioTier!=s.tier)sfx.PlayOneShot(upgradeCue,Settings.sfx);
                else if(audioPhase!=s.phase && s.phase==RunPhase.EventPending)sfx.PlayOneShot(warning,Settings.sfx);
                else if(audioPhase!=s.phase && (s.phase==RunPhase.Delivery || s.phase==RunPhase.Complete))sfx.PlayOneShot(reward,Settings.sfx);
                audioTier=s.tier;audioPhase=s.phase;
            }
        }
        public void ShowDetails(){details=true;dirty=true;}
        public void Escape()
        {
            if(menu){if(dialog!=""){dialog="";dirty=true;}return;}
            if(dialog!=""){dialog="";dirty=true;return;}
            if(details){details=false;dirty=true;return;}
            Paused=!Paused;dirty=true;
        }
        void Update()
        {
            bool blocked=menu || Paused || dialog!="" || !focused;
            if(!PackagedSmokeRunner.Requested)Session.Tick(Time.unscaledDeltaTime,blocked);
            if(!blocked){checkpoint+=Time.unscaledDeltaTime;if(checkpoint>=definitions.checkpointSeconds){checkpoint=0;saves.Save(Session.State);}}
            if(dirty)Render();
            var s=Session.State;
            hud.text=$"{s.credits} <size=14>credits</size>";
            objective.text=menu?"A small port. A bigger horizon.":Heading();
            standingLabel.text=$"Tier {s.tier}   •   {s.standing}";
            if(travel)
            {
                float eta=Mathf.Max(0,Session.Duration-(s.voyage?.elapsed??0));
                travel.text=Session.AtSea?$"{(s.phase==RunPhase.EventPending?"Decision required":$"At sea · {eta:0}s")}   •   Hull {s.condition}%":s.serviceRemaining>0?$"Repairs · {s.serviceRemaining:0}s":"Freighter docked";
            }
            if(ambience)ambience.volume=menu?0:Settings.ambience*.22f;
            AudioListener.volume=Settings.mute?0:Settings.master;
            AudioListener.pause=Paused || !focused || menu;
        }
        string Objective()
        {
            var s=Session.State;
            if(menu)return "A small port. One ship. Your first commercial expansion.";
            switch(s.phase)
            {
                case RunPhase.Port:return s.deliveries==0?"1 / 6 • Accept the grain contract to Beacon Bay.":!s.upgraded?"3 / 6 • Invest 300 credits in the commercial port upgrade.":"4 / 6 • Choose textiles or machine parts, then compare routes.";
                case RunPhase.Preparing:return "Prepare • Compare cost, expected incident and net earnings before dispatch.";
                case RunPhase.Traveling:return s.deliveries==0?"2 / 6 • Your first shipment is underway. Inspect the port while you wait.":"5 / 6 • Resolve the voyage incident and documentation check.";
                case RunPhase.EventPending:return "Decision required • Travel pauses while you read. Every consequence is shown.";
                case RunPhase.Delivery:return "Delivery complete • Earnings have already been credited. Review the ledger.";
                case RunPhase.Review:return "6 / 6 • Scheduled GSA review. Your recorded decisions determine the outcome.";
                default:return "Demo complete • Inspect your upgraded port or replay a different route.";
            }
        }
        void Command(Func<CommandReceipt> command)
        {
            var receipt=command();notice=receipt.message;commandFailed=!receipt.success;dirty=true;
            if(receipt.success && sfx)sfx.PlayOneShot(click,Settings.sfx);
        }
        void StartRun(bool continuing)
        {
            Bind(new GameSession(definitions,continuing?saved:null));menu=false;Paused=false;inspect=false;details=false;expanded=false;dialog="";routeId=null;checkpoint=0;notice=continuing?saves.Message:"";
            Harbor.ResetSelection();Harbor.Refresh();saves.Save(Session.State);saved=Session.State;dirty=true;focused=true;commandFailed=false;
            if(ambience && !ambience.isPlaying)ambience.Play();
#if UNITY_WEBGL && !UNITY_EDITOR
            StartCoroutine(NotifyBrowserHarbor());
#endif
        }
        void ReturnMenu(){saves.Save(Session.State);saved=Session.State;menu=true;Paused=false;dialog="";dirty=true;}
        void CreateAudio()
        {
            sfx=gameObject.AddComponent<AudioSource>();ambience=gameObject.AddComponent<AudioSource>();
            const int rate=22050;var samples=new float[2205];for(int i=0;i<samples.Length;i++)samples[i]=Mathf.Sin(i*2*Mathf.PI*660/rate)*.15f*(1-i/(float)samples.Length);
            click=AudioClip.Create("Harbor bell",samples.Length,1,rate,false);click.SetData(samples,0);
            reward=ToneSequence("Delivery chime",new[]{523f,659f,784f},rate);
            warning=ToneSequence("Voyage alert",new[]{440f,330f},rate);
            upgradeCue=ToneSequence("Port expansion",new[]{392f,523f,659f,784f},rate);
            var wave=new float[rate*4];var random=new System.Random(2701);float low=0;
            for(int i=0;i<wave.Length;i++){low=Mathf.Lerp(low,(float)random.NextDouble()*2-1,.025f);wave[i]=low*(.4f+.15f*Mathf.Sin(i*2*Mathf.PI/(rate*4)));}
            var clip=AudioClip.Create("Harbor wash",wave.Length,1,rate,false);clip.SetData(wave,0);ambience.clip=clip;ambience.loop=true;ambience.Play();
        }
        static AudioClip ToneSequence(string name,float[] notes,int rate)
        {
            int length=rate/6;var samples=new float[length*notes.Length];
            for(int n=0;n<notes.Length;n++)for(int i=0;i<length;i++)samples[n*length+i]=Mathf.Sin(i*2*Mathf.PI*notes[n]/rate)*.17f*Mathf.Sin(i*Mathf.PI/length);
            var clip=AudioClip.Create(name,samples.Length,1,rate,false);clip.SetData(samples,0);return clip;
        }
        void OnApplicationFocus(bool hasFocus){focused=hasFocus;if(!hasFocus && !menu){Paused=true;dirty=true;saves?.Save(Session.State);}}
        void OnApplicationPause(bool paused){if(paused && !menu){Paused=true;dirty=true;saves?.Save(Session.State);}}
        void OnApplicationQuit(){if(!menu)saves?.Save(Session.State);Settings?.Save();}
        void OnDestroy(){if(Session!=null)Session.Changed-=OnChanged;}
        // Integration fixtures drive the same commands and clock as the UI.
        public void BeginTestRun(){StartRun(false);}
        // Website lifecycle commands never start or replace a run.
        [UnityEngine.Scripting.Preserve]
        public void BrowserPause()
        {
            if(!menu){Paused=true;dirty=true;saves?.Save(Session.State);}
            Settings?.Save();
        }
        [UnityEngine.Scripting.Preserve]
        public void BrowserPrepareClose()
        {
            BrowserPause();
#if UNITY_WEBGL && !UNITY_EDITOR
            BrowserDemoFlush();
#endif
        }
        System.Collections.IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
#if UNITY_WEBGL && !UNITY_EDITOR
            BrowserDemoReady();
#endif
        }
#if UNITY_WEBGL && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        static extern void BrowserDemoReady();
        [System.Runtime.InteropServices.DllImport("__Internal")]
        static extern void BrowserDemoFlush();
        [System.Runtime.InteropServices.DllImport("__Internal")]
        static extern void BrowserDemoHarbor();
        System.Collections.IEnumerator NotifyBrowserHarbor()
        {
            yield return new WaitForEndOfFrame();
            BrowserDemoHarbor();
        }
#endif
    }
}
