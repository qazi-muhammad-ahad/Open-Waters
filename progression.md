# Open Waters - progression and handoff tracker

**Last updated:** 2026-10-20, 17:52 (Asia/Karachi)  
**Current status:** COMPLETE - requested planning deliverables reviewed and ready.  
**Current owner:** None active; Codex completed the planning session.  
**Implementation status:** NOT STARTED in this checkout.  
**Scope:** 3D top-down/isometric demo; one Tier 1 base + one Tier 2 upgrade; see `plan.md`.

## 1. Read-first restart summary

1. Read `AGENTS.md`, then this file, then the relevant `plan.md` sections.
2. The two supplied PDFs have been reviewed in full: 25-page pitch deck and 10-page applicant presentation, including image-only slides.
3. The repository is a Unity 6000.3.23f1 URP starter project. `Official_Game.unity` contains template camera/light/volume objects; no game-specific scripts, prefabs, or models were found.
4. Applicant presentation page 3 claims a playable prototype exists. That claim is **unverified in this checkout**; do not treat it as completed development.
5. The current user request was for planning and handoff files. Do not start gameplay implementation merely because this tracker contains an implementation backlog.
6. On a later implementation request, begin at M1.1 unless new repository evidence changes the starting point.

## 2. Active work / next actions

| Field | Current value |
|---|---|
| Active task | None; M0.1-M0.5 are complete |
| Objective | Current planning request fulfilled; gameplay implementation remains future work |
| Files being edited | None; three new planning documents remain uncommitted |
| Last completed step | Validated document structure/task IDs and all 24 proposed economy branches; removed temporary PDF review outputs |
| Immediate next action | On an implementation request, verify repository state and begin M1.1 |
| Blockers | None for planning |
| Running commands/processes | None; both PDF render processes completed successfully |
| Tests still needed this session | None for planning; Unity runtime/build checks remain unperformed |

**Next implementation actions, only after implementation is requested:**

1. Reinspect `git status`, current branch, and files; check whether prototype code was added elsewhere.
2. Open using Unity 6000.3.23f1 and record import/compile/Play Mode results.
3. Complete M1.2-M1.4: establish folders, input actions, bounded camera, and selectable graybox harbor.
4. Establish tier/content definitions and state/transaction contracts before building the first shipment.
5. Build the guaranteed-safe Tier 1 loop before any visual polish.

## 3. Quota and handoff status

**Mandatory rule:** At **10% remaining usage or less**, stop implementation immediately and focus solely on handoff files. See `AGENTS.md` for the full procedure.

| Observation | Value |
|---|---|
| Last quota observation | 2026-10-20, approximately 17:52 Asia/Karachi; account limits tool response |
| Active limit bucket | `codex` |
| Five-hour window | 25% used / 75% remaining at observation |
| Weekly window | 4% used / 96% remaining at observation |
| Threshold reached? | No, at that observation |
| Ongoing automatic monitor? | No; recheck at session start, milestone boundaries, and before long tasks |
| Handoff-only status | Not active |

Do not interpret this historical observation as current quota. Missing usage information means unknown. If a future check reaches the threshold, change status to `HANDOFF_ONLY_QUOTA`, document partial work and next steps, and end the implementation session.

## 4. Status vocabulary and update rules

- `TODO`: not started.
- `IN_PROGRESS`: a named owner is actively working on it; record partial state.
- `DONE`: exit condition verified with evidence.
- `BLOCKED`: record the dependency/error and what is needed.
- `DEFERRED`: deliberately outside current demo scope, with reason.

Checkboxes are for quick scanning; the status text and evidence remain authoritative. Update after each small milestone, not only at session end. Add new small tasks when discoveries require them, preserving existing IDs.

## 5. Small-task backlog

### M0 - Planning and source review

- [x] M0.1 `DONE` - Inspect repository, engine version, packages, build scene, and working tree.
- [x] M0.2 `DONE` - Read every page of both PDFs, including raster-only applicant slides.
- [x] M0.3 `DONE` - Specify two-tier scope, gameplay, economy, events, GSA, UI, architecture, and validation in `plan.md`.
- [x] M0.4 `DONE` - Create root `AGENTS.md` with startup workflow and mandatory <=10% remaining-quota handoff rule.
- [x] M0.5 `DONE` - Verified document structure, 82 unique sequential task IDs, and 24 economy branches; finalized handoff and removed temporary review artifacts.

