using UnityEngine;
using UnityEngine.UI;

namespace OpenWaters
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class RouteMapGraphic : MaskableGraphic
    {
        protected RouteMapGraphic(){useLegacyMeshGeneration=false;}
        public DemoController controller;
        public int destination;
        static readonly Vector2 Home=new Vector2(.12f,.48f);
        static readonly Vector2[] Ports={new Vector2(.63f,.82f),new Vector2(.84f,.5f),new Vector2(.65f,.15f)};
        void Update(){if(controller && controller.Session.AtSea)SetVerticesDirty();}
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();var bounds=rectTransform.rect;
            for(int i=0;i<Ports.Length;i++)
            {
                Color c=i==destination?new Color(.95f,.7f,.27f):new Color(.18f,.4f,.47f);
                Vector2 a=Point(Home,bounds),b=Point(Ports[i],bounds),normal=new Vector2(-(b-a).y,(b-a).x).normalized*1.4f;
                Quad(vh,a-normal,a+normal,b+normal,b-normal,c);Square(vh,b,5,c);
            }
            Square(vh,Point(Home,bounds),7,Color.white);
            if(controller && controller.Session.AtSea)
            {
                float t=controller.Session.State.voyage.elapsed/controller.Session.Duration;
                t=t<.5f?t*2:(1-t)*2;
                Square(vh,Vector2.Lerp(Point(Home,bounds),Point(Ports[destination],bounds),t),4,new Color(.4f,.95f,.84f));
            }
        }
        static Vector2 Point(Vector2 p,Rect r)=>r.min+Vector2.Scale(p,r.size);
        static void Square(VertexHelper v,Vector2 p,float s,Color c)=>Quad(v,p+new Vector2(-s,-s),p+new Vector2(-s,s),p+new Vector2(s,s),p+new Vector2(s,-s),c);
        static void Quad(VertexHelper v,Vector2 a,Vector2 b,Vector2 c,Vector2 d,Color color)
        {
            int start=v.currentVertCount;v.AddVert(a,color,Vector2.zero);v.AddVert(b,color,Vector2.zero);v.AddVert(c,color,Vector2.zero);v.AddVert(d,color,Vector2.zero);v.AddTriangle(start,start+1,start+2);v.AddTriangle(start,start+2,start+3);
        }
    }
}
