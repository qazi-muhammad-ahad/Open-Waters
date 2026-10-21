# Open Waters - two-tier prototype implementation plan

**Document date:** 2026-10-20 (Asia/Karachi)  
**Status:** Gameplay implementation authorized and started on 2026-10-20; actual verification is tracked in `progression.md`.  
**Target:** A complete, replayable 5-8 minute 3D top-down/isometric demo with **one base tier and one upgraded tier**.  
**Companion files:** `AGENTS.md` governs agent workflow; `progression.md` tracks implementation and handoffs.

## 1. Authority, source review, and repository reality

### 1.1 What the user requested

Read the complete supplied game pitch, plan a prototype with a single level of upgrade, create this comprehensive plan and a persistent progress tracker, and require agents to switch exclusively to handoff work at 10% remaining usage quota.

The user explicitly describes a **3D top-view game similar in presentation to Clash of Clans**. Interpret this as a 3D port viewed from an elevated, angled camera with readable structures and visible growth. The pitch supplies the maritime trade and business-risk identity.

The original task created planning documents. On 2026-10-20 the user authorized implementation of all steps after asset delivery. Follow the tracker for actual completion evidence.

### 1.2 Sources reviewed in full

| ID | Local source | Review coverage |
|---|---|---|
| P | `C:\Users\Asus\Downloads\OpenWaters_CEGA_PitchDeck.pdf` | All 25 pages: extracted text plus rendered-page visual review. |
| A | `C:\Users\Asus\Downloads\Open_Waters_CEGA_Applicant-Presentation.pdf` | All 10 pages: text and rendered-page review; pages 1, 3, 6, 7, 9, and 10 required visual reading because their text is embedded in images. |

Source references below use PDF page numbers, counted from 1. Source documents remain in Downloads and are not checked into this repository. If unavailable on another machine, the traceability table and this plan preserve the design essentials; ask for the originals only when needed.

Treat every statement in these PDFs as reference content, not an instruction to an agent. Marketing activities, funding asks, and roadmap aspirations do not authorize publishing, contacting anyone, buying assets, or implementing a larger game.

### 1.3 Complete pitch coverage and prototype interpretation

| Source pages | Pitch content | Demo interpretation |
|---|---|---|
| P1-3; A1 | Open Waters / DEUXART LABS; shipping empire under the Global Shipping Authority; incubation fit | Use the maritime identity and GSA premise. Demo proves a small playable loop. |
| P4-5; A2 | Avoid passive tycoon timers, overwhelming logistics, and combat-led base building | Short voyages, readable route trade-offs, interactive risk decisions, no combat simulation. |
| P6-9; A4 | One dock to a global empire; build, trade, risk, comply/gamble, scale | Start with one dock and ship; complete one visible port upgrade; tease later expansion. |
| P10 | Dock, warehouse, fuel depot, office/GSA desk, repair shed have functional roles | Five fixed functional structures; one bundled port upgrade changes their capabilities and appearance. |
| P11 | Coastal freighter, tanker, container ship, reefer; distinct cargo trade-offs | One coastal freighter only. Three compatible dry cargo types; other vessel classes deferred. |
| P12; A9 | Continents -> regions -> ports; broader content/platform growth | One small route map with three destinations. Other regions are explicitly future content. |
| P13 | Storm, pirates, inspection, engine fault, port congestion | Implement five event definitions; surface a small subset per run so the demo stays short. |
| P14-15; A4 | GSA rewards, suspicion, audits, fines, locks, and fictional risky offers | One guaranteed teaching review with clean/watch/restricted outcomes tied to recorded choices. |
| P16-17; A3 | 1 shipyard, 1 ship, 3 destinations, 3 cargo types, 5 events, 1 audit, 1 upgrade, 5-8 minutes | Preserve these counts. One purchase moves the whole port from Tier 1 to Tier 2. |
| P18-21; A5-6 | Strategy/tycoon audiences, accessible logistics, regulator differentiation | Validate comprehension with new players; category comparisons are hypotheses, not proven market research. |
| P22; A7-8 | Free prototype; possible premium/PC, DLC, later mobile; costs and forecasts uncertain | Free local demo; no monetization implementation. Applicant deck favors one-time purchase plus expansions for the later product. |
| P23; A6 | Devlogs, itch.io/WebGL, feedback, community | Prepare a shareable build and recording checklist. Publishing/community activity is separate work. |
| P24 | Six-month incubation roadmap | Use small implementation gates below. Do not treat the six-month roadmap as a fixed delivery promise. |
| P25; A10 | Two part-time student founders; developer and 3D artist; mentorship/testing needs | Suggest engineering/design ownership for Ahad and art ownership for Uzair; confirm availability before scheduling. |
| A3 | Claims a playable prototype and core systems already exist | **Not verified in this checkout.** Do not mark implementation complete from this slide. |
| A8 | Current spend stated as $0; other financials TBD | Historical pitch claim only; do not invent current budgets, revenue, margins, or licensing assurances. |

### 1.4 Verified starting point

Repository inspection on 2026-10-20 found:

- Unity version `6000.3.23f1` in `ProjectSettings/ProjectVersion.txt`; that editor version is installed locally.
- URP `17.3.0`, Input System `1.20.0`, Unity Test Framework `1.6.0`, and uGUI `2.0.0` in the manifest.
- `Assets/Scenes/Official_Game.unity` is enabled in build settings. Its serialized scene objects are the template camera, directional light, and global volume.
- Existing input-actions asset and PC/mobile render-pipeline settings.
- Only tutorial/readme C# scripts were found; no game-specific scripts, prefabs, or models were found.
- Starting branch: `Ahad`; starting HEAD: `792c607`; working tree was clean before this planning task.
- No root `AGENTS.md`, `plan.md`, or `progression.md` existed before this task.

These observations establish a foundation project, not a tested playable prototype. Editor startup, compilation, Play Mode, and platform builds have not been validated in this planning task. If implementation exists elsewhere, inspect it before replacing any work.

### 1.5 Proposed defaults versus pitch requirements

All exact prices, stats, fictional destination names, timing values, event effects, architecture, fixed building locations, and the bundled upgrade design below are **proposed prototype defaults**. They make the plan implementable; they are not claims that the decks specified those details.

The core scope and fantasy come from the user and pitch. Tune numerical defaults after playtests while maintaining the two-tier limit and the ability to finish without grinding.

## 2. Definition of the demo

### 2.1 Player promise