### M1 - Foundation

- [ ] M1.1 `TODO` - Verify correct editor opens project; record baseline compilation and Play Mode result.
- [ ] M1.2 `TODO` - Create game folders/namespaces and Edit Mode/Play Mode test assemblies with stable metadata.
- [ ] M1.3 `TODO` - Review existing input actions; add select/pan/zoom/pause/recenter actions and UI input gating.
- [ ] M1.4 `TODO` - Create graybox harbor, five fixed building anchors, dock lane, and one placeholder ship.
- [ ] M1.5 `TODO` - Add elevated orthographic camera, bounded pan/zoom, and recenter.
- [ ] M1.6 `TODO` - Implement building/ship selection and minimal details panel.
- [ ] M1.7 `TODO` - Define tier, cargo, contract, route, event, and balance data with stable IDs.
- [ ] M1.8 `TODO` - Add authoritative session state, command/receipt pattern, and save DTO/interface.
- [ ] M1.9 `TODO` - Validate references/counts and confirm graybox Play Mode without errors.

**Exit:** selectable graybox port, usable camera, data foundations, clean baseline.

### M2 - Base-tier shipment

- [ ] M2.1 `TODO` - Initialize credits 200, reputation 50, suspicion 0, tier 1, one docked ship.
- [ ] M2.2 `TODO` - Implement idempotent economy transactions and affordability validation.
- [ ] M2.3 `TODO` - Author grain/Beacon Bay contract and locked previews of upgraded contracts.
- [ ] M2.4 `TODO` - Implement accept/cancel, capacity/range validation, preparation panel.
- [ ] M2.5 `TODO` - Implement route preview and one-time 50-credit dispatch charge.
- [ ] M2.6 `TODO` - Animate departure/return; add 35-second abstract voyage and route marker.
- [ ] M2.7 `TODO` - Implement one-time 300-credit settlement and +5 reputation ledger.
- [ ] M2.8 `TODO` - Add basic first-voyage tutorial objectives and hints.
- [ ] M2.9 `TODO` - Test eligibility, repeated commands, and 200 -> 150 -> 450 arithmetic.
- [ ] M2.10 `TODO` - Integrate first-voyage save checkpoints with stable transaction IDs.

**Exit:** fresh run reaches first-delivery ledger with 450 credits and 55 reputation.

### M3 - Single upgrade

- [ ] M3.1 `TODO` - Author Tier 1 and Tier 2 capability/visual definitions; reject any third tier.
- [ ] M3.2 `TODO` - Add upgrade preview, requirements, and 300-credit confirmation.
- [ ] M3.3 `TODO` - Implement one atomic upgrade command while docked after first delivery.
- [ ] M3.4 `TODO` - Swap all five graybox building variants and show brief upgrade reveal.
- [ ] M3.5 `TODO` - Apply warehouse 10 -> 20, range 1 -> 2, and repair improvement.
- [ ] M3.6 `TODO` - Unlock textiles and machine-parts contracts; show maximum demo tier.
- [ ] M3.7 `TODO` - Persist upgrade; test duplicate click/load and invalid tier handling.

**Exit:** one purchase leaves 150 credits; same ship; two unlocked contracts; no further upgrade.

### M4 - Upgraded voyage and five events

