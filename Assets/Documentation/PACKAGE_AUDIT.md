# PACKAGE_AUDIT.md — Stencil Relay

> CURRENT 2026-09-07: **19 direct dependencies**, all Unity packages/modules.
> Removed `com.anklebreaker.unity-mcp`, `com.unity.modules.unitywebrequest`,
> `com.unity.ide.rider`, and `com.unity.ide.visualstudio`. All four are absent from
> the resolved lockfile. The former claim that UnityWebRequest was required by IDE/URP
> was incorrect: the actual dependency graph resolved successfully without it.
> Post-removal tests: 56 EditMode + 11 PlayMode PASS. See POLISH_OFFLINE_AUDIT.md.
> The package-retention instructions and 23-dependency table below are historical.

Trimmed 2026-09-06 (M10). Was 40 dependency entries → **23** (23 incl. the Editor-only
MCP dev package — see below). After each change: resolved, compiled (0 errors), console
clean, **65/65 tests green**, all 3 scenes load, Windows + iOS builds succeed.

## Third-party runtime SDK audit — PASS
No ads / analytics / attribution / social / crash-reporting / remote-config SDK in the
manifest or `Assets/Plugins` (there is no `Assets/Plugins`). Only Unity-provided
packages plus the Editor-only MCP dev tool. See OFFLINE_AUDIT.md.

## Kept (Packages/manifest.json)

| Package | Why |
|---|---|
| com.unity.render-pipelines.universal 17.3.0 | URP + URP 2D renderer + Light2D (core visuals) |
| com.unity.inputsystem 1.19.0 | touch / pointer input (PlateInput, EventSystem) |
| com.unity.ugui 2.0.0 | uGUI + TextMeshPro (all scene-authored UI) |
| com.unity.2d.sprite 1.0.0 | Sprite import/edit (procedural sprite assets) |
| com.unity.test-framework 1.6.0 | EditMode + PlayMode tests (editor-only) |
| com.unity.ide.rider / ide.visualstudio | IDE integration (editor-only, harmless) |
| com.unity.modules.{audio, ui, uielements, imgui, jsonserialize, animation, director, particlesystem, physics2d, imageconversion, screencapture, androidjni, accessibility, vectorgraphics, unitywebrequest} | engine modules pulled in by the above or used at runtime (audio/UI/JsonUtility/Light2D shadow-anim/2D physics-free but module present/androidjni for haptics). `unitywebrequest` is a transitive dep of the IDE/URP packages — **no project code uses it** (OFFLINE_AUDIT.md). |

## Removed
com.unity.multiplayer.center · com.unity.visualscripting · com.unity.collab-proxy ·
com.unity.2d.{animation, aseprite, psdimporter, spriteshape, tilemap, tilemap.extras, tooling} ·
com.unity.timeline · com.unity.modules.{ai, adaptiveperformance, assetbundle, cloth,
physics(3D), terrain, terrainphysics, tilemap, umbra, unityanalytics,
unitywebrequestassetbundle, unitywebrequestaudio, unitywebrequesttexture,
unitywebrequestwww, vehicles, video, vr, wind, xr}

Rationale: offline single-player 2D puzzle — no multiplayer, no visual scripting, no
Plastic SCM, no 3D physics / terrain / XR / video, custom cell grid (not Tilemap),
no SpriteShape/Aseprite/PSB source art, coroutine sequences (not Timeline), and
**`unityanalytics` removed for offline clarity** (it was inert but its presence is noise).

## MCP production cleanup (§33) — RESOLVED: KEPT, with evidence it is Editor-only

`com.anklebreaker.unity-mcp` is **retained in `Packages/manifest.json`** (per the
project owner, and per §33's clause: "If it is Editor-only and does not affect runtime
output, document the evidence rather than deleting required development infrastructure").
Concrete evidence it contributes **nothing to the player build**:
- Both of its asmdefs (`AnkleBreaker.UnityMCP.Editor`, `UnityMCP.Editor.Welcome`) are
  `"includePlatforms": ["Editor"]` and live under `.../Editor/`.
- No runtime asmdef references it — `StencilRelay.Runtime` references only Unity
  runtime; `StencilRelay.Game` references URP / URP-2D / InputSystem / TMP.
- **Absent from the shipped Managed DLLs** of both the iOS export
  (`StencilRelayBuilds/iOS/Data/Managed/`) and the Windows build
  (`StencilRelayBuilds/Win/StencilRelay_Data/Managed/`) — verified by directory listing.
- Its only runtime effect is a local **editor** HTTP server on port 7890 — a dev tool
  active only while the Unity Editor is open; it is not compiled into any build.

Optional final step for a store submission: removing the one manifest line and
re-resolving produces an identical build (it was verified once during this project that
removal recompiles clean with 0 errors). Not required for correctness — the package is
provably build-inert.

## Accepted engine warnings (§30)
The URP core `TraceVirtualOffset` shader emits `-Wconversion` precision hints
(`min16float`) in `com.unity.render-pipelines.core/ShaderLibrary/Sampling/*.hlsl` during
**d3d11 editor** shader-variant compilation. This is Unity package code for the Adaptive
Probe Volume feature the game does not use (URP `m_LightProbeSystem` = Light Probe
Groups). Not in the shipped iOS/Metal shaders, not our code, not release-blocking;
appears only when the variants recompile and is cached after.
