using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace OpenWaters
{
    public sealed partial class DemoController
    {
        Text standingLabel;
        RectTransform floatingRoot;
        Sprite rounded;
        bool commandFailed;
        string Heading()
        {
            var s=Session.State;
            switch(s.phase)
            {
                case RunPhase.Port:return s.deliveries==0?"Your first delivery":!s.upgraded?"Grow your port":"Choose your next shipment";
                case RunPhase.Preparing:return "Ready to set sail?";
                case RunPhase.Traveling:return "Shipment underway";
                case RunPhase.EventPending:return "Your captain needs a decision";
                case RunPhase.Delivery:return "Delivery complete";
                case RunPhase.Review:return "GSA review";
                default:return "Your port has grown";
            }
        }
        void BuildShell()
        {
            font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var es=new GameObject("Event system",typeof(EventSystem),typeof(InputSystemUIInputModule));es.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            var c=new GameObject("Open Waters interface",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));canvas=c.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            var scaler=c.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1280,720);scaler.matchWidthOrHeight=1;
            rounded=MakeRounded();
            var brand=Panel("Brand",canvas.transform,new Vector2(0,1),new Vector2(0,1),new Vector2(20,-20),250,76);
            ImageAt(brand,Resources.Load<Sprite>("Branding/Logo"),new Vector2(12,10),new Vector2(68,66));
            var title=Label(brand,"OPEN WATERS",16,Gold);Place(title.rectTransform,Vector2.zero,Vector2.one,new Vector2(78,39),new Vector2(-8,-8));
            hud=Label(brand,"",24);Place(hud.rectTransform,Vector2.zero,Vector2.one,new Vector2(78,8),new Vector2(-8,-35));
            var status=Panel("Port status",canvas.transform,Vector2.one,Vector2.one,new Vector2(-20,-20),240,76);
            ImageAt(status,UIIcons.Get("gsa"),new Vector2(14,22),new Vector2(46,54));
            standingLabel=Label(status,"",17);Place(standingLabel.rectTransform,Vector2.zero,Vector2.one,new Vector2(57,30),new Vector2(-8,-8));
            pauseControl=Button(status,"Pause",()=>{if(menu){dialog="settings";}else Paused=!Paused;dirty=true;},true,"settings");
            Place(pauseControl.GetComponent<RectTransform>(),Vector2.zero,new Vector2(1,0),new Vector2(55,7),new Vector2(-10,30));
            var goal=Rect("Current objective",canvas.transform,new Vector2(.5f,1),new Vector2(.5f,1),new Vector2(-190,-140),new Vector2(190,-104));
            objective=Label(goal,"",20);objective.alignment=TextAnchor.MiddleCenter;Place(objective.rectTransform,Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero);
        }
        void Render()
        {
            dirty=false;needsFocus=true;
            if(floatingRoot){floatingRoot.gameObject.SetActive(false);Destroy(floatingRoot.gameObject);}
            floatingRoot=Rect("Floating interface",canvas.transform,Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero);
            modal=null;travel=null;
            var s=Session.State;
            float height=s.phase==RunPhase.Preparing?(definitions.routes.Count(x=>x.contractId==s.acceptedContractId)>1?450:372):Session.AtSea?278:s.tier==2 && s.phase==RunPhase.Port?292:212;
            side=Panel("Action dock",floatingRoot,Vector2.zero,Vector2.zero,new Vector2(20,20),360,height);
            var content=Scroll(side,Navy);
            if(menu)side.gameObject.SetActive(false);
            else
            {
                Header(content,Session.AtSea?"shipment":s.phase==RunPhase.Preparing?"route":s.deliveries==1&&!s.upgraded?"upgrade":"dock",Heading(),20);
                travel=Label(content,"",14,new Color(.58f,.75f,.78f));
                if(commandFailed && notice!="")Label(content,notice,15,Gold);
                if(saves.Message!="")Label(content,saves.Message,14,Gold);
                Operations(content);
                if(details)Details();
                else if(Settings.tutorial)TipsCard();
                var help=Panel("Help",floatingRoot,new Vector2(1,0),new Vector2(1,0),new Vector2(-20,20),116,48);
                var b=Button(help,"Tips",()=>{dialog="tips";expanded=false;dirty=true;},true,"help");Place(b.GetComponent<RectTransform>(),Vector2.zero,Vector2.one,new Vector2(4,4),new Vector2(-4,-4));
            }
            if(dialog!="")RenderDialog();else if(menu)Menu();else if(Paused)PauseMenu();else if(s.phase==RunPhase.EventPending)Event();else if(s.phase==RunPhase.Delivery)Delivery();else if(s.phase==RunPhase.Review)Review();else if(s.phase==RunPhase.Complete&&!inspect)Ending();
            if(modal)foreach(var b in floatingRoot.GetComponentsInChildren<Button>(true))if(!b.transform.IsChildOf(modal))b.interactable=false;
            pauseControl.interactable=!modal;
        }
        void Operations(RectTransform p)
        {
            var s=Session.State;
            if(s.phase==RunPhase.Port)
            {
                if(s.deliveries==1&&!s.upgraded)
                {
                    Label(p,"More cargo. New destinations.",17);Primary(Button(p,"Preview upgrade",()=>Open("upgrade"),true,"upgrade"));return;
                }
                foreach(var c in definitions.contracts.Where(x=>s.deliveries==0?x.tier==1:x.tier==2))
                {
                    var cargo=definitions.cargos.Single(x=>x.id==c.cargoId);
                    var b=Button(p,"Ship to "+c.destination+"\n"+cargo.title+" · "+c.units+" units · +"+c.reward,()=>
                    {
                        Command(()=>Session.Accept(c.id));expanded=false;var routes=definitions.routes.Where(x=>x.contractId==c.id).ToArray();routeId=routes.Length==1?routes[0].id:null;
                    },Session.Eligibility(c.id)=="","shipment");b.name="Accept "+c.destination;b.GetComponent<LayoutElement>().minHeight=64;if(s.deliveries==0)Primary(b);
                }
            }
            else if(s.phase==RunPhase.Preparing)
            {
                var c=definitions.Contract(s.acceptedContractId);Label(p,c.destination+" · "+c.units+" cargo units",17);
                foreach(var r in definitions.routes.Where(x=>x.contractId==c.id))
                {
                    bool chosen=routeId==r.id;
                    var b=Button(p,(chosen?"✓ ":"")+r.title+"\n"+r.cost+" credits · "+r.seconds+"s · "+(string.IsNullOrEmpty(r.eventId)?"Safe":definitions.Event(r.eventId).title),()=>{routeId=r.id;dirty=true;},true,"route");
                    b.name="Select: "+r.title;b.GetComponent<LayoutElement>().minHeight=66;if(chosen)SelectedButton(b);
                }
                var selected=definitions.Route(routeId);bool can=selected!=null&&selected.contractId==c.id&&s.credits>=selected.cost;
                Primary(Button(p,can?"Dispatch • Pay "+selected.cost:"Choose a route",()=>{expanded=false;Command(()=>Session.Dispatch(routeId));},can,"shipment"));
                Button(p,"Route details",()=>Open("routes"),true,"info");
                if(expanded)Label(p,$"Hull {s.condition}% · Capacity {Session.Capacity}\nNet before events: +{(selected==null?0:c.reward-selected.cost)}",15);
            }
            else if(Session.AtSea)
            {
                RouteMap(p,definitions.Contract(s.voyage.contractId));
                Button(p,"Voyage log",()=>Open("log"),true,"operations");
            }
            else if(s.phase==RunPhase.Complete)
            {
                Label(p,"Two deliveries. One thriving port.",16);Button(p,"Show ending",()=>{inspect=false;dirty=true;},true,"gsa");
            }
            else Label(p,"Your shipment is safely home.",16);
        }
        void Details()
        {
            var s=Session.State;string name=Harbor.Selected;
            var p=Scroll(Panel("Selected details",floatingRoot,new Vector2(1,.5f),new Vector2(1,.5f),new Vector2(-20,0),280,242),Navy);
            Header(p,UIIcons.ForBuilding(name),name,21);
            Label(p,name=="Warehouse"?$"{Session.Capacity} cargo capacity":name=="Fuel depot"?$"Range {definitions.Tier(s.tier).range}":name=="Repair shelter"?$"Hull {s.condition}% · Improved repairs at Tier 2":name=="GSA office"?$"{s.standing} permit\nReputation {s.reputation} · Suspicion {s.suspicion}":name=="Coastal freighter"?$"Hull {s.condition}% · {Session.Capacity} cargo units":"Your berth for departing shipments.",18);
            Button(p,"More info",()=>Open("building"),true,"info");Button(p,"Close",()=>{details=false;dirty=true;},true,"close");
        }
        void TipsCard()
        {
            var p=Scroll(Panel("Optional tip",floatingRoot,new Vector2(1,0),new Vector2(1,0),new Vector2(-20,84),280,144),Navy);
            Header(p,"help","Captain's tip",17);Label(p,Objective(),15);
        }
        void RouteMap(RectTransform p,ContractData c)
        {
            var map=new GameObject("Route map",typeof(RectTransform),typeof(LayoutElement),typeof(RouteMapGraphic));map.transform.SetParent(p,false);map.GetComponent<LayoutElement>().minHeight=116;
            var g=map.GetComponent<RouteMapGraphic>();g.controller=this;g.destination=Array.IndexOf(definitions.contracts,c);g.raycastTarget=false;
            MapLabel(map.transform,"HOME",new Vector2(.01f,.49f),new Vector2(.3f,.76f));MapLabel(map.transform,"Beacon Bay",new Vector2(.36f,.83f),Vector2.one);MapLabel(map.transform,"Copperport",new Vector2(.5f,.5f),new Vector2(1,.72f));MapLabel(map.transform,"Meridian",new Vector2(.38f,0),new Vector2(1,.18f));
        }
        void MapLabel(Transform p,string text,Vector2 min,Vector2 max){var t=Label(p,text,12);Place(t.rectTransform,min,max,Vector2.zero,Vector2.zero);}
        void Menu()
        {
            var p=Modal("OPEN WATERS","",470,430);var logo=Image(p,Resources.Load<Sprite>("Branding/Logo"));logo.GetComponent<LayoutElement>().preferredHeight=100;
            Label(p,"Build your port. Choose your risks.",19);
            if(notice!=""&&saved==null)Label(p,notice,14,Gold);
            Primary(Button(p,"New Run",()=>{if(saved!=null)Open("new");else StartRun(false);},true,"play"));Button(p,"Continue",()=>StartRun(true),saved!=null,"shipment");Button(p,"Settings",()=>Open("settings"),true,"settings");Button(p,"Credits",()=>Open("credits"),true,"info");
        }
        void PauseMenu()
        {
            var p=Modal("Paused","settings",440,370);Primary(Button(p,"Resume",()=>{Paused=false;dirty=true;},true,"play"));Button(p,"Settings",()=>Open("settings"),true,"settings");Button(p,"Main Menu • Save run",ReturnMenu,true,"home");
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Button(p,"Development scenarios",()=>Open("scenarios"),true,"operations");
#endif
            if(Application.platform!=RuntimePlatform.WebGLPlayer)Button(p,"Quit",()=>Application.Quit(),true,"close");
        }
        void Event()
        {
            var e=definitions.Event(Session.State.voyage.pendingEventId);var p=Modal(e.title,UIIcons.ForEvent(e.id),560,e.id=="inspection"?(expanded?610:500):(expanded?500:380));
            Label(p,e.id=="inspection"?"A premium offer has irregular paperwork.":e.id=="storm"?"A storm is crossing your route.":e.id=="pirates"?"The shortcut is unprotected.":e.id=="engine"?"The engine needs attention.":"The destination's berths are full.",18);
            foreach(var c in e.choices)
            {
                string effect=e.id=="inspection"?(c.id=="clean"?"Base payout · Clear review":c.id=="fee"?"+120 delivery · Watch · Review −60":"+240 delivery · Restricted · Review −180"):
                    c.useRepairModifiers?"20 credits · +5s":c.dockDelay<0?"Hull −20 · Dock repair +10s":c.fee>0?"−"+c.fee+" at delivery"+(c.damage>0?" · Hull −"+c.damage:""):"+"+c.travelDelay+"s · No fee";
                var b=Button(p,c.title+"\n"+effect,()=>{expanded=false;Command(()=>Session.Resolve(e.id,c.id));},true,e.id=="inspection"?c.severity==0?"gsa":"warning":UIIcons.ForEvent(e.id));b.name=c.title;b.GetComponent<LayoutElement>().minHeight=66;
                if(e.id=="inspection"&&c.severity==2)b.GetComponent<Image>().color=new Color(.42f,.16f,.17f);
                if(expanded)Label(p,c.description,14);
            }
            Button(p,expanded?"Hide consequences":"All consequences",()=>{expanded=!expanded;dirty=true;},true,"info");
        }
        void Delivery()
        {
            var s=Session.State;var v=s.voyage;var c=definitions.Contract(v.contractId);var r=definitions.Route(v.routeId);
            var p=Modal("Delivered to "+c.destination,"shipment",500,expanded?540:330);
            Label(p,"+"+(c.reward+v.bonus-r.cost-v.fees)+" <size=18>net earnings</size>",34,Gold);Label(p,s.credits+" credits in your account",19);
            Primary(Button(p,s.deliveries==1?"Continue to port upgrade":"Continue to scheduled GSA review",()=>{expanded=false;Command(Session.AcknowledgeDelivery);},true,s.deliveries==1?"upgrade":"gsa"));
            Button(p,expanded?"Hide ledger":"View ledger",()=>{expanded=!expanded;dirty=true;},true,"operations");
            if(expanded){Label(p,$"Base +{c.reward}\nPremium +{v.bonus}\nOperating −{r.cost} (already paid)\nEvent fees −{v.fees}\nReputation +{definitions.deliveryReputation}",18);foreach(var x in v.choices)Label(p,x.description,14);}
        }
        void Review()
        {
            var x=Session.State.decisions.Single(d=>d.eventId=="inspection");var p=Modal("Scheduled GSA review","gsa",520,expanded?490:360);
            string result=x.severity==0?"Clear":x.severity==1?"Watch":"Restricted";
            Label(p,result,32,x.severity==2?new Color(1,.5f,.4f):Gold);
            Label(p,x.severity==0?"Clean paperwork. Your next horizon is open.":x.severity==1?"The unofficial fee was flagged.":"The risky shipment breached your permit.",19);
            Label(p,x.severity==0?"+40 credits · +5 reputation":x.severity==1?"−60 credits · −10 reputation":"−180 credits · −20 reputation",20,Gold);
            Primary(Button(p,"Apply review and finish",()=>{expanded=false;Command(Session.Review);},true,"gsa"));
            Button(p,expanded?"Hide evidence":"View evidence",()=>{expanded=!expanded;dirty=true;},true,"info");if(expanded)Label(p,x.description+"\nPremium +"+x.bonus+" · Fee "+x.fee,16);
        }
        void Ending()
        {
            var s=Session.State;var p=Modal("A bigger horizon","upgrade",520,expanded?600:430);
            Label(p,"COMMERCIAL PORT  ·  TIER 2",17,Gold);Label(p,s.credits+" <size=18>credits</size>    "+s.standing,30);
            Label(p,s.standing=="Clear"?"Your clean record opens future regional opportunities.":s.standing=="Watch"?"Rebuild a clean record before expanding.":"Premium access suspended. Reinstatement comes next.",18);
            Primary(Button(p,"Inspect Port",()=>{Command(Session.AcknowledgeEnding);inspect=true;dirty=true;},true,"home"));Button(p,"New Run",()=>Open("new"),true,"play");Button(p,"Main Menu",()=>{Command(Session.AcknowledgeEnding);ReturnMenu();},true,"home");
            Button(p,expanded?"Hide run summary":"Run summary",()=>{expanded=!expanded;dirty=true;},true,"operations");
            if(expanded){Label(p,$"Credits {definitions.startingCredits} → {s.credits}\nReputation {definitions.startingReputation} → {s.reputation}\nSuspicion {s.suspicion}",17);foreach(var x in s.ledger)Label(p,x.description+": "+x.amount.ToString("+0;-0;0"),14);}
        }
        void Open(string value){dialog=value;expanded=false;dirty=true;}
        void RenderDialog()
        {
            if(dialog=="settings")
            {
                var p=Modal("Settings","settings",520,580);
                if(DisplaySettingsController.Supported)
                {
                    Label(p,"DISPLAY",14,Gold);
                    foreach(DisplayMode mode in Enum.GetValues(typeof(DisplayMode)))
                    {
                        var b=Button(p,(Settings.displayMode==mode?"✓ ":"")+(mode==DisplayMode.Borderless?"Windowed fullscreen":mode.ToString()),()=>{DisplaySettingsController.SetMode(Settings,mode);dirty=true;},true,"display");if(Settings.displayMode==mode)SelectedButton(b);
                    }
                }
                Toggle(p,"Show tips",Settings.tutorial,v=>Settings.tutorial=v,"help");Toggle(p,"Reduced motion",Settings.reducedMotion,v=>Settings.reducedMotion=v,"operations");
                Button(p,"Sound levels",()=>Open("audio"),true,"audio");
                Primary(Button(p,"Save settings / Back",()=>{Settings.Save();Harbor.Refresh();dialog="";dirty=true;},true,"back"));
            }
            else if(dialog=="audio")
            {
                var p=Modal("Sound levels","audio",470,370);Toggle(p,"Mute",Settings.mute,v=>Settings.mute=v,"audio");Slider(p,"Master",Settings.master,v=>Settings.master=v);Slider(p,"Ambience",Settings.ambience,v=>Settings.ambience=v);Slider(p,"Effects",Settings.sfx,v=>Settings.sfx=v);
                Primary(Button(p,"Back to settings",()=>Open("settings"),true,"back"));
            }
            else if(dialog=="upgrade")
            {
                var p=Modal("Commercial port upgrade","upgrade",510,expanded?530:350);
                Header(p,"warehouse","10 → 20 cargo units",23);Header(p,"route","New destinations",23);Label(p,"All five buildings improve together.",17);
                Primary(Button(p,"Buy upgrade • 300 credits",()=>{Command(Session.Upgrade);dialog="";dirty=true;},Session.State.credits>=definitions.upgradeCost,"upgrade"));Button(p,"Cancel",()=>{dialog="";dirty=true;},true,"close");
                Button(p,expanded?"Hide details":"Upgrade details",()=>{expanded=!expanded;dirty=true;},true,"info");if(expanded)Label(p,"Range 1 → 2\nRepairs: 40 credits /10s → 20 credits /5s\nTextiles and machine parts unlock.\nOne purchase. Same freighter.",17);
            }
            else if(dialog=="new")
            {
                var p=Modal("Start again?","play",460,250);Label(p,"This replaces the current run.",19);Primary(Button(p,"Replace run and start",()=>StartRun(false),true,"play"));Button(p,"Cancel",()=>{dialog="";dirty=true;},true,"close");
            }
            else if(dialog=="routes")
            {
                var p=Modal("Route details","route",520,470);var c=definitions.Contract(Session.State.acceptedContractId);RouteMap(p,c);
                foreach(var r in definitions.routes.Where(x=>x.contractId==c.id))Label(p,r.title+": "+r.cost+" credits · "+r.seconds+"s\nNet before events +"+(c.reward-r.cost)+(string.IsNullOrEmpty(r.eventId)?"":"\nExpected: "+definitions.Event(r.eventId).title+" + GSA check"),16);
                Button(p,"Cancel contract • No penalty",()=>{dialog="";Command(Session.Cancel);},true,"close");Back(p);
            }
            else if(dialog=="log")
            {
                var p=Modal("Voyage log","operations",500,410);foreach(var x in Session.State.voyage.choices)Label(p,x.description+$"\nFee {x.fee} · Bonus {x.bonus} · Delay {x.travelDelay}s",17);if(Session.State.voyage.choices.Count==0)Label(p,"All clear. Your ship is underway.",19);Back(p);
            }
            else if(dialog=="tips"||dialog=="building")
            {
                var p=Modal(dialog=="tips"?"Captain's tips":Harbor.Selected,dialog=="tips"?"help":UIIcons.ForBuilding(Harbor.Selected),500,410);
                Label(p,Objective(),19);Label(p,dialog=="tips"?"Select a building to inspect it.\nPan: WASD / drag · Zoom: wheel · Home: recenter\nCargo is supplied by your client.\nVoyage fees settle when the ship returns.":BuildingInfo(),17);
                if(dialog=="tips")Toggle(p,"Show contextual tips",Settings.tutorial,v=>{Settings.tutorial=v;Settings.Save();},"help");Back(p);
            }
            else if(dialog=="scenarios")
            {
                var p=Modal("Development scenarios","operations",550,460);Label(p,"Fixtures replace the current run.",16,Gold);
                foreach(var r in definitions.routes.Where(x=>!string.IsNullOrEmpty(x.eventId)))Button(p,r.title+" · "+definitions.Event(r.eventId).title,()=>{StartRun(false);Session.Accept("grain_beacon");Session.Dispatch("beacon_coastal");Session.Tick(35);Session.AcknowledgeDelivery();Session.Upgrade();Session.Accept(r.contractId);Session.Dispatch(r.id);Session.Tick(r.seconds);dirty=true;},true,UIIcons.ForEvent(r.eventId));Back(p);
            }
            else
            {
                var p=Modal("Open Waters","info",480,350);Image(p,Resources.Load<Sprite>("Branding/Logo"));Label(p,"DEUXART LABS\nOriginal harbor art and procedural sound.\nUnity 6 / URP",19);Back(p);
            }
        }
        string BuildingInfo()
        {
            string n=Harbor.Selected;return n=="Warehouse"?"Warehouse limits each load: 10 units at Tier 1, 20 at Tier 2.":n=="Fuel depot"?"Fuel range unlocks destinations. Route operating charges include fuel.":n=="Repair shelter"?"Tier 2 halves engine repair cost and delay. Return service restores hull condition after delivery.":n=="GSA office"?"Your decisions leave a record. One scheduled review follows the second delivery. Violations affect fines, reputation and permit standing.":n=="Coastal freighter"?"One freighter, physical capacity 20. A smaller warehouse limits early loads. It is accessible at the dock.":"Choose cargo, prepare a route, dispatch, then reinvest the delivery earnings in your port.";
        }
        void Back(Transform p)=>Button(p,"Back",()=>{dialog="";dirty=true;},true,"back");
        RectTransform Modal(string title,string icon="",float width=520,float height=430)
        {
            needsFocus=true;var group=side.gameObject.AddComponent<CanvasGroup>();group.interactable=false;group.blocksRaycasts=false;
            modal=Rect("Modal",floatingRoot,Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero);modal.gameObject.AddComponent<Image>().color=new Color(.015f,.04f,.06f,.38f);
            var panel=Panel(title,modal,new Vector2(.5f,.5f),new Vector2(.5f,.5f),Vector2.zero,width,height);var p=Scroll(panel,Navy);if(title!="OPEN WATERS")Header(p,icon,title,27);return p;
        }
        RectTransform Panel(string name,Transform parent,Vector2 anchor,Vector2 pivot,Vector2 position,float width,float height)
        {
            var r=Rect(name,parent,anchor,anchor,Vector2.zero,Vector2.zero);r.pivot=pivot;r.sizeDelta=new Vector2(width,height);r.anchoredPosition=position;
            var i=r.gameObject.AddComponent<Image>();i.sprite=rounded;i.type=UnityEngine.UI.Image.Type.Sliced;i.color=Navy;
            var shadow=r.gameObject.AddComponent<Shadow>();shadow.effectColor=new Color(0,0,0,.28f);shadow.effectDistance=new Vector2(0,-5);return r;
        }
        static RectTransform Rect(string name,Transform parent,Vector2 min,Vector2 max,Vector2 lo,Vector2 hi){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);Place(r,min,max,lo,hi);return r;}
        static void Place(RectTransform r,Vector2 min,Vector2 max,Vector2 lo,Vector2 hi){r.anchorMin=min;r.anchorMax=max;r.offsetMin=lo;r.offsetMax=hi;}
        RectTransform Scroll(RectTransform parent,Color background)
        {
            var scroll=parent.gameObject.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.movementType=ScrollRect.MovementType.Clamped;scroll.scrollSensitivity=30;
            var viewport=Rect("Viewport",parent,Vector2.zero,Vector2.one,new Vector2(18,16),new Vector2(-18,-16));viewport.gameObject.AddComponent<RectMask2D>();
            var p=Rect("Content",viewport,new Vector2(0,1),Vector2.one,Vector2.zero,Vector2.zero);p.pivot=new Vector2(.5f,1);
            var layout=p.gameObject.AddComponent<VerticalLayoutGroup>();layout.spacing=10;layout.childControlHeight=true;layout.childControlWidth=true;layout.childForceExpandHeight=false;
            p.gameObject.AddComponent<ContentSizeFitter>().verticalFit=ContentSizeFitter.FitMode.PreferredSize;scroll.viewport=viewport;scroll.content=p;return p;
        }
        Text Label(Transform p,string value,int size,Color? color=null)
        {
            var t=new GameObject("Text",typeof(RectTransform),typeof(Text)).GetComponent<Text>();t.transform.SetParent(p,false);t.font=font;t.fontSize=size;t.color=color??Ink;t.text=value;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Overflow;t.raycastTarget=false;return t;
        }
        void Header(Transform p,string icon,string title,int size)
        {
            var row=new GameObject("Heading",typeof(RectTransform),typeof(LayoutElement));row.transform.SetParent(p,false);row.GetComponent<LayoutElement>().preferredHeight=size+12;
            if(icon!="")ImageAt(row.transform,UIIcons.Get(icon),new Vector2(0,2),new Vector2(size+8,size+10));
            var t=Label(row.transform,title,size,Gold);Place(t.rectTransform,Vector2.zero,Vector2.one,new Vector2(icon==""?0:size+17,0),Vector2.zero);
        }
        UnityEngine.UI.Image Image(Transform parent,Sprite sprite)
        {
            var i=new GameObject("Icon",typeof(RectTransform),typeof(UnityEngine.UI.Image),typeof(LayoutElement)).GetComponent<UnityEngine.UI.Image>();i.transform.SetParent(parent,false);i.sprite=sprite;i.preserveAspect=true;i.raycastTarget=false;i.GetComponent<LayoutElement>().preferredHeight=72;return i;
        }
        void ImageAt(Transform p,Sprite sprite,Vector2 lo,Vector2 hi){var i=Image(p,sprite);Place(i.rectTransform,Vector2.zero,Vector2.zero,lo,hi);}
        Button Button(Transform p,string value,Action action,bool enabled=true,string icon="")
        {
            var o=new GameObject(value,typeof(RectTransform),typeof(UnityEngine.UI.Image),typeof(Button),typeof(LayoutElement),typeof(ScrollToSelected),typeof(ButtonFocus));o.transform.SetParent(p,false);
            var i=o.GetComponent<UnityEngine.UI.Image>();i.sprite=rounded;i.type=UnityEngine.UI.Image.Type.Sliced;i.color=new Color(.105f,.245f,.29f);
            o.GetComponent<LayoutElement>().minHeight=42;var b=o.GetComponent<Button>();b.interactable=enabled;
            var colors=b.colors;colors.highlightedColor=new Color(.75f,1,1);colors.selectedColor=colors.highlightedColor;colors.disabledColor=new Color(.6f,.65f,.66f,.4f);b.colors=colors;
            if(icon!=""){var picture=Image(o.transform,UIIcons.Get(icon));Place(picture.rectTransform,new Vector2(0,.5f),new Vector2(0,.5f),new Vector2(12,-13),new Vector2(38,13));}
            var t=Label(o.transform,value,17);t.alignment=TextAnchor.MiddleCenter;Place(t.rectTransform,Vector2.zero,Vector2.one,new Vector2(icon==""?10:46,4),new Vector2(-10,-4));
            b.onClick.AddListener(()=>action());if(needsFocus&&enabled){EventSystem.current.SetSelectedGameObject(o);needsFocus=false;}return b;
        }
        void Primary(Button b){b.GetComponent<UnityEngine.UI.Image>().color=Gold;b.GetComponentInChildren<Text>().color=new Color(.05f,.12f,.16f);foreach(var i in b.GetComponentsInChildren<UnityEngine.UI.Image>())if(i.gameObject!=b.gameObject)i.color=Navy;b.GetComponent<ButtonFocus>().primary=true;}
        void SelectedButton(Button b){b.GetComponent<ButtonFocus>().latched=true;var outline=b.gameObject.GetComponent<Outline>()??b.gameObject.AddComponent<Outline>();outline.effectDistance=new Vector2(2,-2);outline.effectColor=new Color(.3f,.95f,.9f);b.GetComponent<UnityEngine.UI.Image>().color=new Color(.11f,.35f,.36f);}
        void Toggle(Transform p,string title,bool current,Action<bool> change,string icon="settings"){Button(p,(current?"✓ ":"○ ")+title,()=>{change(!current);Settings.Save();dirty=true;},true,icon);}
        void Slider(Transform p,string title,float value,Action<float> change)
        {
            var row=new GameObject(title,typeof(RectTransform),typeof(LayoutElement));row.transform.SetParent(p,false);row.GetComponent<LayoutElement>().minHeight=40;
            var t=Label(row.transform,title+"  "+Mathf.RoundToInt(value*100)+"%",16);Place(t.rectTransform,Vector2.zero,Vector2.one,new Vector2(0,8),new Vector2(-116,0));
            var minus=Button(row.transform,"−",()=>{change(Mathf.Clamp01(value-.1f));Settings.Save();dirty=true;});Place(minus.GetComponent<RectTransform>(),new Vector2(1,0),Vector2.one,new Vector2(-105,0),new Vector2(-58,0));
            var plus=Button(row.transform,"+",()=>{change(Mathf.Clamp01(value+.1f));Settings.Save();dirty=true;});Place(plus.GetComponent<RectTransform>(),new Vector2(1,0),Vector2.one,new Vector2(-47,0),Vector2.zero);
        }
        static Sprite MakeRounded()
        {
            const int n=32;var texture=new Texture2D(n,n,TextureFormat.RGBA32,false);texture.wrapMode=TextureWrapMode.Clamp;
            for(int y=0;y<n;y++)for(int x=0;x<n;x++){float dx=Mathf.Max(8-x,Mathf.Max(0,x-23)),dy=Mathf.Max(8-y,Mathf.Max(0,y-23));texture.SetPixel(x,y,new Color(1,1,1,Mathf.Clamp01(8.5f-Mathf.Sqrt(dx*dx+dy*dy))));}
            texture.Apply();return Sprite.Create(texture,new Rect(0,0,n,n),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect,new Vector4(9,9,9,9));
        }
    }
}
