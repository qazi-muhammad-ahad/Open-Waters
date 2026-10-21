# Open Waters - progression and handoff tracker

**Last updated:** 2026-10-21 (Asia/Karachi)
**Current status:** WEB_DEMO_DEPLOYED - browser integration and live smoke passed; broader game release gates remain documented.  
**Current owner:** Codex integration; coordinating GPT-6.1 Sol Medium display/preferences work finished.  
**Implementation status:** Playable Windows and WebGL candidates built; release/manual gates remain open.  
**Scope:** 3D top-down/isometric demo; one Tier 1 base + one Tier 2 upgrade; see `plan.md`.

## 1. Read-first restart summary

1. Read `AGENTS.md`, then this file, then the relevant `plan.md` sections.
2. The two supplied PDFs have been reviewed in full: 25-page pitch deck and 10-page applicant presentation, including image-only slides.
3. Unity 6000.3.23f1 URP baseline retained. Gameplay now lives in Assets/OpenWaters and is wired into Official_Game; the original starter-only observation is historical.
4. Applicant presentation page 3 claims a playable prototype exists. That claim is **unverified in this checkout**; do not treat it as completed development.
5. Current request authorizes implementing the sibling website browser-demo-plan.md; preserve existing gameplay scope and contributor changes.
6. Art delivery and socket correction are complete; earlier quota handoffs below are historical.

## 2. Active work / next actions
### Current quota handoff — 2026-10-21 00:04 Asia/Karachi
- **HANDOFF_ONLY_QUOTA:** user overrides this chat only to4% reserve; observed five-hour96% used/4% remaining, weekly30% used/70% remaining. Stop all implementation/tests/new builds. Default10% applies in later chats unless overridden.
- Objective: implement sibling website `docs/browser-demo-plan.md` for user to push. NOT DONE/not deployable: no website binary release/current.json imported. Complete restart snapshot in `../OpenWaters-Web-repo/progression.md`.
- This session wrote production builder in `Assets/OpenWaters/Editor/DemoBuilder.cs` (Brotli, non-development, DiskSizeLTO, data cache, single thread; release-info.json), lifecycle/readiness/audio changes in `Assets/OpenWaters/Scripts/UI/DemoController.cs`, new `Assets/OpenWaters/Scripts/BrowserDemo.jslib` and `.meta`; updated plan/tracker. Preserved company/product/save keys, game content and existing contributor scene/art/source/settings changes on Ahad. Unity applied production project settings. No commit/push/deployment.
- First production build STILL RUNNING: Unity PID27416, wasm-ld PID25292, `Logs/browser-release-build.log`. Last compiler CPU about1460s and increasing; no final build result. Do not interrupt user-owned unity PID15000. The running candidate compiled BEFORE final save-flush/harbor/audio/Preserve edits and release-info emission; must rebuild final source after current process exits. Do not import it as final.
- Queued final rebuild command session29610 canceled via Ctrl+C at threshold; final-build log absent at last observation. No automatic rebuild should launch. Website server session3206 stopped; validation browser tab closed. No subagents.
- Website source types/Vite compile and2/2 package contract tests passed. Missing-package verifier correctly fails. Browser absent-package error/close/focus restoration passed. NO new Unity test suite run; production game/runtime/lifecycle/storage/performance unverified. Historical game tests below do not validate new bridge.
- Next after quota permits:1) inspect running build log/PID;2) after exit run Unity BuildWebRelease again with `Logs/browser-release-final-build.log`;3) website importer from Builds/WebRelease under new immutable ID, verify<=15MB package;4) final website verifier/types/build and real browser load/play/save/close/reopen/full-guided-run checks;5) record hosted/timing limitations and prepare user push only after actual local acceptance. Exact commands/files in website tracker.


### Browser demo implementation - 2026-10-20
- WEB.1/WEB.2 IN_PROGRESS: inventory confirmed; adding separate production Web builder plus minimal menu-ready/pause/save bridge. Verify build output, then package in sibling website. Existing development builder retained.
- Session start quota: five-hour 77% used / 23% remaining; weekly 27% used / 73% remaining. Default 10% reserve applies.
- No deployment/push authorized. Existing dirty scene/art/settings/source preserved.


### Website demo integration planning - 2026-10-20
- WEB.PLAN `DONE`: comprehensive planning only for a Founders Play Demo button, browser preloading, and a self-contained browser package in sibling `OpenWaters-Web-repo`. No implementation, packaging, builds, commits, or deployment performed or authorized this session.
- Delivered `../OpenWaters-Web-repo/docs/browser-demo-plan.md` and its `progression.md`: repository-specific architecture, loading budgets, packaging/hosting steps, acceptance gates and update/rollback procedure. Updated this game's `plan.md` performance target and website amendment. WEB.1–WEB.8 remain TODO.
- Initial observation: game branch `Ahad` has existing source/art/settings/docs changes; website branch `main` is clean. Preserve all existing work. Founders is `src/components/Team.tsx`; website uses React/Vite and Vercel configuration.
- Verification: source/configuration inspection and official Unity/Vite/Vercel documentation only. Current `Builds/WebGL` has 7 files totaling 115,903,599 bytes; builder uses Development mode. No new tests, builds or browser measurements. Missing `.github/workflows` and two incorrect guesses for SaveService path produced read errors; save-path compatibility remains a future test, not verified here.
- Session changes in game repo: only `plan.md` and `progression.md`; all prior dirty/untracked source/art/settings/docs preserved. Website adds only its plan and tracker. No operations/processes started; no cleanup needed.
- Latest quota observation: 2026-10-20 23:27 Asia/Karachi, five-hour 73% used / 27% remaining; weekly 26% used / 74% remaining. Default 10% reserve applies. Final documentation/status inspection confirmed website changes are only its two new Markdown files; existing game changes remain preserved.
- Next: (1) wait for a later implementation request; (2) recheck quota/status and read website plan; (3) begin WEB.1 baseline/deployment verification, then WEB.2 production size work; (4) package and integrate only after those gates. Live hosting configuration, compressed size and 5–10 second timing remain unverified. Prior gameplay acceptance tasks below are unchanged.

