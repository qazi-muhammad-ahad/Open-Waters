# Open Waters playtest and acceptance record

One user review has been received for the initial Windows candidate. Automated fixtures remain engineering evidence, not player comprehension or timing results.

## User review - initial Windows candidate, 2026-10-20

- Completion time/hardware not supplied. Reviewer reached the risky documentation choice and understood the negative GSA consequence.
- Main finding: too much text shown at once; core loop was not understood. Requested floating UI, optional tips, clear selection and attention cues, and memorable icons for port functions/actions.
- Display request: detect the screen and launch fullscreen; provide Fullscreen and Windowed Fullscreen settings; remove the window title bar during normal play.
- Branding request: replace the engine logo with game branding, using one easily replaceable source logo. Placeholder authorized.
- Voyage presentation request: ship must fully sail beyond the screen, stay absent offshore, and return slowly to the dock before it becomes accessible.
- Changes are being implemented and engine-tested; no follow-up comprehension result is claimed yet. First-time testing targets remain open.

## Automated scenarios

- Deterministic authored run; no random outcome seed required. Procedural ambience seed: 2701.
- Three worked textiles/sheltered/wait endings: Clear 470 credits / 65 reputation / 0 suspicion; Watch 490 / 45 / 25; Restricted 490 / 30 / 50.
- All four upgraded routes × two operational choices × three inspection choices = 24 scenarios. Checkpoint restores at acceptance, dispatch, pending/resolved events, delivery and review; duplicates must not change balances.
- Integrated harbor fixture uses the real scene, UI construction and tier assets; repeated New Run checks stale selectable cleanup.
- Latest actual results, logs and visual evidence are listed in `progression.md`.

## First-time test template

Record build ID, platform/browser, resolution, hardware, start/end time, destination/route/choices, final credits/reputation/suspicion/standing, intervention, confusion, softlocks and voluntary comments. Do not collect personal data by default.

Observe 5-8 first-time testers without coaching except to resolve blockers. Targets from the plan: 4 of first 5 complete unaided, median completion 5-8 minutes, 4 of 5 explain upgrade benefit and a GSA consequence, visible Tier 2 improvement noticed. Record results even when targets are missed.

Questions: What changed with the upgrade? Why did GSA react this way? What would you choose differently? Where were you waiting rather than deciding?

## Remaining manual/platform gates

- Interactive UI selection/input isolation, keyboard focus and scrolling at 720p/1080p and changed aspect ratio.
- Packaged Windows cold launch, normal-speed complete run, pause/focus and save/Continue.
- Hosted WebGL load, compression headers, gesture audio, IndexedDB persistence/recovery, resize/focus and restricted-storage handling.
- CPU/GPU frame times, sustained memory over five complete runs, draw calls, triangles and compressed payload on declared hardware/browser.
