using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace OpenWaters.Editor
{
    public sealed class BrandingImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if(assetPath!=BrandingBuilder.LogoPath&&!assetPath.StartsWith(BrandingBuilder.IconRoot,StringComparison.Ordinal))return;
            var importer=(TextureImporter)assetImporter;importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.maxTextureSize=1024;importer.textureCompression=TextureImporterCompression.Uncompressed;
        }
        static void OnPostprocessAllAssets(string[] imported,string[] deleted,string[] moved,string[] from)
        {
            if(Array.IndexOf(imported,BrandingBuilder.LogoPath)>=0)EditorApplication.delayCall+=BrandingBuilder.ApplyLogo;
        }
    }
    public static class BrandingBuilder
    {
        public const string LogoPath="Assets/OpenWaters/Resources/Branding/Logo.png";
        public const string IconRoot="Assets/OpenWaters/Resources/UI/Icons/";
        static readonly Color Gold=new Color(.96f,.72f,.28f),White=new Color(.89f,.96f,.96f),Navy=new Color(.035f,.12f,.17f);
        [MenuItem("Open Waters/Branding/Apply logo and UI icons")]
        public static void Apply()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogoPath));Directory.CreateDirectory(IconRoot);
            if(!File.Exists(LogoPath))
            {
                var c=new Canvas(512);c.Disc(256,256,246,Navy);c.Circle(256,256,224,9,Gold);
                c.Polygon(new[]{new Vector2(112,210),new Vector2(400,210),new Vector2(352,150),new Vector2(164,150)},Gold);
                c.Line(256,220,256,390,14,White);c.Polygon(new[]{new Vector2(268,378),new Vector2(268,238),new Vector2(382,238)},White);c.Polygon(new[]{new Vector2(241,350),new Vector2(156,238),new Vector2(241,238)},Gold);
                for(int i=0;i<3;i++){float x=130+i*84;c.Line(x,116,x+34,128,8,White);c.Line(x+34,128,x+68,116,8,White);}c.Save(LogoPath);
            }
            string[] keys={"warehouse","dock","shipment","operations","gsa","fuel","repair","route","upgrade","settings","info","help","close","back","play","home","display","audio","warning","storm"};
            foreach(string key in keys)if(!File.Exists(IconRoot+key+".png"))Icon(key).Save(IconRoot+key+".png");
            var materialPath="Assets/OpenWaters/Resources/UI/Highlight.mat";
            if(!AssetDatabase.LoadAssetAtPath<Material>(materialPath))
            {
                AssetDatabase.Refresh();var shader=Shader.Find("Universal Render Pipeline/Unlit");var mat=new Material(shader);mat.SetColor("_BaseColor",Color.white);AssetDatabase.CreateAsset(mat,materialPath);
            }
            AssetDatabase.Refresh();ApplyLogo();AssetDatabase.SaveAssets();
        }
        public static void ApplyLogo()
        {
            var logo=AssetDatabase.LoadAssetAtPath<Texture2D>(LogoPath);if(!logo)return;
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown,new[]{logo},IconKind.Any);
            PlayerSettings.SplashScreen.show=false;
        }
        static Canvas Icon(string key)
        {
            var c=new Canvas(128);Color color=White;
            Action<float,float,float,float> line=(x,y,a,b)=>c.Line(x,y,a,b,6,color);
            if(key=="warehouse"||key=="home")
            {
                line(18,68,64,105);line(64,105,110,68);c.Box(27,22,101,67,5,color);c.Box(51,22,77,60,5,color);if(key=="warehouse"){line(33,38,43,38);line(85,38,95,38);}
            }
            else if(key=="shipment")
            {
                c.Polygon(new[]{new Vector2(17,54),new Vector2(112,54),new Vector2(97,29),new Vector2(34,29)},color);c.Box(40,61,86,88,5,color);line(65,91,65,106);line(19,16,44,21);line(44,21,68,16);line(68,16,93,21);line(93,21,111,16);
            }
            else if(key=="dock")
            {
                c.Box(19,61,109,81,5,color);line(30,60,30,23);line(97,60,97,23);line(48,61,48,35);line(22,17,48,24);line(48,24,75,17);line(75,17,103,24);line(24,90,24,105);line(103,90,103,105);
            }
            else if(key=="gsa")
            {
                line(22,96,64,113);line(64,113,106,96);line(106,96,101,49);line(101,49,64,17);line(64,17,27,49);line(27,49,22,96);line(44,67,59,51);line(59,51,87,83);
            }
            else if(key=="fuel")
            {
                c.Box(28,23,78,101,5,color);c.Box(38,67,68,90,4,color);line(78,78,100,78);line(100,78,100,35);line(100,35,87,35);line(22,19,86,19);
            }
            else if(key=="repair")
            {
                line(29,26,84,83);c.Circle(29,26,10,5,color);line(84,83,80,105);line(80,105,95,96);line(95,96,110,102);line(110,102,104,80);line(104,80,84,83);
            }
            else if(key=="route")
            {
                c.Circle(27,28,12,5,color);c.Circle(101,100,12,5,color);line(40,28,72,28);line(72,28,72,67);line(72,67,37,67);line(37,67,37,100);line(37,100,88,100);
            }
            else if(key=="upgrade")
            {line(29,47,64,81);line(64,81,99,47);line(29,75,64,109);line(64,109,99,75);line(38,24,90,24);}
            else if(key=="operations")
            {
                c.Box(28,17,104,107,5,color);c.Box(46,99,86,115,4,color);
                for(int y=38;y<=82;y+=22){line(41,y,46,y-5);line(46,y-5,55,y+6);line(66,y,91,y);}
            }
            else if(key=="settings")
            {
                c.Circle(64,64,30,6,color);c.Circle(64,64,10,5,color);for(int i=0;i<8;i++){float a=i*Mathf.PI/4;line(64+Mathf.Cos(a)*32,64+Mathf.Sin(a)*32,64+Mathf.Cos(a)*48,64+Mathf.Sin(a)*48);}
            }
            else if(key=="display")
            {c.Box(17,39,111,102,5,color);line(64,39,64,21);line(44,18,84,18);}
            else if(key=="warning"||key=="storm")
            {
                if(key=="warning"){line(14,24,64,111);line(64,111,114,24);line(114,24,14,24);line(64,76,64,49);c.Disc(64,36,4,color);}
                else c.Polygon(new[]{new Vector2(69,114),new Vector2(29,62),new Vector2(61,62),new Vector2(49,14),new Vector2(99,76),new Vector2(69,76)},color);
            }
            else if(key=="audio")
            {c.Polygon(new[]{new Vector2(21,49),new Vector2(40,49),new Vector2(67,27),new Vector2(67,101),new Vector2(40,79),new Vector2(21,79)},color);line(84,45,94,64);line(94,64,84,83);}
            else if(key=="play")c.Polygon(new[]{new Vector2(37,20),new Vector2(105,64),new Vector2(37,108)},color);
            else if(key=="back"){line(21,64,104,64);line(21,64,52,95);line(21,64,52,33);}
            else if(key=="close"){line(32,32,96,96);line(32,96,96,32);}
            else
            {
                c.Circle(64,64,47,5,color);if(key=="help"){line(45,84,53,96);line(53,96,76,96);line(76,96,86,82);line(86,82,64,62);line(64,62,64,48);}else line(64,79,64,43);c.Disc(64,key=="help"?31:95,4,color);
            }
            return c;
        }
        sealed class Canvas
        {
            readonly Texture2D texture;readonly int n;
            public Canvas(int size){n=size;texture=new Texture2D(n,n,TextureFormat.RGBA32,false);texture.SetPixels(new Color[n*n]);}
            public void Disc(float x,float y,float r,Color c){for(int j=Mathf.Max(0,(int)(y-r-1));j<Mathf.Min(n,y+r+2);j++)for(int i=Mathf.Max(0,(int)(x-r-1));i<Mathf.Min(n,x+r+2);i++){float a=Mathf.Clamp01(r+.5f-Vector2.Distance(new Vector2(i,j),new Vector2(x,y)));if(a>0)Paint(i,j,c,a);}}
            void Paint(int x,int y,Color c,float alpha){var old=texture.GetPixel(x,y);float a=alpha*c.a;texture.SetPixel(x,y,new Color(c.r,c.g,c.b,a+old.a*(1-a)));}
            public void Line(float x,float y,float a,float b,float width,Color c){float d=Vector2.Distance(new Vector2(x,y),new Vector2(a,b));int steps=Mathf.Max(1,Mathf.CeilToInt(d*2));for(int i=0;i<=steps;i++){float t=i/(float)steps;Disc(Mathf.Lerp(x,a,t),Mathf.Lerp(y,b,t),width/2,c);}}
            public void Circle(float x,float y,float r,float width,Color c){for(int i=0;i<100;i++){float a=i*Mathf.PI*2/100,b=(i+1)*Mathf.PI*2/100;Line(x+Mathf.Cos(a)*r,y+Mathf.Sin(a)*r,x+Mathf.Cos(b)*r,y+Mathf.Sin(b)*r,width,c);}}
            public void Box(float x,float y,float a,float b,float width,Color c){Line(x,y,a,y,width,c);Line(a,y,a,b,width,c);Line(a,b,x,b,width,c);Line(x,b,x,y,width,c);}
            public void Polygon(Vector2[] points,Color c)
            {
                for(int y=0;y<n;y++)for(int x=0;x<n;x++){bool inside=false;for(int i=0,j=points.Length-1;i<points.Length;j=i++)if((points[i].y>y)!=(points[j].y>y)&&x<(points[j].x-points[i].x)*(y-points[i].y)/(points[j].y-points[i].y)+points[i].x)inside=!inside;if(inside)texture.SetPixel(x,y,c);}
            }
            public void Save(string path){texture.Apply();File.WriteAllBytes(path,texture.EncodeToPNG());UnityEngine.Object.DestroyImmediate(texture);}
        }
    }
}
