# AI_RULES.md — Stencil Relay

Permanent rules for every AI/dev session. Read this file first, every session.

## RULE 1 — READ MEMORY FIRST
Before touching the project, read in order:
1. `AI_RULES.md`
2. `PROJECT_CONTEXT.md`
3. `SUMMARY_BANK.md`
4. `CLAUDE_MASTER_PROMPT.md`
5. Relevant files in `Assets/Documentation/`

Never modify the project before reading memory.

## RULE 2 — REPOSITORY IS AUTHORITATIVE MEMORY
Do not rely on chat history. All meaningful decisions live in these files.

## RULE 3 — MCP-FIRST UNITY MODIFICATION
Use the Unity MCP server (AnkleBreaker unity-mcp, port 7890) for all Unity-supported
state: scenes, hierarchy, GameObjects, prefabs, components, materials, cameras,
lights, UI hierarchy, RectTransforms, Inspector values, Player Settings, validation,
visual inspection. Never call the Unity HTTP bridge directly. C# scripts are authored
as text files then imported/compiled through Unity; scene/prefab wiring goes through MCP.

## RULE 4 — LOG MCP WORK
Every meaningful MCP change batch gets an `MCP-XXXX` entry in `SUMMARY_BANK.md`.

## RULE 5 — NEVER CLAIM UNVERIFIED SUCCESS
Use only `PASS` / `FAIL` / `NOT VERIFIED`. PASS requires real evidence from the
relevant verification class. Never infer one verification class from another.

## RULE 6 — NO FINAL PLACEHOLDERS
No shipping build may contain TODO / Temp / Test / Sample / Coming Soon / Lorem Ipsum
/ placeholder art / placeholder icon / fake settings / dead buttons / inaccessible screens.

## RULE 7 — SCENE-AUTHORED PRIMARY UI
Primary production UI exists as scene/prefab hierarchy. No wholesale runtime
`new GameObject()` / `AddComponent<Button/Image/TextMeshProUGUI>()` construction of
the main interface. Scripts manage state and data, not UI construction.

## RULE 8 — SCENE-AUTHORED ENVIRONMENT
Major environment content visible in Edit Mode via scenes/prefabs.

## RULE 9 — COMPLETELY OFFLINE
Never add network requests, accounts, login, cloud, ads, analytics, telemetry,
tracking, remote config, multiplayer, web views, DLC, remote APIs.

## RULE 10 — NO THIRD-PARTY RUNTIME SDKs
No Firebase / AdMob / Unity Ads / Facebook / AppsFlyer / GameAnalytics / attribution
/ analytics / social SDKs.

## RULE 11 — ORIGINAL PRODUCT
Do not copy any other game's UI, layout, art, icon, music, sound, code, level
layouts, branding, or navigation.

## RULE 12 — DO NOT COPY PREVIOUS PROJECT UI
Must not reuse the design system, navigation, or identity of Misprint Department or
Punchcard Operator. See `Assets/Documentation/UI_UX_SYSTEM.md` differentiation section.

## RULE 13 — SETTINGS MUST FUNCTION
Music, SFX, haptics, reset-data must actually work and persist.

## RULE 14 — NO DEAD CONTROLS
Every production interaction works.

## RULE 15 — TEST AFTER IMPLEMENTATION
Implementation without verification is incomplete. Compile → test → visually inspect.

## RULE 16 — MOBILE FIRST
Landscape, safe areas, touch targets, readability, performance, tablet scaling.

## RULE 17 — ASSET LEGALITY
Only project-generated original assets, properly licensed assets, or verified
open-source assets. Document provenance in `AUDIO_PROVENANCE.md` / `ASSET_PROVENANCE.md`.

## RULE 18 — UPDATE MEMORY
After major work update `SUMMARY_BANK.md`, `PROJECT_CONTEXT.md` (if needed), and the
relevant documentation.

## RULE 19 — DO NOT STOP EARLY
Continue through integration, polish, QA, cleanup, build verification, and final audit
until completion gates pass or a genuine external blocker (signing cert, Apple
credentials, macOS-only operation) prevents further work. Bugs, bad UI, missing
levels, failing tests are NOT external blockers — fix them.

## RULE 20 — AUTONOMOUS FIX LOOP
For every issue: IDENTIFY → ROOT CAUSE → FIX → COMPILE → TEST → VISUAL INSPECT →
REGRESSION CHECK → DOCUMENT → CONTINUE. Do not merely report locally fixable problems.

## RULE 21 — UNITY GOTCHAS (bitten by these, do not repeat)
- Never `GetComponent<T>() ?? AddComponent<T>()` or `?? fallback` on a Unity object —
  the `??` operator does NOT see Unity's fake-null. Use `var c = GetComponent<T>();
  if (c == null) c = AddComponent<T>();`.
- Each `ScriptableObject` / `MonoBehaviour` class must live in a file of the same name.
- `Light2D` is in assembly `Unity.RenderPipelines.Universal.2D.Runtime` (separate from
  the main URP runtime asmdef).
- `SerializedObject.FindProperty(...).objectReferenceValue = <on-disk asset>` can
  silently no-op after repeated recompiles (see BUG-002); load shared data from
  `Resources/` instead of wiring it per-scene.
- `GraphicsSettings.defaultRenderPipeline` must point at the URP asset (not just the
  quality tier) or the camera clear / URP features misbehave.
- `unity_screenshot_game` returns stale frames while the editor is unfocused; call
  `UnityEditorInternal.InternalEditorUtility.RepaintAllViews()` first, expect flakiness.
