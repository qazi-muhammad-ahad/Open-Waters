using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OpenWaters
{
    public sealed class ScrollToSelected : MonoBehaviour,ISelectHandler
    {
        public void OnSelect(BaseEventData eventData)
        {
            var scroll=GetComponentInParent<ScrollRect>();if(!scroll)return;
            Canvas.ForceUpdateCanvases();
            var item=(RectTransform)transform;var content=scroll.content;if(!item.IsChildOf(content))return;
            float available=content.rect.height-scroll.viewport.rect.height;if(available<=0)return;
            Vector3[] corners=new Vector3[4];item.GetWorldCorners(corners);
            float top=-content.InverseTransformPoint(corners[1]).y;
            float bottom=-content.InverseTransformPoint(corners[0]).y;
            float current=(1-scroll.verticalNormalizedPosition)*available;
            if(top<current)scroll.verticalNormalizedPosition=1-Mathf.Clamp01(top/available);
            else if(bottom>current+scroll.viewport.rect.height)scroll.verticalNormalizedPosition=1-Mathf.Clamp01((bottom-scroll.viewport.rect.height)/available);
        }
    }
}