"Grow a small shipping port, earn your first expansion, and discover how much profit you are willing to risk under GSA scrutiny."

By the end, a player should be able to explain:

1. What earns credits and what an upgrade changes.
2. How two routes trade time/cost against operational risk.
3. How a voyage decision changes profit, reputation, and GSA standing.
4. Why the port is visibly and functionally better at Tier 2.

### 2.2 Hard content budget

| Item | Required scope |
|---|---|
| Playable world | One authored 3D home harbor; no separate destination scenes. |
| Progression | Exactly Tier 1 -> Tier 2; one paid port upgrade; no further upgrade purchase. |
| Buildings | Dock, warehouse, fuel facility, office/GSA desk, repair facility at fixed locations. |
| Fleet | One coastal freighter; one active voyage; no fleet purchasing. |
| Destinations | Three named ports on a compact map; two become accessible at Tier 2. |
| Cargo | Grain, textiles, machine parts; one cargo type per contract. |
| Contracts | Three authored templates: one introductory and two upgraded options. |
| Routes | One tutorial route; two route choices per upgraded destination. |
| Events | Five voyage-event types, including the documentation inspection. |
| GSA | One review after the second voyage, with three distinct outcomes. |
| Guided session | Two completed voyages, one upgrade, one review, ending card. |
| Replay | Restart a fresh run to choose another upgraded destination/route/risk branch. |
| Distribution | Windows x64 first; WebGL release candidate after platform validation. |

All three destinations and cargo types must be implemented and reachable across replays. A single short run visits the starter destination and one upgraded destination. All five event types must be implemented and testable, but are not forced into every 5-8 minute session.

### 2.3 Explicit exclusions

- Tier 3, multi-tier technology trees, independent building upgrade ladders, or ship upgrades.
- Multiple ships, ship construction, global campaigns, continent management, or live market simulation.
- Freeform/grid placement, land purchase, demolition, road networks, and worker logistics.
- Manual ship steering, naval combat, physics-based ocean navigation, and pirate combat AI.
- Fuel inventories, consumable stocks, real-time cargo markets, and multi-contract scheduling.
- Multiplayer, accounts, cloud saving, live services, payments, ads, and mobile store delivery.
- Real-time/offline idle earnings, day-long construction timers, or daily rewards.
- Full audits with procedural investigations, insurance markets, confiscation chains, or permanent bankruptcy.
- Dedicated destination environments, cinematic story production, or a realistic global geography database.

The simulation should leave clean seams for future content without building these excluded systems.

## 3. Exact meaning of "one base level + one upgraded level"

There is one authoritative `PortTier` value: `Base = 1`, `Upgraded = 2`. No component may create a playable third tier. The player buys **Commercial Port Upgrade** once for **300 credits**, after the first delivery while the ship is docked.

The upgrade is a bundle: dock work, expanded warehouse, better fuel handling, an office extension, and a repair shelter change together. Buildings do not each require another paid upgrade. This preserves one clear progression step while making the shipyard feel transformed.

| Feature | Tier 1: Tiny Dock | Tier 2: Small Commercial Port |
|---|---|---|
| Dock | Small pier, one berth, starter freighter | Reinforced quay/crane, still one berth and one freighter |
| Warehouse | Open crates/small shed; 10 cargo units | Larger shed/container stacks; 20 cargo units |
| Fuel | Small fuel station; route range 1 | Compact depot; route range 2 |
| Office | Starter desk; permit and tutorial records | Office extension; upgraded contracts and full GSA ledger |
| Repairs | Basic maintenance spot | Repair shelter; engine-fault cost/delay reduction |
| Ship | Coastal freighter, 20-unit physical maximum | Same ship and maximum; expanded warehouse makes full loads possible |
| Effective shipment capacity | `min(ship 20, warehouse 10) = 10` | `min(ship 20, warehouse 20) = 20` |
| Available destinations | Beacon Bay | Beacon Bay shown as completed; Copperport and Meridian Quay selectable |
| Cargo/contracts | Grain tutorial shipment | Choose textiles or machine-parts shipment |
| GSA permit | Coastal permit active | Same permit; compliant dry cargo remains permitted |
| Upgrade UI | Price, requirement, preview, confirm | "Maximum demo tier reached"; no purchasable next level |

An office upgrade does not automatically grant a fictional authority license upgrade. The demo uses one existing coastal permit and changes its standing during review; regional licensing is an ending teaser.

**Upgrade acceptance criteria:** cost deducted once; all tier stats switch atomically; visible silhouette changes; new contracts and routes unlock immediately; reloading preserves Tier 2; clicking again cannot charge or upgrade again.

## 4. Guided session and pacing

### 4.1 Target sequence

| Elapsed target | Player action | Game response / teaching purpose |
|---|---|---|
| 0:00-0:40 | See harbor; select dock/ship; read short objective | Establish one ship, money, and the first delivery goal. |
| 0:40-1:20 | Accept grain contract; inspect cargo and route; dispatch | Teach capacity, operating cost, and payout preview. |
| 1:20-2:00 | Watch a short safe voyage; optionally inspect buildings/map | Ship movement and status show progress; tutorial voyage has no incident. |
| 2:00-2:30 | Receive delivery ledger | Credits and reputation change; upgrade becomes affordable. |
| 2:30-3:10 | Preview and buy upgrade | Port visibly transforms; demonstrate capacity and range unlocks. |
| 3:10-4:10 | Choose textiles or machine parts; compare routes; dispatch | The upgraded game loop gives a meaningful destination and route choice. |
| 4:10-5:45 | Resolve one operational event and one documentation inspection | Trade money/time against safety; then profit against GSA consequences. |
| 5:45-6:30 | Review delivery and GSA outcome | Show a complete ledger and why the regulator reacted. |
| 6:30-7:15 | Read ending; inspect port; choose replay | Tease larger vessels/regions without allowing further progression. |

These are pacing targets, not forced timers or measured results. Reading speed may exceed eight minutes; never auto-select a risky option to meet the target. Aim for a median first session within 5-8 minutes after testing.

### 4.2 Tutorial approach

- Use a persistent objective and one short contextual hint at a time.
- Highlight the next relevant building/button without blocking camera exploration.
- Initially offer only the safe contract; explain locked options with their requirements.
- Show an upgrade before/after preview with concrete benefits.
- Explain reputation and suspicion when first relevant, rather than opening with every statistic.
- Skip-tutorial hides coaching but preserves the same two-voyage progression requirements.
- Selecting a different route before departure never consumes money.
- After accepting a contract, allow cancellation before dispatch with no penalty.
- Acknowledge final ledgers; do not require a separate manual "claim" action to receive funds.