### Feedback iteration active - 2026-10-20
- Latest milestone: compact floating UI, 20 original pictograms, opt-in tips/details, cyan selection +gold next-action footprints, native display preferences, shared branding, and full offscreen ship departure/slow dock approach implemented. No economy/scope expansion.
- Canonical logo is now `Assets/OpenWaters/Resources/Branding/Logo.png`; retain its `.meta` when replacing it. Editor reimport updates UI/app icon settings; rebuilding updates Windows icon and WebGL loader/favicon. See `docs/branding.md`.
- Validation: **53/53 Edit Mode**, **5/5 final Play Mode** (`Logs/ux-playmode-final.xml`), plus **1/1** extra pointer/render check. Captures cover 16:9,4:3,ultrawide. Square browser testing found objective/header overlap; source corrected and final five tests passed afterward.
- Latest Windows build succeeded: 185,992,464 reported bytes,0errors/2warnings,38.24s; `Logs/ux-windows-release-build.log`. Final WebGL correction build succeeded:115,882,104bytes,0errors/2warnings,1m44s; `Logs/ux-webgl-final-build.log`. Both builders have exited; active editor target is WebGL.
- Browser local smoke: branded loader/menu, keyboard input, shipment preparation/dispatch, offshore ship absence, first delivery450credits, reload/Continue restored voyage; no captured warning/error logs. `Logs/ux-browser-offshore.jpg`. Hosted origin/audio gesture/full browser matrix still open.
- Focused Windows display fixture PASS both exclusive/borderless (`Logs/ux-visible-smoke.json`):109.29s normal clock,470credits/65reputation/0suspicion/Clear,0loggederrors,native1920x1080,tipsfalse/logotrue. Hidden/background runs fell back with DXGI887a0022. Final Windows gameplay smoke also PASS (`Logs/ux-shipping-smoke.json`), startupExclusiveFullScreen. Normal-clock automated time is not human pacing evidence.
- Current quota: 2026-10-20 23:11, five-hour51%used/49%remaining; weekly23%used/77%remaining. No threshold reached. User authorized5% reserve exception for this instance; default10% remains for later sessions.
- Final WebGL package loaded and Continue restored first-delivery state; inspected889x884 viewport with corrected header/objective, docked ship and gold upgrade attention footprint. No captured warning/error logs. Final proof: `Logs/ux-browser-final.jpg`. Temporary tab closed and viewport override reset; loopback server stopped. No agent-owned Unity/player/server operations remain.
- User playtest: UI too text-heavy; gameplay not understood. Risk choice and GSA consequence made sense. Replace persistent information wall with compact floating panels, one current action, optional tips/details, and clear selection/attention highlights.
- UX1-UX4 DONE: UI simplification/icons/selection, native fullscreen settings, shared branding and rebuilt Windows validation. M6.8 remains IN_PROGRESS until follow-up human comprehension feedback.
- Root owns DemoController, harbor highlights, branding, integration tests and docs. Coordinating GPT-6.1 Sol Medium agent owns display preferences/service + builder display defaults; no parallel Unity processes.
- Quota restart was verified; latest observation above governs. User-authorized5% instance reserve retained; default10% applies to future sessions.
- Prior WebGL build completed successfully: 115,725,026 reported bytes, zero errors /2 warnings, 9m15s; browser runtime was unverified at that earlier point and is superseded by the local smoke above.
- Logo placeholder delivered at Assets/OpenWaters/Resources/Branding/Logo.png; rebuilding installs updated package branding.
- Future only: deploy demo/development tracking to the user's game-website repository; record requirements, no repository or hosting changes yet.
- Next3 actions: (1) collect a fresh human run on Builds/Windows/OpenWaters.exe and record comprehension/completion time in docs/playtests.md; (2) finish M8.5 hosted-browser/audio/focus/storage matrix; (3) measure M7.9 sustained performance and close release gates against evidence. Website deployment remains future work pending repository/workflow details. Source remains uncommitted on Ahad; prior art/settings changes preserved.

Historical prior-session handoff follows:


### Current gameplay handoff - 2026-10-20 22:26 Asia/Karachi

**HANDOFF_ONLY_QUOTA.** The user authorized the full plan and a GPT-6.1 Sol Medium coordinating agent, then explicitly changed this instance's reserve to 5% remaining. Observed 95% used / 5% remaining in five-hour window; weekly 15% used / 85% remaining. Stopped implementation immediately. Default 10% reserve remains for future sessions unless explicitly overridden again; recheck usage before resuming.

**Last completed step:** final tested Windows x64 development package and isolated packaged smoke, followed by starting the first WebGL candidate build. WebGL has no final result at the last observation. No source or test changes after observing the quota threshold.

**Delivered and verified:**
- Exactly two tiers, two voyages, one freighter, three cargos/destinations/contracts, five routes, five events /11 choices, one scheduled GSA review. Runtime rules/receipts/ledgers are independent from UI.
- Contract accept/cancel, dispatch affordability/capacity/range, deterministic event queue and deferred costs, one atomic upgrade, service after delivery, Clear/Watch/Restricted endings, Inspect Port/New Run/menu, persistence/backup recovery, settings and input gating.
- Original art integrated into Official_Game with cargo/wake/smoke attachments, tier swaps, selection/loading/arrival/upgrade cues, pan/zoom/recenter and scrollable keyboard-aware interface. Original procedural sound cues; browser audio gate remains open.
- **50/50 Edit Mode PASS**: all 24 branch combinations, worked endings, checkpoint restoration/idempotency, corrupt schema/ID/phase/ledger/receipt rejection and backup-preserving writes. `Logs/gameplay-editmode.xml` / `.log`.
- **3/3 Play Mode PASS**: real scene clean run, five New Run resets, UI commands and all ending presentations, mandatory-decision pause/focus, synthetic pointer selection and UI/world isolation. `Logs/gameplay-playmode.xml` / `.log`.
- **Final Windows build SUCCEEDED**, 183,558,948 reported bytes, 0 errors /2 warnings, 18.5s incremental. `Builds/Windows/OpenWaters.exe` plus all sibling runtime/data files; `Logs/gameplay-windows-build.log`, `Logs/build-StandaloneWindows64.txt`.
- **Final packaged fast smoke PASS**, zero logged errors; 470 credits /65 reputation /0 suspicion /Clear. Actual packaged UI render inspected. `Logs/windows-final-smoke.json`, `.json.png`, `windows-final-player.log`.
- Renders inspected at 720p/1080p. Metadata audit found no missing file/folder metadata under Assets/OpenWaters. These do not establish every aspect ratio, human comprehension or 5-8 minute pacing.

