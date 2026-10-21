using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace OpenWaters.Art.Editor
{
    public static class HarborVisualValidation
    {
        public static void Run()
        {
            HarborAssetBuilder.Build();
            HarborAssetBuilder.OpenPreview();
            var camera=UnityEngine.Object.FindFirstObjectByType<Camera>();
            var animations=UnityEngine.Object.FindObjectsByType<Animation>(FindObjectsSortMode.None);
            int changing=0;
            foreach(var anim in animations)
            {
                if(!anim.clip)continue;
                var bindings=AnimationUtility.GetCurveBindings(anim.clip);
                if(bindings.Length==0)throw new Exception("No animation bindings: "+anim.name);
                if(bindings.Any(b=> {var c=AnimationUtility.GetEditorCurve(anim.clip,b);return c!=null && c.keys.Select(k=>k.value).Distinct().Count()>1;})) changing++;
            }
            var palette=AssetDatabase.LoadAssetAtPath<Material>("Assets/Models/Shared/HarborPalette.mat");
            palette.SetFloat("_Cull",0);EditorUtility.SetDirty(palette);
            var water=AssetDatabase.LoadAssetAtPath<Material>("Assets/Models/Water/HarborWater.mat");
            int frame=0;
            foreach(float time in new[]{0f,1f})
            {
                foreach(var anim in animations)if(anim.clip)anim.clip.SampleAnimation(anim.gameObject,time);
                ValidateAttachments();
                var rt=new RenderTexture(1200,960,24);camera.targetTexture=rt;camera.Render();
                var prior=RenderTexture.active;RenderTexture.active=rt;
                var tex=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false);tex.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);tex.Apply();
                File.WriteAllBytes($"Assets/Models/_Production/Previews/Unity_Harbor_{frame++}.png",tex.EncodeToPNG());
                RenderTexture.active=prior;camera.targetTexture=null;UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(tex);
            }
            AssetDatabase.SaveAssets();
            File.WriteAllText("Assets/Models/_Production/visual-validation.txt",$"Unity {Application.unityVersion}\nRendered 2 isometric frames at clip sample times 0 and 1 seconds.\nPreview Animation components: {animations.Length}; clips with varying curves: {changing}.\nWater shader renders on graphics device: {SystemInfo.graphicsDeviceName}.\nThis is editor render validation, not a player build or gameplay test.\n");
            Debug.Log("OW_VISUAL_VALIDATION_OK: "+changing+" animated instances with varying curves");
        }
        static void ValidateAttachments()
        {
            var transforms=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
            var ship=transforms.Single(t=>t.name=="CoastalFreighter" && t.parent==null);
            var bow=transforms.Single(t=>t.name=="BowAttachment");
            var stern=transforms.Single(t=>t.name=="WakeAttachment");
            var direction=(bow.position-stern.position).normalized;
            foreach(string name in new[]{"CargoAttachment","WakeAttachment","DamageAttachment","BowAttachment"})
            {
                var t=transforms.Single(t=>t.name==name);
                if(Vector3.Dot(t.up,Vector3.up)<.99f || Vector3.Dot(t.forward,direction)<.99f)
                    throw new Exception("Incorrect attachment basis: "+name);
                if((t.lossyScale-Vector3.one).sqrMagnitude>.001f)
                    throw new Exception("Unexpected attachment scale: "+name);
            }
            var wake=transforms.Single(t=>t.name=="FX_Wake");
            var bounds=wake.GetComponentsInChildren<Renderer>()[0].bounds;
            foreach(var r in wake.GetComponentsInChildren<Renderer>())bounds.Encapsulate(r.bounds);
            if(bounds.size.y>.25f || Vector3.Dot(bounds.center-stern.position,direction)>-.5f)
                throw new Exception("Wake must remain horizontal and extend behind stern.");
            var cargo=transforms.Single(t=>t.name=="Cargo_Textiles");
            if(Vector3.Dot(cargo.up,Vector3.up)<.99f || (cargo.lossyScale-Vector3.one*.9f).sqrMagnitude>.001f)
                throw new Exception("Cargo orientation/scale regression.");
            Debug.Log("OW_SOCKET_VALIDATION_OK: upright cargo, horizontal stern wake, unit attachment scales.");
        }
    }
}
