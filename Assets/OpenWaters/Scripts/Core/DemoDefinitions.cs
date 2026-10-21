using System;
using System.Linq;
using UnityEngine;

namespace OpenWaters
{
    [Serializable] public class TierData
    {
        public int tier, capacity, range, repairFee, repairDelay, dockDelay;
        public GameObject[] buildings;
    }
    [Serializable] public class CargoData
    {
        public string id, title;
        public Sprite icon;
        public GameObject prefab;
    }
    [Serializable] public class ContractData
    {
        public string id, destination, cargoId;
        public int units, tier, range, reward;
    }
    [Serializable] public class RouteData
    {
        public string id, title, contractId, eventId;
        public int cost;
        public float seconds;
    }
    [Serializable] public class ChoiceData
    {
        public string id, title, description;
        public int fee, bonus, damage, reputation, suspicion, severity;
        public float travelDelay, dockDelay;
        public bool useRepairModifiers;
    }
    [Serializable] public class EventData
    {
        public string id, title, description;
        public ChoiceData[] choices;
    }
    // This authored asset is the single source of balance and immutable content.
    [CreateAssetMenu(menuName="Open Waters/Demo definitions")]
    public sealed class DemoDefinitions : ScriptableObject
    {
        public int startingCredits=200, startingReputation=50, upgradeCost=300, shipCapacity=20;
        public int deliveryReputation=5, cleanBonus=40, cleanReputation=5, minorFine=60, minorReputation=-10, majorFine=180, majorReputation=-20;
        public float serviceSeconds=3, checkpointSeconds=5;
        public TierData[] tiers;
        public CargoData[] cargos;
        public ContractData[] contracts;
        public RouteData[] routes;
        public EventData[] events;
        public GameObject ship, coast, ocean, foam, wake, damage, selection, upgrade, loading, arrival;
        public ContractData Contract(string id) => contracts.SingleOrDefault(x=>x.id==id);
        public RouteData Route(string id) => routes.SingleOrDefault(x=>x.id==id);
        public EventData Event(string id) => events.SingleOrDefault(x=>x.id==id);
        public TierData Tier(int tier) => tiers.Single(x=>x.tier==tier);

