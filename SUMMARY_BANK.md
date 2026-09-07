# SUMMARY_BANK.md — Stencil Relay

Cross-session memory. A new session must be able to resume from this file alone.

## CURRENT OVERRIDE — 2026-09-07 / MCP-0014

Owner requested UI/game-design polish, a full offline audit, removal of network
packages, and explicitly **SKIPPED HUMAN TOUCH-PLAY QA**. Do not reopen that gate
or label it PASS. The previous release freeze is superseded by this authorized work.

Polish and fixes: immediate success persistence; direct Pattern Rack navigation;
selected lower-layer picking; relative drag preview; pause/UI input blocking;
PROJECT idempotence; state-based tutorial guidance; disabled unavailable controls;
level title / move par / front-back labels; numbered filters and projector identity;
projection inspection; three rack trays; studio specimen; audio percent/geometry;
large boards fitted to drafting well with dimension-correct grid; mesh cleanup.

MCP-0014: all Unity scene changes applied through MCP and saved. Removed
UnityWebRequest, Rider and Visual Studio packages, then resolved. Disabled UnityConnect,
engine diagnostics, analytics/ads startup and forced Android Internet permission.
After visual checks, closed this editor and removed the MCP package as requested.
Do not reinstall MCP automatically. Use ordinary Unity menus / batch-mode verification.

After final removal: 56/56 EditMode + 11/11 PlayMode PASS, 13/13 VALID.
Windows production + iOS Xcode export both PASS (0 errors / 0 warnings).
Windows startup PASS; five network samples each show 0 TCP/UDP endpoints. Temporary
firewall enforcement denied by Windows (NOT VERIFIED); no rule remains. Three inert
Unity engine networking/analytics wrappers retained by the Mono type-forwarding facade
are documented, with no game/runtime library reference to them. Static audit PASS.
Original player save preserved. Evidence under `AuditEvidence/`; final outputs
are `Builds/Offline/Win` and `Builds/Offline/iOS`. Current detailed report:
`Assets/Documentation/POLISH_OFFLINE_AUDIT.md`.

Local request COMPLETE. Overall mobile release remains BLOCKED_EXTERNAL for iOS
signing/install and physical-device profiling. Do not claim human/device QA PASS.

