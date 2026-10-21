// Import only this authored kit. No gameplay scene or project setting is modified.
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace OpenWaters.Art.Editor
{
    public sealed class HarborModelImporter : AssetPostprocessor
    {
        const string Root = "Assets/Models/";
        void OnPreprocessModel()
        {
            if (!assetPath.StartsWith(Root, StringComparison.Ordinal) || !assetPath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase)) return;
            var importer = (ModelImporter)assetImporter;
            importer.importCameras = false;
            importer.importLights = false;
            importer.importVisibility = false;
            importer.importBlendShapes = true;
            importer.animationType = ModelImporterAnimationType.Legacy;
            importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            importer.isReadable = true;
            importer.importNormals = ModelImporterNormals.Import;
        }
        Material OnAssignMaterialModel(Material material, Renderer renderer)
        {
            if (!assetPath.StartsWith(Root, StringComparison.Ordinal)) return null;
            string path = assetPath.EndsWith("/OceanSurface.fbx", StringComparison.Ordinal) ? "Water/HarborWater.mat" : "Shared/HarborPalette.mat";
            return AssetDatabase.LoadAssetAtPath<Material>(Root + path);
        }
    }

    public static class HarborAssetBuilder
    {
        const string Root = "Assets/Models/";
        [Serializable] public sealed class Entry { public string name; public string group; public string fbx; public int triangles; public bool animatedFBX; }
        [Serializable] public sealed class Manifest { public Entry[] assets; }
        static Manifest LoadManifest() => JsonUtility.FromJson<Manifest>(File.ReadAllText(Root + "asset-manifest.json"));

        [MenuItem("Open Waters/Art/Rebuild modular prefabs and preview")]
        public static void Build()
        {
            var manifest = LoadManifest();
            Directory.CreateDirectory(Root + "Preview");
            Directory.CreateDirectory(Root + "Freighter/Animations");
            AssetDatabase.Refresh();
            ConfigureTexture(Root + "Shared/HarborPalette.png", true, FilterMode.Point);
            ConfigureTexture(Root + "Water/OceanPattern.png", false, FilterMode.Bilinear);
            foreach (string path in Directory.GetFiles(Root, "*.png", SearchOption.AllDirectories).Select(p => p.Replace('\\','/')))
            {
                if (!path.Contains("/Icons/") && !Path.GetFileName(path).StartsWith("CargoIcon_")) continue;
                var ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti == null) continue;
                ti.textureType=TextureImporterType.Sprite; ti.alphaIsTransparency=true; ti.mipmapEnabled=false; ti.maxTextureSize=512; ti.SaveAndReimport();
            }
            var lit=Shader.Find("Universal Render Pipeline/Lit");
            var waterShader=Shader.Find("OpenWaters/Harbor Water");
            if (!lit || !waterShader) throw new InvalidOperationException("Required URP/harbor shader unavailable.");
            var palette=MaterialAt(Root+"Shared/HarborPalette.mat",lit);
            palette.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"Shared/HarborPalette.png"));
            palette.SetColor("_BaseColor",Color.white);palette.SetFloat("_Smoothness",.25f);palette.SetFloat("_Cull",0);EditorUtility.SetDirty(palette);
            var water=MaterialAt(Root+"Water/HarborWater.mat",waterShader);
            water.SetTexture("_Pattern",AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"Water/OceanPattern.png"));EditorUtility.SetDirty(water);
            int animated=0;
            foreach (var entry in manifest.assets)
            {
                string path=Root+entry.fbx;
                var importer=(ModelImporter)AssetImporter.GetAtPath(path);
                importer.importAnimation=entry.animatedFBX;
                if (entry.animatedFBX)
                {
                    var defaults=importer.defaultClipAnimations;
                    if (defaults.Length>0)
                    {
                        var clip=defaults[0]; clip.name=entry.name+"_"+ClipName(entry.name);
                        clip.firstFrame=0;clip.lastFrame=120;
                        clip.loopTime=entry.name!="FX_Upgrade";clip.loopPose=clip.loopTime;
                        clip.wrapMode=clip.loopTime?WrapMode.Loop:WrapMode.Once;
                        importer.clipAnimations=new[]{clip};
                    }
                }
                importer.SaveAndReimport();
                var model=AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (!model) throw new InvalidOperationException("Missing model "+path);
                var outer=new GameObject(entry.name);
                var visual=(GameObject)PrefabUtility.InstantiatePrefab(model);
                visual.name="Visuals";visual.transform.SetParent(outer.transform,false);
                if(entry.name=="CoastalFreighter")
                {
                    // FBX empties retain Blender's Z-up basis. Do not modify those
                    // animated transforms: add stable Unity Y-up attachment children.
                    foreach(string socketName in new[]{"Cargo","Wake","Damage","Bow"})
                    {
                        var socket=visual.GetComponentsInChildren<Transform>().Single(t=>t.name==socketName+"Socket");
                        var attachment=new GameObject(socketName+"Attachment").transform;
                        attachment.SetParent(socket,false);
                        attachment.localRotation=Quaternion.Euler(90,0,0);
                    }
                }
                // Keep imported object animation and cargo/wake sockets intact beneath Visuals.
                foreach(var renderer in outer.GetComponentsInChildren<Renderer>())
                {
                    renderer.sharedMaterials=Enumerable.Repeat(entry.name=="OceanSurface"?water:palette,renderer.sharedMaterials.Length).ToArray();
                    if(entry.group=="Water" || entry.group=="Effects") renderer.shadowCastingMode=ShadowCastingMode.Off;
                }
                if(entry.animatedFBX)
                {
                    var clips=AssetDatabase.LoadAllAssetsAtPath(path).OfType<AnimationClip>().Where(c=>!c.name.StartsWith("__preview__")).ToArray();
                    if(clips.Length==0) throw new InvalidOperationException("Missing animation "+entry.name);
                    var anim=visual.GetComponent<Animation>() ?? visual.AddComponent<Animation>();
                    foreach(var clip in clips) anim.AddClip(clip,clip.name);
                    anim.clip=clips[0];anim.playAutomatically=entry.name!="FX_Upgrade";
                    anim.wrapMode=entry.name=="FX_Upgrade"?WrapMode.Once:WrapMode.Loop;
                    animated++;
                }
                if(new[]{"Dock","Warehouse","Fuel","Office","Repair","Freighter"}.Contains(entry.group))
                {
                    var renders=outer.GetComponentsInChildren<Renderer>();
                    var bounds=renders[0].bounds;
                    foreach(var r in renders.Skip(1)) bounds.Encapsulate(r.bounds);
                    var collider=outer.AddComponent<BoxCollider>();collider.center=bounds.center;collider.size=bounds.size;
                }
                PrefabUtility.SaveAsPrefabAsset(outer,Root+entry.group+"/"+entry.name+".prefab");
                UnityEngine.Object.DestroyImmediate(outer);
            }
            CreateTravelClip("Departure",0,10);
            CreateTravelClip("Arrival",10,0);
            AssetDatabase.SaveAssets();
            BuildPreview();
            Validate();
            Debug.Log($"OW_ART_COMPLETE: {manifest.assets.Length} modular prefabs, {animated} imported animations, 2 travel clips.");
        }
        static string ClipName(string name)
        {
            if(name=="CoastalFreighter")return "Idle";
            if(name=="Dock_T2")return "Hoist";
            if(name=="Repair_T2")return "Service";
            if(name.StartsWith("Office"))return "FlagBreeze";
            return name.StartsWith("FX_")?name.Substring(3):"Attention";
        }
        static void ConfigureTexture(string path,bool srgb,FilterMode filter)
        {
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            importer.sRGBTexture=srgb;importer.filterMode=filter;importer.wrapMode=TextureWrapMode.Repeat;
            importer.textureCompression=TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled=filter!=FilterMode.Point;importer.SaveAndReimport();
        }
        static Material MaterialAt(string path,Shader shader)
        {
            var material=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(!material){material=new Material(shader);AssetDatabase.CreateAsset(material,path);}
            else material.shader=shader;
            return material;
        }
        static void CreateTravelClip(string name,float from,float to)
        {
            string path=Root+"Freighter/Animations/"+name+".anim";
            var clip=AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if(!clip){clip=new AnimationClip();AssetDatabase.CreateAsset(clip,path);}
            clip.legacy=true;clip.frameRate=30;clip.wrapMode=WrapMode.Once;
            clip.SetCurve("",typeof(Transform),"localPosition.z",AnimationCurve.EaseInOut(0,from,4,to));
            EditorUtility.SetDirty(clip);
        }
        static GameObject Place(string group,string name,Vector3 position,Vector3? scale=null,float yaw=0)
        {
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Root+group+"/"+name+".prefab");
            var obj=(GameObject)PrefabUtility.InstantiatePrefab(prefab);
            obj.transform.position=position;obj.transform.rotation=Quaternion.Euler(0,yaw,0);
            if(scale.HasValue)obj.transform.localScale=scale.Value;
            return obj;
        }
        [MenuItem("Open Waters/Art/Open modular harbor preview")]
        public static void OpenPreview() => EditorSceneManager.OpenScene(Root+"Preview/Harbor_ArtPreview.unity");
        static void BuildPreview()
        {
            // Dedicated art scene. Official_Game and build settings are deliberately untouched.
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var water=Place("Water","OceanSurface",new Vector3(0,-.12f,0),new Vector3(1.7f,1,1.7f));
            Place("Coast","CoastTile",new Vector3(0,-.05f,3),new Vector3(2.4f,1,1.9f));
            Place("Water","ShoreFoam",new Vector3(0,-.08f,3),new Vector3(2.4f,1,1.9f));
            Place("Dock","Dock_T2",new Vector3(0,.12f,-5.3f),null,180);
            Place("Warehouse","Warehouse_T2",new Vector3(-3,.3f,2.4f),null,180);
            Place("Office","Office_T2",new Vector3(3.5f,.3f,4.6f),null,180);
            Place("Fuel","Fuel_T2",new Vector3(-5.5f,.3f,-1.3f),null,180);
            Place("Repair","Repair_T2",new Vector3(3.6f,.3f,-.5f),null,180);
            var ship=Place("Freighter","CoastalFreighter",new Vector3(0,.1f,-9),null,90);
            var cargo=Place("Cargo_Textiles","Cargo_Textiles",Vector3.zero,Vector3.one*.9f);
            cargo.transform.SetParent(ship.GetComponentsInChildren<Transform>().First(t=>t.name=="CargoAttachment"),false);
            var wake=Place("Effects","FX_Wake",Vector3.zero);
            wake.transform.SetParent(ship.GetComponentsInChildren<Transform>().First(t=>t.name=="WakeAttachment"),false);
            foreach(var p in new[]{new Vector3(-8,.3f,4),new Vector3(-7,.3f,6),new Vector3(0,.3f,8),new Vector3(5,.3f,7),new Vector3(8,.3f,3)})Place("Vegetation","CoastalPine",p);
            foreach(var p in new[]{new Vector3(-8,.1f,-.4f),new Vector3(7,.1f,-1.8f),new Vector3(-4,.1f,-4)})Place("Coast","RockCluster",p);
            foreach(var p in new[]{new Vector3(-2,.3f,-2.7f),new Vector3(2,.3f,-2.7f)})Place("Props","HarborLamp",p);
            var camera=new GameObject("Isometric Art Camera").AddComponent<Camera>();camera.orthographic=true;camera.orthographicSize=14;
            camera.transform.position=new Vector3(22,33,-29);camera.transform.LookAt(Vector3.zero);camera.backgroundColor=new Color(.07f,.27f,.35f);camera.clearFlags=CameraClearFlags.SolidColor;camera.nearClipPlane=.1f;camera.farClipPlane=150;
            var sun=new GameObject("Sun").AddComponent<Light>();sun.type=LightType.Directional;sun.intensity=1.5f;sun.transform.rotation=Quaternion.Euler(45,-35,0);sun.shadows=LightShadows.Soft;
            RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.52f,.62f,.7f);
            EditorSceneManager.SaveScene(scene,Root+"Preview/Harbor_ArtPreview.unity");
        }
        public static void Validate()
        {
            var manifest=LoadManifest();int triangles=0,clips=0;
            foreach(var entry in manifest.assets)
            {
                var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Root+entry.group+"/"+entry.name+".prefab");
                if(!prefab)throw new Exception("Missing prefab "+entry.name);
                foreach(var r in prefab.GetComponentsInChildren<Renderer>(true))
                    foreach(var m in r.sharedMaterials)if(!m || !m.shader || !m.shader.isSupported)throw new Exception("Unsupported material: "+entry.name);
                foreach(var mf in prefab.GetComponentsInChildren<MeshFilter>(true))
                {
                    if(!mf.sharedMesh || mf.sharedMesh.vertexCount==0)throw new Exception("Empty mesh: "+entry.name);
                    triangles+=mf.sharedMesh.triangles.Length/3;
                }
                if(entry.animatedFBX)
                {
                    var anim=prefab.GetComponentInChildren<Animation>(true);
                    if(!anim || !anim.clip || anim.clip.length<3.9f)throw new Exception("Invalid animation: "+entry.name);
                    clips++;
                }
            }
            var shader=Shader.Find("OpenWaters/Harbor Water");
            var errors=ShaderUtil.GetShaderMessages(shader).Where(m=>m.severity.ToString()=="Error").ToArray();
            if(errors.Length>0)throw new Exception(string.Join("\n",errors.Select(e=>e.message)));
            File.WriteAllText(Root+"_Production/unity-validation.json",JsonUtility.ToJson(new Validation{prefabs=manifest.assets.Length,animatedPrefabs=clips,meshFilterTriangles=triangles,unityVersion=Application.unityVersion,waterShaderErrors=errors.Length},true));
            Debug.Log("OW_ART_VALIDATION_OK");
        }
        [Serializable] sealed class Validation {public int prefabs,animatedPrefabs,meshFilterTriangles,waterShaderErrors;public string unityVersion;}
    }
}

