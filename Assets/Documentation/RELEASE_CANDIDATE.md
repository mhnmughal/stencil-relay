# RELEASE_CANDIDATE.md — Stencil Relay

> Superseded by the authorized 2026-09-07 polish/offline pass. The freeze and MCP
> retention instructions below are historical. See POLISH_OFFLINE_AUDIT.md.
> Human touch-play QA is skipped by owner request; current builds: `Builds/Offline/`.

Local development phase **frozen** 2026-09-06 after the final closure pass, then a
follow-up **environment / UI art polish pass** (owner request — no gameplay,
architecture, feature or level change; see UI_QA.md "Environment / art polish pass").
Do not change verified gameplay after this point unless external QA finds a defect.

**Polish pass 2026-09-06:** both scenes re-composed as a crafted "optical drafting
studio" from `ProceduralSpriteBuilder`'s 19-sprite set (bevel panels, brushed metal,
glass, brass dial/pins/brackets, lens, lamp globe, projector beam, vignette). MCP
`com.anklebreaker.unity-mcp` package **kept** (owner's call). Re-verified: 65/65
tests, 13/13 validator, console clean, Windows + iOS builds green, Windows smoke
test 0 exceptions.

## Build environment
- Unity **6000.3.19f1** (iOS Build Support installed)
- URP 17.3.0, URP 2D renderer, Linear colour space
- Scripting backend: iOS = IL2CPP / ARM64 / Metal ; Android = IL2CPP / ARM64

## Verification snapshot

| Check | Result |
|---|---|
| Automated tests | **65 / 65 PASS** — 56 EditMode + 9 PlayMode |
| Level validation | **13 / 13 VALID** (0 errors, 0 warnings) |
| Controller-path playthrough | **13 / 13 PASS** (every level, real slide/rotate/flip/PROJECT/reset) |
| Console (editor + play mode) | 0 compile errors, 0 runtime exceptions, 0 missing references |
| Build-time warnings | Windows 0 ; iOS 0 errors, only Unity URP-core APV shader precision hints (§30, accepted) |
| Windows Standalone build | **SUCCEEDED** — `StencilRelayBuilds/Win/StencilRelay.exe` (~108 MB) |
| Windows smoke test | **PASS** — launches, Bootstrap→LightStudio, Player.log 0 exceptions |
| iOS Unity Xcode export | **SUCCEEDED** — `StencilRelayBuilds/iOS/Unity-iPhone.xcodeproj` (~1 GB, IL2CPP C++ incl.) |
| Offline audit | **PASS — NO RUNTIME NETWORK FUNCTIONALITY** |
| Third-party runtime SDK audit | **PASS** (none) |
| Package audit | 40 → **23** deps; MCP dev package kept (Editor-only, build-inert, evidenced) |
| Originality audit | **PASS** vs Punchcard Operator / Misprint Department / generic mobile puzzle |
| No-placeholder scan | **PASS** (no TODO/placeholder/dummy in shipping code; SampleScene deleted) |
| Responsive UI | **PASS** at 16:9 / 19.5:9 / wide-phone / 4:3 (screenshots); SafeAreaPanel + edge anchors |
| Safe area | **PASS** (all modals rebuilt as safe-area-inset overlays; controls anchored inside) |
| Icon | **PASS** — original 1024² opaque, uncompressed, assigned Default/iOS/Android |
| Product / iOS settings | **PASS** (name, bundle id, version, landscape-only, 60 fps, no permissions) |

## Remaining checks (all external — see EXTERNAL_QA_HANDOFF.md)
- Human 13/13 touch-play QA — `NOT VERIFIED — HUMAN REQUIRED`
- iOS archive / code-signing / device install — `NOT VERIFIED — MACOS/XCODE/CREDENTIALS REQUIRED`
- Physical iPhone/iPad run — `NOT VERIFIED — DEVICE REQUIRED`
- Level 11 / Level 12 device performance profiling — `NOT VERIFIED — DEVICE REQUIRED`

## Artifacts
- `I:/UnityProject/StencilRelayBuilds/Win/` — Windows player
- `I:/UnityProject/StencilRelayBuilds/iOS/` — Xcode project for the Mac stage
- Screenshots: `Assets/Screenshots/` (m5_*, m6_*, m8_*, qa*_*, p4_* = post-RC art polish)

## Version control
Not a git repository (git initialization was not authorized in the project
instructions). No commits, no remote. The frozen state is this working tree +
`SUMMARY_BANK.md` MCP change log entries MCP-0001 … MCP-0013.

## Reproducible generators (menu: Stencil Relay/)
Rebuild All Levels · Rebuild Materials · Rebuild Sprites · Rebuild Audio · Rebuild Icon
· Rebuild Scenes · Apply Mobile Settings · Validate Levels

## PROJECT STATUS
`BLOCKED_EXTERNAL`