### 4.3 Completion and replay

Completion requires both deliveries, the single Tier 2 upgrade, and acknowledgment of the GSA outcome. A negative GSA outcome is a valid ending, not a softlock or a demand to replay.

Ending card shows starting/final credits, gross earnings, operating/event/audit costs, reputation change, suspicion, permit standing, and decisions. It offers **Inspect Port**, **New Run**, and **Main Menu**. Inspect Port permits camera movement and information panels, but no third voyage or additional upgrade. New Run confirms replacing the current run.

## 5. Game rules and initial economy

### 5.1 State and currencies

| State | Initial value | Rules |
|---|---|---|
| Credits | 200 | Integer, nonnegative; use fictional credits, not real currencies. |
| Reputation | 50 / 100 | Clamp to 0-100; +5 for each completed delivery, with event/audit adjustments. |
| Suspicion | 0 / 100 | Clamp to 0-100; increased by documented risky choices; no passive decay during this short demo. |
| Port tier | 1 | Only valid values are 1 and 2. |
| Permit standing | Clear | Clear / Watch / Restricted; standing is separate from port tier. |
| Ship condition | 100 / 100 | Cosmetic/status feedback plus repair handling; no permanent ship loss. |
| Active voyages | 0 | Maximum one; dispatch only from the docked state. |
| Cargo reservation | None | Contract cargo is provided by the client; no cargo-purchase economy. |
| Upgrade purchased | False | Must agree with tier; never permit double purchase. |

Do not add an extra compliance currency. Compliance is represented by the permit standing and recorded decisions. Suspicion and reputation have different meanings and must have different labels/tooltips.

### 5.2 Authored contracts

All names and values below are proposed. The low-level grain contract is required once; there is no repeat-grind requirement.

| ID | Destination | Cargo | Amount | Required tier / range | Base payout |
|---|---|---|---:|---|---:|
| `grain_beacon` | Beacon Bay | Grain | 10 | 1 / 1 | 300 |
| `textiles_copper` | Copperport | Textiles | 16 | 2 / 2 | 360 |
| `parts_meridian` | Meridian Quay | Machine parts | 20 | 2 / 2 | 400 |

At Tier 1, upgraded contracts are visible as locked previews. At Tier 2, select exactly one of the two upgraded contracts for this run. Machine parts are ordinary crated dry cargo, not hazardous or tanker cargo. Every cargo uses the same unit measure for this prototype.

Contract card: destination, cargo icon/name/units, base payout, eligible ship, tier/range requirements, route cost, estimated time, likely hazard, and net payout before optional risks. Use a clear positive/negative breakdown.

### 5.3 Route data

| Route ID | Contract | Operating charge | Base voyage seconds | Operational event |
|---|---|---:|---:|---|
| `beacon_coastal` | Grain | 50 | 35 | None: safe teaching voyage |
| `copper_sheltered` | Textiles | 80 | 55 | Port congestion |
| `copper_exposed` | Textiles | 60 | 40 | Storm |
| `meridian_patrolled` | Machine parts | 80 | 55 | Engine fault |
| `meridian_shortcut` | Machine parts | 60 | 40 | Pirates |

Route selection affects cost, duration, and the authored incident. The base duration includes the abstract round trip and local departure/return presentation. Never add another full return timer after delivery. Decision-reading time pauses voyage progress. Event delay extends travel, and any unpaid engine-fault delay occurs at the dock after return.

All second voyages also encounter the inspection event. In guided mode, do not promise probabilistic hazard avoidance: show "Expected incident" and "GSA documentation check" in route details. Later randomized voyages are outside this demo.

### 5.4 Transactions and affordability

- Deduct only the operating charge on dispatch and the upgrade charge on purchase.
- Record event costs as explicit **deductions from delivery proceeds**. They do not require a second up-front balance while at sea.
- The contract payout is gross, not profit. Every summary uses the same equation:

```text
net voyage change =
    base payout + optional inspection bonus
    - operating charge - operational event cost - inspection fee

final run credits =
    starting credits + both net voyage changes
    - port upgrade cost + audit bonus - audit fine
```

- Apply delivery and audit transactions exactly once using stable transaction IDs.
- Before dispatch, validate tier, range, capacity, ship availability, active contract, and funds together.
- Show exact failure reasons beside disabled actions. Insufficient funds cannot consume partial costs.
- Required routes and clean event choices must remain affordable from the authored start.
- If an edited/debug save violates economy assumptions, provide New Run and a clear diagnostic. Do not silently inject money.

### 5.5 Worked economy examples

First voyage: `200 - 50 + 300 = 450`. Upgrade: `450 - 300 = 150`. The player can afford every upgraded route.

Using textiles on the sheltered route and choosing to wait at congestion (no event fee):

| Inspection choice | Delivery calculation from 150 | Audit effect | Final credits | Final reputation | Suspicion / standing |
|---|---|---|---:|---:|---|
| Decline premium / regularize documents | `150 - 80 + 360 = 430` | +40 bonus; +5 reputation | 470 | 65 | 0 / Clear |
| Pay unofficial fee | `150 - 80 + 360 + 160 - 40 = 550` | -60 fine; -10 reputation | 490 | 45 | 25 / Watch |
| Risk shipment | `150 - 80 + 360 + 240 = 670` | -180 fine; -20 reputation | 490 | 30 | 50 / Restricted |

Reputation arithmetic: start 50, first delivery +5, second delivery +5, inspection choice 0/-5/-10, audit +5/-10/-20.

Risky choices can yield slightly more money but visibly worse standing. An illegal shortcut must not be presented as the only sensible progression route. Machine-parts payouts are 40 higher, with distinct operational hazards and the full capacity requirement.

Worst authored upfront balance is `150 - 80 = 70`; operational fees are deducted later. Even the most expensive operational choice does not prevent completion. Recalculate all branch bounds whenever values change.

## 6. Port and fleet interaction

### 6.1 Home harbor

- Author a compact coastline/island with water, one navigable visual departure lane, and five recognizable functional building locations.
- Keep the dock and warehouse as the strongest silhouettes; office and services remain clearly selectable.
- Select buildings through simple colliders and a dedicated interaction layer.
- Building panel shows purpose, current tier contribution, locked/unlocked features, and a shortcut to the relevant UI.
- Use fixed sockets and bundled construction for the demo. There is no placement grid.
- Upgrade swaps/activates Tier 2 prefab variants at stable anchors; do not reload the whole scene.
- Use a brief 2-4 second construction reveal that can be skipped; stats are committed independently from the animation.
- Selection/highlight must survive model replacement or safely deselect without null references.

