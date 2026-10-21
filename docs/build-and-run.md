# Open Waters demo - build and run

## Editor

Use Unity **6000.3.23f1**, the checked-in packages and `Assets/Scenes/Official_Game.unity`. Press Play, then New Run. The art preview is separate from gameplay.

Gameplay source is under `Assets/OpenWaters`. `Data/Demo.asset` contains immutable content rows and all numerical balance values. No runtime economy rule depends on UI callbacks or animation completion. To regenerate scene wiring and the initial default data, use **Open Waters > Gameplay > Build gameplay scene**. This resets the authored demo data to defaults; preserve deliberate balance edits before invoking it. The scene and existing asset GUIDs are retained.

## Controls and loop

- Left click selects a building or the freighter; interface buttons issue validated commands.
- WASD/arrows or right/middle drag pan; scroll zooms; Home recenters.
- Escape closes ordinary details/dialogs, then pauses. Mandatory voyage decisions remain pending.
- Tab/arrow navigation and Enter/Space activate focused interface controls. Scroll panels expose lower controls.
- Settings: saved volume, mute, reduced motion and tutorial hints.

The feedback revision starts desktop builds at the detected display resolution in fullscreen. Settings offers **Fullscreen**, **Windowed fullscreen** (borderless), and a **Windowed** recovery mode. Preferences persist. Windows uses Direct3D 11 first: this desktop validation honored both display choices, while the previous Direct3D 12 backend fell back to borderless. Editor Play Mode does not change the editor window; browsers own their fullscreen gesture.

Tips are off by default, including a one-time migration from the old automatic hints. Use **Tips** for help or enable contextual tips in Settings. Ledgers, route rules, building explanations and sound levels open only when requested. Cyan marks the selected building/route; gold marks the next relevant port structure/action. The freighter sails completely out of view, remains hidden offshore, and returns along a six-second approach; selection becomes available after docking.

See `docs/branding.md` for the single replaceable `Logo.png` source used by game UI, Windows icons and WebGL loading/favicon.

Accept grain, select the coastal route, dispatch for 50 credits, and return for 300. The balance becomes 450. A single 300-credit upgrade leaves 150 and unlocks textiles/machine parts. Choose one upgraded route, resolve its incident and documentation check, then review the delivery and scheduled GSA outcome. All standings finish the demo; Inspect Port allows exploration without a third voyage or tier.

Route travel is deterministic and includes the return presentation. Reading a decision stops the voyage clock. Event fees/bonuses settle with delivery. Dock service never blocks settlement or the ending. There is no offline advancement.

## Saves and recovery

Desktop run storage: `Application.persistentDataPath/run-v1.json`, one `.bak` and transient `.tmp`; use the Unity Console/application log to locate the platform folder. New Run requires replacement confirmation when a current run exists. Keep a copy of both run and backup if diagnosing corruption.

JSON schema 1 stores primitive IDs, state, financial and decision receipts. Saves occur after commands, at five-second travel checkpoints, and on focus/pause/quit. Invalid schema/tier/IDs/phase/history produce a recovery message and a backup attempt. The menu does not overwrite failed saves automatically. Settings use `OpenWaters.Settings` in Unity PlayerPrefs. WebGL has a separate PlayerPrefs-backed run adapter; hosted browser persistence must pass the browser gate before release.

Automated Play Mode validation uses a temporary validation slot rather than the player's run slot. Edit Mode tests use in-memory storage.

## Validation commands

Run the installed Unity executable with these arguments from the repository root (replace `Unity` with its installed executable path):

```text
Unity -batchmode -nographics -projectPath . -runTests -testPlatform EditMode -testResults Logs/gameplay-editmode.xml -logFile Logs/gameplay-editmode.log
Unity -batchmode -projectPath . -runTests -testPlatform PlayMode -testResults Logs/gameplay-playmode.xml -logFile Logs/gameplay-playmode.log
Unity -batchmode -quit -projectPath . -executeMethod OpenWaters.Editor.DemoBuilder.BuildWindows -logFile Logs/gameplay-windows-build.log
Unity -batchmode -quit -projectPath . -executeMethod OpenWaters.Editor.DemoBuilder.BuildWeb -logFile Logs/gameplay-webgl-build.log
```

Do not open a second editor against the same project while a batch command owns it. `Logs` and `Builds` are ignored generated output. Test XML, engine logs and build summaries are authoritative; check `progression.md` for the latest results and unfinished gates.

## Packages

The builder targets `Builds/Windows/OpenWaters.exe` and `Builds/WebGL`. Windows development candidates require the entire sibling data/runtime folder. WebGL must be served over HTTP with matching compression/MIME headers; opening `index.html` with a file URL is insufficient. Build creation does not publish either package.

For a local WebGL candidate, run `python tools/serve_webgl.py Builds/WebGL --port 8765` and open `http://127.0.0.1:8765`. The provided host binds only to loopback and sends Unity compression/MIME and cross-origin headers. Stop it after validation.

Windows development builds support an opt-in isolated smoke fixture: `OpenWaters.exe --ow-smoke --ow-smoke-fast --ow-report=<absolute-report-path>`. It replaces only the temporary validation slot, drives the real session commands, renders an ending image beside the report, and exits with success/failure. Omit `--ow-smoke-fast` for normal travel timing. Optional `--ow-route=meridian_patrolled` and `--ow-inspection=risk` select other fixtures. This automated fixture does not establish human usability or performance acceptance.

Add `--ow-settings-check` to exercise the real fullscreen/borderless settings buttons and record native resolution, tips and shared-logo state. Use a visible, focused player window for exclusive fullscreen acceptance: Windows can reject exclusive requests from a hidden/background process and Unity then uses borderless. The focused normal-speed check passed both modes on the validation desktop; see `Logs/ux-visible-smoke.json`.

## Known acceptance limits

Human first-time playtests, hosted WebGL behavior, and sustained target-device profiling require measured evidence. Short automated fixtures advance the clock explicitly and do not establish the 5-8 minute completion target or actual frame-rate/memory budgets. Current gate status lives in `progression.md` and `docs/playtests.md`.