**Current unfinished substep / running operation:**
- M8.4 WebGL build: Unity PID **12784**, spawned with `-batchmode -buildTarget WebGL -quit -projectPath <repo> -executeMethod OpenWaters.Editor.DemoBuilder.BuildWeb -logFile <repo>/Logs/gameplay-webgl-build.log`. First browser-target reimport/shader pass completed; Bee native/Wasm compilation remains active. Last observed `Builds/WebGL/Build/WebGL.data` = 6,896,260 bytes; no final Wasm/index or build summary yet.
- Compiler CPU increases confirmed before handoff; no evidence of a hung process. Leave the agent-started build to finish; do not launch another Unity against this checkout while it owns the project. No build kill or further tests attempted at threshold. Active editor target may now be WebGL.
- No loopback server or browser tab started. In-app browser is available; its documentation was loaded. No active subagents or packaged player processes remain. No user-owned processes were stopped.

**Workspace:** branch `Ahad`, observed HEAD `4478907`, no agent commits. Existing contributor/art changes preserved. Changed/untracked inventory known at last observation:
- New `Assets/OpenWaters.meta` and `Assets/OpenWaters/`: runtime asmdef, Core/DemoDefinitions.cs and GameSession.cs; Persistence/SaveService.cs; Presentation/HarborPresenter.cs and PackagedSmokeRunner.cs; UI/DemoController.cs, RouteMapGraphic.cs and ScrollToSelected.cs; Editor/DemoBuilder.cs; Data/Demo.asset; EditMode/PlayMode tests and all metadata.
- Modified `Assets/Scenes/Official_Game.unity` with preserved scene GUID.
- Prior/new art `Assets/Models.meta` and `Assets/Models/` preserved; this session also clarified Models/README.md's historical asset-only limits.
- Unity build/import serialization changes: `Assets/Settings/DefaultVolumeProfile.asset`, `Mobile_RPAsset.asset`, `PC_RPAsset.asset`, `UniversalRenderPipelineGlobalSettings.asset`; `ProjectSettings/ProjectSettings.asset` (intentional window/resolution/frame-timing changes plus Input System preloaded asset and Unity batching serialization). EditorBuildSettings/GraphicsSettings/ShaderGraphSettings also appear modified at status level; some are line-ending-only. Review actual diffs, do not discard prior contributor work.
- Prior `.vscode/settings.json`; updated `plan.md`, `progression.md`; new/updated `docs/asset-register.md`, `build-and-run.md`, `playtests.md`, `validation-summary.md`; new `tools/serve_webgl.py`. Builds/Logs/Library remain ignored.

**Fixed findings / decisions:** aggregate immutable definition rows in Demo.asset; no package/engine migration. JsonUtility empty-voyage normalization is intentionally narrow. Backup recovery preserves the good backup when corrupt primary is replaced. Authored transaction/decision consistency is checked; event trigger comparisons allow 0.0001s JSON/float rounding. Pointer fixture uses temporary IgnoreFocus/editor routing and restores settings; production focus pauses gameplay. Initial batch screenshot failure was fixed with explicit camera/UI texture capture and superseded by the final passing packaged smoke. `git diff --check` only reported Unity blank YAML value and intentional Markdown hard-break/trailing-blank whitespace; no code whitespace issue claimed.

**Remaining gates (12 tasks open):** M6.7 arbitrary aspect changes; M6.8 observed-player tutorial tuning; M7.7 browser-gesture sound acceptance; M7.9 sustained CPU/GPU/memory profiling; M8.2 full manual matrix; M8.4 current WebGL build; M8.5 hosted browser behavior; M8.6 first-time human playtests; M8.7 measured-issue fixes; M8.8 final build docs; optional M8.9 capture; M8.10 final IDs/package/definition-of-done closure. Do not mark the entire plan complete.

Short fast-batch diagnostics observed ~67 MB allocated peak, 112 peak draw calls, ~15.7ms CPU average on Ryzen 5 3600 / Radeon RX 5600 XT; GPU timing unavailable. These are **not sustained performance-budget evidence**. No human first-time test result exists. Optional feedback requested from the user on the concrete Windows package; no response yet.

**Next 5 actions:**
1. Recheck account limits; honor the default 10% reserve in a later session unless the user explicitly extends the exception. Read this snapshot and inspect actual branch/files before edits.
2. Inspect `Logs/gameplay-webgl-build.log`, `Logs/build-WebGL.txt` if present and PID 12784 status. Record the completed result/errors and output size. Do not rerun/import concurrently or assume the partial data file is a package.
3. If WebGL succeeds, run Python `tools/serve_webgl.py Builds/WebGL --port 8765`, open `http://127.0.0.1:8765` in the in-app browser, and verify load, input, compression/MIME, gesture audio, save/Continue, focus and resize. Stop only the agent-started host afterward. If failed, diagnose the recorded compiler result and make a minimal fix.
4. Perform normal-speed Windows/browser profiling and pending manual cases; gather first-time playtest feedback in `docs/playtests.md`. Existing passing domain suites need reruns only for affected changes or new concerns.
5. Update this tracker and `docs/validation-summary.md` / `build-and-run.md`, retain exact package IDs/locations and unresolved issues, and close remaining gates only with actual evidence. No publishing authorized.

Prior art delivery remains verified: 45 separate prefab/FBX/source triplets, 18 animated prefabs and corrected Y-up ship attachment children. Historical notes below retain the art and gameplay work sequence; this current snapshot supersedes stale active-state claims.

## 3. Quota and handoff status

User authorized a **5% remaining** reserve exception for this instance. Default10% applies to later sessions. Latest observation 2026-10-20 23:11: five-hour51%used/49%remaining; weekly23%used/77%remaining. No threshold reached; implementation is authorized. Earlier HANDOFF_ONLY_QUOTA blocks are historical and superseded by the current snapshot. Telemetry is point-in-time, not an automatic monitor. Recheck at milestones and before costly runs.
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