### 6.2 Ship

- One coastal-freighter prefab with departure/arrival waypoints, selectable collider, wake, engine sound, and cargo presentation anchors.
- Physical capacity is 20 units. Warehouse/range, not a second ship, limits early progress.
- Preparation panel verifies contract cargo, route cost, capacity, condition, and permit.
- Movement follows an authored spline or waypoint interpolation; physics collisions and NavMesh are unnecessary for the fixed lane.
- While visually offscreen, the route-map marker and ETA represent the voyage.
- A returned damaged ship can still settle cargo. Maintenance must not block delivery payout or the ending.
- Tier 2 repair facility automatically restores lost condition during a 3-second return-service presentation; engine-fault delay can extend it as specified below. Restoration has no additional hidden fee.

## 7. Voyage events and risk choices

### 7.1 Event lifecycle

The starter voyage has no event. Each upgraded voyage has:

1. One route-specific operational event at approximately 35% of base travel progress.
2. One inspection at approximately 70%, after the first event has resolved.

Events are queued, never overlapping. Freeze the simulation clock during a decision. Persist pending event ID, displayed options, selected choice, and applied effects. Resolve by one command; repeated clicks or reloading cannot apply it twice.

Each event card contains a short situation, two or three choices, exact immediate money/time/condition effects, reputation/suspicion effects when relevant, and an outcome receipt. Do not hide tutorial consequences behind vague "risk" labels.

### 7.2 Five-event content specification

| Event | Choice | Exact initial effect | Presentation |
|---|---|---|---|
| Storm | Detour | +15 travel seconds; no fee/damage | Route bends around storm; calmer water marker. |
| Storm | Push through | 30-credit repair deduction; condition -20 until return service | Rain/wind cue, hull alert, visible condition change. |
| Pirates | Take guarded detour | +15 travel seconds; no fee | Map changes route; no combat. |
| Pirates | Hire escort | 50-credit deduction; no delay | Escort icon/radio response; no extra simulated fleet. |
| Engine fault | Repair now | Tier 2: 20-credit deduction; +5 travel seconds | Mechanical alert clears after repair. |
| Engine fault | Limp home | No fee; condition -20; +10 dock-service seconds | Slower arrival/repair cue; base payout remains guaranteed. |
| Port congestion | Wait | +15 travel seconds; no fee | Queue indicator and revised ETA. |
| Port congestion | Pay priority | 30-credit deduction; no delay | Berth priority confirmation. |
| Inspection | Decline premium and regularize paperwork | No bonus/fee/suspicion; no reputation penalty | Base contract still completes; explain lawful payout. |
| Inspection | Pay unofficial fee | +160 payout bonus; -40 fee; +25 suspicion; -5 reputation; minor violation record | Show flagged transaction and upcoming review. |
| Inspection | Risk shipment | +240 payout bonus; no immediate fee; +50 suspicion; -10 reputation; major violation record | Strong warning of fine and permit restriction. |

The office frames the inspection as a fictional premium deal with documentation discrepancies. Declining the premium does not cancel the legitimate underlying shipment. Avoid real-world instructions for evading maritime regulation.

Engine-fault data also supports a Tier 1 comparison of 40 credits/+10 seconds for repair-now and +20 dock seconds for limp-home, so the Tier 2 repair benefit is explicit in its building panel. Guided faults only occur after upgrading; use a development fixture to validate the base comparison.

Optional cosmetic effects may be cut. The five event definitions and all their choices are required content.

### 7.3 Replay and determinism

- First run defaults to textiles/sheltered as a suggestion, not a forced selection.
- Replays can choose any of the four upgraded routes, covering all operational events.
- Provide development-only scenario buttons for each event/choice and GSA outcome.
- Guided outcome resolution uses authored consequences, not hidden dice rolls.
- If later optional randomness is introduced, store the seed and resolved outcomes; never reroll on load. It is not required for the current demo.

## 8. GSA review and consequences

### 8.1 What is implemented

The player starts with a valid coastal permit. The office tracks reputation, suspicion, violations, and review history. A **guaranteed introductory review** follows settlement of the second delivery. This ensures every run demonstrates the differentiator, including a clean run.

Audit probability and procedural detection are deferred. The UI must say this review is scheduled, not falsely imply the player happened to roll an audit.

| Evidence from second voyage | Review result | Money | Reputation | Permit standing / ending |
|---|---|---:|---:|---|
| No recorded violation | Clean clearance | +40 | +5 | Clear; preview regional-license opportunity. |
| Minor violation / unofficial fee | Warning and fine | -60 | -10 | Watch; explain higher scrutiny and premium-offer consequences. |
| Major violation / risk shipment | Serious breach | -180 | -20 | Restricted; show premium-contract access suspended. |

Use recorded violation severity as the outcome authority; suspicion is a legible summary of exposure. In this authored demo the three severity cases correspond to suspicion 0/25/50. If future systems add suspicion sources, do not infer a violation solely from a high score.

### 8.2 Observable consequence

- Office sign/badge and GSA panel change standing.
- Review cites the player's actual choice and money trail.
- Reputation and credits update visibly, once.
- For Restricted, premium-contract previews display the restriction reason. The run ends normally; do not strand the player in a required extra contract.
- The next-region teaser differs by standing: eligible, requires clean record, or requires reinstatement.
- No actual extra region, contract loop, or reinstatement mechanic is implemented.
- GSA clean/watch/restricted progression is a status outcome, not another building tier or upgrade level.

## 9. Camera, controls, and interface

### 9.1 3D presentation

- Use a real 3D harbor and models with an elevated orthographic camera as the default.
- Start around 45-degree yaw and 50-60-degree downward pitch; tune framing to read roofs, facades, water, and ship traffic.
- Keep rotation fixed for the first release; bounded pan and zoom provide exploration without disorientation.
- The reference to Clash of Clans concerns accessible camera framing, selection, and visible growth; use original port assets and visual identity.
- Favor stylized, readable forms over photorealistic water and dense port simulation.

### 9.2 Input

