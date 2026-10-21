using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace OpenWaters
{
    public sealed class HarborSelectable : MonoBehaviour { public string label; }
    public sealed class HarborPresenter : MonoBehaviour
    {
        public static readonly string[] BuildingNames={"Dock","Warehouse","Fuel depot","GSA office","Repair shelter"};
        static readonly Vector3[] Anchors={new Vector3(0,.12f,-5.3f),new Vector3(-3,.3f,2.4f),new Vector3(-5.5f,.3f,-1.3f),new Vector3(3.5f,.3f,4.6f),new Vector3(3.6f,.3f,-.5f)};
        readonly GameObject[] buildings=new GameObject[5];
        DemoController controller;
        Transform ship;
        GameObject cargo,wake,smoke,selection,loading,arrival;
        GameObject ocean;
        int tier;
        string cargoId;
        float revealRemaining;
        GameObject reveal;
        RunPhase lastPhase;
        float arrivalRemaining;
        HarborHighlights highlights;
        Vector3 offshore;
        string visualVoyage;
        bool returning;
        static readonly Vector3 Berth=new Vector3(0,.1f,-9);
        const float ManeuverSeconds=6;
        public bool ShipVisible=>ship&&ship.gameObject.activeSelf;
        public bool HasSelection {get;private set;}
        public string AttentionTarget
        {
            get
            {
                var s=controller.Session.State;
                return s.phase==RunPhase.Port?(s.deliveries==1&&!s.upgraded?"Warehouse":"Dock"):s.phase==RunPhase.Preparing?"Coastal freighter":s.phase==RunPhase.Review?"GSA office":"";
            }
        }
        public Transform SelectedTransform=>Selected=="Coastal freighter"?ship:buildings[System.Array.IndexOf(BuildingNames,Selected)].transform;
        public Transform AttentionTransform=>AttentionTarget==""?null:AttentionTarget=="Coastal freighter"?ship:buildings[System.Array.IndexOf(BuildingNames,AttentionTarget)].transform;
        public void ResetSelection(){HasSelection=false;Selected="Dock";visualVoyage=null;returning=false;}
        public Camera View {get;private set;}
        public string Selected {get;private set;}="Dock";
        public void Initialize(DemoController owner)
        {
            controller=owner;
            var d=owner.definitions;
            Place(d.ocean,new Vector3(0,-.12f,0),new Vector3(4,1,4));
            ocean=transform.GetChild(transform.childCount-1).gameObject;
            Place(d.coast,new Vector3(0,-.05f,3),new Vector3(2.4f,1,1.9f));
            Place(d.foam,new Vector3(0,-.08f,3),new Vector3(2.4f,1,1.9f));
            ship=Place(d.ship,new Vector3(0,.1f,-9),Vector3.one,90).transform;
            AddSelection(ship.gameObject,"Coastal freighter");
            wake=Place(d.wake,Vector3.zero,Vector3.one);Attach(wake,"WakeAttachment");
            smoke=Place(d.damage,Vector3.zero,Vector3.one);Attach(smoke,"DamageAttachment");
            selection=Place(d.selection,Vector3.zero,Vector3.one);
            selection.SetActive(false);
            highlights=gameObject.AddComponent<HarborHighlights>();highlights.Initialize(this,owner);
            loading=Place(d.loading,new Vector3(0,.3f,-7),Vector3.one);
            arrival=Place(d.arrival,new Vector3(0,.3f,-9),Vector3.one);arrival.SetActive(false);
            View=Camera.main;
            View.orthographic=true;View.orthographicSize=15;View.transform.rotation=Quaternion.Euler(55,45,0);
            View.backgroundColor=new Color(.055f,.2f,.26f);View.clearFlags=CameraClearFlags.SolidColor;
            View.nearClipPlane=.1f;View.farClipPlane=160;
            var cameraControl=View.gameObject.AddComponent<HarborCamera>();cameraControl.Initialize(controller);
            RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=new Color(.52f,.62f,.7f);
            Refresh();
        }
        GameObject Place(GameObject prefab,Vector3 position,Vector3 scale,float yaw=0)
        {
            var obj=Instantiate(prefab,position,Quaternion.Euler(0,yaw,0),transform);obj.transform.localScale=scale;return obj;
        }
        void Attach(GameObject obj,string socket)
        {
            obj.transform.SetParent(ship.GetComponentsInChildren<Transform>().Single(x=>x.name==socket),false);
            obj.transform.localPosition=Vector3.zero;obj.transform.localRotation=Quaternion.identity;
        }
        static void AddSelection(GameObject obj,string label)
        {
            obj.AddComponent<HarborSelectable>().label=label;
            var box=obj.AddComponent<BoxCollider>();var rs=obj.GetComponentsInChildren<Renderer>();var b=new Bounds(obj.transform.position,Vector3.zero);
            foreach(var r in rs)b.Encapsulate(r.bounds);
            box.center=obj.transform.InverseTransformPoint(b.center);box.size=new Vector3(b.size.x,b.size.y,b.size.z);
        }
        public void Refresh()
        {
            if(controller.Session==null)return;
            var s=controller.Session.State;var d=controller.definitions;
            loading.SetActive(s.phase==RunPhase.Preparing && !controller.Settings.reducedMotion);
            if(s.phase==RunPhase.Delivery && lastPhase!=RunPhase.Delivery && !controller.Settings.reducedMotion){arrival.SetActive(true);arrivalRemaining=2;}
            lastPhase=s.phase;
            if(tier!=s.tier)
            {
                bool upgraded=tier==1 && s.tier==2;tier=s.tier;
                for(int i=0;i<5;i++)
                {
                    if(buildings[i]){buildings[i].SetActive(false);Destroy(buildings[i]);}
                    buildings[i]=Place(d.Tier(tier).buildings[i],Anchors[i],Vector3.one,180);AddSelection(buildings[i],BuildingNames[i]);
                }
                if(upgraded && !controller.Settings.reducedMotion){reveal=Place(d.upgrade,Vector3.zero,Vector3.one*2);revealRemaining=3;}
            }
            string contractId=s.acceptedContractId;
            if(controller.Session.AtSea)contractId=s.voyage.contractId;
            string next=string.IsNullOrEmpty(contractId)?null:d.Contract(contractId).cargoId;
            if(cargoId!=next)
            {
                cargoId=next;if(cargo){cargo.SetActive(false);Destroy(cargo);}
                if(next!=null){cargo=Place(d.cargos.Single(x=>x.id==next).prefab,Vector3.zero,Vector3.one*.9f);Attach(cargo,"CargoAttachment");}
            }
            smoke.SetActive(s.condition<100);
            foreach(var a in GetComponentsInChildren<Animation>(true))a.enabled=!controller.Settings.reducedMotion;
            var mr=ocean.GetComponentInChildren<Renderer>();mr.material.SetFloat("_Speed",controller.Settings.reducedMotion?0:1);
        }
        void Update()
        {
            if(controller?.Session==null)return;
            var session=controller.Session;float progress=session.Duration>0?session.State.voyage.elapsed/session.Duration:0;
            bool atSea=session.AtSea;float elapsed=atSea?session.State.voyage.elapsed:0,remaining=atSea?session.Duration-elapsed:0;
            if(atSea&&visualVoyage!=session.State.voyage.id){visualVoyage=session.State.voyage.id;offshore=OffscreenPoint();returning=false;}
            bool approaching=atSea&&remaining<=ManeuverSeconds;
            if(approaching&&!returning){offshore=OffscreenPoint();returning=true;}
            bool visible=!atSea||elapsed<ManeuverSeconds||approaching;
            ship.gameObject.SetActive(visible);
            ship.GetComponent<BoxCollider>().enabled=!atSea;
            ship.position=!atSea?Berth:approaching?Vector3.Lerp(offshore,Berth,Mathf.SmoothStep(0,1,1-remaining/ManeuverSeconds)):Vector3.Lerp(Berth,offshore,Mathf.SmoothStep(0,1,elapsed/ManeuverSeconds));
            ship.rotation=Quaternion.Euler(0,!atSea||approaching?90:270,0);
            wake.SetActive(session.AtSea && !controller.Paused && session.State.phase!=RunPhase.EventPending);
            if(reveal){revealRemaining-=Time.unscaledDeltaTime;if(revealRemaining<=0)Destroy(reveal);}
            if(arrival.activeSelf){arrivalRemaining-=Time.unscaledDeltaTime;if(arrivalRemaining<=0)arrival.SetActive(false);}
        }
        Vector3 OffscreenPoint()
        {
            // Authored lane extends beyond every supported camera pan/zoom; the
            // ship is then hidden offshore so camera exploration cannot reveal it.
            float exit=float.PositiveInfinity;
            foreach(var plane in GeometryUtility.CalculateFrustumPlanes(View))
            {
                float direction=Vector3.Dot(plane.normal,Vector3.left),inside=plane.GetDistanceToPoint(Berth);
                if(direction<-.001f&&inside>=0)exit=Mathf.Min(exit,inside/-direction);
            }
            float margin=ship.GetComponent<BoxCollider>().size.magnitude*.6f+3;
            return Berth+Vector3.left*((float.IsInfinity(exit)?60:exit)+margin);
        }
        public void SelectAt(Vector2 pointer)
        {
            if(controller.BlockWorld || EventSystem.current.IsPointerOverGameObject())return;
            if(Physics.Raycast(View.ScreenPointToRay(pointer),out var hit,200))
            {
                var item=hit.collider.GetComponent<HarborSelectable>();if(item){Selected=item.label;HasSelection=true;controller.ShowDetails();}
            }
        }
    }
    public sealed class HarborCamera : MonoBehaviour
    {
        DemoController controller;
        InputActionMap actions;
        InputAction pan,zoom,drag,delta,select,pointer,home,pause;
        Vector3 target;
        Camera view;
        public void Initialize(DemoController owner)
        {
            controller=owner;view=GetComponent<Camera>();
            // Dedicated map leaves the project's Player/UI maps intact.
            actions=new InputActionMap("Harbor");
            pan=actions.AddAction("Pan",InputActionType.Value);pan.AddCompositeBinding("2DVector").With("Up","<Keyboard>/w").With("Down","<Keyboard>/s").With("Left","<Keyboard>/a").With("Right","<Keyboard>/d");
            pan.AddCompositeBinding("2DVector").With("Up","<Keyboard>/upArrow").With("Down","<Keyboard>/downArrow").With("Left","<Keyboard>/leftArrow").With("Right","<Keyboard>/rightArrow");
            zoom=actions.AddAction("Zoom",InputActionType.Value,"<Mouse>/scroll/y");
            drag=actions.AddAction("Drag",InputActionType.Button,"<Mouse>/rightButton");drag.AddBinding("<Mouse>/middleButton");
            delta=actions.AddAction("Delta",InputActionType.Value,"<Pointer>/delta");
            pointer=actions.AddAction("Pointer",InputActionType.Value,"<Pointer>/position");
            select=actions.AddAction("Select",InputActionType.Button,"<Mouse>/leftButton");
            home=actions.AddAction("Recenter",InputActionType.Button,"<Keyboard>/home");pause=actions.AddAction("Pause",InputActionType.Button,"<Keyboard>/escape");
            actions.Enable();Recenter();
        }
        public void Recenter(){target=new Vector3(-2,0,0);Apply();}
        void Update()
        {
            if(controller==null)return;
            if(pause.WasPressedThisFrame())controller.Escape();
            if(controller.BlockWorld)return;
            bool over=EventSystem.current && EventSystem.current.IsPointerOverGameObject();
            if(!over)
            {
                var movement=pan.ReadValue<Vector2>()*Time.unscaledDeltaTime*12;
                if(drag.IsPressed())movement-=delta.ReadValue<Vector2>()*.025f;
                target+=Quaternion.Euler(0,45,0)*new Vector3(movement.x,0,movement.y);
                target.x=Mathf.Clamp(target.x,-10,10);target.z=Mathf.Clamp(target.z,-10,10);
                view.orthographicSize=Mathf.Clamp(view.orthographicSize-zoom.ReadValue<float>()*.012f,9,22);
                if(select.WasPressedThisFrame())controller.Harbor.SelectAt(pointer.ReadValue<Vector2>());
            }
            if(home.WasPressedThisFrame())Recenter();Apply();
        }
        void Apply(){transform.position=target-transform.forward*45;}
        void OnDestroy(){actions?.Dispose();}
    }
}