- [x] M1.1 `DONE` - Verify correct editor opens project; record baseline compilation and Play Mode result.
- [x] M1.2 `DONE` - Create game folders/namespaces and Edit Mode/Play Mode test assemblies with stable metadata.
- [x] M1.3 `DONE` - Review existing input actions; add select/pan/zoom/pause/recenter actions and UI input gating.
- [x] M1.4 `DONE` - Create graybox harbor, five fixed building anchors, dock lane, and one placeholder ship.
- [x] M1.5 `DONE` - Add elevated orthographic camera, bounded pan/zoom, and recenter.
- [x] M1.6 `DONE` - Implement building/ship selection and minimal details panel.
- [x] M1.7 `DONE` - Define tier, cargo, contract, route, event, and balance data with stable IDs.
- [x] M1.8 `DONE` - Add authoritative session state, command/receipt pattern, and save DTO/interface.
- [x] M1.9 `DONE` - Validate references/counts and confirm graybox Play Mode without errors.

**Exit:** selectable graybox port, usable camera, data foundations, clean baseline.

### M2 - Base-tier shipment

- [x] M2.1 `DONE` - Initialize credits 200, reputation 50, suspicion 0, tier 1, one docked ship.
- [x] M2.2 `DONE` - Implement idempotent economy transactions and affordability validation.
- [x] M2.3 `DONE` - Author grain/Beacon Bay contract and locked previews of upgraded contracts.
- [x] M2.4 `DONE` - Implement accept/cancel, capacity/range validation, preparation panel.
- [x] M2.5 `DONE` - Implement route preview and one-time 50-credit dispatch charge.
- [x] M2.6 `DONE` - Animate departure/return; add 35-second abstract voyage and route marker.
- [x] M2.7 `DONE` - Implement one-time 300-credit settlement and +5 reputation ledger.
- [x] M2.8 `DONE` - Add basic first-voyage tutorial objectives and hints.
- [x] M2.9 `DONE` - Test eligibility, repeated commands, and 200 -> 150 -> 450 arithmetic.
- [x] M2.10 `DONE` - Integrate first-voyage save checkpoints with stable transaction IDs.

**Exit:** fresh run reaches first-delivery ledger with 450 credits and 55 reputation.

### M3 - Single upgrade

- [x] M3.1 `DONE` - Author Tier 1 and Tier 2 capability/visual definitions; reject any third tier.
- [x] M3.2 `DONE` - Add upgrade preview, requirements, and 300-credit confirmation.
- [x] M3.3 `DONE` - Implement one atomic upgrade command while docked after first delivery.
- [x] M3.4 `DONE` - Swap all five graybox building variants and show brief upgrade reveal.
- [x] M3.5 `DONE` - Apply warehouse 10 -> 20, range 1 -> 2, and repair improvement.
- [x] M3.6 `DONE` - Unlock textiles and machine-parts contracts; show maximum demo tier.
- [x] M3.7 `DONE` - Persist upgrade; test duplicate click/load and invalid tier handling.

**Exit:** one purchase leaves 150 credits; same ship; two unlocked contracts; no further upgrade.

### M4 - Upgraded voyage and five events

- [x] M4.1 `DONE` - Author Copperport/textiles and Meridian Quay/machine-parts contracts.
- [x] M4.2 `DONE` - Author four upgraded routes and cost/time/hazard previews.
- [x] M4.3 `DONE` - Implement deterministic event queue and pause-safe choice modal.
- [x] M4.4 `DONE` - Implement storm detour/push-through choices and effects.
- [x] M4.5 `DONE` - Implement pirate detour/escort choices and effects.
- [x] M4.6 `DONE` - Implement engine repair/limp-home and Tier 2 repair modifiers.
- [x] M4.7 `DONE` - Implement congestion wait/priority choices and effects.
- [x] M4.8 `DONE` - Implement inspection clean/unofficial-fee/risk choices and violation records.
- [x] M4.9 `DONE` - Add event receipts, voyage delays, condition changes, and return service.
- [x] M4.10 `DONE` - Include every deferred event fee/bonus in one-time delivery settlement.
- [x] M4.11 `DONE` - Add development scenario fixtures for every event/choice.
- [x] M4.12 `DONE` - Test all route/choice combinations for accounting and completion.
- [x] M4.13 `DONE` - Save/resume pending events and resolved choices without reruns.

**Exit:** all three cargos/destinations and five event types work across replays/fixtures.

### M5 - GSA and completed demo

- [x] M5.1 `DONE` - Add office compliance ledger and Clear/Watch/Restricted badges.
- [x] M5.2 `DONE` - Schedule exactly one GSA review after second-delivery settlement.
- [x] M5.3 `DONE` - Implement clean outcome: +40 credits, +5 reputation, Clear.
- [x] M5.4 `DONE` - Implement minor-violation outcome: -60 credits, -10 reputation, Watch.
- [x] M5.5 `DONE` - Implement major-violation outcome: -180 credits, -20 reputation, Restricted.
- [x] M5.6 `DONE` - Explain outcome evidence and show permit/teaser consequences.
- [x] M5.7 `DONE` - Implement ending summary, Inspect Port, New Run, and menu flow.
- [x] M5.8 `DONE` - Prevent third voyage/third tier and handle negative review as valid completion.
- [x] M5.9 `DONE` - Verify all three worked-example endings and idempotent review.

**Exit:** both-tier loop and all three GSA endings are playable from a fresh run.

### M6 - Persistence and usability hardening

- [x] M6.1 `DONE` - Complete versioned save validation, backup, and recovery messages.
- [x] M6.2 `DONE` - Integrate main menu, Continue, new-run confirmation, settings persistence.
- [x] M6.3 `DONE` - Verify snapshots at dispatch, pending/resolved events, settlement, upgrade, audit, ending.
- [x] M6.4 `DONE` - Add periodic travel checkpoints, application-pause handling, no offline advancement.
- [x] M6.5 `DONE` - Test pause, Escape behavior, modal focus, UI/world input isolation.
- [x] M6.6 `DONE` - Add keyboard focus, sufficient contrast, text/icon risk cues, reduced-motion option.
- [x] M6.7 `DONE` - Inspected720p/1080p,4:3,ultrawide and square browser layouts; corrected square objective overlap. Future devices remain manual acceptance.
- [ ] M6.8 `IN_PROGRESS` - Tune tutorial copy and net-profit previews with observed player confusion.
- [x] M6.9 `DONE` - Verify five consecutive new runs and save/resume failure cases.

