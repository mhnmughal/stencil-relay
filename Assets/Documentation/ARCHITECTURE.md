# ARCHITECTURE.md — Stencil Relay

Status: historical M0 architecture sketch; current implementation summary below
supersedes the planned names and behavior in the remaining design sketch.

## Implemented architecture (2026-09-07)

- Runtime assembly: deterministic `CellGrid`, `StencilShape`, `StencilPlate`,
  `StencilLayerStack`, `ProjectionEvaluator`; level definitions, solver, validator,
  `LevelManipulation`; versioned JSON `SaveSystem` and progression.
- Game assembly: `GameServices`, `ProjectionTableController`, `HudController`,
  `LightStudioController`, `PlateInput`, `AudioController`, `FinaleController`,
  `SafeAreaPanel`, camera feedback and views.
- Editor assembly: reproducible level/scene/material/sprite/audio/icon builders,
  mobile settings, level validator and `ReleaseVerification` batch build entry points.
- Test assemblies: EditMode and PlayMode, including scene-UI regressions.
- Scenes: Bootstrap -> LightStudio -> ProjectionTable. Main UI and environments
  are saved scene hierarchy. Gameplay plate/light/grid meshes and sensor/filter
  labels are dynamic because level data changes; generated meshes are released.
- The board uses 7.2 / max(width,height) cell size; grid, pointer coordinates,
  plates and sensor markers share that mapping. Successful PROJECT immediately
  persists completion; result buttons control navigation, never whether progress saves.
- Content database loads from local Resources. No network package, online service
  initialization, runtime HTTP/socket API, or third-party runtime SDK is used.
- Final package set deliberately omits MCP; ordinary menus/batch mode remain usable.

## Assembly layout *(planned)*
```
Assets/Scripts/Runtime/   StencilRelay.Runtime.asmdef   (no editor refs)
Assets/Scripts/Editor/    StencilRelay.Editor.asmdef    (refs Runtime + UnityEditor)
Assets/Tests/EditMode/    StencilRelay.Tests.EditMode.asmdef
Assets/Tests/PlayMode/    StencilRelay.Tests.PlayMode.asmdef
```

## Deterministic core (pure C#, no UnityEngine scene deps) *(planned)*
The puzzle is evaluated on an integer cell grid. Rendering reads the same data, so
visuals can never change logic (master prompt §17 Determinism).

- `CellMask` — immutable bitset over a `w×h` grid. Ops: `Or`, `And`, `Not`, `Any`,
  `Overlaps`, cell test. Value-equatable.
- `StencilShape` — authored per plate: which local cells are OPEN (light passes),
  which are BLOCKER (light stops), plus optional per-cell `FilterTag`. Has a FRONT
  and a BACK face (BACK used by flip).
- `PlateTransform` — `(int2 offset, Rot90 rotation, bool flipped)`. All discrete.
- `StencilPlate` — `StencilShape` + current `PlateTransform` + movement permissions
  (allowed offsets, allowed rotations, flip allowed). `Resolve()` → world-space
  `open` / `blocker` `CellMask` for the current transform.
- `StencilLayerStack` — ordered plates. `Composite()`: light starts as the projector
  aperture mask; for each layer, `light = (light AND layer.open) AND NOT layer.blocker`
  (filters handled in evaluator). Produces the final projected `CellMask` per projector.
- `ProjectionEvaluator` — given composited light mask(s) + `TargetSpec[]` +
  `ForbiddenSpec[]` (+ filter requirements), returns `ProjectionResult` with a
  per-condition breakdown (which target lit/unlit, which forbidden exposed, filter
  mismatch, which projector failed). No generic pass/fail only (§16).
- `MoveCounter` — counts committed manipulations (slide-to-new-snap, rotate step,
  flip). Used for PRECISE / PERFECT REGISTER ratings.

Determinism contract: `Composite` + `Evaluate` are pure functions of
`(level definition, list of PlateTransform)`. Covered by `StencilCoreTests`.

## Data-driven content *(planned)*
- `StencilShapeAsset : ScriptableObject` — front/back cell layout, filter tags.
- `StencilLevelDefinition : ScriptableObject` — id (00–12), title, set, board size,
  plate list (shape ref + initial transform + permissions), targets, forbidden
  sensors, filters, projector config (1 or 2), optimal transforms, move par,
  tutorial steps, visual modifiers.
- `LevelDatabase : ScriptableObject` — ordered list of the 13 definitions.
- `LevelValidator` (Editor) — validates ids 00–12, uniqueness, references, legal
  transforms, that `optimal transforms` actually solve, move par ≥ optimal moves,
  Level 0 tutorial flag, Level 11 dual-projector config. Output: `13/13 VALID` or
  exact level+reason. Never suppresses invalid data (§27).

## Scene flow *(planned)*
`Bootstrap` (persistent `GameServices`: Save, Settings, Audio, SceneRouter) →
`LightStudio` (menu + Pattern Rack) → `ProjectionTable` (loads chosen level;
`ProjectionTableController` owns `LevelController`, `LayerStackController`,
`ProjectionController`, `ResultController`) → back to `LightStudio`.
Additive vs single-scene load: single-scene load with `Bootstrap` services kept via
`DontDestroyOnLoad`.

## Save *(planned)*
`SaveData` versioned JSON at `Application.persistentDataPath/stencilrelay.save`.
Persists: tutorial done, unlocked level index, per-level best rating + best moves,
music/sfx volume, haptics on/off, final-completion flag. Handles missing / corrupt
(back up + reset to defaults) / newer-version (refuse downgrade, keep safe subset) /
explicit reset (confirmation required). Never overwrites a better rating (§25).

## Rendering approach (2D URP)
Plates are semi-transparent sprites on sorting layers `Plate_A..D` with small Z/scale
parallax. The projected pattern is a sprite/mesh driven from the composited `CellMask`
(a generated `Texture2D` mask or per-cell quads). URP 2D lights: one key studio light,
one projector spot light enabled during PROJECT. Soft shadows = blurred offset sprite
copies, not realtime shadow casters, for mobile cost.

## Input
New Input System. Pointer (touch/mouse unified) for plate drag + control taps.
`EnhancedTouch` for multi-touch guard (ignore 2nd finger during a drag).