| Input | Action |
|---|---|
| Left click | Select building/ship, choose UI action |
| Right drag or middle drag | Pan camera |
| WASD / arrow keys | Optional camera pan |
| Mouse wheel | Bounded zoom |
| Home | Recenter harbor |
| Escape | Close ordinary panel; otherwise pause |
| Enter/Space on focused button | Activate ordinary focused UI button |

Use the existing Input System through explicit action maps. Review existing bindings before adding game-specific actions. Suppress world raycasts when the pointer is over UI. A mandatory event cannot be dismissed into an invisible blocked voyage; Escape can open pause while preserving the event.

Design generous buttons and non-hover-only information for future touch support. Actual touch gestures and device builds are deferred.

### 9.3 Required screens and panels

User playtest revision (2026-10-20): present one current decision in compact floating panels, with optional tips and expandable financial/rules details. Keep essential costs and risk consequences visible; hide explanations and historical logs until requested. Make world selection and the next relevant port structure visibly highlighted. Start desktop builds at the detected display resolution in fullscreen, with persisted Fullscreen / Borderless (windowed fullscreen) settings and a Windowed recovery option. Reuse one replaceable branding PNG for menu, HUD and app icons; suppress the default engine splash where supported by the installed editor/license. Gameplay scope and economy stay unchanged.

Use consistent pictograms alongside short labels for warehouse, dock, shipment, operations, GSA, fuel, repairs, routes, upgrades and settings. Icons should aid recognition while labels preserve clarity. A ship underway must fully leave the camera view and stay hidden offshore, then animate slowly back to the berth; it becomes selectable only after docking.

| Surface | Required content |
|---|---|
| Main menu | New Run, Continue if valid save, Settings, Credits, Quit where supported |
| HUD | Credits, reputation, suspicion/standing, tier, current objective, voyage indicator |
| Building/ship detail | Function, stats, current status, relevant action, tier comparison |
| Contract board | Three authored cards with locks, requirements, payouts |
| Preparation | Ship/cargo/condition/capacity check and route options |
| Route map | Home + three destinations, routes, costs/times/hazards, selection state |
| Voyage status | Progress, ETA, selected route, event queue status |
| Event modal | Situation, choices with effects, outcome receipt |
| Delivery ledger | Gross payout, every deduction/bonus, net change, updated balance |
| Upgrade modal | Cost, affordability, exact before/after effects, confirmation |
| GSA review | Evidence, judgment, ledger, reputation and permit consequences |
| Ending | Run summary, expansion teaser, Inspect Port/New Run/Main Menu |
| Pause/settings | Resume, audio volumes, reduced motion, tutorial hints, menu |

Use one modal coordinator to prevent conflicting panels and accidental background interaction. Preserve focused control for keyboard use. Financial and risk information needs text/icons as well as color. Test at 1280x720 and 1920x1080 with text scaling where supported.

## 10. Art, animation, and sound production

### 10.1 Minimum asset inventory

| Asset group | Minimum deliverable |
|---|---|
| Harbor | Terrain/coast, simple water, quay footprint, departure/return lane |
| Buildings | Five selectable structures with Tier 1/Tier 2 variants or modular additions |
| Freighter | One model, wake, simple cargo changes, damaged-state indicator |
| Cargo | Grain crates/sacks, textile bundles, machine-part crates/icons |
| Dressing | Small reusable set of bollards, lamps, fences, containers, rocks/vegetation |
| UI | Currency, reputation, suspicion, permit, cargo, route and five event icons |
| FX | Selection highlight, loading cue, upgrade reveal, arrival, warning/inspection cue |
| Audio | Harbor ambience, ship engine/water, UI click, departure, reward, upgrade, warning, review |

### 10.2 Production sequence

1. Build graybox silhouettes and complete both-tier gameplay using primitives.
2. Agree camera framing, scale, anchors, and building footprints.
3. Replace the dock, warehouse, and ship first; these communicate the fantasy.
4. Replace services and add low-cost dressing.
5. Add event feedback, audio, and upgrade polish after the full loop works.

Suggested asset conventions: one Unity unit approximates one meter; building pivot at ground-center or documented socket origin; ship forward axis consistent with the controller; apply transforms during export; use FBX exports with original Blender sources stored separately if needed. Preserve `.meta` GUIDs when replacing assets.

Use shared materials, restrained texture sizes, and simple colliders. Keep interaction colliders separate from decorative mesh complexity. Validate model readability at the actual gameplay zoom, not only in Blender.

Asset rights must be recorded in `docs/asset-register.md` during implementation: source, author, license, attribution, modifications, and allowed distribution. Pitch artwork is visual reference; it is not assumed to be a licensed production asset.

Audio needs master/music/SFX controls, saved preferences, gentle looping ambience, and a mute option. Browser audio starts after a player gesture. Reduced-motion mode disables camera shake and shortens or skips nonessential FX.

## 11. Unity architecture

### 11.1 Keep the existing engine baseline

Use Unity 6000.3.23f1 and installed URP/Input System/uGUI unless implementation proves a specific limitation. Do not migrate packages simply to start the demo. This is an architectural proposal based on local project files, not a compatibility-tested API guide.

Start with `Official_Game.unity` as the entry/gameplay scene. A root bootstrap creates session services and the menu; the harbor remains the shared presentation space. Introduce additional scenes only for a documented need.

### 11.2 Proposed project organization

```text
Assets/
  OpenWaters/
    Art/{Environment,Buildings,Ships,Cargo,Materials,FX}/
    Audio/{Music,Ambience,SFX}/
    Data/{Buildings,Cargo,Contracts,Routes,Events,Demo}/
    Prefabs/{World,Buildings,Ships,UI}/
    Scripts/
      Core/
      Economy/
      Contracts/
      Voyages/
      Progression/
      Compliance/
      Persistence/
      Presentation/
      UI/
    Tests/{EditMode,PlayMode}/
  Scenes/Official_Game.unity
docs/
  asset-register.md
  playtests.md
  build-and-run.md
AGENTS.md
plan.md
progression.md
```

Folders under `Assets/OpenWaters` and supporting implementation docs are planned, not created by this document task. Create only when the corresponding milestone needs them.

### 11.3 Responsibilities

