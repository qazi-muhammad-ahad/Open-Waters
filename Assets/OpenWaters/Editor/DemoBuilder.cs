using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace OpenWaters.Editor
{
    public static class DemoBuilder
    {
        public const string DataPath="Assets/OpenWaters/Data/Demo.asset";
        [MenuItem("Open Waters/Gameplay/Build gameplay scene")]
        public static void Build()
        {
            BrandingBuilder.Apply();
            Directory.CreateDirectory("Assets/OpenWaters/Data");
            var data=AssetDatabase.LoadAssetAtPath<DemoDefinitions>(DataPath);
            if(!data){data=ScriptableObject.CreateInstance<DemoDefinitions>();AssetDatabase.CreateAsset(data,DataPath);}
            data.AuthorDefaults();
            string[] groups={"Dock","Warehouse","Fuel","Office","Repair"};
            foreach(var tier in data.tiers)tier.buildings=groups.Select(g=>Prefab(g,g+"_T"+tier.tier)).ToArray();
            string[] cargoNames={"Cargo_Grain","Cargo_Textiles","Cargo_MachineParts"};
            for(int i=0;i<3;i++)
            {
                data.cargos[i].prefab=Prefab(cargoNames[i],cargoNames[i]);
                var icons=AssetDatabase.FindAssets(cargoNames[i]+" t:Sprite");
                if(icons.Length>0)data.cargos[i].icon=AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(icons[0]));
            }
            data.ship=Prefab("Freighter","CoastalFreighter");data.coast=Prefab("Coast","CoastTile");data.ocean=Prefab("Water","OceanSurface");data.foam=Prefab("Water","ShoreFoam");
            data.wake=Prefab("Effects","FX_Wake");data.damage=Prefab("Effects","FX_DamageSmoke");data.selection=Prefab("Effects","FX_Selection");data.upgrade=Prefab("Effects","FX_Upgrade");
            data.loading=Prefab("Effects","FX_Loading");data.arrival=Prefab("Effects","FX_Arrival");
            data.Validate();EditorUtility.SetDirty(data);AssetDatabase.SaveAssets();
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/Official_Game.unity");
            var existing=UnityEngine.Object.FindFirstObjectByType<DemoController>();if(existing)UnityEngine.Object.DestroyImmediate(existing.gameObject);
            var root=new GameObject("Open Waters Game");root.AddComponent<DemoController>().definitions=data;
            if(!Camera.main)throw new InvalidOperationException("Official_Game requires its existing MainCamera.");
            Camera.main.orthographic=true;Camera.main.orthographicSize=15;Camera.main.transform.rotation=Quaternion.Euler(55,45,0);Camera.main.transform.position=-Camera.main.transform.forward*45;
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.Refresh();
            Debug.Log("OW_GAMEPLAY_SCENE_BUILT");
        }
        static GameObject Prefab(string group,string name)
        {
            string path="Assets/Models/"+group+"/"+name+".prefab";
            var p=AssetDatabase.LoadAssetAtPath<GameObject>(path);if(!p)throw new InvalidOperationException("Missing gameplay asset: "+path);return p;
        }
        [MenuItem("Open Waters/Gameplay/Build Windows candidate")]
        public static void BuildWindows()=>BuildPlayer(BuildTarget.StandaloneWindows64,"Builds/Windows/OpenWaters.exe");
        [MenuItem("Open Waters/Gameplay/Build WebGL candidate")]
        public static void BuildWeb()=>BuildPlayer(BuildTarget.WebGL,"Builds/WebGL");
        [MenuItem("Open Waters/Gameplay/Build production Web demo")]
        public static void BuildWebRelease()
        {
            PlayerSettings.WebGL.compressionFormat=WebGLCompressionFormat.Brotli;
            PlayerSettings.WebGL.decompressionFallback=false;
            PlayerSettings.WebGL.dataCaching=true;
            PlayerSettings.WebGL.threadsSupport=false;
            UnityEditor.WebGL.UserBuildSettings.codeOptimization=UnityEditor.WebGL.WasmCodeOptimization.DiskSize;
            BuildPlayer(BuildTarget.WebGL,"Builds/WebRelease",BuildOptions.None);
            File.WriteAllText("Builds/WebRelease/release-info.json","{\"unityVersion\":\""+Application.unityVersion+"\",\"development\":false,\"compression\":\"br\",\"threads\":false,\"optimization\":\"DiskSize\"}");
        }
        static void BuildPlayer(BuildTarget target,string path,BuildOptions options=BuildOptions.Development)
        {
            Directory.CreateDirectory("Logs");
            BrandingBuilder.Apply();
            PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=720;
            PlayerSettings.defaultIsNativeResolution=true;
            PlayerSettings.defaultWebScreenWidth=1280;PlayerSettings.defaultWebScreenHeight=720;
            PlayerSettings.resizableWindow=true;PlayerSettings.fullScreenMode=FullScreenMode.ExclusiveFullScreen;
            PlayerSettings.enableFrameTimingStats=true;
            if(target==BuildTarget.StandaloneWindows64)
            {
                // Prefer the broadly compatible backend; retain D3D12 as fallback.
                PlayerSettings.SetUseDefaultGraphicsAPIs(target,false);
                PlayerSettings.SetGraphicsAPIs(target,new[]{UnityEngine.Rendering.GraphicsDeviceType.Direct3D11,UnityEngine.Rendering.GraphicsDeviceType.Direct3D12});
            }
            if(target==BuildTarget.WebGL)PlayerSettings.WebGL.template="PROJECT:OpenWaters";
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/Official_Game.unity"},target=target,locationPathName=path,options=options});
            File.WriteAllText("Logs/build-"+target+".txt",$"Result: {report.summary.result}\nSize: {report.summary.totalSize}\nTime: {report.summary.totalTime}\nErrors: {report.summary.totalErrors}\nWarnings: {report.summary.totalWarnings}");
            if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed: "+report.summary.result);
            if(target==BuildTarget.WebGL)File.Copy(BrandingBuilder.LogoPath,Path.Combine(path,"Logo.png"),true);
        }
    }
}