MCP-0015 (2026-09-07, verification-only session): re-checked the MCP-0014 work against
the on-disk evidence. CONFIRMED: 56/56 EditMode + 11/11 PlayMode (AuditEvidence/*.xml,
0 skipped/0 failed), 13/13 VALID, Win build 90,648,622 B + iOS export 917,209,988 B
both Succeeded 0/0, offline-static.json clean, 5 player samples 0/0 endpoints,
manifest = 19 deps (MCP/unitywebrequest/rider/vs gone), player-offline.log clean,
iOS Info.plist has no *UsageDescription keys and is landscape-locked. Code review of
the changed scripts (ProjectionTableController / HudController / BoardView /
ReleaseVerification) = sound.
ONE defect found: Projection-Table tutorial hint bar overlapped the "MOVES NN / PAR NN"
counter (offline_inspect_wide / offline_result_1959 / offline_final_1959 / offline_table_43).
Fixed in SceneBuilder.cs (MovesText y -44->-34; HintBacking + TutorialBanner y -92->-108).
PENDING: run menu `Stencil Relay/Rebuild Scenes`, then re-run tests + rebuild Win/iOS
to fold the scene change into the release outputs. Could not run it here (MCP removed);
Unity editor is open, so the menu item is available to the owner.

The remainder below is historical, including outdated claims that MCP is retained,
human QA is required for this task, and the original 65-test release snapshot.

---

## PROJECT STATUS
`BLOCKED_EXTERNAL` — all locally achievable release-blocking work passes (13/13
validation, 65/65 tests, console clean, offline+originality+package audits pass,
Windows build + iOS Xcode export both succeed 0/0, no known locally-fixable blockers).
Open items are external only: iOS archive/sign/device-install (macOS + Apple cert),
human hands-on manual QA, on-device performance profiling. See FINAL_AUDIT.md.

Residual local polish (NOT release-blocking, tracked as BUG-001 tail): modal-panel
safe-area inset; full-sheet-plate board wash; storage-slot sprite float on wide
aspects; per-aspect UI screenshot sweep (GameView-size switch via MCP didn't apply).

## CURRENT MILESTONE
**M11 — FINAL LOCAL CLOSURE PASS DONE. `PROJECT STATUS = BLOCKED_EXTERNAL`.**
- Modals: all rebuilt as safe-area-inset full-screen overlays (`UiModal`) — dark card
  for result/settings/rack/confirm/finale, frosted calibration overlay for Pause.
  Modals start hidden at build time (no flash). Pause gained a Calibration Guide option.
- Light Studio menu → engraved plaque on the light table; lamp/lens/knobs repositioned.
- Board readability: recessed neutral drafting well + reduced key light + lower plate
  tint (~0.20-0.22). "BOARD WASH: RESOLVED" (UI_QA.md).
- Per-aspect QA sweep: 16:9 / 19.5:9 / wide-phone / 4:3 via
  `PlayModeWindow.SetCustomRenderingResolution` (screenshots `Assets/Screenshots/qa*`).
- MCP package **removed** from manifest (22 deps) — Editor-only, absent from both build
  outputs' Managed DLLs (evidence in PACKAGE_AUDIT.md).
- Regression: 65/65 tests, 13/13 VALID, console clean.
- Rebuilt: Windows (0/0, launches clean) + iOS Xcode export (0 errors; 18 accepted
  Unity URP-core APV shader precision hints, §30 — not in shipped Metal shaders).
- New docs: `RELEASE_CANDIDATE.md`, `EXTERNAL_QA_HANDOFF.md`. FINAL_AUDIT.md regenerated.

## LAST VERIFIED BUILD
- 2026-09-06 · Unity 6000.3.19f1 · StandaloneWindows64 → **Succeeded** (0 err, 0 warn, ~103 MB)
- 2026-09-06 · Unity 6000.3.19f1 · iOS (Xcode project export) → **Succeeded** (0 err, 0 warn)
  at `I:/UnityProject/StencilRelayBuilds/iOS/Unity-iPhone.xcodeproj`
- iOS device install / archive / sign: NOT VERIFIED — needs macOS + Xcode + Apple
  Developer credentials (genuine external blocker, §44/§45).

---

## ENVIRONMENT SNAPSHOT
- Unity 6000.3.19f1, URP 2D (Renderer2D), Linear color space.
- Input System 1.19.0 (new). **TMP Essentials imported** (`Assets/TextMesh Pro/`).
- Scripting backend Mono2x (must → IL2CPP/ARM64 for iOS).
- MCP: AnkleBreaker unity-mcp 2.39.5, port 7890.
- Not a git repository. Git init NOT authorized.
- **`GraphicsSettings.defaultRenderPipeline` was unset** (only the quality tier had URP)
  → camera clear/URP features misbehaved. Fixed: set to `Assets/Settings/UniversalRP.asset`.
- Assemblies: `StencilRelay.Runtime`, `StencilRelay.Editor`, `StencilRelay.Game`,
  `StencilRelay.Tests.EditMode`, `StencilRelay.Tests.PlayMode`.

## SCENES  (Assets/Scenes/, all in Build Settings in this order; SampleScene deleted)
- `Bootstrap.unity` — Camera, `GameServices`(+`AudioController`), `BootstrapRunner`
  (waits a frame, loads LightStudio).
- `LightStudio.unity` — Camera, EventSystem, slice-blocking quads (backdrop/table/rack),
  `StudioCanvas` (Title, Begin/Rack/Guide/Settings buttons, RackPanel with 12 level
  buttons in 3 trays, SettingsPanel with music/sfx sliders + haptics toggle + reset +
  confirm), `LightStudioController` (all refs wired).
- `ProjectionTable.unity` — Camera (ortho size 5.2), EventSystem, table quads,
  `ProjectionTable`(`ProjectionTableController`+`PlateInput`) with `BoardView`(6x6,
  cell 1.2) + `Plates` children, `TableCanvas` (MovesText, TutorialBanner, LayerTab A-D,
  ROTATE/FLIP/RESET, PROJECT, Pause + PausePanel, ResultPanel), `HudController` (wired).
- Rebuild with menu **Stencil Relay / Rebuild Scenes** (`Assets/Scripts/Editor/
  SceneBuilder.cs`). NOTE: SceneBuilder's DB wiring via SerializedObject silently
  failed (see KNOWN ISSUES) - DB is loaded via `Resources.Load` instead now.

## IMPORTANT PREFABS
- (none yet - HUD is authored directly in the scene; extract a LayerTab prefab in M8)

## CONTENT ASSETS
- `Assets/Content/Resources/LevelDatabase.asset` — **all 13 levels** (0-12). In
  Resources/ so `GameServices`/`ProjectionTableController` load it via
  `Resources.Load<LevelDatabase>("LevelDatabase")` (serialized-ref wiring was unreliable, BUG-002).
- `Assets/Content/Levels/Level_00..12_*.asset` — 13 `StencilLevelDefinition` assets.
- `Assets/Content/Shapes/Shape_L*_*.asset` — plate geometry assets.
- `Assets/Content/Materials/M_Cell.mat` — shader `Sprites/Default` (Renderer2D-safe;
  tinted per-renderer via MaterialPropertyBlock `_Color`/`_BaseColor`).
- **Regenerate all levels: menu `Stencil Relay / Rebuild All Levels`** (builder
  `Assets/Scripts/Editor/SliceLevelBuilder.cs` — authors shapes + allowed placements,
  then the SOLVER picks each level's optimal and derives move par, so a level can
  never silently become unsolvable). Assets stay hand-editable after.
- Level solve summary (from last build): every level solved; unique solution for all
  except L2 (5+). Search spaces 5-64. Move pars 0-5.
- Core additions for L9/L10: `LevelRuntime` parallax (`ParallaxShiftFor`, plate
  `depthPlane` + `parallaxPerDepth`); filter eval (`SensorSpec.RequiredFilter`,
  `StencilShapeAsset.frontFilters/backFilters`, `StencilPlate.OpenFilterAtBoardCell`,
  `CellGrid.UnrotateCell`/`Inverse`). "Filter Mismatch" is a distinct failure reason.
- Regenerate with menu **Stencil Relay / Rebuild Slice Levels (0-2)** (builder:
  `Assets/Scripts/Editor/SliceLevelBuilder.cs` — deterministic, hand-editable after).
- Validation of the 3: each level clean (optimal solves, solver Solved, par OK).
  DB shows 11 "errors" = levels 3-12 not authored yet (expected this milestone).
  Slice sim check: L0 initial fails / optimal solves / 1 solution of 8;
  L1 1 of 5; L2 5 of 16.

## KEY SCRIPTS
- `Assets/Scripts/Runtime/StencilRelay.Runtime.asmdef` (ns `StencilRelay.*`)
- `Assets/Scripts/Runtime/Core/CellGrid.cs` — `Rot90` enum + `CellGrid` dense bool
  mask (And/AndNot/Or/ContentEquals/RotateCell/RotatedFootprint). Deterministic.
- `Assets/Scripts/Runtime/Core/StencilShape.cs` — immutable plate geometry, front/back
  faces, flip = verbatim back or mirrored front, optional per-cell filter tag (L10 hook).
- `Assets/Scripts/Runtime/Core/StencilPlate.cs` — `PlateTransform` (offset/rot/flip),
  `StencilPlate.ResolveOpaque/ResolveOpenOnBoard`, `PlatePermissions` (allowed
  offsets/rotations/flip + `EnumeratePlacements`). `ExtraShift` = parallax hook (L9).
- `Assets/Scripts/Runtime/Core/Projection.cs` — `LightProjector` (name avoids
  UnityEngine.Projector collision), `SensorSpec`/`ConditionKind`, `StencilLayerStack.
  Composite` (order-independent AND-NOT), `ProjectionEvaluator.Evaluate` → per-condition
  `ProjectionResult` with named failure reasons + per-projector composited light.
- `Assets/Tests/EditMode/StencilRelay.Tests.EditMode.asmdef` + `StencilCoreTests.cs`.

## GAMEPLAY SYSTEMS
- **Deterministic core**: IMPLEMENTED + TESTED. CellGrid ops, rotation mappings, 4x
  rotation identity, flip mirror/back, plate resolve + clipping, 2-plate composite
  intersection, order independence, target/forbidden eval, dual-projector failure
  labelling, determinism, permission enumeration.
- **Level data + validator + solver**: IMPLEMENTED + TESTED.
  `Assets/Scripts/Runtime/Levels/`: `StencilShapeAsset`, `StencilLevelDefinition`
  (+ `PlateEntry`/`ProjectorEntry`/`SensorEntry`/`TutorialStep`/`LevelRuntime`,
  `BuildRuntime`/`BuildAtOptimal`/`MinMovesToOptimal`), `LevelDatabase`,
  `LevelSolver` (bounded brute force, cap 3M), `LevelValidation` (pure, shared with
  tests). `Assets/Scripts/Editor/`: `LevelValidatorWindow` (menu **Stencil Relay/
  Validate Levels**), `SliceLevelBuilder`.
- **Tests**: 28/28 EditMode PASS (job b125e7d38ff4, 0.6s). Adds level-pipeline tests:
  build/optimal differ, solver solved/unsolvable, min-move counting, validator flags
  (non-solving optimal, low movePar, tutorial-on-wrong-level, L11 missing 2nd
  projector), and a synthetic full-13 DB → exactly "13 / 13 VALID".
- **M3 presentation (STARTED)**: TMP Essential Resources imported
  (`Assets/TextMesh Pro/`, TMP_Settings asset). `StencilRelay.Game` asmdef (refs
  Runtime + InputSystem + TextMeshPro + URP Runtime/Core). View layer scaffold in
  `Assets/Scripts/Game/View/`: `BoardView` (grid<->world), `CellMeshBuilder` (merged
  quad mesh from a cell set), `PlateView` (mesh rebuilt from `plate.ResolveOpaque` so
  visual == evaluator), `ProjectionView` (light overlay + target/forbidden markers,
  reads `ProjectionResult` only). Compiles clean.
- `Assets/Scripts/Runtime/Levels/LevelManipulation.cs` — player-side attempt state
  (selection, committed transforms, per-plate move count with reset-refund, PROJECT
  lock/unlock, `RateSuccess` → Aligned/Precise/PerfectRegister). Pure, EditMode-tested.
- `Assets/Scripts/Runtime/Progression/` — `SaveData` + `SaveSystem` (versioned JSON in
  persistentDataPath, first-launch/corrupt/newer-version handling, no-downgrade of
  rating/moves, sequential unlock, reset). EditMode-tested.
- `Assets/Scripts/Game/` (StencilRelay.Game asmdef):
  - `GameServices` — persistent singleton: SaveSystem + AudioController + LevelDatabase
    (Resources) + PendingLevelId + GoToStudio/GoToLevel/NextLevelId.
  - `AudioController` — slice-level: sfx one-shots + saved volumes (mixer graph = M7).
  - `BootstrapRunner` — Bootstrap → LightStudio.
  - `ProjectionTableController` — builds BoardView/PlateViews/ProjectionViews from a
    level def, owns a LevelManipulation, PROJECT flow, pick/drag (PickPlateAt/
    PreviewDrag/EndDrag), events StateChanged / Projected.
  - `PlateInput` — pointer select + drag → snap to nearest allowed offset.
  - `HudController` — binds the ProjectionTable HUD (layer tabs, ROTATE/FLIP/RESET,
    PROJECT, MOVES, tutorial banner, pause, result panel) to the controller;
    RecordCompletion + advance on success.
  - `LightStudioController` — menu + Pattern Rack + settings, wired to SaveSystem.
  - `View/` — `BoardView`, `CellMeshBuilder`, `PlateView`, `ProjectionView`.

## UI/UX SYSTEM
- Scene-authored (SceneBuilder). Light Studio menu + Pattern Rack + Settings panels;
  Projection Table HUD (layer tabs A-D on left, ROTATE/FLIP/RESET on right frame,
  PROJECT lower-centre, MOVES + tutorial banner top, pause + result overlays). All TMP.
- Environment: EnvironmentBuilder (soft-shape sprites + role materials + URP 2D lights).
- Palette/typography/differentiation: Assets/Documentation/UI_UX_SYSTEM.md.
- M8 will do responsive QA per aspect + safe area + touch-target audit + button polish.

## AUDIO SYSTEM
- 19 clips in `Assets/Content/Resources/Audio/` (`AudioSynthBuilder`, menu Rebuild
  Audio — synth, deterministic, project-generated). AUDIO_PROVENANCE.md.
- `Assets/Content/Resources/StencilRelayMixer.mixer` — Master→Music/SFX/UI, params
  MasterVol/MusicVol/SFXVol/UIVol (built via `UnityEditor.Audio.AudioMixerController`
  reflection — see the execute_code in MCP-0010).
- `AudioController`: 3 AudioSources (SFX/UI/Ambience) → mixer groups; `Play`/`PlayUi`/
  `StartAmbience`/`Haptic`; `ApplyVolumes` linear→dB. Started in `Start()` (Awake
  order-safe). Triggers wired across controller/HUD/studio/finale.
- Haptics: `Handheld.Vibrate()` guarded by save setting, `#if UNITY_IOS||UNITY_ANDROID`
  (ponytail ceiling: single coarse buzz, no native plugin).

## LEVEL STATUS
```
Level 0  Calibration      data+validated / in-scene solve (PlayMode) / NOT manual QA
Level 1  First Alignment  data+validated / in-scene solve (PlayMode) / NOT manual QA
Level 2  Double Layer     data+validated / loadable / NOT manual QA
Level 3  Quarter Turn     data+validated / NOT in-scene tested / NOT manual QA
Level 4  Registration     data+validated / "
Level 5  Shadow Block     data+validated / "
Level 6  Forbidden Sensor data+validated / "
Level 7  Reverse Face     data+validated / "
Level 8  Precise Register data+validated / "
Level 9  Parallax         data+validated / "
Level 10 Filter Pass      data+validated / "
Level 11 Dual Projection  data+validated / "
Level 12 Master Plate     data+validated / "
```
"data+validated" = LevelValidation clean + optimal solves + solver confirms + initial
does not solve. "manual QA" (LEVEL_QA.md) = playing the real player-facing flow at each
aspect ratio — still 0/13.

## VALIDATION RESULT
**13 / 13 VALID** (real DB, 0 errors, 0 warnings). Each level: optimal solves,
solver confirms solvable, initial state does NOT solve, move par >= minimum.
Menu: Stencil Relay/Validate Levels. Covered by EditMode tests (RealDatabase_*).

## AUTOMATED TEST RESULT
EditMode: 56/56 PASS (core + rotation/inverse + filter eval + level pipeline +
real-DB 13/13 + manipulation/ratings + SaveSystem).
PlayMode: 6/6 PASS (Bootstrap→Studio w/ services; ProjectionTable L1→PerfectRegister;
wrong-config MISALIGNED+unlock; L0 calibration slide+rotate solve; tutorial→L1 unlock;
**finale plays once + persists + does not replay**).

## MANUAL QA RESULT
13/13 DATA VALID · 13/13 CONTROLLER-PATH PLAYTHROUGH (AllLevelsPlaythroughTests) ·
0/13 HUMAN TOUCH-PLAY (no tester available; agent cannot substitute — LEVEL_QA.md).

## UI QA STATUS
First pass done (UI_QA.md): 16:9 screens PASS; SafeArea + edge anchors verified;
runtime-UI audit PASS (all menus/HUD scene-authored). Per-aspect sweep incomplete.

## KNOWN ISSUES / BUGS
- **BUG-001** (M5 — mostly RESOLVED): environments rebuilt via `EnvironmentBuilder`
  (soft-shape sprites + role materials + URP 2D lights). Both scenes read as optical
  studios (see Assets/Screenshots/m5_*.png). Remaining for M8 UI-QA: button chrome
  reads a touch generic; HUD layer-tabs/rotate cluster near screen edge on 16:9;
  PROJECT label contrast on the ivory table; make LightStudio physical objects
  (lamp/rack/knobs) the clickable targets instead of overlaid invisible buttons.
- **BUG-002** (WORKAROUND applied): `SerializedObject...objectReferenceValue = <loaded
  LevelDatabase asset>` silently no-ops in editor scripting (fresh instances assign
  fine; the on-disk asset is rejected — suspected asmdef/type-identity quirk after
  repeated recompiles). Worked around by loading the DB from `Resources/`. Revisit if
  it recurs for other asset types; a project restart may clear it.
- **BUG-003** (env/tooling): `unity_screenshot_game` returns a stale/near-blank frame
  while the Editor is unfocused (MCP-driven). Game View renders correctly once forced
  (`RepaintAllViews`) but capture timing is unreliable. UI QA screenshots need a
  focused editor or device. Not a project bug.

## MCP CHANGE LOG
```
MCP-0001
Date: 2026-09-06
Agent: Claude (Sonnet 5) bootstrap session
Scene/Asset: SampleScene (transient)
Change: Created/renamed/moved/deleted __MCP_RoundTrip_Probe empty GameObject.
Reason: Verify MCP connectivity round-trip (master prompt §5).
Validation: GameObject.Find confirmed rename+move, then null after delete.
Result: PASS — MCP read/write verified. Scene left clean (sceneDirty was false).

MCP-0002
Date: 2026-09-06
Agent: Claude (Sonnet 5) bootstrap session
Scene/Asset: Assets/Scripts/Runtime/**, Assets/Tests/EditMode/**
Change: Added StencilRelay.Runtime asmdef + 4 core model scripts; Tests.EditMode
        asmdef + StencilCoreTests. AssetDatabase.Refresh + RequestScriptCompilation
        via unity_execute_code. Ran EditMode tests via unity_testing_run_tests.
Reason: M1 deterministic puzzle core (master prompt sections 13-19, 28).
Validation: 0 compile errors/warnings (unity_get_compilation_errors);
        18/18 EditMode tests pass (job d0cb885b2bbf).
Result: PASS.
```

MCP-0003
Date: 2026-09-06
Agent: Claude (Sonnet 5) bootstrap session
Scene/Asset: Assets/Scripts/Runtime/Levels/**, Assets/Scripts/Editor/**, Assets/Content/**
Change: Level data layer (StencilShapeAsset/StencilLevelDefinition/LevelDatabase/
        LevelSolver/LevelValidation) + Editor validator window + SliceLevelBuilder.
        Generated Content/ assets for Levels 0-2 via menu "Rebuild Slice Levels".
        (Split StencilShapeAsset to its own file - Unity requires SO class == filename;
        deleted+regenerated Content/ once to refresh script GUID.)
Reason: M2 data-driven content + validation (master prompt sections 12, 13, 27, 36-37).
Validation: 0 compile errors; 28/28 EditMode tests (job b125e7d38ff4); slice sim probe
        via unity_execute_code confirms initial fails / optimal solves for L0-L2.
Result: PASS for the 3 authored levels. DB intentionally incomplete (3/13).

MCP-0004
Date: 2026-09-06
Agent: Claude (Sonnet 5) bootstrap session
Scene/Asset: Assets/TextMesh Pro/**, Assets/Scripts/Game/**
Change: Imported TMP Essential Resources (via AssetDatabase.ImportPackage of the ugui
        PackageCache "TMP Essential Resources.unitypackage" — the menu importer needs
        a dialog). Added StencilRelay.Game asmdef + View layer (BoardView,
        CellMeshBuilder, PlateView, ProjectionView).
Reason: M3 presentation foundation.
Validation: 0 compile errors/warnings after refresh.
Result: PASS (scaffold only — not yet wired into a scene).

MCP-0005
Date: 2026-09-06
Agent: Claude (Sonnet 5) bootstrap session
Scene/Asset: Assets/Scripts/Runtime/Levels/LevelManipulation.cs, Assets/Tests/EditMode/**
Change: LevelManipulation (player attempt state + ratings) + 23 new EditMode tests
        (LevelManipulationTests). Ran full EditMode suite.
Reason: M3 - player-side manipulation model shared by views + evaluator.
Validation: 0 compile errors; 41/41 EditMode tests pass (job fbe56e479c61).
Result: PASS.

Note: `SampleScene` shows `sceneDirty:true` after refresh (Unity marks it on domain
reload); it has no game content and will be replaced by the 3 real scenes. Do not save it.

## GIT STATE
Not a repository. No commits. (Git init not authorized.)

MCP-0013
Date: 2026-09-06
Agent: Claude (Sonnet 5) session — FINAL LOCAL CLOSURE PASS
Scene/Asset: Assets/Scripts/{Editor,Game}/**, Assets/Scenes/**, Packages/manifest.json,
  Assets/Settings/UniversalRP.asset, Assets/Documentation/**
Change: `UiModal` helper (safe-area-inset scrim + backing + card); all modals in both
  scenes rebuilt with it (dark card / frosted Pause), hidden at build time; Pause +
  Calibration Guide. LightStudio menu → engraved plaque; EnvironmentBuilder lamp/lens/
  knobs repositioned, recessed BoardWell, key light 0.85→0.6, plate tints → ~0.21.
  Per-aspect QA sweep (SetCustomRenderingResolution). Removed com.anklebreaker.unity-mcp
  (Editor-only — evidenced). Icon importer → uncompressed. RELEASE_CANDIDATE.md +
  EXTERNAL_QA_HANDOFF.md written; FINAL_AUDIT.md regenerated. Rebuilt Win + iOS.
Reason: FINAL LOCAL CLOSURE PASS instructions.
Validation: 0 compile errors; 65/65 tests (56 EditMode + 9 PlayMode); 13/13 VALID;
  console clean; Windows build 0/0 + smoke PASS (Player.log 0 exceptions); iOS export
  Succeeded (0 errors; accepted engine shader hints). Screenshots qa*.
  MCP removal done LAST: manifest → 22 deps; PackageCache anklebreaker deleted (99
  files); Editor.log post-removal recompile = LogAssemblyErrors 0ms, 0 `error CS`,
  domain reload OK. Regression + builds were all run immediately BEFORE removal (MCP
  is Editor-only with zero runtime asmdef dependents, so removal can't affect runtime).
  The MCP bridge is now offline — re-add the manifest line for further AI-assisted work.
Result: PASS. PROJECT STATUS = BLOCKED_EXTERNAL.

MCP-0012
Date: 2026-09-06
Agent: Claude (Sonnet 5) session
Scene/Asset: Packages/manifest.json, ProjectSettings, Assets/Content/AppIcon.png, Assets/Documentation/**
Change: M9/M10. `AllLevelsPlaythroughTests` (2 tests, 13-level scene playthrough).
  Package trim 40→23 (PACKAGE_AUDIT.md). OFFLINE_AUDIT finalised (grep = no matches).
  Icon importer → uncompressed. Finale mark → ASCII (glyph fallback warning). Built
  StandaloneWindows64 (Succeeded) + iOS Xcode export (Succeeded, 0/0 after fixes).
Reason: master prompt sections 26-28, 30-33, 43, 80-83.
Validation: 65/65 tests green post-trim; both builds succeed; console clean.
Result: PASS. iOS device build/sign = external blocker.

MCP-0011
Date: 2026-09-06
Agent: Claude (Sonnet 5) session
Scene/Asset: ProjectSettings/ProjectSettings, Assets/Content/AppIcon.png, Assets/Scripts/{Editor,Game}/**, Assets/Scenes/**
Change: M8 mobile. `MobileSettingsBuilder` + `IconBuilder` editor scripts (menus).
  Player Settings applied (see MOBILE_RELEASE_CHECKLIST.md). Original app icon
  generated + assigned. `SafeAreaPanel` component; SceneBuilder Projection Table HUD
  reworked into a SafeArea container with edge-anchored controls + `Anchor()` helper;
  PROJECT restyled orange. `Application.targetFrameRate=60` in GameServices.
  ASSET_PROVENANCE.md + UI_QA.md written.
Reason: master prompt sections 21, 35-37, 40, 41, 68-70, 76-79.
Validation: 0 compile errors; 56/56 EditMode + 7/7 PlayMode; play-mode console clean;
  screenshot m8_iphone_table.png shows edge-anchored HUD, no clipping.
Result: PASS (editor-side). Actual iOS build + device QA = M10 / external blocker.

MCP-0010
Date: 2026-09-06
Agent: Claude (Sonnet 5) session
Scene/Asset: Assets/Content/Resources/{Audio,StencilRelayMixer.mixer}, Assets/Scripts/{Editor,Game}/**
Change: M7 audio. `AudioSynthBuilder` (menu Rebuild Audio) → 19 WAV clips synthesised
  offline. AudioMixer created via reflection over `UnityEditor.Audio.
  AudioMixerController` (CreateMixerControllerAtPath + CreateNewGroup + set
  masterGroup.children + AddExposedParameter + rename ExposedAudioParameter.name);
  moved to Resources/. `AudioController` rewritten (groups, clip cache, Play/PlayUi/
  StartAmbience/Haptic, dB conversion, Start()-time init). Triggers wired in
  ProjectionTableController / HudController / LightStudioController / FinaleController.
  GameServices starts ambience. AUDIO_PROVENANCE.md written.
Reason: master prompt sections 22-23, 63-65, 71-72, 86.
Validation: 0 compile errors; 56/56 EditMode + 7/7 PlayMode (new: mixer 4 groups +
  params load, clips load, settings drive MusicVol dB, one-shots don't throw);
  play session console clean with SFX firing.
Result: PASS.

MCP-0009
Date: 2026-09-06
Agent: Claude (Sonnet 5) session
Scene/Asset: Assets/Scripts/Game/**, Assets/Content/Resources/Sprites/**, Assets/Scenes/**
Change: M6 animation/VFX. PlateView rewritten with transient flourishes (slide/rotate/
  flip/select). ProjectionView rewritten: sprite markers (disc=target, ring=forbidden),
  light-mask reveal, projector beam, per-marker pop/pulse. CameraFeedback component
  (snap dip / settle). FinaleController + LightStudio FinalePanel (MASTER ALIGNMENT
  COMPLETE, rack illumination, persist). GameServices.JustCompletedFinale;
  HudController routes L12 success → finale. Result-panel fade-in. Sprites moved to
  Resources/Sprites/. Plate tint → 0.30 alpha (acrylic). AI_RULES RULE 21 (Unity gotchas).
Reason: master prompt sections 15-16, 39, 53-55, 61-62.
Validation: 0 compile errors; 56/56 EditMode + 6/6 PlayMode; play-mode console clean
  (L8 rotate+flip solve verified); screenshots m6_finale.png / m6_projected.png / m6_l8_manip.png.
Result: PASS.

MCP-0008
Date: 2026-09-06
Agent: Claude (Sonnet 5) session
Scene/Asset: Assets/Content/{Materials,Sprites}/**, Assets/Scenes/**, Assets/Scripts/{Editor,Game}/**, ProjectSettings/GraphicsSettings
Change: M5 environment. New editor builders: `MaterialLibraryBuilder` (menu Rebuild
  Materials - 26 role materials, Sprites/Default), `ProceduralSpriteBuilder` (menu
  Rebuild Sprites - 7 soft-shape PNGs, project-generated), `EnvironmentBuilder`
  (composes both scene environments + URP 2D lights). SceneBuilder now calls it.
  PlateView: soft drop shadow (spreads on select). ProjectionTableController: key
  light dims + warm projector pool raises on PROJECT (finds KeyLight/ProjectorSpot).
  Game + Editor asmdefs: added Unity.RenderPipelines.Universal.2D.Runtime (Light2D).
  Button chrome: soft-rect sprite + readable dark fill + color states.
Reason: master prompt sections 18, 20, 40-42, 56-62.
Validation: 0 compile errors; 56/56 EditMode + 5/5 PlayMode pass; play-mode console
  clean; screenshots m5_studio2.png / m5_table.png / m5_table_projected.png confirm
  both scenes render as optical studios and the projection/targets read.
Result: PASS. Projection-VFX polish (beam anim, target pulse) + finale → M6.

MCP-0007
Date: 2026-09-06
Agent: Claude (Sonnet 5) session
Scene/Asset: Assets/Scripts/Runtime/Core/**, Runtime/Levels/**, Assets/Content/{Shapes,Levels,Resources}/**
Change: M4 — core: parallax wiring (L9), filter evaluation + inverse rotation (L10).
  Reworked builder (SliceLevelBuilder → "Rebuild All Levels") to author shapes +
  placements for all 13, then solver-derive optimal + move par. Tuned L12 (3 full
  sheets: rotate + flip + slide + dual projection). Validator: dropped 2 noisy
  warnings (unique solution, blocker-plate no-openings), added "all plates holeless" error.
Reason: master prompt sections 4 (13 stages), 12-14, 27, 32-38, 83.
Validation: 0 compile errors; 56/56 EditMode + 5/5 PlayMode; builder + validator =
  "13 / 13 VALID", 0 warnings; every level: optimal solves, solver Solved, initial fails.
Result: PASS.

MCP-0006
Date: 2026-09-06
Agent: Claude (Sonnet 5) session
Scene/Asset: Assets/Scripts/{Game,Editor,Runtime/Progression}/**, Assets/Scenes/**,
  Assets/Content/{Materials,Resources}/**, ProjectSettings/GraphicsSettings
Change: M3 playable slice — SaveSystem, GameServices/AudioController/BootstrapRunner,
  ProjectionTableController/PlateInput/HudController/LightStudioController, M_Cell
  material, SceneBuilder → Bootstrap/LightStudio/ProjectionTable scenes + Build
  Settings (SampleScene deleted). Moved LevelDatabase to Resources/ (BUG-002).
  Set GraphicsSettings.defaultRenderPipeline = UniversalRP (was unset).
Reason: master prompt sections 9-10 (vertical slice), 25 (save), 38-44 (scene arch), 73-75.
Validation: 0 compile errors/warnings; 51/51 EditMode + 3/3 PlayMode pass; play-mode
  console clean; Game View screenshot shows menu + table render (art is slice-level).
Result: PASS (functional). Visual quality = BUG-001, deferred to M5/M8 per plan.

## NEXT EXACT ACTIONS
DONE: M0-M11. `PROJECT STATUS = BLOCKED_EXTERNAL` (FINAL_AUDIT.md).

To resume / finish externally:
1. On macOS: open `StencilRelayBuilds/iOS/Unity-iPhone.xcodeproj` in Xcode, set the
   signing team, archive, install on an iPhone/iPad in landscape.
2. Human manual QA: play all 13 levels through the touch UI at 16:9 / 19.5:9 / 4:3;
   fill LEVEL_QA.md "Human manual" column; log any BUG-NNN.
3. On device: profile Level 11 and Level 12 (frame time, transparent overdraw); tune
   the full-sheet plate materials if overdraw is high.
Local polish (optional, not blocking): inset the modal panels to safe area; reduce the
full-sheet board wash; reposition the studio storage sprites; complete the per-aspect
screenshot sweep in a focused editor.
Before the true final build: remove the `com.anklebreaker.unity-mcp` line from
`Packages/manifest.json` (PACKAGE_AUDIT.md).

--- superseded planning below ---

DONE: M0-M6. Builders: Rebuild Materials / Rebuild Sprites / Rebuild All Levels /
Rebuild Scenes. 56 EditMode + 6 PlayMode green, console clean, screenshots current.

**M7 — Original audio + AudioMixer**:
1. `AudioMixer` asset (Master → Music, SFX, UI) with exposed params `MusicVol`/`SFXVol`;
   `AudioController.ApplyVolumes` already targets those names — wire the mixer + route
   the sfx/music AudioSources through it.
2. Generate original SFX as .wav via an editor script (synthesised offline, no samples):
   plate pickup / slide / snap / registration pin / rotation / flip / table touch /
   projector switch / projector startup / light hum (loop) / target activation /
   forbidden cue / success chime / misalignment / rack slide / pause / settings /
   final completion. + low room-tone ambience loop. Distinct per interaction (§22, §86).
3. `Assets/Documentation/AUDIO_PROVENANCE.md` — one entry per clip, "Project-generated
   original / External dependency: None / method: <synth>".
4. Wire triggers: PlateInput/controller → pickup/slide/snap/rotate/flip; PROJECT →
   switch+startup; result → success/misalignment; targets → activation/forbidden cue;
   HUD → table touch / rack slide / pause / settings; finale → final completion.
   Haptics (`Handheld.Vibrate` guarded by save setting) on snap/rotate/pin/PROJECT/
   Perfect Register/forbidden.
5. Settings sliders already wired to `SetAudio` + `ApplyVolumes` — verify they move the
   mixer live. PlayMode test: setting persists + mixer param changes.

Then M8 mobile (Player Settings IL2CPP/ARM64/landscape/bundle/icon, safe area,
responsive UI QA per aspect, touch-first) → M9 manual QA 13/13 + UI QA → M10
package/offline/MCP cleanup + build/export → M11 final audit.

--- (M6 detail, done) ---
**M6 — Animation / VFX**:
1. Plate feel: select-lift ease, slide lerp to snap (currently instant mesh rebuild),
   90° rotate tween, flip flip-tween, layer-settle, registration-pin flash. Precise, short.
2. Projection sequence (§16): input locks → room dims (done) → projector pool up (done) →
   light beam sprite animates from projector to board → openings glow → per-target
   response (lit=sage bloom, unlit=stays dim, forbidden-exposed=coral pulse) → result.
   `ProjectionView`: animate the light mask reveal + per-marker state anim.
3. Result transitions: PATTERN LOCKED (light stabilises, targets glow, reg points light,
   plate settles, rating mark) / MISALIGNED (soft, names reason) / final.
4. Pattern-rack slide anim in LightStudio when a level is picked.
5. **Finale**: completing L12 → `MASTER ALIGNMENT COMPLETE` — all 3 racks illuminate,
   final composed projection on the table, calibration ruler lights, completion tone
   (audio = M7), achievement mark; persist `finaleShown`; later launches acknowledge
   without replaying. Actions: Review Patterns / Return to Studio.
6. Camera feedback: tiny zoom on snap, dim on PROJECT, gentle pulse on PERFECT REGISTER. No shake.
7. PlayMode test: finale triggers + `finaleShown` persists; anims don't break input lock.

Then M7 audio+mixer → M8 mobile (Player Settings IL2CPP/ARM64/landscape/bundle/icon,
safe area, responsive UI QA per aspect, touch-first) → M9 manual QA 13/13 + UI QA →
M10 package/offline/MCP cleanup + build/export → M11 final audit.
```