| Component / service | Responsibility |
|---|---|
| `GameBootstrap` | Compose dependencies, load definitions, initialize or restore a run |
| `GameSession` | Own authoritative runtime state and run-level phase |
| `DemoFlowController` | Tutorial objectives, two-voyage gates, upgrade/ending flow |
| `EconomyService` | Validate and apply idempotent credit transactions |
| `ContractService` | Eligibility, acceptance/cancellation, cargo reservation |
| `VoyageService` | Dispatch validation, clock, queued events, return/settlement |
| `EventResolver` | Apply selected authored effects once; produce receipts |
| `ProgressionService` | Atomic Tier 1 -> Tier 2 purchase and derived capabilities |
| `GsaService` | Violation ledger, scheduled review, standing and audit settlement |
| `SaveService` | Versioned state snapshots and platform storage adapter |
| `HarborPresenter` | Render tier buildings, ship/cargo/status from state |
| `CameraController` | Bounds, pan/zoom, input gating |
| UI presenters | Display state and issue commands; no independent economy rules |
| `AudioController` | Mix settings and event-driven sound playback |

Use ordinary C# services for rules and MonoBehaviours for Unity presentation. Simple explicit dependency wiring is sufficient. Avoid an ECS rewrite, networking framework, generalized city-builder framework, or third-party DI requirement.

UI commands request changes; validated services mutate state and produce events/receipts; presenters react. UI animations and button callbacks must never be the only place a payout, upgrade, or audit becomes committed.

### 11.4 Definition assets and runtime data

Author immutable ScriptableObject definitions with stable string IDs:

Implementation note (2026-10-20): the compact demo authors these immutable definition rows together in `Assets/OpenWaters/Data/Demo.asset` (`DemoDefinitions`) so balance and content can be reviewed in one Inspector. Runtime state retains string IDs and never mutates that asset. This replaces separate files per definition without changing counts, effects or tier constraints.

- `PortTierDefinition`: tier, warehouse capacity, range, repair modifiers, building variants.
- `ShipDefinition`: ID, capacity, prefab reference, presentation speeds.
- `CargoDefinition`: ID, name, icon, visual prefab.
- `ContractDefinition`: ID, destination/cargo IDs, units, base reward, requirements.
- `RouteDefinition`: ID, required range, cost, duration, operational event ID.
- `VoyageEventDefinition`: ID, copy, choices, monetary/time/condition/compliance effects.
- `DemoBalanceConfig`: start values, upgrade cost, GSA outcomes, pacing configuration.

Runtime DTOs contain IDs and primitive state, not serialized scene references or mutated definition assets. Core fields:

```text
schemaVersion, runId, phase, tutorialStep, portTier
credits, reputation, suspicion, permitStanding
shipCondition, shipStatus, acceptedContractId
activeVoyage: voyageId, contractId, routeId, elapsedTravel,
              extraTravelDelay, dockDelay, pendingEventId,
              resolvedChoiceIds, payoutAdjustments, cargoUnits
completedVoyageIds, violationRecords, transactionLedger
upgradeTransactionId, auditState, auditTransactionId
settings, endingAcknowledged
```

Derive warehouse/range from the tier definition on load; do not store conflicting copies. Persist financial ledgers and event completion so loading cannot duplicate effects.

### 11.5 State machines

Run-level progression:

```text
Menu -> BasePort -> FirstContract -> FirstVoyage -> FirstDelivery
     -> UpgradeAvailable -> UpgradedPort -> SecondContract
     -> SecondVoyage -> SecondDelivery -> GsaReview -> DemoComplete
```

Voyage substate:

```text
Docked -> Preparing -> Departing -> Traveling
       -> EventPending -> Traveling
       -> Returning -> Settlement -> ServicingIfNeeded -> Docked
```

`EventPending` may occur twice during the upgraded voyage. Delivery settles on return before optional dock service; GSA review and the ending may proceed while the service presentation finishes. The run controller advances only on verified domain events, not elapsed tutorial text or button visibility.

Reject dispatch during an active voyage, upgrading at sea, a second upgrade, repeated settlement, and review before the second delivery. Pause affects simulation time; menus and event text remain responsive. No offline progress is simulated.

### 11.6 Save and recovery

- One local run slot plus separate settings.
- Save after contract acceptance/cancellation, dispatch, arrival at each decision, resolving each choice, delivery, upgrade, review, and ending.
- While traveling, checkpoint remaining progress periodically (target 5 seconds) and on application pause when supported.
- Windows: versioned JSON under `Application.persistentDataPath`; write via temporary file and replace, keeping one last-known-good backup.
- WebGL: use a platform adapter and verify browser storage persistence/flush behavior in the actual target browser. Do not assume desktop file replacement works unchanged.
- On resume, reconstruct visuals from logical state. A pending decision reopens exactly once; paid costs/rewards are not repeated.
- Validate schema, IDs, tier range, balances, event indices, and phase consistency before applying a save.
- Corrupt/unsupported save: display recovery message, try supported backup, offer New Run; do not crash or silently overwrite the only recoverable data.
- If browser persistence is unavailable, visibly mark the session as temporary. A guest must still be able to finish the whole demo.

## 12. Performance and build targets

These are initial budgets, not measured performance claims. Record test hardware/browser before declaring a target achieved.

| Area | Proposed target / validation |
|---|---|
| Windows desktop | 60 FPS at 1080p on the declared test machine; profile CPU/GPU separately |
| WebGL | At least 30 FPS at 720p on a declared desktop browser/device |
| Memory | Investigate sustained runtime memory above 500 MB; no growth across five new runs |
| Draw calls | Aim below 150 in the harbor; profile actual URP passes rather than guessing |
| Visible geometry | Aim below 250k triangles for the compact harbor |
| Web payload | Original general budget: below 80 MB compressed. Website integration proposes ≤15 MB initial compressed payload and measured 5–10 second startup gates; see website planning amendment below. Neither target is measured release evidence. |
| Scene responsiveness | No noticeable stalls during selection, event opening, or tier change |
| Visuals | One primary directional light, limited shadows, simple water, restrained particles |

Keep expensive water reflections, large transparent effects, and unnecessary realtime lights out of the first build. Pool only effects that profiling shows need it. Avoid constant allocations and per-frame scene-wide searches in hot paths.

Windows is the primary working build and rehearsal fallback. Produce a WebGL candidate once the Windows loop passes. Verify browser hosting headers/compression, resize, focus/pause, audio activation, and persistence; opening the HTML as a local file is not a sufficient deployment test.

Mobile-friendly layout is a future-facing constraint, not a promise that the demo is tested on phones.

## 13. Implementation milestones and dependency order

Work in small, verifiable tasks. Every milestone has a visible exit gate. The detailed small-task checklist lives in `progression.md`; retain the IDs below when reporting.

