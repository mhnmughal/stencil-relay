# VERTICAL_SLICE_REVIEW.md — Stencil Relay

Master prompt §10 / §93 exit review. Date: 2026-09-06. Reviewer: build session (Claude).

Verdict: **ARCHITECTURE PASS — proceed to content (M4), with art/mobile weaknesses
tracked for M5/M8.** The slice uses the final architecture; no throwaway systems.

## CORE GAMEPLAY
| Item | State | Evidence |
|---|---|---|
| plate selection | PASS | LevelManipulation.Select; PlateInput; HUD tabs; tests |
| slide | PASS | TrySlide/TrySlideToNearest; PlateInput drag→snap; tests |
| deterministic snapping | PASS | offsets restricted to PlatePermissions.AllowedOffsets; tests |
| rotation (90°) | PASS | RotateStep cycles AllowedRotations; PlayMode L0 solve |
| flip | PASS (core) / NOT EXERCISED in slice | core tests; first used at L7 |
| layer overlap / composition | PASS | StencilLayerStack.Composite AND-NOT; order-independent; tests |
| projection | PASS | ProjectionEvaluator; PlayMode L1→PerfectRegister |
| required target eval | PASS | tests + PlayMode |
| forbidden sensor eval | PASS (core) | core tests (first used at L6) |
| failure feedback | PASS | MISALIGNED + named reasons; PlayMode test |
| success | PASS | PATTERN LOCKED / CALIBRATION COMPLETE |
| reset | PASS | ResetPlate (move-refund) / ResetAll; tests |
| determinism | PASS | pure functions; repeat-call test |

## UI/UX
Light Studio, Pattern Rack, Projection Table, layer selector, manipulation controls,
PROJECT control, pause (frosted overlay), settings foundation, tutorial guidance —
all **scene-authored** and wired (`SceneBuilder`, `HudController`, `LightStudioController`).
No wholesale runtime UI construction (Rule 7 held).

WEAKNESS (BUG-001): visual quality is slice-level blocking — pale-on-pale, plain-quad
table, low-contrast buttons, edge clipping at 16:9. Not a systems problem; the views
read the model correctly. → M5 (materials/environment) + M8 (responsive UI QA).

## PRESENTATION
- Environment: slice quads only → M5.
- Materials: one `M_Cell` (Sprites/Default) → M5 (acrylic/glass/brass set).
- Projection readability: data correct, visuals minimal → M5.
- Lighting: none → M5.
- Camera: fixed ortho, no diagonal drafting angle yet → M5.
- Interaction animation: none → M6.
- Audio identity: none → M7.

## MOBILE
- Landscape orientation / IL2CPP / ARM64 / bundle id / icon: NOT SET → M8.
- CanvasScaler = ScaleWithScreenSize 1920×1080, match 0.5 — set, not QA'd.
- Safe area: NOT implemented → M8.
- Touch targets: buttons 70–84 px; layer tabs 70 px — at/below the 88 px guideline → M8.

## ORIGINALITY (vs Misprint Department / Punchcard Operator)
- Core interaction = physical layer alignment, not commands/punch-cards/printing. PASS.
- No Status Rail / Tool Dock / Control Room / Card Vault / top HUD bar / rectangular
  level-card grid. Controls are embedded in the table frame; layer tabs on the table
  edge; PROJECT is a lower-frame lamp switch; pause is a frosted calibration overlay;
  Pattern Rack is trays of plates. PASS (structural, not recolour).
- Ratings ALIGNED / PRECISE / PERFECT REGISTER — unique terms. PASS.
- Scene names Bootstrap / LightStudio / ProjectionTable. PASS.
- Full audit again at M9 once art exists.

## ENGINEERING
- Console: clean (0 errors/warnings, clean in play mode). PASS.
- Automated tests: 51 EditMode + 5 PlayMode. PASS.
- Save: versioned JSON, tested. PASS.
- Scene flow: Bootstrap→Studio→Table→Studio, PlayMode-verified. PASS.
- Memory docs: current. PASS.

## DECISION
Proceed to **M4 (Levels 3–12 data)** — level authoring is architecture-complete and
independent of art; the validator + solver + per-level tests prove each level. The
art/lighting/animation/audio/mobile passes (M5–M8) then cover all 13 levels together.
Deviation from a strict "fix every weakness first" reading is deliberate and recorded:
the weaknesses are scheduled milestones, not architectural faults.
