using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenWaters
{
    public enum RunPhase { Port, Preparing, Traveling, EventPending, Delivery, Review, Complete }
    [Serializable] public sealed class LedgerEntry
    {
        public string id, description;
        public int amount;
    }
    [Serializable] public sealed class ChoiceReceipt
    {
        public string eventId, choiceId, description;
        public int fee, bonus, severity;
        public float travelDelay, dockDelay;
    }
    [Serializable] public sealed class VoyageState
    {
        public string id, contractId, routeId, pendingEventId;
        public float elapsed, travelDelay, dockDelay;
        public int eventIndex, fees, bonus;
        public List<ChoiceReceipt> choices=new List<ChoiceReceipt>();
    }
    [Serializable] public sealed class RunState
    {
        public int schemaVersion=1;
        public string runId;
        public RunPhase phase;
        public int tier=1, credits, reputation, suspicion, condition=100, deliveries;
        public string standing="Clear", acceptedContractId;
        public float serviceRemaining;
        public bool upgraded, reviewed, endingAcknowledged;
        public VoyageState voyage;
        public List<string> completedVoyages=new List<string>();
        public List<LedgerEntry> ledger=new List<LedgerEntry>();
        public List<ChoiceReceipt> decisions=new List<ChoiceReceipt>();
    }
    public readonly struct CommandReceipt
    {
        public readonly bool success;
        public readonly string message;
        public CommandReceipt(bool success,string message){this.success=success;this.message=message;}
    }
    // All financial and progression mutations live here. Presentation issues commands only.
    public sealed class GameSession
    {
        public DemoDefinitions Data {get;}
        public RunState State {get;private set;}
        public event Action Changed;
        public GameSession(DemoDefinitions data,RunState restore=null)
        {
            Data=data;data.Validate();
            State=restore??new RunState{runId=Guid.NewGuid().ToString("N"),credits=data.startingCredits,reputation=data.startingReputation};
            ValidateState(State,data);
        }
        public int Capacity => Math.Min(Data.shipCapacity,Data.Tier(State.tier).capacity);
        public bool AtSea => State.phase==RunPhase.Traveling || State.phase==RunPhase.EventPending;
        public float Duration => State.voyage==null?0:Data.Route(State.voyage.routeId).seconds+State.voyage.travelDelay;
        public string Eligibility(string contractId)
        {
            var c=Data.Contract(contractId);
            if(c==null)return "Unknown contract.";
            if(State.deliveries>=2)return "Demo complete: two voyages only.";
            if(State.phase!=RunPhase.Port)return "Finish the current action first.";
            if(State.serviceRemaining>0)return "Ship is receiving dock service.";
            if(State.tier<c.tier)return "Requires the Tier 2 commercial upgrade.";
            if(State.deliveries==0 && c.tier!=1)return "Complete the grain delivery first.";
            if(State.deliveries==1 && c.tier==1)return "Beacon Bay contract already completed.";
            if(State.deliveries==1 && !State.upgraded)return "Upgrade the port before the second shipment.";
            if(c.units>Capacity)return "Cargo exceeds ship / warehouse capacity.";
            if(c.range>Data.Tier(State.tier).range)return "Destination exceeds fuel range.";
            return "";
        }
        CommandReceipt Reject(string text)=>new CommandReceipt(false,text);
        CommandReceipt Commit(string text){Changed?.Invoke();return new CommandReceipt(true,text);}
        bool Transact(string id,int amount,string description)
        {
            if(State.ledger.Any(x=>x.id==id))return false;
            if(State.credits+amount<0)throw new InvalidOperationException("Transaction would overdraw credits.");
            State.credits+=amount;State.ledger.Add(new LedgerEntry{id=id,amount=amount,description=description});return true;
        }
        public CommandReceipt Accept(string id)
        {
            string reason=Eligibility(id);if(reason!="")return Reject(reason);
            State.acceptedContractId=id;State.phase=RunPhase.Preparing;return Commit("Cargo reserved; choose a route.");
        }
        public CommandReceipt Cancel()
        {
            if(State.phase!=RunPhase.Preparing)return Reject("Only a prepared contract can be cancelled.");
            State.acceptedContractId=null;State.phase=RunPhase.Port;return Commit("Contract cancelled without charge.");
        }
        public CommandReceipt Dispatch(string routeId)
        {
            if(State.phase!=RunPhase.Preparing)return Reject("Prepare a contract before dispatch.");
            var c=Data.Contract(State.acceptedContractId);var r=Data.Route(routeId);
            if(r==null || r.contractId!=c.id)return Reject("Route does not serve the accepted contract.");
            if(State.tier<c.tier || Capacity<c.units || Data.Tier(State.tier).range<c.range || State.condition<=0 || State.serviceRemaining>0)return Reject("Ship, capacity or range requirements are unmet.");
            if(State.credits<r.cost)return Reject("Insufficient credits for operating charge.");
            string id=State.runId+"/voyage/"+(State.deliveries+1);
            Transact(id+"/dispatch",-r.cost,"Operating charge: "+r.title);
            State.voyage=new VoyageState{id=id,contractId=c.id,routeId=r.id};State.phase=RunPhase.Traveling;
            return Commit("Dispatched. Operating charge paid once.");
        }
        public void Tick(float seconds,bool paused=false)
        {
            if(paused || seconds<=0 || float.IsNaN(seconds) || float.IsInfinity(seconds))return;
            if(State.serviceRemaining>0)
            {
                State.serviceRemaining=Math.Max(0,State.serviceRemaining-seconds);
                if(State.serviceRemaining==0){State.condition=100;Changed?.Invoke();}
            }
            if(State.phase!=RunPhase.Traveling)return;
            var v=State.voyage;var r=Data.Route(v.routeId);v.elapsed+=seconds;
            if(!string.IsNullOrEmpty(r.eventId) && v.eventIndex<2)
            {
                // Delays do not move trigger points. Oversized test ticks stop at the next decision.
                float trigger=r.seconds*(v.eventIndex==0?.35f:.70f);
                if(v.elapsed>=trigger)
                {
                    v.elapsed=trigger;v.pendingEventId=v.eventIndex==0?r.eventId:"inspection";State.phase=RunPhase.EventPending;Changed?.Invoke();return;
                }
            }
            if(v.elapsed>=Duration){v.elapsed=Duration;Settle();}
        }
        public CommandReceipt Resolve(string eventId,string choiceId)
        {
            if(State.phase!=RunPhase.EventPending || State.voyage.pendingEventId!=eventId)return Reject("This decision is no longer pending.");
            var e=Data.Event(eventId);var c=e?.choices.SingleOrDefault(x=>x.id==choiceId);
            if(c==null)return Reject("Unknown choice.");
            var v=State.voyage;if(v.choices.Any(x=>x.eventId==eventId))return Reject("Decision already resolved.");
            var tier=Data.Tier(State.tier);
            var receipt=new ChoiceReceipt{eventId=eventId,choiceId=choiceId,description=e.title+": "+c.title,fee=c.useRepairModifiers?tier.repairFee:c.fee,bonus=c.bonus,severity=c.severity,travelDelay=c.useRepairModifiers?tier.repairDelay:c.travelDelay,dockDelay=c.dockDelay<0?tier.dockDelay:c.dockDelay};
            v.choices.Add(receipt);State.decisions.Add(receipt);v.fees+=receipt.fee;v.bonus+=receipt.bonus;v.travelDelay+=receipt.travelDelay;v.dockDelay+=receipt.dockDelay;
            State.condition=Math.Clamp(State.condition-c.damage,0,100);State.reputation=Math.Clamp(State.reputation+c.reputation,0,100);State.suspicion=Math.Clamp(State.suspicion+c.suspicion,0,100);
            v.eventIndex++;v.pendingEventId=null;State.phase=RunPhase.Traveling;return Commit(receipt.description+". Effects recorded for delivery.");
        }
        public CommandReceipt Settle()
        {
            if(State.phase!=RunPhase.Traveling || State.voyage.elapsed<Duration)return Reject("Voyage has not returned.");
            var v=State.voyage;var r=Data.Route(v.routeId);
            if(!string.IsNullOrEmpty(r.eventId) && v.eventIndex!=2)return Reject("Resolve required decisions first.");
            if(State.completedVoyages.Contains(v.id))return Reject("Voyage already settled.");
            var c=Data.Contract(v.contractId);
            // Commit one net transaction; line items are retained on the voyage receipts.
            Transact(v.id+"/settlement",c.reward+v.bonus-v.fees,"Delivery: "+c.destination+" (base "+c.reward+", bonus "+v.bonus+", fees -"+v.fees+")");
            State.completedVoyages.Add(v.id);State.deliveries++;State.reputation=Math.Clamp(State.reputation+Data.deliveryReputation,0,100);
            State.serviceRemaining=State.condition<100?Data.serviceSeconds+v.dockDelay:0;State.phase=RunPhase.Delivery;State.acceptedContractId=null;
            return Commit("Delivery settled. Credits "+State.credits+".");
        }
        public CommandReceipt AcknowledgeDelivery()
        {
            if(State.phase!=RunPhase.Delivery)return Reject("No delivery ledger to acknowledge.");
            State.phase=State.deliveries==2?RunPhase.Review:RunPhase.Port;return Commit("Ledger acknowledged.");
        }
        public CommandReceipt Upgrade()
        {
            if(State.tier!=1 || State.upgraded)return Reject("Maximum demo tier reached.");
            if(State.phase!=RunPhase.Port || State.deliveries!=1 || State.serviceRemaining>0)return Reject("Complete the first delivery and dock before upgrading.");
            if(State.credits<Data.upgradeCost)return Reject("Upgrade requires "+Data.upgradeCost+" credits.");
            Transact(State.runId+"/upgrade",-Data.upgradeCost,"Commercial port upgrade");State.tier=2;State.upgraded=true;
            return Commit("Tier 2 unlocked: 20 cargo units, range 2, improved repairs.");
        }
        public CommandReceipt Review()
        {
            if(State.phase!=RunPhase.Review || State.deliveries!=2 || State.reviewed)return Reject("Scheduled review is unavailable or already completed.");
            int severity=State.decisions.Count==0?0:State.decisions.Max(x=>x.severity);
            int money=severity==2?-Data.majorFine:severity==1?-Data.minorFine:Data.cleanBonus;
            int rep=severity==2?Data.majorReputation:severity==1?Data.minorReputation:Data.cleanReputation;
            if(State.credits+money<0)return Reject("Invalid balance for review; restore a valid save or start a new run.");
            State.standing=severity==2?"Restricted":severity==1?"Watch":"Clear";
            Transact(State.runId+"/review",money,"Scheduled GSA review: "+State.standing);State.reputation=Math.Clamp(State.reputation+rep,0,100);State.reviewed=true;State.phase=RunPhase.Complete;
            return Commit("GSA: "+State.standing+". Review applied once.");
        }
        public CommandReceipt AcknowledgeEnding()
        {
            if(State.phase!=RunPhase.Complete)return Reject("Finish the review first.");
            State.endingAcknowledged=true;return Commit("Ending acknowledged. Inspect your port or start a new run.");
        }
        public static void ValidateState(RunState s,DemoDefinitions d)
        {
            if(s==null || s.schemaVersion!=1 || string.IsNullOrEmpty(s.runId) || s.tier<1 || s.tier>2 || !Enum.IsDefined(typeof(RunPhase),s.phase) || s.credits<0 || s.reputation<0 || s.reputation>100 || s.suspicion<0 || s.suspicion>100 || s.condition<0 || s.condition>100 || s.deliveries<0 || s.deliveries>2 || s.upgraded!=(s.tier==2))throw new InvalidOperationException("Unsupported or invalid run state.");
            if(s.ledger==null || s.decisions==null || s.completedVoyages==null || s.ledger.Any(x=>x==null || string.IsNullOrEmpty(x.id)) || s.ledger.Select(x=>x.id).Distinct().Count()!=s.ledger.Count || s.completedVoyages.Count!=s.deliveries || s.completedVoyages.Distinct().Count()!=s.deliveries)throw new InvalidOperationException("Invalid transaction history.");
            // JsonUtility writes a null nested serializable class as an empty object.
            // Normalize only the genuinely empty pre-voyage placeholder; reject partial voyages.
            var empty=s.voyage;
            if(empty!=null && s.deliveries==0 && (s.phase==RunPhase.Port || s.phase==RunPhase.Preparing) && string.IsNullOrEmpty(empty.id) && string.IsNullOrEmpty(empty.contractId) && string.IsNullOrEmpty(empty.routeId) && string.IsNullOrEmpty(empty.pendingEventId) && empty.elapsed==0 && empty.travelDelay==0 && empty.dockDelay==0 && empty.eventIndex==0 && empty.fees==0 && empty.bonus==0 && empty.choices?.Count==0)s.voyage=null;
            if(s.credits!=(long)d.startingCredits+s.ledger.Sum(x=>(long)x.amount))throw new InvalidOperationException("Balance disagrees with ledger.");
            if(float.IsNaN(s.serviceRemaining) || float.IsInfinity(s.serviceRemaining) || s.serviceRemaining<0 || (s.tier==2 && s.deliveries==0) || (s.deliveries==2 && !s.upgraded) || s.reviewed!=(s.phase==RunPhase.Complete) || (s.endingAcknowledged && !s.reviewed) || !new[]{"Clear","Watch","Restricted"}.Contains(s.standing))throw new InvalidOperationException("Inconsistent progression.");
            if(s.phase==RunPhase.Port && s.deliveries==2)throw new InvalidOperationException("No third voyage is permitted.");
            if((s.phase==RunPhase.Delivery && s.deliveries==0) || (s.deliveries>0 && s.voyage==null))throw new InvalidOperationException("Missing delivery history.");
            if(s.phase!=RunPhase.Preparing && s.phase!=RunPhase.Traveling && s.phase!=RunPhase.EventPending && !string.IsNullOrEmpty(s.acceptedContractId))throw new InvalidOperationException("Unexpected accepted contract.");
            for(int i=0;i<s.completedVoyages.Count;i++)
                if(s.completedVoyages[i]!=s.runId+"/voyage/"+(i+1))throw new InvalidOperationException("Invalid completed voyage ID.");
            if(s.phase==RunPhase.Preparing)
            {
                var accepted=d.Contract(s.acceptedContractId);
                if(accepted==null || s.deliveries==2 || accepted.tier!=s.tier || accepted.units>d.Tier(s.tier).capacity || (s.deliveries==0)!=(accepted.tier==1) || (s.deliveries==1 && !s.upgraded))throw new InvalidOperationException("Invalid accepted contract.");
            }
            foreach(var decision in s.decisions)
            {
                var authored=decision==null?null:d.Event(decision.eventId)?.choices.SingleOrDefault(x=>x.id==decision.choiceId);
                if(authored==null)throw new InvalidOperationException("Invalid compliance record.");
                ValidateReceipt(decision,d,2);
            }
            if(s.decisions.Select(x=>x.eventId).Distinct().Count()!=s.decisions.Count || s.decisions.Count>2)throw new InvalidOperationException("Duplicate decisions.");
            bool voyagePhase=s.phase==RunPhase.Traveling || s.phase==RunPhase.EventPending || s.phase==RunPhase.Delivery || s.phase==RunPhase.Review || s.phase==RunPhase.Complete;
            if(voyagePhase && s.voyage==null)throw new InvalidOperationException("Missing voyage.");
            if((s.phase==RunPhase.Review || s.phase==RunPhase.Complete) && s.deliveries!=2)throw new InvalidOperationException("Review before two deliveries.");
            if(s.voyage!=null)
            {
                var v=s.voyage;var c=d.Contract(v.contractId);var r=d.Route(v.routeId);
                if(c==null || r==null || r.contractId!=c.id || string.IsNullOrEmpty(v.id) || v.choices==null || v.choices.Any(x=>x==null) || v.eventIndex<0 || v.eventIndex>2 || v.eventIndex!=v.choices.Count || float.IsNaN(v.elapsed) || float.IsInfinity(v.elapsed) || v.elapsed<0 || float.IsNaN(v.travelDelay) || float.IsInfinity(v.travelDelay) || v.travelDelay<0 || float.IsNaN(v.dockDelay) || float.IsInfinity(v.dockDelay) || v.dockDelay<0 || v.elapsed>r.seconds+v.travelDelay || v.fees<0 || v.bonus<0 || v.fees!=v.choices.Sum(x=>(long)x.fee) || v.bonus!=v.choices.Sum(x=>(long)x.bonus) || v.travelDelay!=v.choices.Sum(x=>x.travelDelay) || v.dockDelay!=v.choices.Sum(x=>x.dockDelay))throw new InvalidOperationException("Invalid voyage.");
                foreach(var choice in v.choices)ValidateReceipt(choice,d,c.tier);
                if(v.choices.Select(x=>x.eventId).Distinct().Count()!=v.choices.Count || (v.choices.Count>0 && v.choices[0].eventId!=r.eventId) || (v.choices.Count>1 && v.choices[1].eventId!="inspection"))throw new InvalidOperationException("Invalid decision queue.");
                bool active=s.phase==RunPhase.Traveling || s.phase==RunPhase.EventPending;
                if(active && (c.tier!=s.tier || (s.deliveries==0)!=(c.tier==1) || s.deliveries>=2 || s.completedVoyages.Contains(v.id)))throw new InvalidOperationException("Invalid active voyage phase.");
                int voyageNumber=active?s.deliveries+1:s.deliveries;
                if(v.id!=s.runId+"/voyage/"+voyageNumber || c.tier!=voyageNumber || (active && s.acceptedContractId!=c.id))throw new InvalidOperationException("Voyage disagrees with progression.");
                if(!active && (!s.completedVoyages.Contains(v.id) || v.elapsed!=r.seconds+v.travelDelay))throw new InvalidOperationException("Delivery was not completed.");
                // Serialized floats and Mono's intermediate arithmetic can differ by a few ULPs.
                const float timeTolerance=.0001f;
                if(s.phase==RunPhase.EventPending && (v.eventIndex>=2 || v.pendingEventId!=(v.eventIndex==0?r.eventId:"inspection") || d.Event(v.pendingEventId)==null || Math.Abs(v.elapsed-r.seconds*(v.eventIndex==0?.35f:.70f))>timeTolerance))throw new InvalidOperationException("Invalid pending event.");
                if(s.phase!=RunPhase.EventPending && !string.IsNullOrEmpty(v.pendingEventId))throw new InvalidOperationException("Unexpected pending event.");
                if(string.IsNullOrEmpty(r.eventId) && v.eventIndex!=0)throw new InvalidOperationException("Unexpected route decisions.");
                if(!active && !string.IsNullOrEmpty(r.eventId) && v.eventIndex!=2)throw new InvalidOperationException("Incomplete delivered decisions.");
                if(active && !string.IsNullOrEmpty(r.eventId) && ((v.eventIndex>0 && v.elapsed+timeTolerance<r.seconds*(v.eventIndex==1?.35f:.70f)) || (s.phase==RunPhase.Traveling && v.eventIndex<2 && v.elapsed>r.seconds*(v.eventIndex==0?.35f:.70f)+timeTolerance)))throw new InvalidOperationException("Invalid event progress.");
                if(s.decisions.Count!=v.choices.Count || s.decisions.Where((x,i)=>!SameReceipt(x,v.choices[i])).Any())throw new InvalidOperationException("Conflicting decision histories.");
            }
            else if(s.decisions.Count!=0)throw new InvalidOperationException("Decisions without a voyage.");

            var expected=new Dictionary<string,int>();
            var first=d.routes.Single(x=>d.Contract(x.contractId).tier==1);
            string firstId=s.runId+"/voyage/1";
            if(s.deliveries>0 || s.voyage!=null)expected.Add(firstId+"/dispatch",-first.cost);
            if(s.deliveries>0)expected.Add(firstId+"/settlement",d.Contract(first.contractId).reward);
            if(s.upgraded)expected.Add(s.runId+"/upgrade",-d.upgradeCost);
            if(s.voyage!=null && d.Contract(s.voyage.contractId).tier==2)
            {
                var v=s.voyage;expected.Add(v.id+"/dispatch",-d.Route(v.routeId).cost);
                if(s.deliveries==2)expected.Add(v.id+"/settlement",d.Contract(v.contractId).reward+v.bonus-v.fees);
            }
            int severity=s.decisions.Count==0?0:s.decisions.Max(x=>x.severity);
            if(s.reviewed)expected.Add(s.runId+"/review",severity==2?-d.majorFine:severity==1?-d.minorFine:d.cleanBonus);
            if(s.ledger.Count!=expected.Count || s.ledger.Any(x=>!expected.TryGetValue(x.id,out int amount) || x.amount!=amount))throw new InvalidOperationException("Transactions disagree with progression.");
            string standing=s.reviewed?(severity==2?"Restricted":severity==1?"Watch":"Clear"):"Clear";
            int reputation=d.startingReputation,suspicion=0;
            if(s.deliveries>0)reputation=Math.Clamp(reputation+d.deliveryReputation,0,100);
            foreach(var decision in s.decisions)
            {
                var choice=d.Event(decision.eventId).choices.Single(x=>x.id==decision.choiceId);
                reputation=Math.Clamp(reputation+choice.reputation,0,100);suspicion=Math.Clamp(suspicion+choice.suspicion,0,100);
            }
            if(s.deliveries==2)reputation=Math.Clamp(reputation+d.deliveryReputation,0,100);
            if(s.reviewed)reputation=Math.Clamp(reputation+(severity==2?d.majorReputation:severity==1?d.minorReputation:d.cleanReputation),0,100);
            if(s.standing!=standing || s.reputation!=reputation || s.suspicion!=suspicion || (s.serviceRemaining>0 && (s.deliveries!=2 || s.phase==RunPhase.Traveling || s.phase==RunPhase.EventPending || s.serviceRemaining>d.serviceSeconds+s.voyage.dockDelay)))throw new InvalidOperationException("Metrics disagree with history.");
        }
        static void ValidateReceipt(ChoiceReceipt receipt,DemoDefinitions d,int tier)
        {
            var choice=receipt==null?null:d.Event(receipt.eventId)?.choices.SingleOrDefault(x=>x.id==receipt.choiceId);
            var modifiers=d.Tier(tier);
            if(choice==null || receipt.severity!=choice.severity || receipt.fee!=(choice.useRepairModifiers?modifiers.repairFee:choice.fee) || receipt.bonus!=choice.bonus || receipt.travelDelay!=(choice.useRepairModifiers?modifiers.repairDelay:choice.travelDelay) || receipt.dockDelay!=(choice.dockDelay<0?modifiers.dockDelay:choice.dockDelay))throw new InvalidOperationException("Invalid choice receipt.");
        }
        static bool SameReceipt(ChoiceReceipt a,ChoiceReceipt b)=>a.eventId==b.eventId && a.choiceId==b.choiceId && a.fee==b.fee && a.bonus==b.bonus && a.severity==b.severity && a.travelDelay==b.travelDelay && a.dockDelay==b.dockDelay;
    }
}