**Exit:** robust two-tier loop with resumable decisions and no duplicate transactions.

### M7 - Visual/audio production and profiling

- [x] M7.1 `DONE` - Agree asset scale, pivots, camera framing, silhouettes, and style.
- [x] M7.2 `DONE` - Produce/replace dock and warehouse Tier 1/Tier 2 assets.
- [x] M7.3 `DONE` - Produce/replace fuel, office, and repair Tier 1/Tier 2 assets.
- [x] M7.4 `DONE` - Produce one freighter and three cargo visual/icon sets.
- [x] M7.5 `DONE` - Add simple harbor water/coast, reusable dressing, and lighting.
- [x] M7.6 `DONE` - Add selection, departure/arrival, upgrade, and five event feedback cues.
- [ ] M7.7 `IN_PROGRESS` - Add ambience/SFX, volume settings, mute, and browser-gesture audio handling.
- [x] M7.8 `DONE` - Create asset provenance/attribution register; verify distribution rights.
- [ ] M7.9 `IN_PROGRESS` - Profile CPU/GPU/memory, optimize measured problems, record hardware.
- [x] M7.10 `DONE` - Verify tier readability and UI legibility using final assets.

**Exit:** readable, visibly improved harbor and measured performance with licensed assets.

### M8 - Release validation and packaging

- [x] M8.1 `DONE` - Run relevant Edit Mode and Play Mode suites; record logs/results.
- [ ] M8.2 `TODO` - Run all route/event/GSA/manual acceptance cases from `plan.md`.
- [x] M8.3 `DONE` - Build Windows x64 and verify from packaged output.
- [x] M8.4 `DONE` - Windows/WebGL candidates built; branded WebGL local browser smoke passed. Hosted acceptance remains M8.5.
- [ ] M8.5 `IN_PROGRESS` - Local HTTP load/input/storage smoke passed. Hosted compression, audio, focus/resize matrix and performance remain.
- [ ] M8.6 `TODO` - Conduct small first-time playtest round; record time, comprehension, and blockers.
- [ ] M8.7 `TODO` - Fix measured blockers/balance issues and rerun affected checks.
- [ ] M8.8 `IN_PROGRESS` - Write build/run/control/save/reset/known-issue documentation.
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
| D13 | 2026-10-20 | Website planning only: self-contained versioned browser package in sibling web repo, stable same-origin player, progressive prefetch, measured ≤10s cold / ≤5s warm startup on declared profile; proposed ≤15 MB compressed startup budget. No implementation/deployment this session. | Plan delivered; WEB.1–WEB.8 TODO |

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

### 2026-10-20 - Asset production active snapshot

- User explicitly requests models and animations first, saved under `Assets/Models`, with original stylized isometric art and special attention to animated water. This supersedes the graybox-first ordering for art only.
- Branch: `Ahad`. Initial working tree: untracked `Assets/Models/Test.blend` and `Assets/Models/Untitled.blend`; preserve both.
- Verified Unity 6000.3.23f1, URP 17.3.0, Input System 1.20.0. Blender MCP responds with Blender 5.2.1; protocol 13 fallback supported (server expects 14).
- Active: M7.1 conventions/inventory, then M7.2-M7.6 asset-only portions and M7.8 provenance. No gameplay or audio scope implied.
- Intended outputs: original Blender sources, FBX interchange meshes/animations, shared materials, URP water shader/textures, preview imagery, asset manifest and integration notes.
- Verification: inspect isometric renders, check exports/geometry/animation loops, validate generated file inventory; Unity import/runtime must be separately verified.
- Quota observed 2026-10-20 around 21:10 Asia/Karachi: five-hour 1% used / 99% remaining; weekly 0% used / 100% remaining. No threshold reached.
- Next: establish reproducible asset generator, build harbor kit, inspect previews, export/validate, update this snapshot after each milestone.

#### Art milestone 1 - native kit created and first visual review

- Created 44 asset collections (802 authored objects consolidated to 72 mesh objects) in a separate Blender scene; original scenes and both pre-existing files remain intact.
- Saved native master: `Assets/Models/_Production/Sources~/HarborWorkshop.blend`. Sources~ is intentionally ignored by Unity import.
- Added generator, preparation and preview scripts under `Assets/Models/_Production`; shared palette and designed seamless ocean pattern generated locally.
- First isometric render exposed mismatched UV layers after joining meshes and excessive water stripes. Corrected UV layer merge and color encoding; redesigned foam into short curved crests. Reviewing revised image before export.
- Added `Assets/Models/Water/HarborWater.shader` (URP procedural swells); Unity shader import not yet tested.
- Quota at this milestone: five-hour 25% used / 75% remaining; weekly 4% used / 96% remaining. Art remains IN_PROGRESS.
- Next: export per-asset FBX/native sources, render tier comparison/icons and animation samples, validate mesh/animation data and Unity import if editor is available.

#### Art milestone 2 - separate exports; prefab scope confirmed

- Exported all 44 individual FBX files and collection-library Blender sources. Whole kit totals 80,522 source triangles, including the 8,192-triangle ocean surface.
- User explicitly reiterated separate editable prefabs per scene part and idle motion. Preparing one prefab per asset with separate moving children/sockets and shared materials.
- Blender closed unexpectedly during `bpy.data.libraries.write` of a newly created temporary source scene. Master save survived. Background Blender successfully exported all assets using collection-library writes instead. No user file was overwritten; live MCP is currently disconnected pending restart.
- Added Unity art-only builder/importer to create modular prefabs, loop clips, two travel clips, shared URP materials, and a separate art preview scene. Unity import has not yet been run.
- Collection libraries are interim native files; next normalize these to directly openable source scenes, render icons/comparison, then run Unity import and validation.

#### Art milestone 3 - Unity prefab delivery and validation

