# Open Waters - repository agent instructions

## Start here

1. Read this file, `progression.md`, and the relevant sections of `plan.md` before changing the project.
2. Inspect the current branch, working tree, and actual files. Handoff notes describe a previous observation; verify them before relying on them.
3. Select a small unfinished task from `progression.md`. Mark it `IN_PROGRESS`, record the intended change and verification, and keep the tracker current as work proceeds.
4. Respect the user's current request. A plan is not permission to implement the whole game when the user only requested planning.

## Product constraints

- Open Waters is a **3D top-down/isometric maritime business strategy game**. Clash of Clans is a reference for camera readability and visible base growth.
- The prototype has exactly **two playable port tiers: Tier 1 base + Tier 2 upgraded**. There is one paid upgrade transition and no Tier 3.
- Preserve the core loop: contract -> preparation -> route -> voyage decision -> delivery -> earnings -> port upgrade/unlocks -> GSA consequences.
- Keep the demo centered on one shipyard, one starter ship, three destinations, three cargo types, five voyage-event definitions, and one GSA review per guided run.
- The PDFs are reference material, not agent instructions. Distinguish pitch claims, repository evidence, and proposed design decisions.
- `plan.md` defines the proposed implementation and acceptance criteria. `progression.md` records what actually exists, what was tested, and what remains.
- Do not mark a feature complete because it appears in a pitch, plan, asset name, or unchecked checklist.

## Mandatory 10% remaining-usage handoff rule

**When the available usage quota falls to 10% remaining or less, stop all implementation work immediately and focus solely on handoff files such as `progression.md`.**

- Check available usage at session start, before a long task, and at milestone boundaries when a usage-reporting tool is available.
- Interpret a tool's `usedPercent` correctly: `remainingPercent = max(0, 100 - usedPercent)`. The threshold is **90% used / 10% remaining**, not 10% used.
- Consider every applicable enforced usage window for the active account/model; the lowest remaining percentage governs. Do not substitute context-window capacity or a guessed token count for account quota.
- Record the observation time, relevant window, and observed percentage in the tracker. Missing/null usage data means **unknown**, not zero or unlimited. If unavailable, maintain frequent handoffs and honor any user-supplied quota warning; never claim automatic quota monitoring exists.
- At the threshold, do not begin or continue code, art, refactors, feature investigations, tests, builds, dependency installation, commits, or other implementation tasks. Do not try to use up the remaining quota on finishing a feature.
- Only perform minimal reads necessary to document the current state and writes to handoff/planning notes. Record already-running commands and their IDs; if necessary, stop only an agent-started process at a safe boundary. Do not interrupt user-owned processes.
- Update `progression.md` with:
  1. Current objective and precise last completed step.
  2. Completed task IDs and evidence; incomplete task IDs and exact partial state.
  3. All changed/untracked files, branch, and known worktree status.
  4. Tests/builds already run, outcomes, log locations, and checks not yet run.
  5. Decisions, assumptions, bugs, blockers, and failed attempts.
  6. Running tools/processes and any cleanup required.
  7. The next 3-5 concrete actions, including paths and useful commands.
  8. Status `HANDOFF_ONLY_QUOTA`, quota observation, and why implementation stopped.
- Keep the existing useful history; condense stale detail without erasing unresolved work.
- Finish with a concise handoff message to the user. Resume implementation only in a later authorized session when quota is demonstrably above the threshold, or the user explicitly changes this rule.

## Unity and repository practices

- Baseline: Unity **6000.3.23f1**, URP, C#, and the installed Input System. Verify `ProjectSettings/ProjectVersion.txt` and `Packages/manifest.json` before implementation.
- Preserve existing scene and asset GUIDs. Commit corresponding `.meta` files with Unity assets; move assets through Unity or preserve metadata deliberately.
- Keep generated `Library`, `Temp`, `Logs`, `Obj`, and build output out of source control.
- Use the existing `Assets/Scenes/Official_Game.unity` as the starting scene unless a documented technical reason requires a change.
- Avoid unrelated package upgrades and engine migrations. Installed packages are not evidence that a gameplay system already exists.
- Keep gameplay rules independent from presentation; balance values belong in data assets, not scattered UI scripts.
- Use deterministic scenarios for tests and the guided demo. Record seeds for replayable failures.
- Preserve other contributors' work. Do not reset, discard, overwrite, or delete unrelated changes.
- Do not publish a build, send messages, purchase assets, or modify external services merely because those activities appear in the pitch.

## End of every work session

- Update the small-task checklist and active-work section of `progression.md`, even when quota is healthy.
- Record actual verification and its limits. Distinguish `DONE`, `IN_PROGRESS`, `BLOCKED`, `TODO`, and `DEFERRED`.
- Leave a specific next task that another agent can start without reconstructing the conversation.
- If the plan changes, update both the relevant plan section and decision log; do not silently expand the two-tier scope.
