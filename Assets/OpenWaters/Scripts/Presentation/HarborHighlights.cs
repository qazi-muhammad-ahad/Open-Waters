using UnityEngine;
using UnityEngine.UI;

namespace OpenWaters
{
    public sealed class HarborHighlights : MonoBehaviour
    {
        HarborPresenter harbor;
        DemoController owner;
        LineRenderer selected,attention;
        Transform tinted;
        MaterialPropertyBlock block;
        RectTransform badge;
        Text label;
        public void Initialize(HarborPresenter presenter,DemoController controller)
        {
            harbor=presenter;owner=controller;block=new MaterialPropertyBlock();selected=Line("Selected footprint",new Color(.3f,1,.9f));attention=Line("Attention footprint",new Color(1,.76f,.3f));
            var canvas=Object.FindFirstObjectByType<Canvas>();
            badge=new GameObject("World selection badge",typeof(RectTransform),typeof(Image)).GetComponent<RectTransform>();badge.SetParent(canvas.transform,false);badge.anchorMin=badge.anchorMax=new Vector2(.5f,.5f);badge.sizeDelta=new Vector2(200,36);
            badge.GetComponent<Image>().color=new Color(.03f,.11f,.15f,.96f);badge.GetComponent<Image>().raycastTarget=false;
            label=new GameObject("Selected label",typeof(RectTransform),typeof(Text)).GetComponent<Text>();label.transform.SetParent(badge,false);label.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");label.fontSize=15;label.alignment=TextAnchor.MiddleCenter;label.raycastTarget=false;
            label.rectTransform.anchorMin=Vector2.zero;label.rectTransform.anchorMax=Vector2.one;label.rectTransform.offsetMin=label.rectTransform.offsetMax=Vector2.zero;
        }
        LineRenderer Line(string name,Color color)
        {
            var line=new GameObject(name).AddComponent<LineRenderer>();line.transform.SetParent(transform,false);line.loop=true;line.positionCount=8;line.widthMultiplier=.13f;line.numCornerVertices=4;line.numCapVertices=3;line.useWorldSpace=true;
            line.material=new Material(Resources.Load<Material>("UI/Highlight"));line.material.SetColor("_BaseColor",color);line.startColor=line.endColor=color;return line;
        }
        void LateUpdate()
        {
            if(!harbor||!harbor.View)return;
            Transform target=harbor.HasSelection?harbor.SelectedTransform:null;
            if(tinted!=target)
            {
                if(tinted)foreach(var r in tinted.GetComponentsInChildren<Renderer>(true))r.SetPropertyBlock(null);
                tinted=target;
                if(tinted){block.SetColor("_BaseColor",new Color(.58f,1,.92f));foreach(var r in tinted.GetComponentsInChildren<Renderer>(true))r.SetPropertyBlock(block);}
            }
            Draw(selected,target,.18f);
            Draw(attention,harbor.AttentionTransform,.1f);
            attention.widthMultiplier=owner.Settings.reducedMotion?.1f:.1f+.045f*(.5f+.5f*Mathf.Sin(Time.unscaledTime*3));
            var anchor=target?target:harbor.AttentionTransform;
            bool show=anchor&&anchor.gameObject.activeInHierarchy&&!owner.BlockWorld;
            if(show)
            {
                var box=anchor.GetComponent<BoxCollider>();Vector3 world=anchor.TransformPoint(box.center+Vector3.up*(box.size.y*.5f+.5f));
                var point=harbor.View.WorldToScreenPoint(world);show=point.z>0&&point.x>20&&point.x<harbor.View.pixelWidth-20&&point.y>80&&point.y<harbor.View.pixelHeight-70;
                var canvas=badge.GetComponentInParent<Canvas>();var uiCamera=canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera;
                RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)badge.parent,point,uiCamera,out var local);badge.anchoredPosition=local;
                label.text=(target?"Selected · ":"↓ ")+(target?harbor.Selected:harbor.AttentionTarget);label.color=target?new Color(.3f,1,.9f):new Color(1,.76f,.3f);
            }
            badge.gameObject.SetActive(show);
        }
        static void Draw(LineRenderer line,Transform target,float padding)
        {
            bool show=target&&target.gameObject.activeInHierarchy;line.enabled=show;if(!show)return;
            var box=target.GetComponent<BoxCollider>();Vector3 center=target.TransformPoint(box.center);float x=box.size.x/2+padding+.25f,z=box.size.z/2+padding+.25f,c=.35f;
            Vector3[] points={new Vector3(-x+c,0,-z),new Vector3(x-c,0,-z),new Vector3(x,0,-z+c),new Vector3(x,0,z-c),new Vector3(x-c,0,z),new Vector3(-x+c,0,z),new Vector3(-x,0,z-c),new Vector3(-x,0,-z+c)};
            for(int i=0;i<points.Length;i++){var p=target.TransformPoint(new Vector3(points[i].x+box.center.x,0,points[i].z+box.center.z));p.y=.46f;line.SetPosition(i,p);}
        }
        void OnDestroy(){if(selected)Destroy(selected.material);if(attention)Destroy(attention.material);if(badge)Destroy(badge.gameObject);}
    }
}
