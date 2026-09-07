# LEVEL_QA.md — Stencil Relay

> 2026-09-07 override: human touch-play QA is **SKIPPED BY USER REQUEST**, not PASS.
> The historical NOT DONE column remains evidence of what was not performed.
> Current automated and scene-UI regressions are in POLISH_OFFLINE_AUDIT.md.

Two verification classes, kept separate (master prompt §13, §26, §54):

1. **DATA / SIMULATION** — `LevelValidation` + `LevelSolver` + EditMode tests.
   Status: **13 / 13 VALID** (0 errors, 0 warnings).
2. **CONTROLLER-PATH PLAYTHROUGH** — each level driven through the real Projection
   Table scene + `ProjectionTableController` + `LevelManipulation` via the actual
   slide / rotate / flip / PROJECT / reset verbs (PlayMode test
   `AllLevelsPlaythroughTests.EveryLevel_LoadsSolvesResets_ThroughTheScene`).
   Status: **13 / 13 PASS**.
3. **HUMAN MANUAL QA** — a person playing the touch UI end to end at each aspect ratio.
   Status: **NOT DONE (0 / 13)** — no human tester available to this session; the agent
   cannot substitute for hands-on touch play. This is the one gate that stays open.

## Per level

| L | Title | Data valid | Loads | Not solved at start | Wrong→MISALIGNED+reason | Optimal solves (verbs) | Rating given | Reset→initial | Unlock next | Human manual |
|--:|-------|:--:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|
| 0 | Calibration | ✓ | ✓ | ✓ | ✓ | ✓ (slide+rotate) | Aligned | ✓ | ✓ (→L1) | NOT DONE |
| 1 | First Alignment | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | NOT DONE |
| 2 | Double Layer | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | NOT DONE |
| 3 | Quarter Turn | ✓ | ✓ | ✓ | ✓ | ✓ (rotate) | ✓ | ✓ | ✓ | NOT DONE |
| 4 | Registration | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | NOT DONE |
| 5 | Shadow Block | ✓ | ✓ | ✓ | ✓ (Forbidden Sensor Lit) | ✓ (blocker) | ✓ | ✓ | ✓ | NOT DONE |
| 6 | Forbidden Sensor | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | NOT DONE |
| 7 | Reverse Face | ✓ | ✓ | ✓ | ✓ | ✓ (flip) | ✓ | ✓ | ✓ | NOT DONE |
| 8 | Precise Register | ✓ | ✓ | ✓ | ✓ | ✓ (slide+rotate+flip) | ✓ | ✓ | ✓ | NOT DONE |
| 9 | Parallax | ✓ | ✓ | ✓ | ✓ | ✓ (depth offset) | ✓ | ✓ | ✓ | NOT DONE |
| 10 | Filter Pass | ✓ | ✓ | ✓ | ✓ (Filter Mismatch possible) | ✓ | ✓ | ✓ | ✓ | NOT DONE |
| 11 | Dual Projection | ✓ | ✓ | ✓ | ✓ ("(T, P2)" labelled) | ✓ | ✓ | ✓ | ✓ | NOT DONE |
| 12 | Master Plate | ✓ | ✓ | ✓ | ✓ | ✓ (rotate+flip+slide, dual) | ✓ | ✓ | → finale | NOT DONE |

Level 0 tutorial: step progression (select→slide→rotate→project→CALIBRATION COMPLETE)
and persistence (`tutorialDone` unlocks L1) verified by
`Level0_Calibration_...` and `SaveSystem_RecordsTutorial_...` PlayMode tests.

Finale (after L12): `Finale_PlaysOnce_AfterLevel12_AndPersists` — plays once, persists,
does not replay.

## Honest summary
`13 / 13 DATA VALID` · `13 / 13 CONTROLLER-PATH PASS` · `0 / 13 HUMAN MANUAL`.
The last line can only be closed by a person playing the shipped build.