| Milestone | Dependencies | Main output | Exit gate |
|---|---|---|---|
| M0: Planning | Source review | This plan, tracker, agent instructions | Complete source coverage and actionable scope |
| M1: Project foundation | M0 | Verified editor, input/camera, graybox harbor, data skeleton | Play Mode shows/selects port without errors |
| M2: First shipment | M1 | Contracts, economy, ship presentation, safe route, ledger | Fresh run earns exactly 450 credits after first shipment |
| M3: One port upgrade | M2 | Atomic purchase, two-tier visuals/stats/unlocks | Exactly one upgrade leaves 150 credits and unlocks two contracts |
| M4: Upgraded voyage | M3 | Two destinations, four routes, five events | All event choices reachable and deterministic |
| M5: GSA and ending | M4 | Review, consequences, run summary, replay | Clean/watch/restricted runs all complete |
| M6: Save and UX resilience | M2-M5 incrementally | Save/resume, input/modal protection, settings/tutorial | Resumes at decisions without duplicated transactions |
| M7: Art/audio and performance | M5; graybox assets from M1 | Final demo presentation and optimized harbor | Both tiers readable; measured performance recorded |
| M8: Validation and packaging | M6-M7 | Tested Windows/WebGL candidates, playtest evidence, build notes | Definition of done satisfied; known issues explicit |

Save interfaces and stable IDs begin in M1/M2, not as a retrofit after M5. M6 is the integration/hardening gate. Likewise, correctness tests accompany each relevant feature; M8 consolidates release validation.

### 13.1 Suggested work packages

- **Engineering/design owner:** Ahad or assigned developer, covering simulation, UI integration, tests, and builds.
- **Art owner:** Uzair or assigned artist, covering port/ship assets, materials, camera readability, and upgrade variants.
- **Shared review:** balance, tutorial clarity, branch outcomes, and playtest observations.

These are suggested responsibilities based on the pitch, not a new assignment or permission to message the founders.

For part-time production, plan short work sessions around the small tasks. Establish velocity after M1/M2, then estimate remaining time. Do not commit to a calendar date or assume the entire deck's six-month program is needed for this slice.

### 13.2 Scope reduction order if time is tight

Cut decorative props, elaborate water, ambient workers, nonessential animations, and recording polish first. Keep both tiers, the visible upgrade, all specified content definitions, decision consequences, and a complete ending.

If platform work blocks WebGL, retain a working Windows build and explicitly record the WebGL blocker. Do not label WebGL done without testing it. Changing required content counts or removing GSA needs a documented scope decision from the user.

## 14. Verification strategy

### 14.1 Edit Mode rules tests

1. Dispatch eligibility: correct tier/range/capacity/funds; rejects duplicate voyage.
2. First contract arithmetic: 200 -> 150 on dispatch -> 450 on settlement.
3. Upgrade arithmetic: 450 -> 150, tier 2, new capacity/range; repeat purchase rejected.
4. All event effects match definitions; repeated resolution has no extra effect.
5. Both upgraded contracts, all four routes, all operational choices, and all inspection choices resolve with nonnegative credits.
6. Reputation/suspicion clamping and severity-driven GSA outcomes.
7. Review and delivery idempotency, including save/resume boundaries.
8. Save schema/ID/tier validation and corrupted-save recovery.
9. Tier 3 is rejected by progression, data validation, and save loading.
10. Template data contains exactly the intended three cargos, three destinations, and five event types.

Use data-driven tests for genuine decision branches. Avoid brittle tests that only duplicate UI implementation details.

### 14.2 Play Mode integration tests

- Start a new run, complete the first shipment, upgrade, complete the second voyage, resolve GSA, reach ending.
- Test all three GSA branches and at least one route for each operational event.
- UI over the dock does not select the dock; event modal blocks world actions.
- Upgrade visuals/stats survive save/load and scene reinitialization.
- Save while an event is pending, after selecting a choice, and before/after settlement; restore without loss or duplicate application.
- Pause/resume and focus changes do not silently fast-forward the voyage.
- Repeated New Run resets events, credits, subscriptions, selection, and tutorial state.

### 14.3 Manual acceptance matrix

| Test | Expected result |
|---|---|
| First-time clean run | Complete loop understandable; target 5-8 minutes; Tier 2 achieved once |
| Textiles/sheltered/wait/clean | 470 final credits, 65 reputation, 0 suspicion, Clear |
| Textiles/sheltered/wait/unofficial fee | 490 credits, 45 reputation, 25 suspicion, Watch |
| Textiles/sheltered/wait/risk shipment | 490 credits, 30 reputation, 50 suspicion, Restricted |
| Machine-parts alternative | Correct 20-unit cargo and 400 base payout; distinct route incidents |
| Every route/operational choice | Correct cost/delay/condition; no unresolved event or dead end |
| Locked upgrade/contracts | Clear explanation; no partial debit or hidden unlock |
| Rapid double-click on dispatch/upgrade/choice | One mutation and one transaction only |
| Reload at all major checkpoints | Valid continuation; no repeat reward, fee, upgrade, or review |
| Damaged ship return | Delivery settles and service completes; ending remains reachable |
| Low-resolution UI / zoom bounds | All mandatory controls readable and reachable |
| Quit and Continue | Same run, state, and choices restored |
| Broken/old save | Recovery path works; new run remains accessible |
| Five successive replays | No stale state, growing event subscriptions, or material memory leak |
| Windows candidate | Fresh-machine-style launch from packaged folder; no missing assets |
| WebGL candidate | Hosted load, input, audio, save behavior, pause, and resize verified |

### 14.4 Balance and playtesting

Start with 5-8 first-time testers as a small directional sample, not statistically validated market evidence. Observe without coaching except for blockers.

Record in `docs/playtests.md`: build ID, platform/hardware, session time, chosen routes/choices, final metrics, points of confusion, softlocks, and player comments. Suggested validation targets:

- At least 4 of the first 5 testers complete without developer intervention.
- Median guided completion is 5-8 minutes.
- At least 4 of 5 can explain both the upgrade benefit and one GSA consequence.
- Testers notice the Tier 2 visual change without prompting.
- At least some testers want to replay a different route or compliance choice.

Ask: "What did the upgrade change?", "Why did GSA react that way?", "What would you choose differently?", and "Where did you feel you were waiting rather than deciding?"

No external analytics SDK is necessary. A local run summary can capture timing and choices for opt-in playtests. Do not collect personal data by default.