- 45 modular prefabs, 45 FBX exports, 45 directly openable per-asset Blender sources, 16 transparent PNG icons, shared palette, procedural water texture/shader, and separate preview scene now exist under `Assets/Models`.
- Unity 6000.3.23f1 import succeeded: 18 animated prefabs, two additional travel clips, zero water shader errors. Evidence: `_Production/unity-validation.json`; `Logs/art-import.log` (ignored).
- Unity rendered two animation sample frames successfully. Final follow-up corrects preview building yaw and attaches cargo/wake to ship sockets; final rerender currently running in the agent-started Unity batch process (tool session 19628).
- Added asset guide `Assets/Models/README.md` and provenance `docs/asset-register.md`. Blender reopened from saved master with MCP startup requested.
- Quota observation around 21:34 Asia/Karachi: five-hour 84% used / 16% remaining; weekly 13% used / 87% remaining. No threshold reached at that observation.
- Need final render review, revert only Unity-generated unrelated project-setting changes, and refresh this snapshot. Gameplay, audio, player builds and profiling remain unimplemented/unverified.

### Final art-session handoff - 2026-10-20 21:37 Asia/Karachi

Status: **HANDOFF_ONLY_QUOTA**. Five-hour window 91% used / 9% remaining; weekly 14% used / 86% remaining. No further implementation, tests, cleanup or commits after observing the threshold.

Completed evidence:
- 45 independent prefab/FBX pairs and per-asset Blender files, including ten building variants, freighter, cargo, props, map markers, icons, effects, ocean and shoreline wash. `asset-manifest.json` lists exact relative paths.
- Shared palette atlas, original analytic wave/foam texture and URP water shader; no downloaded art. Provenance: `docs/asset-register.md`.
- Unity import and shader/reference checks passed on 6000.3.23f1; 18 animated prefab clips and two separate travel clips. `_Production/unity-validation.json` reports 45 prefabs and zero water shader errors.
- Two actual Unity camera renders at animation sample times 0 and 1 seconds; six animated preview instances have varying curves. `visual-validation.txt` records hardware and limits. Some static imported models also have unused Animation components; these do not constitute additional animated assets.
- Reviewed final Unity screenshot: building fronts now face camera, but wake is not visibly rendered after socket parenting and cargo scale merits inspection. Do not mark this visual integration issue resolved. Actual engine Play Mode and player builds remain untested.
- Asset guide: `Assets/Models/README.md`. Master: `Assets/Models/_Production/Sources~/HarborWorkshop.blend`. Existing user .blend files preserved. Official_Game was not edited.

Partial task status: M7.1 conventions DONE; M7.8 provenance DONE. M7.2-M7.6 asset production portions delivered but runtime integration, visual polish and acceptance remain IN_PROGRESS. M7.7 audio, M7.9 profiling and M7.10 final gameplay readability remain TODO. Other gameplay milestones remain unchanged.

Failures/recovery: live Blender crashed when writing a temporary scene library; saved master survived. Background collection export and normalization produced the separate source files successfully. Sandboxed Unity stalled at licensing/WMI initialization; its agent-started process was stopped, and authorized unsandboxed batch import/render passed. Blender was reopened with the saved master and MCP startup requested; verify connection in the next session.

Pending cleanup: metadata audit for recent files, inspect `.vscode/settings.json` import-generated change, confirm no actual unrelated project-settings delta, optionally remove empty generated Cargo directory (only after verifying it is empty). No cleanup was attempted after quota threshold. Logs are under ignored `Logs/`; all useful validation summaries are under `_Production/`. No publishing, commits, engine upgrades or gameplay implementation occurred.

### Socket correction resumed
- IN_PROGRESS: M7.6 socket attachment debugging and final asset import validation. Quota rechecked: 68% five-hour and 95% weekly remaining. Will inspect actual imported transforms, fix attachment basis/scale, then render and audit prefab/source metadata. Existing working-tree changes preserved.


### Socket correction completed
- DONE: M7.6 asset socket correction. Actual Unity diagnostic ruled out scale: raw socket up pointed sideways and wake bounds extended downward. Corrected attachment basis in the ship prefab, retaining imported animation transforms.
- Rebuilt 45 separate prefabs; validated orientations/scales and wake bounds at 0 and 1 seconds, inspected final Unity image, and audited source/export/metadata inventory. All checks passed; no water shader errors. Gameplay-specific M7 acceptance remains open.
- Quota latest: 53% five-hour / 93% weekly remaining. No threshold reached this session. No pending batch process or blockers for asset delivery.



### Gameplay implementation milestone 1 - code authored; engine verification pending
- Added Assets/OpenWaters runtime definitions, authoritative session rules, command receipts, transaction/decision ledgers, versioned save validation/recovery adapters, harbor/camera and uGUI interface.
- Full two-voyage loop, five event definitions, upgrade, scheduled GSA review and ending are authored but not yet declared verified.
- Added Edit Mode tests for 24 branches, three worked endings, duplicate commands, corruption recovery and tier/content restrictions; Play Mode scene integration fixture.
- Editor builder wires original prefabs into Demo.asset and Official_Game. Existing asset GUIDs preserved. New metadata awaits Unity import.
- Validation: initial sandboxed Unity launch produced no log; retrying installed editor outside sandbox. No passing compilation/test claim yet.
- Quota 2026-10-20: five-hour 58% used / 42% remaining; weekly 9% used / 91% remaining.
- Next: fix compiler/import findings, execute rules and Play Mode tests, inspect UI at both resolutions, then build candidates.


### Gameplay milestone 2 - engine rules validated
- Unity 6000.3.23f1 compiled successfully and generated Demo.asset / Official_Game wiring. No package upgrades.
- Edit Mode: 34/34 passed, Logs/gameplay-editmode.xml. All 24 branch scenarios and three worked endings pass with checkpoint restore and duplicate rejection.
- Initial test exposed JsonUtility serializing a null voyage as an empty nested object; fixed narrowly scoped pre-voyage normalization. Backup recovery and fresh/preparing roundtrips now pass. Added stricter phase/queue/ledger checks.
- First integrated Play Mode test passed 1/1. Latest fixture rerun adds five resets and rendered UI captures; still pending.
- DONE IDs above describe verified domain behavior/data. Interactive camera/UI polish, complete persistence/platform hardening, sound, profiling, builds and first-time playtests remain open.
- Added docs/build-and-run.md and docs/playtests.md with controls, storage, validation commands and explicit unmeasured acceptance gates.
- Current next action: inspect gameplay captures, resolve UI issues, validate final Play Mode and build Windows/WebGL candidates.