- [ ] M4.1 `TODO` - Author Copperport/textiles and Meridian Quay/machine-parts contracts.
- [ ] M4.2 `TODO` - Author four upgraded routes and cost/time/hazard previews.
- [ ] M4.3 `TODO` - Implement deterministic event queue and pause-safe choice modal.
- [ ] M4.4 `TODO` - Implement storm detour/push-through choices and effects.
- [ ] M4.5 `TODO` - Implement pirate detour/escort choices and effects.
- [ ] M4.6 `TODO` - Implement engine repair/limp-home and Tier 2 repair modifiers.
- [ ] M4.7 `TODO` - Implement congestion wait/priority choices and effects.
- [ ] M4.8 `TODO` - Implement inspection clean/unofficial-fee/risk choices and violation records.
- [ ] M4.9 `TODO` - Add event receipts, voyage delays, condition changes, and return service.
- [ ] M4.10 `TODO` - Include every deferred event fee/bonus in one-time delivery settlement.
- [ ] M4.11 `TODO` - Add development scenario fixtures for every event/choice.
- [ ] M4.12 `TODO` - Test all route/choice combinations for accounting and completion.
- [ ] M4.13 `TODO` - Save/resume pending events and resolved choices without reruns.

**Exit:** all three cargos/destinations and five event types work across replays/fixtures.

### M5 - GSA and completed demo

- [ ] M5.1 `TODO` - Add office compliance ledger and Clear/Watch/Restricted badges.
- [ ] M5.2 `TODO` - Schedule exactly one GSA review after second-delivery settlement.
- [ ] M5.3 `TODO` - Implement clean outcome: +40 credits, +5 reputation, Clear.
- [ ] M5.4 `TODO` - Implement minor-violation outcome: -60 credits, -10 reputation, Watch.
- [ ] M5.5 `TODO` - Implement major-violation outcome: -180 credits, -20 reputation, Restricted.
- [ ] M5.6 `TODO` - Explain outcome evidence and show permit/teaser consequences.
- [ ] M5.7 `TODO` - Implement ending summary, Inspect Port, New Run, and menu flow.
- [ ] M5.8 `TODO` - Prevent third voyage/third tier and handle negative review as valid completion.
- [ ] M5.9 `TODO` - Verify all three worked-example endings and idempotent review.

**Exit:** both-tier loop and all three GSA endings are playable from a fresh run.

### M6 - Persistence and usability hardening

- [ ] M6.1 `TODO` - Complete versioned save validation, backup, and recovery messages.
- [ ] M6.2 `TODO` - Integrate main menu, Continue, new-run confirmation, settings persistence.
- [ ] M6.3 `TODO` - Verify snapshots at dispatch, pending/resolved events, settlement, upgrade, audit, ending.
- [ ] M6.4 `TODO` - Add periodic travel checkpoints, application-pause handling, no offline advancement.
- [ ] M6.5 `TODO` - Test pause, Escape behavior, modal focus, UI/world input isolation.
- [ ] M6.6 `TODO` - Add keyboard focus, sufficient contrast, text/icon risk cues, reduced-motion option.
- [ ] M6.7 `TODO` - Test layout and required buttons at 720p/1080p and aspect-ratio changes.
- [ ] M6.8 `TODO` - Tune tutorial copy and net-profit previews with observed player confusion.
- [ ] M6.9 `TODO` - Verify five consecutive new runs and save/resume failure cases.

**Exit:** robust two-tier loop with resumable decisions and no duplicate transactions.

### M7 - Visual/audio production and profiling

- [ ] M7.1 `TODO` - Agree asset scale, pivots, camera framing, silhouettes, and style.
- [ ] M7.2 `TODO` - Produce/replace dock and warehouse Tier 1/Tier 2 assets.
- [ ] M7.3 `TODO` - Produce/replace fuel, office, and repair Tier 1/Tier 2 assets.
- [ ] M7.4 `TODO` - Produce one freighter and three cargo visual/icon sets.
- [ ] M7.5 `TODO` - Add simple harbor water/coast, reusable dressing, and lighting.
- [ ] M7.6 `TODO` - Add selection, departure/arrival, upgrade, and five event feedback cues.
- [ ] M7.7 `TODO` - Add ambience/SFX, volume settings, mute, and browser-gesture audio handling.
- [ ] M7.8 `TODO` - Create asset provenance/attribution register; verify distribution rights.
- [ ] M7.9 `TODO` - Profile CPU/GPU/memory, optimize measured problems, record hardware.
- [ ] M7.10 `TODO` - Verify tier readability and UI legibility using final assets.

**Exit:** readable, visibly improved harbor and measured performance with licensed assets.

### M8 - Release validation and packaging

