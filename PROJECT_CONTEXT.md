# PROJECT_CONTEXT.md — Stencil Relay

> STATUS 2026-09-07: local UI/game-design polish and offline hardening COMPLETE.
> Human touch-play QA SKIPPED BY USER REQUEST. 13/13 levels valid; 67/67 tests pass
> after MCP removal; Windows + iOS export both pass (0 errors / 0 warnings).
> Mobile release remains BLOCKED_EXTERNAL for signing/install and device profiling.
> Evidence: `AuditEvidence/`; fresh outputs: `Builds/Offline/`.
> Current report: `Assets/Documentation/POLISH_OFFLINE_AUDIT.md`.


## Game
**Stencil Relay** — 2.5D offline optical layer-alignment puzzle game.

## Core mechanic
The player manipulates translucent stencil plates (slide / 90° rotate / flip) to
compose a combined mask, then activates PROJECT. Light passes through the combined
open regions; REQUIRED TARGETS must be lit, FORBIDDEN SENSORS must stay dark. The
result is deterministic — the same stencil configuration always evaluates the same.

## Presentation
2.5D optical drafting laboratory ("Light Studio"). Fixed angled top-down / drafting-
table camera. No free camera rotation.

## Platform
iOS primary, Android-compatible. Landscape only. Completely offline.

## Unity
- Editor: **6000.3.19f1**
- Render pipeline: **URP 2D** (`Renderer2D`, 2D lights, Lit2D). Installed and kept.
- Input: **Input System 1.19.0** (new).
- Scripting backend: Windows Mono; iOS/Android **IL2CPP + ARM64** configured.
- MCP: removed after authoring, by owner request to remove network-related packages.

## KEY INTERPRETATION DECISIONS (authoritative)
1. **Rendering = 2D URP.** "2.5D" is achieved through layered sprites with subtle
   depth offset + parallax + URP 2D lights + soft drop-shadow sprites, NOT a 3D
   scene. This matches the mobile-performance mandate ("prefer deterministic
   optimized masking/material logic over physically correct expensive optics") and
   the pre-installed 2D toolchain. Environment art = authored sprite prefabs.
2. **Projection evaluation = deterministic grid mask**, computed in C# on an integer
   cell grid. Visuals are driven from the same mask so rendering never affects logic.
3. **No Git.** The project is not a git repo and Git initialization has not been
   authorized. Milestones are recorded in `SUMMARY_BANK.md` instead of commits.
4. **iOS device build/signing is an external blocker** (needs macOS + Xcode + Apple
   signing). Xcode-project *export* is attempted if iOS Build Support is installed;
   otherwise recorded `NOT VERIFIED` with evidence. All other work proceeds.
5. **Bundle id** `com.rehmanmughal.stencilrelay`, Product Name `Stencil Relay`,
   version `1.0.0`, build `1` (from master prompt §77; no pre-existing intentional id).

## Scenes (target architecture)
- `Bootstrap` — init, save load, settings, audio, routes to first scene.
- `LightStudio` — main menu + Pattern Rack level select, scene-authored studio.
- `ProjectionTable` — the gameplay scene, loads a `StencilLevelDefinition`.

## Content scope
13 stages: Level 0 (Calibration tutorial) + Levels 1–12.
- Set A — Alignment (1–4)
- Set B — Occlusion (5–8)
- Set C — Projection (9–12)
No filler. Every level has a stated design reason (`LEVEL_DESIGN.md`).

## Major systems (see ARCHITECTURE.md for detail)
StencilPlate · StencilLayerStack · plate movement/rotation/flip · ProjectionEvaluator
· target & forbidden-sensor evaluation · StencilLevelDefinition (ScriptableObject)
· LevelValidator (Editor) · SaveSystem (versioned local JSON) · progression ·
scene flow · AudioController + mixer · haptics · UI (Light Studio / Pattern Rack /
Projection Table HUD) · animation/VFX.

## Ratings (unique terminology — do NOT use stars / "verified" / "proof")
- **ALIGNED** — completed.
- **PRECISE** — completed within move par.
- **PERFECT REGISTER** — optimal configuration.

## Originality guardrails
Not a programming / command-sequencing / punch-card / printer / industrial-console /
postal / generic-hypercasual game. Must not structurally resemble Misprint Department
or Punchcard Operator (no Status Rail, Tool Dock, Control Room, Card Vault, top HUD
bar, rectangular level-card grid, machine-console controls). The optical workspace
dominates the screen; controls are embedded in the table frame.