        public void Validate()
        {
            if(tiers==null || tiers.Length!=2 || tiers.Select(x=>x.tier).OrderBy(x=>x).SequenceEqual(new[]{1,2})==false)
                throw new InvalidOperationException("Demo requires exactly tiers 1 and 2.");
            if(cargos?.Length!=3 || contracts?.Length!=3 || events?.Length!=5 || routes?.Length!=5)
                throw new InvalidOperationException("Expected 3 cargos/contracts, 5 events/routes.");
            if(contracts.Select(x=>x.destination).Distinct().Count()!=3 || cargos.Select(x=>x.id).Distinct().Count()!=3 || contracts.Select(x=>x.id).Distinct().Count()!=3 || routes.Select(x=>x.id).Distinct().Count()!=5 || events.Select(x=>x.id).Distinct().Count()!=5)
                throw new InvalidOperationException("Content IDs and destinations must be unique.");
            foreach(var c in contracts)
                if(!cargos.Any(x=>x.id==c.cargoId) || c.tier<1 || c.tier>2 || c.units>Math.Min(shipCapacity,Tier(c.tier).capacity) || c.reward<0)
                    throw new InvalidOperationException("Invalid contract: "+c.id);
            foreach(var r in routes)
                if(Contract(r.contractId)==null || r.seconds<=0 || r.cost<0 || (!string.IsNullOrEmpty(r.eventId) && Event(r.eventId)==null))
                    throw new InvalidOperationException("Invalid route: "+r.id);
            foreach(var e in events)
                if(e.choices==null || e.choices.Length<2 || e.choices.Select(x=>x.id).Distinct().Count()!=e.choices.Length)
                    throw new InvalidOperationException("Invalid choices: "+e.id);
        }
        public void AuthorDefaults()
        {
            tiers=new[]{new TierData{tier=1,capacity=10,range=1,repairFee=40,repairDelay=10,dockDelay=20},new TierData{tier=2,capacity=20,range=2,repairFee=20,repairDelay=5,dockDelay=10}};
            cargos=new[]{new CargoData{id="grain",title="Grain"},new CargoData{id="textiles",title="Textiles"},new CargoData{id="parts",title="Machine parts"}};
            contracts=new[]{
                new ContractData{id="grain_beacon",destination="Beacon Bay",cargoId="grain",units=10,tier=1,range=1,reward=300},
                new ContractData{id="textiles_copper",destination="Copperport",cargoId="textiles",units=16,tier=2,range=2,reward=360},
                new ContractData{id="parts_meridian",destination="Meridian Quay",cargoId="parts",units=20,tier=2,range=2,reward=400}};
            routes=new[]{
                new RouteData{id="beacon_coastal",title="Coastal passage",contractId="grain_beacon",cost=50,seconds=35,eventId=""},
                new RouteData{id="copper_sheltered",title="Sheltered passage",contractId="textiles_copper",cost=80,seconds=55,eventId="congestion"},
                new RouteData{id="copper_exposed",title="Exposed passage",contractId="textiles_copper",cost=60,seconds=40,eventId="storm"},
                new RouteData{id="meridian_patrolled",title="Patrolled passage",contractId="parts_meridian",cost=80,seconds=55,eventId="engine"},
                new RouteData{id="meridian_shortcut",title="Short passage",contractId="parts_meridian",cost=60,seconds=40,eventId="pirates"}};
            events=new[]{
                new EventData{id="storm",title="Storm ahead",description="A squall crosses the exposed passage. Protect the hull or keep your schedule.",choices=new[]{
                    new ChoiceData{id="detour",title="Detour",description="+15 travel seconds. No fee or damage.",travelDelay=15},
                    new ChoiceData{id="push",title="Push through",description="30 deducted at delivery; hull -20 until service.",fee=30,damage=20}}},
                new EventData{id="pirates",title="Unprotected waters",description="Reports of pirates along the shortcut. Choose a guarded passage or pay for an escort.",choices=new[]{
                    new ChoiceData{id="detour",title="Guarded detour",description="+15 travel seconds; no fee.",travelDelay=15},
                    new ChoiceData{id="escort",title="Hire escort",description="50 deducted at delivery; no delay.",fee=50}}},
                new EventData{id="engine",title="Engine fault",description="A pump needs attention. Your upgraded repair team reduces repair costs and delay.",choices=new[]{
                    new ChoiceData{id="repair",title="Repair now",description="Tier 2: 20 deducted at delivery; +5 travel seconds. Tier 1: 40 / +10s.",useRepairModifiers=true},
                    new ChoiceData{id="limp",title="Limp home",description="No fee; hull -20. Tier 2: +10 dock-service seconds (Tier 1: +20).",damage=20,dockDelay=-1}}},
                new EventData{id="congestion",title="Port congestion",description="Copperport has a queue for its berths. Wait or purchase a priority slot.",choices=new[]{
                    new ChoiceData{id="wait",title="Wait",description="+15 travel seconds; no fee.",travelDelay=15},
                    new ChoiceData{id="priority",title="Pay priority",description="30 deducted at delivery; no delay.",fee=30}}},
                new EventData{id="inspection",title="GSA documentation check",description="A premium offer has paperwork discrepancies. The legitimate base contract remains payable. A scheduled GSA review follows delivery.",choices=new[]{
                    new ChoiceData{id="clean",title="Decline premium; regularize paperwork",description="Base payout. No fee, suspicion or reputation penalty."},
                    new ChoiceData{id="fee",title="Pay unofficial fee",description="+160 bonus, -40 fee at delivery. Suspicion +25, reputation -5. Minor violation: review fine 60, reputation -10 / Watch.",bonus=160,fee=40,suspicion=25,reputation=-5,severity=1},
                    new ChoiceData{id="risk",title="Risk shipment",description="+240 bonus at delivery. Suspicion +50, reputation -10. Major violation: review fine 180, reputation -20 / Restricted.",bonus=240,suspicion=50,reputation=-10,severity=2}}}};
        }
    }
}