- [ ] M8.1 `TODO` - Run relevant Edit Mode and Play Mode suites; record logs/results.
- [ ] M8.2 `TODO` - Run all route/event/GSA/manual acceptance cases from `plan.md`.
- [ ] M8.3 `TODO` - Build Windows x64 and verify from packaged output.
- [ ] M8.4 `TODO` - Produce early WebGL smoke build; verify module/platform setup before final polish.
- [ ] M8.5 `TODO` - Validate WebGL hosting/compression, input/audio, storage, focus, resize, and performance.
- [ ] M8.6 `TODO` - Conduct small first-time playtest round; record time, comprehension, and blockers.
- [ ] M8.7 `TODO` - Fix measured blockers/balance issues and rerun affected checks.
- [ ] M8.8 `TODO` - Write build/run/control/save/reset/known-issue documentation.
- [ ] M8.9 `TODO` - Optional: capture a short safe-voyage -> upgrade -> risk -> GSA clip.
- [ ] M8.10 `TODO` - Finalize build IDs, artifact locations, definition-of-done checklist, and handoff.

**Exit:** verified demo packages and truthful release notes. Publishing remains separate work.

**Scheduling note:** M8.4 should be pulled forward once the first playable loop exists to expose platform issues early. Its grouping here is for release tracking, not a reason to delay platform discovery.

## 6. Decision log

| ID | Date | Decision / rationale | Status |
|---|---|---|---|
| D01 | 2026-10-20 | Use existing Unity 6000.3.23f1 URP project; no engine/package migration needed for planning. | Proposed implementation baseline |
| D02 | 2026-10-20 | Exactly two port tiers, reached by one 300-credit bundled upgrade. | Proposed interpretation of user's scope |
| D03 | 2026-10-20 | One ship with fixed 20-unit capacity; warehouse unlocks 10 -> 20 effective cargo. | Proposed balance |
| D04 | 2026-10-20 | Fixed functional building locations; freeform construction deferred. | Proposed scope control |
| D05 | 2026-10-20 | Two voyages per guided run; three destinations/cargos accessible across replays. | Proposed pacing |
| D06 | 2026-10-20 | Five implemented events; one operational event plus inspection on second voyage. | Proposed scope/pacing |
| D07 | 2026-10-20 | Deterministic teaching review, based on recorded violations; no hidden audit roll. | Proposed clarity/testing |
| D08 | 2026-10-20 | Event costs deducted from delivery proceeds; dispatch/upgrade costs paid upfront. | Proposed softlock prevention |
| D09 | 2026-10-20 | Windows first; validate WebGL candidate; mobile release deferred. | Proposed delivery order |
| D10 | 2026-10-20 | Pitch prototype-exists claim is not implementation evidence for this checkout. | Verified discrepancy |
| D11 | 2026-10-20 | Stop implementation at <=10% remaining quota and write handoff only. | Explicit user requirement |
| D12 | 2026-10-20 | Canonical root agent file is `AGENTS.md`; do not create a case-only duplicate `agents.md` on Windows. | Repository convention |

## 7. Evidence and checks

### Completed source/repository checks

- Read all 25 pages of the pitch deck using extracted text and rendered-page overview.
- Read all 10 applicant pages, individually inspecting full-size image-only pages 1, 3, 6, 7, 9, and 10.
- `ProjectSettings/ProjectVersion.txt`: 6000.3.23f1.
- `Packages/manifest.json`: URP/Input System/Test Framework/uGUI baseline recorded in plan.
- `ProjectSettings/EditorBuildSettings.asset`: `Assets/Scenes/Official_Game.unity` enabled.
- Scene serialized names: Main Camera, Directional Light, Global Volume.
- Source inventory: only template readme C# scripts; no game-specific prefabs/models/scripts found.
- Initial `git status --short`: clean; branch `Ahad`; HEAD `792c607`.
- Required editor version found under `C:\Program Files\Unity\Hub\Editor`.
- Both PDF rendering processes finished with exit code 0.

### Completed planning validation

