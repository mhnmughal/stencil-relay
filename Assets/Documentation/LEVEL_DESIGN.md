# LEVEL_DESIGN.md — Stencil Relay

13 stages. Every level states the one thing it exists to teach or test. Exact grids,
plate shapes and optimal transforms are recorded here as each level is authored (see
"Data" subsections — filled during implementation, must match the ScriptableObject).

Board grid convention: origin bottom-left, `(col,row)`, cells are integers.
Rot90: CCW positive. Plate offset in cells.

---

## SET A — ALIGNMENT (slide, then rotate, then combine)

### Level 0 — CALIBRATION  (tutorial, no rating)
Reason: teach select → slide → read registration mark → rotate → PROJECT → success.
1 plate, 1 target, slide + one rotate. Contextual highlights, no text walls.
Steps: highlight Plate A → "Select the stencil" → highlight slide direction → "Slide
it onto the registration mark" → "Rotate the plate" → highlight PROJECT → "Project the
pattern" → target lights → CALIBRATION COMPLETE → BEGIN PATTERN 01.
Tutorial completion saves; replayable from Calibration Guide.

### Level 1 — FIRST ALIGNMENT
Reason: first unaided puzzle. One movable plate, slide only, one target. The plate's
opening must be slid so its aperture sits over the target.

### Level 2 — DOUBLE LAYER
Reason: two plates must *both* align; teaches that light must pass every layer
(AND composition). Slide only. Overlap of two apertures forms the needed opening.

### Level 3 — QUARTER TURN
Reason: rotation as a reasoning tool. One plate's aperture only reaches the target
after a 90° turn; a distractor orientation is "close but wrong".

### Level 4 — REGISTRATION
Reason: first mastery gate for Set A. Two plates, slide + rotate, multiple alignment
marks that must all line up. Introduces move par (PRECISE).

---

## SET B — OCCLUSION (blockers, forbidden sensors, flip, precision)

### Level 5 — SHADOW BLOCK
Reason: introduce BLOCKER regions. One plate carves the light path; a second plate
must be positioned so its blocker kills stray light that would otherwise spill.

### Level 6 — FORBIDDEN SENSOR
Reason: introduce FORBIDDEN sensors. Targets A & B must be lit while sensor C stays
dark — the obvious wide aperture exposes C, forcing a narrower composed opening.

### Level 7 — REVERSE FACE
Reason: introduce FLIP. The front face cannot solve it; the back face has the needed
geometry. Player must infer which side is required before committing.

### Level 8 — PRECISE REGISTER
Reason: Set B mastery. slide + rotate + flip + blockers + required targets, with a
tight move par. Rewards planning the manipulation order (move efficiency).

---

## SET C — PROJECTION (depth, filters, dual projection, mastery)

### Level 9 — PARALLAX
Reason: controlled layer-depth. Plates on different depth planes shift the projected
opening by a fixed designed offset per plane. Deterministic authored offsets, not
simulated optics; the shift is shown clearly.

### Level 10 — FILTER PASS
Reason: light/filter categories. Each target accepts one filter category, shown by a
distinct symbol + border pattern (not color alone). Plates carry filter tags on
their apertures; player routes the right category to each target.

### Level 11 — DUAL PROJECTION  (major milestone)
Reason: two projectors evaluate the same plate stack simultaneously. Both projected
results must satisfy their own targets. UI shows both boards; failure names which
projector/target condition failed. Performance profiled here.

### Level 12 — MASTER PLATE  (campaign culmination)
Reason: a single coherent final puzzle combining a chosen subset — slide, rotation,
flip, blockers, multi-target, forbidden sensor, dual projection — only where each
adds to one clean idea. Deliberate, not a checklist dump. Completion triggers
MASTER ALIGNMENT COMPLETE.

---

---

## AUTHORED DATA (as built 2026-09-06 — all 13/13 VALID, solver-confirmed)

Source of truth = the `.asset` files; regenerate via **Stencil Relay / Rebuild All
Levels** (`SliceLevelBuilder.cs`). Optimal + move par are solver-derived. Board origin
bottom-left. "sheet" = footprint == board (always covers the board).

| L | Board | Plates (shape · initial · permits) | Sensors | Par |
|--:|:--|:--|:--|--:|
| 0 | 6×6 | A: sheet, hole (4,1); slide{(0,0),(2,2),(1,2),(2,1)} + rot{None,CW90} | T (3,3) | tutorial |
| 1 | 6×6 | A: sheet, hole (1,1); slide 5 pos | T (3,3) | 1 |
| 2 | 6×6 | A: H-slot; B: V-slot; both slide | T (3,3) | 2 |
| 3 | 6×6 | A: V-domino (2,2)(2,3); slide + rot{None,CW90} | T (3,3),(4,3) | 2 |
| 4 | 6×6 | A: hole (1,1) slide; B: hole (4,1) slide + rot{None,CW90} | T (3,3) | 3 |
| 5 | 6×6 | A: V-column x=3 (y1-4) slide; B: 3×2 SOLID blocker slide | T (3,1) · X (3,4) | 3 |
| 6 | 6×6 | A: H-row y=2 slide; B: gate holes (1,3)(5,3) slide | A (1,3) · B (5,3) · X (3,3) | 3 |
| 7 | 6×6 | A: front V-slot x=1, **back** hole (4,1); slide + flip | T (4,1) · X (4,0),(4,2) | 2 |
| 8 | 6×6 | A: V-domino slide+rot{None,CW90}; B: front V-domino / **back** H-domino, slide+flip | T (2,3),(3,3) · X (4,3) | 5 |
| 9 | 7×6 | A depth0, B depth2 (parallax 1 cell/step); both hole (1,2), slide | T (4,2) · X (5,2) | 4 |
| 10 | 7×7 | A,B: holes (1,1)=filter1, (3,1)=filter2; slide | Square→(3,3) f1 · Triangle→(5,3) f2 | 4 |
| 11 | 6×6 | A,B: holes (1,3)(4,3); slide. P1 aperture x≤2, P2 aperture x≥3 | L (1,3)/P1 · R (4,3)/P2 · XL (1,1)/P1 · XR (4,5)/P2 | 3 |
| 12 | 7×7 | A: sheet, figure pre-rotated (needs CW90); B: sheet, figure on **back** (needs flip); C: sheet, wide patch (needs slide). P1+P2 full | Bar (2,3),(3,3)/P1 · Post (4,2),(4,3)/P2 · XP1 (3,2) · XP2 (4,4) | 5 |

## Solvability rule (§13)
Each main level (1–12) carries an authored `optimalTransforms` that `LevelValidator`
runs through `Composite`+`Evaluate` to confirm `DATA / SIMULATION VALID`. That is
separate from `MANUALLY PLAYABLE` (recorded in LEVEL_QA.md). Neither substitutes for
the other.
