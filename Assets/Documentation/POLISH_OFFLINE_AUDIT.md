# UI, game design and offline audit

Date: 2026-09-07. This report supersedes the 2026-09-06 release snapshot.
Human touch-play QA is **SKIPPED BY USER REQUEST**, not PASS. Device signing and
performance measurements remain separate from local verification.

## Changes and defects resolved

- Successful PROJECT saves completion immediately, including unlocks and ratings.
  Leaving via Pattern Rack no longer loses a successful attempt. Pattern Rack opens
  directly on arrival. The final completion presentation is preserved.
- Selected lower layers remain draggable. Live drag applies only the displacement
  from the committed position, fixing the double-offset jump.
- Pointer raycasts respect UI overlays; pause cancels drag and blocks projection.
  Repeated PROJECT cannot duplicate evaluation, effects, or saving.
- Tutorial prompts follow the required transform state rather than arbitrary move
  count. Slide-only layers disable ROTATE. Front/back state, level identity, move par,
  and concise mechanic guidance are visible.
- Projection is visible for 0.65 seconds before the result. Inspect Projection exposes
  the board, with an Adjust Layers action replacing PROJECT while input is locked.
- Matching filter numbers appear on openings and targets. Dual-projector sensor
  labels identify their projector; feedback matches both sensor and projector IDs.
- Seven-cell boards fit the same drafting well as six-cell boards. The grid is
  generated from the actual level dimensions, once per load; rendering, pointer
  mapping, masks and sensor coordinates use the same cell size.
- Pattern Rack has three vertical trays with number, title and lock/rating state.
  The studio contains an authored stencil composition and clearer primary action.
- Audio sliders have larger input areas, contained handles, numeric percentages,
  and consistent fill/handle geometry. Haptics label overlap is fixed. Reset applies
  the reset audio values to the actual mixer as well as the UI and save.
- Reduced selected-plate opacity; cached projection material property block;
  generated plate/light/grid meshes are destroyed with their views.

## Package and offline audit

Removed direct dependencies:

- `com.unity.modules.unitywebrequest`
- `com.unity.ide.rider`
- `com.unity.ide.visualstudio`
- `com.anklebreaker.unity-mcp` (removed after scene authoring and visual checks)

The remaining manifest contains only Unity graphics, input, audio, UI, test and
engine modules. UnityConnect, analytics/ads startup, engine diagnostics, cloud crash
reporting and purchasing are disabled. Android force-Internet permission is false.

Runtime C# scan: no HTTP, sockets, UnityWebRequest, URL launching, remote loading,
analytics, telemetry, ads, cloud or authentication code. No bundled native or
managed third-party plugins under Assets. Levels, audio and sprites load from local
Resources; saves use a local JSON file. Unity's package registry URLs and disabled
service endpoint strings are editor configuration, not active game requests.

Final lockfile, build assembly and runtime endpoint evidence: `AuditEvidence/`.
Final post-removal tests/builds: **PASS** (2026-09-07).

The first Windows Mono build copied unused Unity networking/analytics modules.
`ReleaseVerification` now applies Medium managed stripping, reducing output from
107,871,622 to 90,648,622 bytes. Three built-in wrappers remain in Windows:
UnityEngine.UnityWebRequestModule, UnityEngine.UnityAnalyticsModule, and
UnityEngine.UnityAnalyticsCommonModule. Metadata inspection found only UnityEngine's
type-forwarding facade and the wrappers reference these; no game or runtime library
references them. They are not installed networking/analytics packages and are not
manually deleted from the engine. The repeatable audit checks that distinction.

Windows startup smoke: PASS, process remained alive through five endpoint samples;
all five recorded zero TCP and UDP endpoints, and Player.log contains no error or
exception. Endpoint enumeration was separately confirmed readable. **A forced-offline
firewall test was NOT VERIFIED**: Windows denied creation of the temporary per-game
rule. No rule remains. The endpoint samples do not prove absence of every transient
connection or substitute for an on-device airplane-mode session.

## Verification

- After MCP removal: **56/56 EditMode + 11/11 PlayMode = 67/67 PASS**, zero skipped
  automated tests (`AuditEvidence/editmode.xml`, `playmode.xml`).
- **13/13 levels VALID** (`AuditEvidence/level-validation.txt`).
- Windows production build: PASS, 0 errors / 0 warnings, 90,648,622 bytes.
- iOS Xcode export: PASS, 0 errors / 0 warnings, 917,209,988 bytes. Generated Info.plist
  contains no sensitive usage-description keys; export is landscape. Signing and
  physical installation are not implied by export success.
- Built in an isolated saved-project snapshot because the interactive editor reopened.
  Hash comparison confirms saved scripts, scenes, content, graphics assets and packages
  match that snapshot. The only PlayerSettings difference is build-time Standalone
  stripping, enforced by ReleaseVerification. See `snapshot-comparison.txt`.
- The player save still matches the pre-test backup (`player-save-preserved.txt`).
- New regressions exercise the real authored buttons, result-to-rack navigation,
  pause raycast blocking, immediate save/unlock, lower-layer picking, drag displacement,
  projection spam and slider/save/mixer agreement.
- `AuditEvidence/audio-ui.txt`: 0 / 25 / 50 / 100 percent map to normalized positions
  0 / .25 / .5 / 1, saved values matching, and -80 / -12.0412 / -6.0206 / 0 dB.
- `AuditEvidence/input-ui.txt`: modal blocks board after canvas layout; pause state
  is asserted independently. An immediate pre-layout raycast is recorded separately.
- Actual Game View screenshots: `Assets/Screenshots/offline_*`. Inspected 16:9 studio,
  rack, settings and filters; 4:3 studio/settings/table/pause; 19.5:9 final level/result;
  wide-phone projection inspection. These are automated editor visual checks.
- Human touch-play QA: SKIPPED BY USER REQUEST. No human or physical-device PASS
  is inferred from the controller tests or screenshots.
- iOS archive/sign/install and physical-device profiling: NOT VERIFIED.

## Reproduction

Unity 6000.3.19f1. Run the EditMode and PlayMode suites using Unity Test Runner.
`StencilRelay.Editor.ReleaseVerification.BuildWindows` and `.BuildIos` are batch-mode
entry points. Outputs go to `Builds/Offline/` with summaries in `AuditEvidence/`.
MCP is intentionally absent; ordinary Unity menus and batch-mode commands remain usable.

## Final status

Requested local polish, package removal and offline audit: **COMPLETE**.
Human touch-play QA: **SKIPPED BY USER REQUEST**.
Full mobile release status: **BLOCKED_EXTERNAL** for iOS signing/install and device
profiling. No store readiness or physical-device/offline-session PASS is claimed.