### Gameplay milestone 3 - coordinated hardening and initial package
- User explicitly requested GPT-6.1 Sol Medium second agent. Persistence-review sub-agent owns SaveService.cs / validation portion of GameSession.cs / SessionTests.cs; root owns presentation, UI, scene, builds, integration tests and tracker.
- Initial Windows x64 development build SUCCEEDED: 183,550,904 reported bytes, 0 errors, 1 warning, 2m25s. Logs/gameplay-windows-build.log and build-StandaloneWindows64.txt. This package predates final persistence and map fixes; rebuild required.
- Initial packaged fast smoke reached expected 470/65/0 Clear, but screenshot capture in batch mode logged an error so the smoke correctly reported failure. Replaced unsupported batch screen capture with explicit camera/UI render-texture capture; not yet rebuilt.
- Persistence agent added backup-preserving writes, authored receipt validation, stable voyage/transaction/phase consistency and settings bounds; 16 regression cases (50 total Edit Mode cases). First expanded run: 26 passed /24 failed due too-strict float trigger equality on checkpoint restore. Agent fixed boundary tolerance to 0.0001s; rerun pending. Earlier 34/34 result remains historical, not final acceptance of current edits.
- Previous UI Play Mode run: 2/2 passed, including actual UI button command callbacks, pause preserving mandatory event, Watch/Restricted endings, modal keyboard focus and inspection mode. Pointer-device raycast/isolation fixture added; current 3-test run pending.
- Added original procedural ambience/reward/alert/upgrade sound, loading/arrival effects, fixed Dispatch footer, bounded ocean framing, explicit development route fixtures. Actual browser sound and sustained profiling remain open.
- Last quota observation from coordinated agent: five-hour 74% used /26% remaining; weekly 12% used /88% remaining. No reserve threshold reached.
- Next: inspect 3-test Play Mode result, rerun 50 Edit Mode cases, final Windows rebuild/smoke, WebGL candidate build and browser smoke if feasible.


### Gameplay milestone 4 - coordinated tests pass
- Edit Mode 50/50 PASS after persistence float-boundary fix (0.0001s tolerance). Logs/gameplay-editmode.xml.
- Play Mode 3/3 PASS on current source. Includes full scene clean run / five resets; UI button flow / pause / Watch and Restricted endings; actual synthetic pointer world selection and UI/world click isolation.
- Pointer failures were batch test focus behavior: Input System resets background devices. Fixture now temporarily uses IgnoreFocus and restores prior settings; production focus/pause behavior unchanged.
- Inspected Tier 1/Tier 2/event/route-map/ending renders at 720p and 1080p. Fixed map mesh generation to use VertexHelper, retained masked scrolling, fixed Dispatch footer. Keyboard focus scrolls to selected controls and modal blocks background navigation.
- Current Windows rebuild running; previous initial package is stale until replaced. No running player. Branch Ahad, actual HEAD 4478907; no agent commits.
- M6.7 visual gates cover the two target resolutions, not every future device/aspect ratio. M7.10 verifies current original art readability, not human playtest comprehension. M8.1 automation passed; M8.2 human/manual full matrix remains open.
- Next: finish current Windows build, verify packaged isolated smoke, build WebGL candidate, then browser/platform and human acceptance gates.


### Gameplay milestone 5 - tested Windows candidate
- Final Windows development build SUCCEEDED: 183,558,948 reported bytes; 0 errors; 2 warnings; 18.5s incremental rebuild. Package: Builds/Windows/OpenWaters.exe plus all sibling runtime/data files.
- Final packaged fast smoke PASS, zero logged errors: 470 credits /65 reputation /0 suspicion /Clear. Logs/windows-final-smoke.json, windows-final-player.log and windows-final-smoke.json.png. Actual packaged UI render inspected. Prior initial screenshot failure is superseded.
- Diagnostic hardware: Ryzen 5 3600, Radeon RX 5600 XT. Short fast-clock run observed ~67 MB Unity allocated peak, 112 peak draw calls, ~15.7ms average CPU frame sample; GPU timing unavailable (zero counter). These are diagnostic samples, not sustained frame-rate/memory/GPU acceptance.
- WebGL candidate build started (Unity PID 12784), current log Logs/gameplay-webgl-build.log; platform switch/import/compiler active. No local server started yet.
- Added tools/serve_webgl.py for loopback compression/MIME/COOP/COEP hosting and docs/validation-summary.md with reproducible evidence and remaining limits.
- Render settings/URP global/profile assets were rewritten by Unity build serialization/prefiltering; preserve these observed build-generated changes for review. ProjectSettings intentional changes: 1280x720 defaults, resizable window, windowed mode, frame-timing stats; Input System preloaded action asset also serialized by Unity.
- Remaining: finish WebGL build/browser acceptance, normal-speed profiling and first-time human playtests; M6.7 arbitrary aspect-ratio changes, M6.8 player-observed tutorial tuning and M8 manual gates remain open.



### Browser build in flight / session-only reserve exception
- User explicitly authorized continuing this instance until 5% remaining (95% used) before handoff. Do not change the default rule for later sessions.
- Latest observed limit before this note: five-hour 92% used /8% remaining; weekly 14% used /86% remaining. Work is above the current-session threshold.
- WebGL native build remains active in Unity PID 12784 and its Bee/clang children. Confirmed compiler CPU increases; no final build result yet. Builds/WebGL/Build/WebGL.data exists (6,896,260 bytes), but no finished Wasm/index candidate at this observation. Do not call this a successful WebGL build.
- Browser inventory: in-app browser available and documentation loaded. No browser tab or loopback server created yet. Next after successful build: run bundled/installed Python tools/serve_webgl.py Builds/WebGL --port 8765; visit http://127.0.0.1:8765; verify actual canvas/menu/input and storage behavior. Stop the agent-started server after testing.
- Requested optional user feedback on the concrete Windows executable: timing/confusion/blockers and upgrade/GSA comprehension. No feedback received yet; no first-time human test result claimed.