- Document script confirmed balanced code fences and 82 unique sequential task IDs across M0-M8.
- Programmatic design check covered all 24 upgraded route / operational-choice / compliance-choice combinations.
- Proposed final credits ranged from 440 to 550; no branch requires a negative balance.
- Worked textiles/sheltered/wait endings matched the plan: clean 470/65/0, unofficial fee 490/45/25, risk 490/30/50 (credits/reputation/suspicion).
- Reviewed settlement/service ordering and clarified that delivery pays before dock service, so service cannot block GSA or the ending.
- Final source-control status contains only the three new requested Markdown files; no existing tracked files changed.
- Temporary PDF images/text were removed after verifying the resolved cleanup path and expected filenames.
- These checks validated the proposed design and documents, not a Unity implementation. Validation was run inline in the terminal; no persistent test suite or test logs were created.

### Not run / not claimed

- Unity editor import/compilation/Play Mode.
- Game tests, platform builds, performance measurements, or playtests.
- Verification of a prototype on another branch or machine.
- Current market, pricing, Unity license terms, or commercial asset rights research.

## 8. Files and workspace handoff

| Path | Current role |
|---|---|
| `AGENTS.md` | New root agent instructions and quota handoff policy |
| `plan.md` | New comprehensive proposed demo design/implementation plan |
| `progression.md` | This live tracker |
| `tmp/pdfs/` | Removed after full review; no temporary artifacts remain |
| `Assets/Scenes/Official_Game.unity` | Inspected only; unchanged |
| `Packages/manifest.json` | Inspected only; unchanged |
| `ProjectSettings/` | Inspected only; unchanged |

No commits, branches, dependency changes, external uploads, or game implementation were made by this planning task.

## 9. Open questions and known limitations

- Alternate existing prototype code/assets may exist outside the inspected checkout; no location was supplied.
- Bundled upgrade, names, prices, timing, and fixed plots are defaults, not user-approved final balance.
- Final art style, delivery date, founder availability, and platform priority remain open.
- A future agent needs to validate Unity compilation and installed build modules.
- Source PDFs live in this user's Downloads directory; they are not portable repository dependencies.
- Quota reporting is point-in-time; the policy cannot guarantee automatic interruption between checks.

None of these prevents completion of the current planning request.

## 10. Deferred work

`DEFERRED`: third tier, extra upgrade ladders, free placement, additional ships/classes, more regions, randomized audit economy, multiplayer, mobile builds, cloud saves, monetization, live markets, combat, public publishing, community management, and financial forecasting.

Do not quietly reintroduce these through an implementation task. Capture future ideas without making them prerequisites for this demo.

## 11. Session log

### 2026-10-20 - Planning baseline

- User requested complete pitch review, comprehensive two-tier demo plan, durable progress tracking, and 10%-remaining quota handoff instructions.
- Inspected the Unity starter project and both full PDFs.
- Found the applicant deck's existing-prototype claim differs from this checkout's contents.
- Wrote a complete demo specification with one bundled upgrade, deterministic branches, concrete economy, architecture, tests, art/build scope, and milestones.
- Created root agent instructions and this small-task backlog.
- Completed document and proposed-economy checks; 82 task IDs tracked, with five planning tasks DONE and 77 future implementation tasks TODO.
- Removed temporary review outputs; only `AGENTS.md`, `plan.md`, and `progression.md` are new.
- Rechecked quota at approximately 17:52 Asia/Karachi: 75% remaining in the five-hour window, 96% weekly. No quota handoff threshold was reached.
- Planning request is complete. No Unity gameplay was implemented or claimed tested.

## 12. Reusable handoff template

Copy/update this information in the active-work section whenever another agent must take over:

```text
Timestamp / timezone:
Agent / current user objective:
Status: IN_PROGRESS | BLOCKED | HANDOFF_ONLY_QUOTA | COMPLETE
Quota source / observation / limiting window / remaining %:
Branch / HEAD / working-tree changes:
Active milestone and small-task ID:
Last completed action and evidence:
Partial edits and exact paths:
Tests/builds run, outcomes, and log locations:
Checks not run:
Decisions/assumptions changed:
Blocker/error and attempts:
Running commands/process IDs:
Temporary artifacts/cleanup:
Next 3-5 concrete actions:
Do not repeat / avoid:
```
