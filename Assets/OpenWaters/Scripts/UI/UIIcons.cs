using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OpenWaters
{
    public static class UIIcons
    {
        static readonly Dictionary<string,Sprite> Cache=new Dictionary<string,Sprite>();
        public static Sprite Get(string key){if(!Cache.TryGetValue(key,out var icon)){icon=Resources.Load<Sprite>("UI/Icons/"+key);Cache[key]=icon;}return icon;}
        public static string ForBuilding(string name)=>name=="Warehouse"?"warehouse":name=="Fuel depot"?"fuel":name=="Repair shelter"?"repair":name=="GSA office"?"gsa":name=="Coastal freighter"?"shipment":"dock";
        public static string ForEvent(string id)=>id=="inspection"?"gsa":id=="engine"?"repair":id=="storm"?"storm":id=="pirates"?"warning":"operations";
    }
    public sealed class ButtonFocus : MonoBehaviour,ISelectHandler,IDeselectHandler,IPointerEnterHandler,IPointerExitHandler
    {
        public bool primary,latched;
        bool focused,hovered;
        Outline outline;
        void Refresh()
        {
            if(!outline){outline=GetComponent<Outline>()??gameObject.AddComponent<Outline>();outline.effectColor=new Color(.35f,.95f,.9f);outline.effectDistance=new Vector2(2,-2);}
            outline.effectColor=latched?new Color(.3f,1,.9f):new Color(.9f,1,1,.8f);
            outline.enabled=latched||focused||hovered;
        }
        public void OnSelect(BaseEventData e){focused=true;Refresh();}
        public void OnDeselect(BaseEventData e){focused=false;Refresh();}
        public void OnPointerEnter(PointerEventData e){hovered=true;Refresh();}
        public void OnPointerExit(PointerEventData e){hovered=false;Refresh();}
    }
}
