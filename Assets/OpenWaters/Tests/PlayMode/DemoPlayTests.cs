using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace OpenWaters.Tests
{
    public sealed class DemoPlayTests
    {
        string preferences;
        bool hadPreferences;
        [SetUp] public void PreservePreferences(){hadPreferences=PlayerPrefs.HasKey("OpenWaters.Settings");preferences=PlayerPrefs.GetString("OpenWaters.Settings");}
        [TearDown] public void RestorePreferences(){if(hadPreferences)PlayerPrefs.SetString("OpenWaters.Settings",preferences);else PlayerPrefs.DeleteKey("OpenWaters.Settings");PlayerPrefs.Save();DemoController.UseValidationStorage=false;}
        [UnityTest] public IEnumerator CompleteHarborRun()
        {
            DemoController.UseValidationStorage=true;
            yield return SceneManager.LoadSceneAsync("Official_Game");yield return null;
            var controller=Object.FindFirstObjectByType<DemoController>();Assert.That(controller,Is.Not.Null);controller.BeginTestRun();yield return null;
            Capture(controller,"port-tier1-720",1280,720);
            var game=controller.Session;Assert.That(Object.FindObjectsByType<HarborSelectable>(FindObjectsSortMode.None).Length,Is.EqualTo(6));
            Assert.That(game.Accept("grain_beacon").success,Is.True);Assert.That(game.Dispatch("beacon_coastal").success,Is.True);game.Tick(35);yield return null;
            Assert.That(game.State.credits,Is.EqualTo(450));game.AcknowledgeDelivery();game.Upgrade();yield return null;
            Assert.That(game.State.tier,Is.EqualTo(2));Assert.That(Object.FindObjectsByType<HarborSelectable>(FindObjectsSortMode.None).Length,Is.EqualTo(6));
            game.Accept("textiles_copper");game.Dispatch("copper_sheltered");game.Tick(100);yield return null;
            Capture(controller,"event-720",1280,720);
            Assert.That(controller.BlockWorld,Is.True);game.Resolve("congestion","wait");game.Tick(100);yield return null;
            game.Resolve("inspection","clean");game.Tick(100);yield return null;game.AcknowledgeDelivery();game.Review();yield return null;
            Assert.That(game.State.credits,Is.EqualTo(470));Assert.That(game.State.phase,Is.EqualTo(RunPhase.Complete));
            Assert.That(controller.Harbor.View.orthographic,Is.True);LogAssert.NoUnexpectedReceived();
            Capture(controller,"ending-1080",1920,1080);
            for(int i=0;i<4;i++)
            {
                controller.BeginTestRun();yield return null;var next=controller.Session;
                Assert.That(next.State.credits,Is.EqualTo(200));Assert.That(next.State.decisions,Is.Empty);Assert.That(next.State.tier,Is.EqualTo(1));
                Assert.That(Object.FindObjectsByType<HarborSelectable>(FindObjectsSortMode.None).Length,Is.EqualTo(6));
            }
            LogAssert.NoUnexpectedReceived();
        }
        static void Capture(DemoController controller,string name,int width,int height)
        {
            if(SystemInfo.graphicsDeviceType==UnityEngine.Rendering.GraphicsDeviceType.Null)return;
            Directory.CreateDirectory("Logs/GameplayPreviews");
            var camera=controller.Harbor.View;var canvas=Object.FindFirstObjectByType<Canvas>();
            canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;
            var target=new RenderTexture(width,height,24);camera.targetTexture=target;Canvas.ForceUpdateCanvases();camera.Render();
            RenderTexture.active=target;var image=new Texture2D(width,height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,width,height),0,0);image.Apply();
            File.WriteAllBytes("Logs/GameplayPreviews/"+name+".png",image.EncodeToPNG());
            RenderTexture.active=null;camera.targetTexture=null;canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            Object.Destroy(image);target.Release();Object.Destroy(target);
        }
        [UnityTest] public IEnumerator ShipLeavesViewAndReturnsOnlyForArrival()
        {
            DemoController.UseValidationStorage=true;yield return SceneManager.LoadSceneAsync("Official_Game");yield return null;
            var controller=Object.FindFirstObjectByType<DemoController>();controller.BeginTestRun();yield return null;
            var game=controller.Session;var ship=Object.FindObjectsByType<HarborSelectable>(FindObjectsSortMode.None).Single(x=>x.label=="Coastal freighter");
            game.Accept("grain_beacon");game.Dispatch("beacon_coastal");yield return null;
            Assert.That(ship.GetComponent<BoxCollider>().enabled,Is.False);game.Tick(5.8f);yield return null;
            var bounds=ship.GetComponent<BoxCollider>();var geometry=new Bounds(ship.transform.TransformPoint(bounds.center),bounds.size);
            Assert.That(GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(controller.Harbor.View),geometry),Is.False,"Entire hull must sail beyond the camera edge before going offshore.");
            game.Tick(1);yield return null;Assert.That(controller.Harbor.ShipVisible,Is.False);
            controller.Harbor.View.orthographicSize=22;yield return null;Assert.That(controller.Harbor.ShipVisible,Is.False,"Zooming out must not reveal the offshore ship.");
            game.Tick(game.Duration-game.State.voyage.elapsed-5);yield return null;Assert.That(controller.Harbor.ShipVisible,Is.True);Assert.That(ship.GetComponent<BoxCollider>().enabled,Is.False);
            float before=Vector3.Distance(ship.transform.position,new Vector3(0,.1f,-9));game.Tick(3);yield return null;
            Assert.That(Vector3.Distance(ship.transform.position,new Vector3(0,.1f,-9)),Is.LessThan(before));Capture(controller,"ship-return-720",1280,720);
            game.Tick(3);yield return null;Assert.That(game.State.phase,Is.EqualTo(RunPhase.Delivery));Assert.That(ship.GetComponent<BoxCollider>().enabled,Is.True);Assert.That(ship.transform.position.x,Is.EqualTo(0).Within(.01f));
        }
        [UnityTest] public IEnumerator FloatingUiHidesDetailsAndUsesSharedBranding()
        {
            PlayerPrefs.DeleteKey("OpenWaters.Settings");DemoController.UseValidationStorage=true;yield return SceneManager.LoadSceneAsync("Official_Game");yield return null;
            var controller=Object.FindFirstObjectByType<DemoController>();controller.BeginTestRun();yield return null;
            Assert.That(controller.Settings.tutorial,Is.False);Assert.That(GameObject.Find("Optional tip"),Is.Null);
            Assert.That(Resources.Load<Sprite>("Branding/Logo"),Is.Not.Null);Assert.That(Resources.Load<Sprite>("UI/Icons/warehouse"),Is.Not.Null);
            Assert.That(Object.FindObjectsByType<Text>(FindObjectsSortMode.None).Any(x=>x.text.Contains("Requires the Tier 2")),Is.False,"Locked explanations should not fill the initial interface.");
            Capture(controller,"floating-port-720",1280,720);Capture(controller,"floating-port-4x3",960,720);Capture(controller,"floating-port-wide",1680,720);
            Click("Tips");yield return null;Assert.That(GameObject.Find("Captain's tips"),Is.Not.Null);Click("Back");yield return null;
            controller.Session.Accept("grain_beacon");controller.Session.Dispatch("beacon_coastal");controller.Session.Tick(35);yield return null;
            Assert.That(Object.FindObjectsByType<Text>(FindObjectsSortMode.None).Any(x=>x.text.Contains("Operating −")),Is.False);
            Click("View ledger");yield return null;Assert.That(Object.FindObjectsByType<Text>(FindObjectsSortMode.None).Any(x=>x.text.Contains("Operating −")),Is.True);
            LogAssert.NoUnexpectedReceived();
        }
        static void Click(string name)
        {
            var button=Object.FindObjectsByType<Button>(FindObjectsSortMode.None).Single(x=>x.name==name && x.IsInteractable());
            button.onClick.Invoke();
        }
        [UnityTest] public IEnumerator UiCommandsPauseAndViolationEndings()
        {
            DemoController.UseValidationStorage=true;
            yield return SceneManager.LoadSceneAsync("Official_Game");yield return null;
            var controller=Object.FindFirstObjectByType<DemoController>();controller.BeginTestRun();yield return null;
            Click("Accept Beacon Bay");yield return null;Click("Select: Coastal passage");yield return null;Click("Dispatch • Pay 50");yield return null;
            var game=controller.Session;Assert.That(game.State.credits,Is.EqualTo(150));game.Tick(35);yield return null;
            Click("Continue to port upgrade");yield return null;Click("Preview upgrade");yield return null;Click("Buy upgrade • 300 credits");yield return null;
            Assert.That(game.State.tier,Is.EqualTo(2));Click("Accept Copperport");yield return null;Click("Select: Sheltered passage");yield return null;
            Capture(controller,"route-map-720",1280,720);Click("Dispatch • Pay 80");game.Tick(100);yield return null;
            Assert.That(EventSystem.current.currentSelectedGameObject.name,Is.EqualTo("Wait"));Assert.That(controller.BlockWorld,Is.True);
            Assert.That(Object.FindObjectsByType<Button>(FindObjectsSortMode.None).Single(x=>x.name=="Tips").IsInteractable(),Is.False,"Mandatory decision must keep keyboard focus within its own controls.");
            string selected=controller.Harbor.Selected;controller.Harbor.SelectAt(new Vector2(800,400));Assert.That(controller.Harbor.Selected,Is.EqualTo(selected));
            controller.Escape();yield return null;Assert.That(controller.Paused,Is.True);float before=game.State.voyage.elapsed;
            game.Tick(100,controller.Paused);Assert.That(game.State.voyage.elapsed,Is.EqualTo(before));Click("Resume");yield return null;
            Assert.That(game.State.voyage.pendingEventId,Is.EqualTo("congestion"));Click("Wait");game.Tick(100);yield return null;
            Capture(controller,"inspection-720",1280,720);Click("Pay unofficial fee");game.Tick(100);yield return null;
            Click("Continue to scheduled GSA review");yield return null;Click("Apply review and finish");yield return null;
            Assert.That(game.State.credits,Is.EqualTo(490));Assert.That(game.State.standing,Is.EqualTo("Watch"));Capture(controller,"ending-watch-720",1280,720);
            Click("Inspect Port");yield return null;Assert.That(game.State.endingAcknowledged,Is.True);Assert.That(controller.BlockWorld,Is.False);
            controller.BeginTestRun();yield return null;game=controller.Session;
            game.Accept("grain_beacon");game.Dispatch("beacon_coastal");game.Tick(35);game.AcknowledgeDelivery();game.Upgrade();game.Accept("textiles_copper");game.Dispatch("copper_sheltered");game.Tick(100);game.Resolve("congestion","wait");game.Tick(100);game.Resolve("inspection","risk");game.Tick(100);game.AcknowledgeDelivery();game.Review();yield return null;
            Assert.That(game.State.credits,Is.EqualTo(490));Assert.That(game.State.reputation,Is.EqualTo(30));Assert.That(game.State.standing,Is.EqualTo("Restricted"));Capture(controller,"ending-restricted-720",1280,720);
            LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator PointerSelectionAndUiInputIsolation()
        {
            DemoController.UseValidationStorage=true;
            yield return SceneManager.LoadSceneAsync("Official_Game");yield return null;
            var controller=Object.FindFirstObjectByType<DemoController>();controller.BeginTestRun();yield return null;
#if UNITY_EDITOR
            var oldRouting=InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
#endif
            var oldBackground=InputSystem.settings.backgroundBehavior;
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            var nativeMice=InputSystem.devices.OfType<Mouse>().Where(x=>x.enabled).ToArray();foreach(var device in nativeMice)InputSystem.DisableDevice(device);
            var mouse=InputSystem.AddDevice<Mouse>();
            try
            {
                var warehouse=Object.FindObjectsByType<HarborSelectable>(FindObjectsSortMode.None).Single(x=>x.label=="Warehouse");
                Vector2 worldPointer=controller.Harbor.View.WorldToScreenPoint(warehouse.GetComponent<Collider>().bounds.center);
                Assert.That(controller.BlockWorld,Is.False,"Port must accept world input.");
                Assert.That(Physics.Raycast(controller.Harbor.View.ScreenPointToRay(worldPointer),out var worldHit,200),Is.True,"Warehouse ray must hit.");
                Assert.That(worldHit.collider.GetComponent<HarborSelectable>()?.label,Is.EqualTo("Warehouse"),"Warehouse must be the closest selectable collider.");
                InputSystem.QueueStateEvent(mouse,new MouseState{position=worldPointer});yield return null;yield return null;
                var cameraControl=controller.Harbor.View.GetComponent<HarborCamera>();
                var map=(InputActionMap)typeof(HarborCamera).GetField("actions",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(cameraControl);
                Assert.That(map.enabled,Is.True);Assert.That(map["Pointer"].ReadValue<Vector2>(),Is.EqualTo(worldPointer),"Gameplay action must read synthetic pointer.");
                Assert.That(EventSystem.current.IsPointerOverGameObject(),Is.False,"Warehouse pointer must be outside UI.");
                InputSystem.QueueStateEvent(mouse,new MouseState{position=worldPointer}.WithButton(MouseButton.Left));yield return null;yield return null;
                Assert.That(map["Select"].IsPressed(),Is.True,"Select action must read synthetic click.");
                InputSystem.QueueStateEvent(mouse,new MouseState{position=worldPointer});yield return null;
                Assert.That(controller.Harbor.Selected,Is.EqualTo("Warehouse"));
                Capture(controller,"floating-selected-720",1280,720);
                controller.Escape();yield return null;
                var accept=Object.FindObjectsByType<Button>(FindObjectsSortMode.None).Single(x=>x.name=="Accept Beacon Bay");
                Canvas.ForceUpdateCanvases();Vector2 uiPointer=RectTransformUtility.WorldToScreenPoint(null,accept.transform.position);
                InputSystem.QueueStateEvent(mouse,new MouseState{position=uiPointer});yield return null;yield return null;
                InputSystem.QueueStateEvent(mouse,new MouseState{position=uiPointer}.WithButton(MouseButton.Left));yield return null;
                InputSystem.QueueStateEvent(mouse,new MouseState{position=uiPointer});yield return null;
                Assert.That(controller.Session.State.phase,Is.EqualTo(RunPhase.Preparing));
                Assert.That(controller.Harbor.Selected,Is.EqualTo("Warehouse"),"UI click must not select the dock behind it.");
            }
            finally
            {
                InputSystem.RemoveDevice(mouse);foreach(var device in nativeMice)InputSystem.EnableDevice(device);
                InputSystem.settings.backgroundBehavior=oldBackground;
#if UNITY_EDITOR
                InputSystem.settings.editorInputBehaviorInPlayMode=oldRouting;
#endif
            }
            LogAssert.NoUnexpectedReceived();
        }
    }
}
