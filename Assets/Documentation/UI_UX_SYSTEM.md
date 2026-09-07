# UI_UX_SYSTEM.md — Stencil Relay

Status: original design direction with current 2026-09-07 implementation notes below.
Screens are scene-authored; no wholesale runtime UI construction.

Current interface: engraved studio plaque plus translucent stencil specimen;
three vertical Pattern Rack trays with four named patterns each; level/title/par
guidance on the Projection Table; front/back layer tabs; unavailable controls disabled;
numbered filter openings/targets and projector-labelled sensors. Results wait 0.65s
for the light response, then offer Inspect Projection, progression and rack navigation.
Settings show slider percentages matching handle/fill positions and actual mixer dB.
All current text uses the bundled Liberation Sans family. The font candidates and
physical-object-only navigation below are original design proposals, not claims
about the final implementation. See UI_QA.md and POLISH_OFFLINE_AUDIT.md.

## Design language
Quiet, optical, architectural, tactile, precise, minimal, physical. It is a physical
optical workspace, NOT a digital dashboard. The projection workspace dominates every
gameplay screen; controls are embedded into the light-table frame, not docked panels.

## Palette (distinct from prior projects)
- Warm ivory `#F2EBDD` — drafting surface
- Translucent neutral glass `#C9D2D0` @ low alpha — plates
- Deep blue-black `#0E1418` — background / frame shadow
- Pale cyan `#9FE8F0` — projected light
- Muted brass `#B08D57` — dials, pins, ruler
- Restrained coral `#E4785F` — warning / forbidden
- Pale sage `#A9C9A4` — valid alignment
- Subtle gray `#6E7679` — soft shadows

## Typography (max 2 typefaces, legally safe)
- Headings: a geometric/technical sans (candidate: **Michroma** or **Chivo**, SIL OFL).
- Body/labels: a highly readable neutral sans (candidate: **Inter** or **IBM Plex
  Sans**, SIL OFL). Final choice + license recorded in ASSET_PROVENANCE.md.
- Not the Misprint/Punchcard pairing.

## Screens
### Light Studio (main menu)
Illuminated design studio, slightly elevated diagonal camera across the table.
- **Begin Alignment** = physical table lamp / projector switch. Text appears subtly
  near the lit table.
- **Pattern Rack** = interact with the vertical rack of translucent stencil sheets.
- **Calibration Guide** = slim transparent reference sheet.
- **Light Settings** = physical knobs/sliders along the table edge (Optical Control Strip).
No console, no workbench, no job files, no card archive, no button stack.

### Pattern Rack (level select)
Physical stencil storage rack, three trays: A Alignment (01–04), B Occlusion (05–08),
C Projection (09–12). Each level = a translucent plate in its groove; selecting slides
it out, lights edge markings, shows name + best rating, enables Begin Pattern.
No rectangular grid, no cards, no chapter tabs.

### Projection Table (gameplay HUD)
- **Layer Selector** = thin vertical glass tabs (A/B/C[/D]) on the LEFT EDGE of the
  light table. Selected: plate lifts, tab illuminates, plate border glows, name shows.
- **Manipulation controls** embedded in the table frame: ROTATE = brass dial upper-
  right; FLIP = dual-sided toggle beneath it; RESET LAYER = small recessed button.
  Slide = direct drag on the plate.
- **PROJECT** = lamp switch at center of the lower table frame. Anchors the screen.
- **Move counter** = small engraved counter in the frame: `MOVES 04`.
- **Pause** = small physical icon near a frame edge. No full-width top HUD.

### Pause — Frosted Glass Calibration Overlay
Translucent frosted plate slides over the workspace. Options: Resume Alignment /
Restart Pattern / Calibration Guide / Return to Studio. No clipboard, no service panel.

### Settings — Optical Control Strip
Along the table edge: Music (linear light-intensity slider), Sound (second slider),
Haptics (physical click toggle), Reset Progress (protected recessed control),
Credits (engraved maker plaque). Knobs must not clone Punchcard Operator's.

### Result states
- Success: `PATTERN LOCKED` + rating (Aligned / Precise / Perfect Register). Actions:
  Next Pattern / Reconfigure / Pattern Rack.
- Failure: `MISALIGNED` + reason list. Actions: Adjust Layers / Reset Pattern.
- Final: `MASTER ALIGNMENT COMPLETE` (see GAME_DESIGN.md §Final).

## Responsive rules
Test 16:9 phone, modern iPhone landscape, wider phone, standard iPad, iPad-Pro ratio.
Light table anchored center, scales to shorter screen dimension; frame controls anchor
to table frame corners (not screen edges) so they track the table. Text min legible
size enforced. Safe-area: RectTransform driven by `Screen.safeArea`; decorative studio
art may bleed past safe area, required controls + readable text may not.

## Touch targets
Min ~88 px effective for rotate dial, flip toggle, layer tabs, PROJECT, pause, reset.

## Animation principles
Precise, restrained, short. Channels combine (movement + elevation + snap + light +
shadow + sound + optional haptic). No hypercasual bounce.

## Differentiation — how Stencil Relay differs
### From Punchcard Operator
No Control Room, Card Vault, Status Rail, Tool Dock, punch-card interaction, machine
reader, industrial console language, Punchcard ratings/icon/audio. Stencil Relay is an
optical studio with an embedded-in-table control scheme and Aligned/Precise/Perfect
Register ratings.
### From Misprint Department
No conventional top HUD, no correction tray, no document metaphor, no page-proofing
interaction, no that-project navigation or branding. Stencil Relay navigates through a
physical stencil rack and a light-table workspace.
### From generic mobile puzzle UI
No blue card grid, no giant buttons, no dashboard panels, no neon gradients. The
workspace is the UI.