### Feedback implementation milestone 1 - authored and compiling
- Replaced the text-heavy persistent sidebar with compact floating HUD/action panels; optional tips, ledgers, route details and building explanations; essential costs/risk remain visible. Added 20 consistent UI pictograms and one replaceable Logo.png, automatic app icon configuration, and disabled engine splash.
- Added cyan selection footprint/tint/name badge and gold attention footprint; new preference/display service from coordinating agent defaults to native fullscreen, supports persisted borderless/windowed modes, and keeps browser fullscreen gesture-owned.
- Ship now crosses the current camera frustum boundary along its lane, becomes hidden offshore even when camera zoom/pan changes, and returns over six seconds; collider stays unavailable until docked.
- Setup compiled and regenerated scene/data successfully. First Play Mode attempt exposed native MaterialPropertyBlock allocation in a MonoBehaviour field initializer; moved allocation into Initialize. Retrying 5-test Play Mode suite. No current passing UX-test claim yet.
- Existing 50-rule /3-integration results in historical snapshot refer to prior UI. Coordinating agent added two display/tips regression tests; 52-rule verification pending. Prior WebGL build succeeded but represents the earlier interface; will be stale after this revision until rebuilt.
- Latest observed quota: five-hour10% used /90% remaining; weekly17% used /83% remaining.


### Feedback milestone 2 - current engine checks pass
- UX Play Mode suite 5/5 PASS: original flow/pointer tests plus optional tip/ledger visibility, canonical logo/icons and layout renders at 16:9,4:3,ultrawide; ship full-frustum departure, hidden offshore even on zoom, slow approach and dock-only selection. Logs/ux-playmode.xml and GameplayPreviews/floating-*.png.
- Edit Mode 52/52 PASS including fullscreen defaults, persisted display modes and preference validation; unchanged24branch economy cases pass. Logs/ux-editmode.xml.
- Inspected compact initial layout at1280x720 and960x720; next visuals to inspect: selection highlight, event cards and arrival. Minor follow-up aligns action icons vertically and gives destination buttons explicit Ship to verbs.
- Added persistent selected-route outline/tint and packaged diagnostic display mode/native resolution/logo/tips fields. Requesting one-time migration of old default-on tutorial preference to opt-in (legacy setting was previously automatic, not an explicit user choice). Verification of these follow-ups pending.
- Current batch processes completed. Next: final affected suites, Windows rebuild/icon/fullscreen smoke; updated branding/display instructions and future website backlog.



Browser integration milestone: production build active in Unity PID 27416 (Logs/browser-release-build.log). Website source/player/import/verifier written and type/source compilation plus 2 package contract tests pass; no binary import yet. Latest lifecycle changes to Scripts/UI/DemoController.cs and Scripts/BrowserDemo.jslib require incremental rebuild after first build exits. User explicitly sets this chat only to 4% reserve; latest quota five-hour 84% used /16% remaining, weekly28% used /72% remaining. Website branch codex/browser-demo; Unity remains Ahad, prior work preserved. Loopback website validation server is command session3206 on4173. Missing-release error/close/focus browser checks pass; hosted and performance gates still pending.

Final build queued under command session29610: waits only for agent Unity PID27416, then launches BuildWebRelease with Logs/browser-release-final-build.log. Do not start another Unity process against this checkout. The local validation server remains session3206. Browser-facing methods explicitly preserved against managed stripping. Source changes frozen pending this final rebuild; no imported release yet.


### Browser resume — 2026-10-21
WEB.2 IN_PROGRESS: renewed user authorization; quota five-hour0% used/100% remaining, weekly30% used/70% remaining. Previous Unity/compiler processes absent and no production output/result. Switch production optimization from DiskSizeLTO to DiskSize (no LTO), retain native Brotli/no development/single thread/save identity. Rebuild final bridge/source, measure size, import, validate website and real game runtime. Prior contributor changes remain preserved.

### Browser demo completed locally / main published — 2026-10-21
Production release founders-20261021-r1 (14,382,302bytes) is bundled in sibling website repository. Full embedded guided run completed through Tier2/two deliveries/storm/compliant documentation/ClearGSA ending490credits. Save-close-reopen-Continue and direct/demo/Continue retainedprogress; ten cached reopen trials<0.9seconds with no payload retransfers. Fullscreen focus/resize issue corrected in website shell and verified. Unity53/53EditMode +5/5PlayMode PASS; website13Node checks/type/fullbuild/source+dist artifacts PASS. Separate web-only npm install/build/run succeeded without Unity source.
User requested wrapping up/skipping further tests after a complete browser cycle and authorized main forcepush/live verification. Website main pushed with exact force-with-lease at ff70a57; optional GitHubActions workflow omitted because OAuth scope blocked adding it (Vercel build checks retained). Automatic deployment at https://openwaters-cega.vercel.app/ is being checked; current public page still old at first observation. No Unity repo commit/push. Prior contributor work preserved onAhad. All build/test Unity processes exited; local website validation server29626 stopped. Formal hosted25Mbps timing/browser/storage/performance matrix remains unverified and is documented in website release record.

Final live deployment result: website main feature commitff70a57 successfully deployed to https://openwaters-cega.vercel.app/. Actual game menu/NewRun/contract/dispatch/firstdelivery450credits and close/reopen/Continue passed, zero capturedbrowserwarnings/errors. Live shell/allgameasset responses200, nativeBrotli/MIME/cachecorrect, missingasset404. Complete local two-voyageClearending490credits already passed. User requested wrapping up without further broadtests. Website package independent ofUnityrepo; game source remainsuncommitted onAhad; priorworkpreserved. No agentUnity/testserver processes remain. See siblingwebsite progression/release record for deferredmatrix and updatecommands.


Session end 2026-10-21 07:59 Asia/Karachi: website main clean and pushed at2e9809a (final documentation; runtime featureff70a57 liveverified). Quota five-hour56%used/44%remaining,weekly39%used/61%remaining; chat4%reserve not reached. No pending implementation work for requested browser delivery; broader optional validation limits are documented.
