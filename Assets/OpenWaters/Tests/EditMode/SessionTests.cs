using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace OpenWaters.Tests
{
    public sealed class SessionTests
    {
        DemoDefinitions data;
        GameSession game;
        [SetUp] public void Setup(){data=ScriptableObject.CreateInstance<DemoDefinitions>();data.AuthorDefaults();game=new GameSession(data);}
        [TearDown] public void Teardown(){UnityEngine.Object.DestroyImmediate(data);}
        void Ok(CommandReceipt r){Assert.That(r.success,Is.True,r.message);}
        void First()
        {
            Ok(game.Accept("grain_beacon"));Ok(game.Dispatch("beacon_coastal"));Assert.That(game.State.credits,Is.EqualTo(150));
            game.Tick(35);Assert.That(game.State.credits,Is.EqualTo(450));Assert.That(game.State.reputation,Is.EqualTo(55));
            Ok(game.AcknowledgeDelivery());
        }
        void Upgraded(){First();Ok(game.Upgrade());Assert.That(game.State.credits,Is.EqualTo(150));Assert.That(game.Capacity,Is.EqualTo(20));}
        void Restore(){game=new GameSession(data,JsonUtility.FromJson<RunState>(JsonUtility.ToJson(game.State)));}
        [Test] public void FirstVoyageAndUpgradeAreAtomic()
        {
            Assert.That(game.Accept("parts_meridian").success,Is.False);Assert.That(game.Upgrade().success,Is.False);
            Ok(game.Accept("grain_beacon"));Assert.That(game.Dispatch("copper_exposed").success,Is.False);Assert.That(game.State.credits,Is.EqualTo(200));
            Ok(game.Cancel());Assert.That(game.State.credits,Is.EqualTo(200));First();
            Assert.That(game.Settle().success,Is.False);Ok(game.Upgrade());Restore();Assert.That(game.Upgrade().success,Is.False);Assert.That(game.State.credits,Is.EqualTo(150));Assert.That(game.State.ledger.Count,Is.EqualTo(3));
        }
        public static IEnumerable Branches()
        {
            foreach(var r in new[]{"copper_sheltered","copper_exposed","meridian_patrolled","meridian_shortcut"})
            foreach(int choice in new[]{0,1})foreach(var inspection in new[]{"clean","fee","risk"})yield return new TestCaseData(r,choice,inspection);
        }
        [TestCaseSource(nameof(Branches))] public void All24BranchesCompleteAndSurviveCheckpoints(string routeId,int operational,string inspection)
        {
            Upgraded();var r=data.Route(routeId);var c=data.Contract(r.contractId);Ok(game.Accept(c.id));Restore();Ok(game.Dispatch(r.id));Restore();
            Assert.That(game.Dispatch(r.id).success,Is.False);game.Tick(1000);Assert.That(game.State.phase,Is.EqualTo(RunPhase.EventPending));
            float elapsed=game.State.voyage.elapsed;game.Tick(100);Assert.That(game.State.voyage.elapsed,Is.EqualTo(elapsed));Restore();
            var e=data.Event(r.eventId);var choice=e.choices[operational];Ok(game.Resolve(e.id,choice.id));Restore();Assert.That(game.Resolve(e.id,choice.id).success,Is.False);
            game.Tick(1000);Assert.That(game.State.voyage.pendingEventId,Is.EqualTo("inspection"));Restore();Ok(game.Resolve("inspection",inspection));Restore();
            var v=game.State.voyage;int net=c.reward+v.bonus-v.fees;int before=game.State.credits;game.Tick(1000);Assert.That(game.State.credits,Is.EqualTo(before+net));Restore();
            Assert.That(game.Settle().success,Is.False);Ok(game.AcknowledgeDelivery());Restore();Ok(game.Review());Restore();
            int expected=before+net+(inspection=="clean"?40:inspection=="fee"?-60:-180);
            Assert.That(game.State.credits,Is.EqualTo(expected));Assert.That(game.State.credits,Is.GreaterThanOrEqualTo(0));
            Assert.That(game.State.standing,Is.EqualTo(inspection=="clean"?"Clear":inspection=="fee"?"Watch":"Restricted"));
            Assert.That(game.State.reputation,Is.EqualTo(inspection=="clean"?65:inspection=="fee"?45:30));
            Assert.That(game.State.suspicion,Is.EqualTo(inspection=="clean"?0:inspection=="fee"?25:50));
            Assert.That(game.Review().success,Is.False);Assert.That(game.Accept(c.id).success,Is.False);Assert.That(game.Upgrade().success,Is.False);
            Ok(game.AcknowledgeEnding());Restore();Assert.That(game.State.endingAcknowledged,Is.True);
            game.Tick(1000);Assert.That(game.State.condition,Is.EqualTo(100));Assert.That(game.State.serviceRemaining,Is.Zero);
        }
        [TestCase("clean",470)] [TestCase("fee",490)] [TestCase("risk",490)]
        public void WorkedExamplesMatch(string choice,int expected)
        {
            Upgraded();Ok(game.Accept("textiles_copper"));Ok(game.Dispatch("copper_sheltered"));game.Tick(100);Ok(game.Resolve("congestion","wait"));game.Tick(100);Ok(game.Resolve("inspection",choice));game.Tick(100);Ok(game.AcknowledgeDelivery());Ok(game.Review());Assert.That(game.State.credits,Is.EqualTo(expected));
        }
        [Test] public void PausedTravelDoesNotAdvance(){Ok(game.Accept("grain_beacon"));Ok(game.Dispatch("beacon_coastal"));game.Tick(100,true);Assert.That(game.State.voyage.elapsed,Is.Zero);Assert.That(game.State.credits,Is.EqualTo(150));}
        [Test] public void FreshAndPreparingSavesNormalizeEmptyVoyage(){Restore();Assert.That(game.State.voyage,Is.Null);Ok(game.Accept("grain_beacon"));Restore();Assert.That(game.State.phase,Is.EqualTo(RunPhase.Preparing));Ok(game.Dispatch("beacon_coastal"));}
        [Test] public void InvalidTierAndCorruptLedgerRejected()
        {
            game.State.tier=3;Assert.Throws<InvalidOperationException>(()=>GameSession.ValidateState(game.State,data));game.State.tier=1;
            game.State.credits++;Assert.Throws<InvalidOperationException>(()=>GameSession.ValidateState(game.State,data));
        }
        [Test] public void OverdrawAndWrongCommandsLeaveStateUntouched()
        {
            Ok(game.Accept("grain_beacon"));game.State.credits=49;Assert.That(game.Dispatch("beacon_coastal").success,Is.False);Assert.That(game.State.credits,Is.EqualTo(49));Assert.That(game.State.ledger,Is.Empty);
            Assert.That(game.Review().success,Is.False);Assert.That(game.Resolve("storm","push").success,Is.False);
        }
        sealed class MemoryStorage : IBackupPreservingRunStorage
        {
            public string primary,backup;
            public bool failWrite;
            public void Write(string json)=>Write(json,false);
            public void Write(string json,bool preserveBackup){if(failWrite)throw new System.IO.IOException("Simulated unavailable storage.");if(!preserveBackup)backup=primary;primary=json;}
            public string Read(bool b)=>b?backup:primary;
            public bool Exists(bool b)=>(b?backup:primary)!=null;
        }
        [Test] public void CorruptPrimaryRecoversBackupAndPreservesInvalidData()
        {
            var storage=new MemoryStorage();var service=new SaveService(storage,data);Assert.That(service.Save(game.State),Is.True);
            Ok(game.Accept("grain_beacon"));Assert.That(service.Save(game.State),Is.True);storage.primary="broken json";
            var recovered=service.Load();Assert.That(recovered,Is.Not.Null,service.Message+"\nBackup: "+storage.backup);Assert.That(recovered.phase,Is.EqualTo(RunPhase.Port));StringAssert.Contains("Recovered",service.Message);Assert.That(storage.primary,Is.EqualTo("broken json"));
            storage.backup="{}";Assert.That(service.Load(),Is.Null);StringAssert.Contains("No valid run",service.Message);
        }
        [Test] public void RecoverySavePreservesGoodBackupEvenAfterWriteFailure()
        {
            var storage=new MemoryStorage();var service=new SaveService(storage,data);Assert.That(service.Save(game.State),Is.True);
            string good=storage.primary;storage.backup=good;storage.primary="broken json";
            game=new GameSession(data,service.Load());Ok(game.Accept("grain_beacon"));storage.failWrite=true;
            Assert.That(service.Save(game.State),Is.False);Assert.That(service.Temporary,Is.True);Assert.That(storage.backup,Is.EqualTo(good));
            storage.failWrite=false;Assert.That(service.Save(game.State),Is.True);Assert.That(service.Temporary,Is.False);Assert.That(storage.backup,Is.EqualTo(good));
            storage.primary="broken again";Assert.That(service.Load().phase,Is.EqualTo(RunPhase.Port));
        }
        [Test] public void FileRecoverySavePreservesGoodBackup()
        {
            string directory=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"OpenWatersTest-"+Guid.NewGuid().ToString("N"));
            string path=System.IO.Path.Combine(directory,"run.json");
            try
            {
                var storage=new FileRunStorage(path);var service=new SaveService(storage,data);
                Assert.That(service.Save(game.State),Is.True);string good=storage.Read(false);
                Ok(game.Accept("grain_beacon"));Assert.That(service.Save(game.State),Is.True);
                System.IO.File.WriteAllText(path,"broken json");game=new GameSession(data,service.Load());Ok(game.Accept("grain_beacon"));
                Assert.That(service.Save(game.State),Is.True);Assert.That(storage.Read(true),Is.EqualTo(good));
                Assert.That(service.Load().phase,Is.EqualTo(RunPhase.Preparing));
            }
            finally{if(System.IO.Directory.Exists(directory))System.IO.Directory.Delete(directory,true);}
        }
        [TestCase("unknown_transaction")] [TestCase("forged_dispatch")] [TestCase("foreign_voyage")]
        [TestCase("wrong_contract")] [TestCase("missing_contract")] [TestCase("pending_without_modal")]
        public void ActiveVoyageCorruptionIsRejected(string corruption)
        {
            Ok(game.Accept("grain_beacon"));Ok(game.Dispatch("beacon_coastal"));var s=game.State;
            switch(corruption)
            {
                case "unknown_transaction":s.ledger[0].id="unknown";break;
                case "forged_dispatch":s.ledger[0].amount=-1;s.credits=data.startingCredits-1;break;
                case "foreign_voyage":s.voyage.id="another-run/voyage/1";break;
                case "wrong_contract":s.acceptedContractId="textiles_copper";break;
                case "missing_contract":s.acceptedContractId=null;break;
                case "pending_without_modal":s.voyage.pendingEventId="inspection";break;
            }
            Assert.Throws<InvalidOperationException>(()=>GameSession.ValidateState(s,data));
        }
        [TestCase("missing_decision")] [TestCase("forged_receipt")] [TestCase("repeat_inspection")]
        [TestCase("missing_decisions_at_delivery")] [TestCase("early_event")] [TestCase("foreign_completed_id")]
        public void UpgradedDecisionAndCompletionCorruptionIsRejected(string corruption)
        {
            Upgraded();Ok(game.Accept("textiles_copper"));Ok(game.Dispatch("copper_sheltered"));game.Tick(1000);
            if(corruption=="early_event")game.State.voyage.elapsed=0;
            else if(corruption=="foreign_completed_id")game.State.completedVoyages[0]="foreign";
            else
            {
                Ok(game.Resolve("congestion","wait"));game.Tick(1000);Ok(game.Resolve("inspection","clean"));var v=game.State.voyage;
                switch(corruption)
                {
                    case "missing_decision":game.State.decisions.RemoveAt(1);break;
                    case "forged_receipt":v.choices[0].travelDelay=1;v.travelDelay=1;break;
                    case "repeat_inspection":game.State.phase=RunPhase.EventPending;v.pendingEventId="inspection";v.elapsed=data.Route(v.routeId).seconds*.70f;break;
                    case "missing_decisions_at_delivery":game.Tick(1000);v.choices.Clear();game.State.decisions.Clear();v.eventIndex=0;v.travelDelay=0;v.elapsed=data.Route(v.routeId).seconds;break;
                }
            }
            Assert.Throws<InvalidOperationException>(()=>GameSession.ValidateState(game.State,data));
        }
        [Test] public void CancelAfterUpgradeKeepsValidFirstVoyageHistory()
        {
            Upgraded();Restore();Ok(game.Accept("parts_meridian"));Restore();Ok(game.Cancel());Restore();
            Assert.That(game.State.deliveries,Is.EqualTo(1));Assert.That(game.State.credits,Is.EqualTo(150));
        }
        [Test] public void SettingsVolumesAreClampedAndNonFiniteValuesUseDefaults()
        {
            const string key="OpenWaters.Settings";bool had=PlayerPrefs.HasKey(key);string original=PlayerPrefs.GetString(key);
            try
            {
                PlayerPrefs.SetString(key,"{\"master\":2,\"ambience\":-1,\"sfx\":0.4,\"reducedMotion\":true}");
                var settings=DemoSettings.Load();Assert.That(settings.master,Is.EqualTo(1));Assert.That(settings.ambience,Is.Zero);Assert.That(settings.sfx,Is.EqualTo(.4f));Assert.That(settings.reducedMotion,Is.True);
                settings.master=float.NaN;settings.ambience=float.PositiveInfinity;settings.sfx=float.NegativeInfinity;settings.Save();settings=DemoSettings.Load();
                Assert.That(settings.master,Is.EqualTo(.65f));Assert.That(settings.ambience,Is.EqualTo(.3f));Assert.That(settings.sfx,Is.EqualTo(.7f));
                PlayerPrefs.SetString(key,"broken json");Assert.That(DemoSettings.Load().master,Is.EqualTo(.65f));
            }
            finally{if(had)PlayerPrefs.SetString(key,original);else PlayerPrefs.DeleteKey(key);PlayerPrefs.Save();}
        }
        [Test] public void NewPreferencesDisableTipsAndDefaultToFullscreen()
        {
            const string key="OpenWaters.Settings";bool had=PlayerPrefs.HasKey(key);string original=PlayerPrefs.GetString(key);
            try
            {
                PlayerPrefs.DeleteKey(key);var settings=DemoSettings.Load();
                Assert.That(settings.tutorial,Is.False);Assert.That(settings.displayMode,Is.EqualTo(DisplayMode.Fullscreen));
                // Legacy true was the previous default and must migrate to optional tips.
                PlayerPrefs.SetString(key,"{\"tutorial\":true,\"master\":0.5}");settings=DemoSettings.Load();
                Assert.That(settings.tutorial,Is.False);Assert.That(settings.displayMode,Is.EqualTo(DisplayMode.Fullscreen));
                Assert.That(settings.master,Is.EqualTo(.5f));
            }
            finally{if(had)PlayerPrefs.SetString(key,original);else PlayerPrefs.DeleteKey(key);PlayerPrefs.Save();}
        }
        [Test] public void DisplayPreferenceRoundTripsAndRejectsUnsupportedModeValues()
        {
            const string key="OpenWaters.Settings";bool had=PlayerPrefs.HasKey(key);string original=PlayerPrefs.GetString(key);
            try
            {
                var settings=new DemoSettings{tutorial=true};
                // Editor application is intentionally a no-op while preferences are persisted.
                Assert.That(DisplaySettingsController.SetMode(settings,DisplayMode.Borderless),Is.False);
                settings=DemoSettings.Load();Assert.That(settings.displayMode,Is.EqualTo(DisplayMode.Borderless));Assert.That(settings.tutorial,Is.True);
                DisplaySettingsController.SetMode(settings,DisplayMode.Windowed);Assert.That(DemoSettings.Load().displayMode,Is.EqualTo(DisplayMode.Windowed));
                PlayerPrefs.SetString(key,"{\"uiPreferenceVersion\":1,\"displayMode\":999,\"tutorial\":true}");settings=DemoSettings.Load();
                Assert.That(settings.displayMode,Is.EqualTo(DisplayMode.Fullscreen));Assert.That(settings.tutorial,Is.True);
                settings.displayMode=(DisplayMode)(-1);settings.Save();Assert.That(DemoSettings.Load().displayMode,Is.EqualTo(DisplayMode.Fullscreen));
            }
            finally{if(had)PlayerPrefs.SetString(key,original);else PlayerPrefs.DeleteKey(key);PlayerPrefs.Save();}
        }
        [Test] public void LegacyTipDefaultMigratesOnceAndExplicitOptInSurvivesReload()
        {
            const string key="OpenWaters.Settings";bool had=PlayerPrefs.HasKey(key);string original=PlayerPrefs.GetString(key);
            try
            {
                PlayerPrefs.SetString(key,"{\"tutorial\":true,\"displayMode\":1,\"master\":0.25,\"reducedMotion\":true}");
                var settings=DemoSettings.Load();Assert.That(settings.tutorial,Is.False);Assert.That(settings.uiPreferenceVersion,Is.EqualTo(1));
                Assert.That(settings.displayMode,Is.EqualTo(DisplayMode.Borderless));Assert.That(settings.master,Is.EqualTo(.25f));Assert.That(settings.reducedMotion,Is.True);
                Assert.That(JsonUtility.FromJson<DemoSettings>(PlayerPrefs.GetString(key)).uiPreferenceVersion,Is.EqualTo(1),"Migration marker must be persisted.");
                settings.tutorial=true;settings.Save();settings=DemoSettings.Load();Assert.That(settings.tutorial,Is.True);
                Assert.That(settings.displayMode,Is.EqualTo(DisplayMode.Borderless));
                settings.tutorial=false;settings.Save();Assert.That(DemoSettings.Load().tutorial,Is.False);
            }
            finally{if(had)PlayerPrefs.SetString(key,original);else PlayerPrefs.DeleteKey(key);PlayerPrefs.Save();}
        }
        [Test] public void ContentCountsAndTierRepairComparison()
        {
            data.Validate();Assert.That(data.contracts.Length,Is.EqualTo(3));Assert.That(data.events.Sum(x=>x.choices.Length),Is.EqualTo(11));
            Assert.That(data.Tier(1).repairFee,Is.EqualTo(40));Assert.That(data.Tier(2).repairFee,Is.EqualTo(20));
            data.tiers[1].tier=3;Assert.Throws<InvalidOperationException>(()=>data.Validate());
        }
    }
}
