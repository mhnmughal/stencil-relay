# GAME_DESIGN.md — Stencil Relay

2026-09-07 polish: tutorial instructions follow required placement/rotation state;
move pars and layer faces are visible; filter categories use matching numbers on
openings/targets; projector IDs accompany sensor labels. Success saves immediately.
Inspect Projection exposes the evaluated board without changing its result, and
Adjust Layers returns to manipulation. Human touch-play QA is skipped by owner request.

## Fantasy
You are a registration technician in a quiet optical drafting studio. Each "pattern"
is a stack of translucent stencil plates that must be aligned so a projector lamp
casts one exact geometric figure onto the target board.

## Core loop
INSPECT (read targets + current plates) → SELECT a plate → ALIGN (slide / rotate /
flip through discrete snap states) → PREVIEW (see combined arrangement) → PROJECT
(lock input, lamp on, light composites through layers) → VERIFY (targets / forbidden
sensors respond individually) → ADJUST or LOCK.

## Verbs
- **Slide** — drag a selected plate between allowed grid snap positions. Deterministic
  snapping; valid snaps highlighted; no free positioning.
- **Rotate** — 90° steps (0/90/180/270) via a brass dial on the table frame.
- **Flip** — reveals the plate's BACK face (different geometry). Introduced level 7.
- **Project** — center-lower lamp switch. Locks manipulation, evaluates, shows result.

## Evaluation
Deterministic grid mask. `REQUIRED TARGET` must be lit; `FORBIDDEN SENSOR` must be
dark; later `MULTI-TARGET` and `DUAL PROJECTOR` (two lamps, one plate stack, both
must satisfy). Filters (level 10) gate light by category — always paired with a
shape/symbol, never color alone (§20 accessibility).

## Failure
Never a hard game-over. Result screen `MISALIGNED` names the reason(s): Target Unlit
/ Forbidden Sensor Lit / Projection Incomplete / Filter Mismatch / Second Projector
Failed. `ADJUST LAYERS` returns instantly to manipulation with state intact.

## Success
`PATTERN LOCKED`: light stabilizes → targets glow → registration points illuminate →
selected plate settles → rating mark. Actions: Next Pattern / Reconfigure / Pattern Rack.

## Ratings
- **ALIGNED** — solved.
- **PRECISE** — solved within move par.
- **PERFECT REGISTER** — reached the authored optimal configuration.
Level 0 awards no rating (tutorial).

## Progression
Level 0 available at first launch → completing Calibration unlocks Level 1 → sequential
1→12. Completed levels stay replayable from the Pattern Rack. Better ratings never
overwritten by worse replays.

## Difficulty philosophy (§14)
Difficulty comes from spatial/occlusion/projection reasoning, never from hidden rules,
imprecise controls, unreadable boards, random alignment, excessive layers, or padded
move counts. Interaction stays legible; reasoning gets hard.

## Camera / feel
Fixed angled drafting-table view. Subtle feedback only (slight zoom on snap, dim on
PROJECT, gentle pulse on PERFECT REGISTER). No shake. Animations precise, not bouncy;
precision over animation length (§15).

## Audio identity (§22)
Precise, light, optical, acrylic, glass-like, restrained, calm. Distinct SFX per
interaction (pickup / slide / snap / pin / rotate / flip / table touch / projector
switch / startup / hum / target / forbidden cue / success / misalignment / rack /
pause / settings / final). No shared generic click. Project-generated originals.

## Final completion (§39)
Completing Level 12 → `MASTER ALIGNMENT COMPLETE`: all three pattern racks illuminate,
final composed geometric projection on the table, calibration ruler lights, completion
tone, achievement mark. Actions: Review Patterns / Return to Studio. Persisted; later
launches acknowledge it without replaying the finale.
