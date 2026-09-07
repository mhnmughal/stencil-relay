# UI_QA.md — Stencil Relay

> Latest 2026-09-07 pass: `POLISH_OFFLINE_AUDIT.md`, screenshots `offline_*`.
> Added actual scene-UI tests for modal blocking, available controls, result routing,
> and audio handle/fill/percentage/save/mixer agreement. Human touch-play is skipped
> by explicit owner request. Earlier QA notes below describe the previous release.

M11 final local visual QA. Game View driven in play mode via
`PlayModeWindow.SetCustomRenderingResolution` + `SetPlayModeFocused(true)`; screenshots
in `Assets/Screenshots/qa*_*.png`. Physical-device visual verification remains external.

## Aspect ratios swept
| Config | px | Screens checked |
|---|---|---|
| 16:9 phone | 1920×1080 | Light Studio, Projection Table, Result modal, Finale |
| Modern iPhone (19.5:9) | 2532×1170 | Light Studio, Projection Table |
| Wide phone | 2340×1080 | Projection Table, Pause overlay |
| Standard iPad (4:3) | 2160×1620 | Projection Table + PROJECT result |
| iPad-Pro-ish (4:3) | (4:3 covers it) | — |

## Findings — PASS

**Responsive structure**
- CanvasScaler ScaleWithScreenSize 1920×1080, match 0.5 on all 3 scenes.
- Projection Table HUD under a `SafeArea` (SafeAreaPanel → `Screen.safeArea`), controls
  edge-anchored: layer tabs → left, ROTATE/FLIP/RESET → right, PROJECT → bottom-centre
  (orange lamp switch), MOVES + tutorial banner → top-centre, Pause → top-right.
  No clipping or unsafe controls at any swept aspect.
- Light Studio menu is an **engraved plaque** on the left of the light table
  (STENCIL RELAY + Calibration / Pattern Rack / Calibration Guide / Light Settings);
  lamp / lens / knobs / rack repositioned so nothing overlaps the plaque.

**Modals** — all rebuilt as full-screen `UiModal`: input-blocking scrim + full backing
+ **SafeArea container** + centred card. Verified inset & centred at every aspect.
- Pause = **frosted calibration overlay** (§51): pale frosted plate over the workspace,
  "CALIBRATION" + Resume Alignment / Restart Pattern / Calibration Guide / Return to Studio.
- Result = dark card: `PATTERN LOCKED` / `PERFECT REGISTER` / actions, or `MISALIGNED`
  + the named reason (e.g. "Target Unlit (A)", "Forbidden Sensor Lit", "(T, P2)").
- Settings / Rack (3 trays × 4) / Reset-confirm / Finale = dark cards, readable, buttons
  prominent, scrim dims the HUD behind for focus.
- Modals start **hidden at build time** (SceneBuilder `SetActive(false)`), not just via
  the controller's `Start()` — no first-frame flash. Covered by a PlayMode assertion.

**BOARD WASH — decision**
Full-sheet-plate levels were washing the board pale-cyan. After visual inspection:
the light table now uses a **neutral recessed drafting well** (mid-grey, subtle inner
shadow) with the key light reduced and plate tint lowered to ~0.20–0.22 alpha. Result:
plates read as translucent acrylic on a lit optical surface; targets (discs), forbidden
sensors (rings), blocker plates, holes and the projected cyan opening all read clearly;
overlap/occlusion is visible. The glowing-table look is retained (thematic, §20) but
no longer dominates. **BOARD WASH: RESOLVED — recessed well + tuned tints/light.**

**Touch targets** — layer tabs 78×86, frame controls 180×76, PROJECT 300×92, pause
76×64, modal buttons ≥200×58: all at/above the comfortable threshold.

## Runtime-UI audit (§19 / §86) — PASS
grep of `Assets/Scripts` for `new GameObject(` / `AddComponent<Button|Image|
TextMeshProUGUI>` at runtime: the only runtime-created objects are gameplay/VFX
(plate meshes, projection light mesh + sensor-marker sprites, audio sources, one
`CanvasGroup` added to the authored ResultPanel for its fade). **All menus / HUD /
panels are authored by `SceneBuilder` at build time.** No wholesale runtime UI.

## Environment / art polish pass (post-RC) — PASS

The scene-authored environment was re-composed from a crafted procedural sprite set
(`ProceduralSpriteBuilder` → 19 sprites: bevel panels, brushed-metal strip, glass,
dome pins, brass dial, lens, lamp globe, corner bracket, vignette, beam cone, …) so
both scenes read as an **optical drafting studio** rather than flat abstract shapes.

- **Projection Table** (`p4_table_*` screenshots): brushed-aluminium frame → warm
  drafting surface → recessed board well; brass registration rails with engraved
  ticks top & bottom; brass corner brackets + dome alignment pins at the board
  corners; a projector head seated **above the frame, clear of the top-centre MOVES
  counter** (verified iPhone 19.5:9 + iPad 4:3 — no HUD overlap, no stray lines);
  faint cyan alignment shaft; warm halo behind the PROJECT control; ambient vignette.
  Board + plates + targets + forbidden sensors still read clearly (BOARD WASH
  decision unchanged).
- **Light Studio** (`p4_studio_iphone`): drafting table with brushed-alu frame,
  brass T-square rail + ticks, a **pendant calibration lamp** (stem from above the
  frame, globe + warm 2D point light over the table), a magnifier loupe with brass
  handle, a glass set-square, the translucent **Pattern Rack** tucked against the
  right edge (8 acrylic sheets, cyan rack light), a brass control-knob strip, and a
  vignette. Menu plaque + all four buttons legible over it at every swept aspect.
- Regression after the pass: **65/65 tests** (56 EditMode + 9 PlayMode), validator
  **13/13 VALID**, console clean, Windows + iOS builds green (iOS: same 18 accepted
  engine APV-shader precision hints, §30).

## Not verified (external)
- Physical-device look (real notch / Dynamic Island / home indicator).
- Touch feel, accidental taps, animation-delay comfort — human, on device.