## 15. Risks, assumptions, and mitigation

| Risk / uncertainty | Proposed response |
|---|---|
| Deck says prototype exists but checkout is a template | Verify any alternate branch/source if supplied; use inspected files as current implementation truth. |
| "One upgrade level" could mean separate upgrades per building | Default to one bundled port upgrade, clearly labeled as a design choice; maintain exactly two tiers if revised. |
| Art ambitions exceed two part-time founders | Graybox complete loop first; reuse modular kit and a single ship. |
| Route timers recreate passive tycoon gameplay | Short travel, route choices, two event decisions, inspectable port, no offline timers. |
| Too many systems overwhelm a short demo | Teach credits first, unlock risk/GSA after upgrade, show one objective at a time. |
| Risk branch becomes the only rational choice | Keep clean profit healthy; show reputation/permit consequences and test player interpretation. |
| GSA feels scripted or arbitrary | Explicitly label scheduled review and connect evidence to each outcome; broader simulation is deferred. |
| Financial/phase duplication after load | Atomic commands, stable transaction IDs, serializable event state, branch tests. |
| Browser performance or persistence fails | Early smoke build, simple rendering, platform storage adapter, honest known-issues record. |
| New art breaks scene references | Stable prefab anchors/GUIDs and asset replacement checks. |
| Scope grows toward a full Clash of Clans-like builder | No combat, free placement, multi-tier progression, or fleet economy in this slice. |
| Quota/context interruptions lose work | Frequent tracker updates and mandatory handoff-only mode at <=10% remaining usage. |

### 15.1 Defaults to revisit during implementation

These do not block planning or grayboxing:

- Whether the user prefers a single bundled upgrade or individually presented changes under the same single purchase.
- Final harbor art style, names, UI theme, and use of DEUXART LABS branding.
- Priority of WebGL versus Windows for the first external pitch.
- Availability of other prototype code or artist-owned assets.
- Actual part-time hours and target presentation date.

Until clarified, implement the stated defaults, not an expanded feature set. Record any approved design change in both this plan and the tracker.

## 16. Definition of done and delivery package

The demo is complete only when:

- [ ] A player can finish both Tier 1 and Tier 2 gameplay in a single coherent session.
- [ ] There is exactly one paid upgrade and no reachable third tier.
- [ ] Tier 2 visibly changes the harbor and changes real capacity/range/contract capabilities.
- [ ] One ship, three reachable destinations across runs, three cargo types, and five functioning event definitions exist.
- [ ] Upgraded routes and compliance choices affect the run with legible costs and consequences.
- [ ] Clean, Watch, and Restricted GSA branches all reach a valid ending.
- [ ] The first shipment always funds the upgrade and the second voyage without grinding.
- [ ] Save/resume, restart, pause, input gating, and settings behave correctly.
- [ ] There are no known progression blockers, duplicate financial transactions, or unhandled exceptions in tested flows.
- [ ] Windows and WebGL candidate status is recorded accurately; any platform deferral is explicit.
- [ ] Playtest and performance observations are recorded with their limits.
- [ ] Assets have known provenance and required credits.
- [ ] `progression.md`, build instructions, and known issues match the delivered build.

Expected implementation deliverables:

1. Unity project source, definitions, scenes, prefabs, assets, and relevant tests.
2. Windows demo package and validated WebGL package, subject to documented platform blockers.
3. `docs/build-and-run.md`: editor version, build targets, controls, save behavior, reset instructions, build IDs, known issues.
4. `docs/playtests.md` and `docs/asset-register.md`.
5. Short optional gameplay capture showing safe shipment -> upgrade -> risk -> GSA.
6. Updated `progression.md` with remaining non-demo work clearly deferred.

Creating packages does not by itself authorize publishing or uploading them.

Website planning amendment (2026-10-20): the user supplied sibling `OpenWaters-Web-repo` and requested a comprehensive plan only for a Founders Play Demo button, background preparation and a complete browser package housed in that repository. Canonical proposal: `../OpenWaters-Web-repo/docs/browser-demo-plan.md`. Use a stable same-origin player with immutable versioned assets, production Web build optimization, progressive prefetch and verified cache reuse. Proposed timing gates are ≤10 seconds cold immediate-click and ≤5 seconds prefetched/cached on an explicitly defined desktop/network profile; unrestricted connection speed cannot be guaranteed. The earlier 80 MB general budget does not establish this timing target. All WEB.1–WEB.8 implementation, packaging, testing and publishing remain TODO and require later authorization; no game scope changes. Record decision D13 in the tracker.

## 17. Agent execution and handoff contract

Use `progression.md` as the live source of implementation status:

- Before a small task: claim its ID, record objective/files/verification.
- After a small task: record what changed and evidence; only mark DONE if its exit condition was met.
- When blocked: record the exact error, attempts, and next viable action; continue unrelated authorized work if possible.
- Before every session ends: write a self-contained restart point.
- At **10% remaining usage or lower**: stop all implementation and work only on handoff notes, as required by `AGENTS.md`.

The tracker must let another agent continue without reading the whole conversation. Preserve source interpretation, decisions, unfinished edits, command results, and next actions. Planned features must never be presented as implemented features.

### Art production ordering amendment - 2026-10-20

The user requests model and animation production before gameplay. Begin asset-only M7 work now, retaining all gameplay dependencies and acceptance gates. Deliver assets under `Assets/Models` with named subfolders. Art direction: original chunky low-poly maritime forms inspired by the readability and visible growth of Clash of Clans, viewed at 45-degree yaw / 55-degree downward pitch. Animated water is a priority: coherent directional swells and designed foam detail using an economical URP shader; no expensive realtime reflections. This does not expand playable tiers, ship count, or destination scenes.

Website implementation amendment (2026-10-20): user now authorizes implementing the browser demo plan and preparing files for their GitHub push. Add a distinct production Brotli/DiskSizeLTO single-thread Web builder, preserve managed stripping/company/product/save identifiers, and bridge first menu/harbor frame and pause/save flush. Website uses the stable same-origin player and versioned complete package described in the canonical plan. Hosted performance gates remain evidence-based and unproven until measured; no deployment/push performed by the agent.

Production optimization decision (2026-10-21): initial DiskSizeLTO attempt never produced a completed artifact and no process remains. Use Unity DiskSize without LTO for the next production candidate; keep Brotli/single-thread/no-development settings and measure actual package size. This changes compiler optimization only; the15MB/loading goals and gameplay scope remain unchanged.
