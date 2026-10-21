# Gameplay validation - 2026-10-20

## Feedback revision - current status

- **53/53 Edit Mode passed** in `Logs/ux-editmode.xml`: prior economy/persistence cases plus display preferences, default-off tips and one-time legacy-tip migration.
- **5/5 Play Mode passed** in `Logs/ux-playmode.xml`: prior flow/pointer cases plus optional tip/ledger visibility, shared logo/icon resources, 16:9 /4:3 /ultrawide layout captures, whole-hull departure beyond the frustum, hidden offshore status on zoom, animated approach and dock-only selection.
- Floating initial layouts visually inspected; text is reduced to the current action and short status. More information is opt-in. No follow-up human comprehension success is claimed yet.
- New Windows candidate built with zero errors. Shared logo loaded in its runtime and was extracted from the executable icon for visual verification. Native 1920×1080 display detection and default-off tips were observed in the packaged run.
- **Focused Windows display check PASS**: `Logs/ux-visible-smoke.json`, normal-speed109.29s,470credits/65reputation/0suspicion/Clear,0loggederrors, native1920×1080, borderlessVerified=true and fullscreenVerified=true. Startup was borderless; after activating the visible player window, settings entered exclusive fullscreen successfully. Hidden/background launches had DXGI887a0022 fallback; do not infer every machine's exclusive support from this desktop result.
- Final five Play Mode checks also passed after moving the objective below header cards to resolve overlap in a square browser viewport (`Logs/ux-playmode-final.xml`). Extra pointer/selected-building capture passed1/1 (`Logs/ux-pointer-final.xml`).
- Branded WebGL template/loading/favicon share the canonical PNG. Local browser smoke observed loading/menu, keyboard controls, first dispatch, absent offshore ship, delivery450credits and reload/Continue restoration, with no captured warning/error logs. `Logs/ux-browser-offshore.jpg`. Final viewport correction package succeeded115,882,104reported bytes,0errors/2warnings,1m44s (`Logs/ux-webgl-final-build.log`); loaded again and inspected889×884 layout with no header overlap (`Logs/ux-browser-final.jpg`). Temporary browser tab/server cleaned up.
- Final Windows package succeeded185,992,464reported bytes,0errors/2warnings (`Logs/ux-windows-release-build.log`). Its isolated gameplay smoke passed with startupExclusiveFullScreen (`Logs/ux-shipping-smoke.json`); display transition flags in that report were not requested. Actual transition evidence is the focused fixture above.
- Browser audible sound, cross-browser/hosted behavior, sustained profiling, completion pacing and a second human comprehension test remain acceptance gates. No external publication occurred.

Historical initial implementation evidence follows.

Unity 6000.3.23f1 / URP 17.3.0 / Input System 1.20.0. Branch `Ahad`, observed base HEAD `4478907`; implementation remains uncommitted with prior art and contributor changes preserved.

## Passing engine checks

- **Edit Mode: 50/50.** All 24 upgraded route/operational/inspection combinations, three worked endings, affordability/duplicate guards, checkpoint JSON roundtrips, schema/tier/ID/phase/transaction/receipt corruption, safe backup recovery (including failed writes) and volume validation.
- **Play Mode: 3/3.** Real scene and prefab setup; first delivery and Tier 2 swap; five New Run resets; UI button command flow; all GSA ending presentations; event pause/resume and modal focus; actual synthetic pointer world selection and UI/world isolation.
- **Visual inspection:** 1280×720 harbor, route preparation/Dispatch footer, congestion and inspection; 1920×1080 ending; Watch and Restricted presentations. These are engine renders of the current interface, not design mockups.
- **Metadata:** no missing file/folder metadata under `Assets/OpenWaters` at the audit.

Local evidence is ignored generated output: `Logs/gameplay-editmode.xml`, `Logs/gameplay-playmode.xml`, corresponding engine logs and `Logs/GameplayPreviews/`. Tests are retained under `Assets/OpenWaters/Tests` for reproduction.

## Findings fixed

1. Unity JsonUtility serializes a null voyage as an empty object; fresh/preparing saves now normalize only that empty placeholder.
2. Recovery writes could rotate corrupt primary over a valid backup; storage adapters now preserve valid recovery data.
3. Save validation now checks authored effects, stable IDs and transaction/decision/phase consistency, with 0.0001s tolerance for float trigger roundtrips.
4. The pointer fixture initially read no movement in an unfocused batch Game View. Test-only routing and IgnoreFocus settings fix the harness and are restored afterward; production focus/pause rules remain active.
5. Route-map custom mesh required non-legacy vertex generation. Dispatch now remains visible in a fixed footer while the comparison/map panel scrolls.
6. ScreenCapture is unsupported in the initial batch player smoke. The diagnostic now explicitly renders camera and UI to a texture; final package outcome is tracked in `progression.md`.

## Build and performance limits

The initial Windows development candidate built successfully (183,550,904 reported bytes, zero errors, one warning, 2m25s). It predates the final coordinated hardening and is superseded by the subsequent rebuild. Current package/platform status is recorded in `progression.md`.

The final Windows candidate also built successfully: **183,558,948 reported bytes, zero errors, two warnings, 18.5s incremental build**. Its isolated packaged smoke **passed with zero logged errors**, expected **470 / 65 / 0 / Clear**, and a visually inspected camera/UI capture. Evidence: `Logs/windows-final-smoke.json`, its `.png`, `windows-final-player.log` and `gameplay-windows-build.log`. This supersedes the initial smoke failure below.

Initial fast-clock packaged simulation reached 470 credits / 65 reputation / 0 suspicion / Clear, but its screenshot failure meant the smoke correctly reported failure. That is not a passing final runtime gate.

Observed graphics device: AMD Radeon RX 5600 XT; CPU: AMD Ryzen 5 3600. Fast batch diagnostics do **not** establish frame-rate, sustained memory, draw-call or GPU budgets. A zero/unavailable GPU timing or draw counter must not be reported as measured zero cost.

No first-time human playtest, 5-8 minute pacing claim, fresh-machine launch, or target-browser persistence/performance claim is made. See `docs/playtests.md` for the outstanding manual acceptance matrix.
