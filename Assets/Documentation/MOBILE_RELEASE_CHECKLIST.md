# MOBILE_RELEASE_CHECKLIST.md — Stencil Relay

> CURRENT 2026-09-07: IL2CPP/ARM64 already configured. Modal safe areas are authored;
> phone/tablet editor visual checks completed. Android force-Internet permission is
> false. Human touch-play QA skipped by user request. Latest local outputs are under
> `Builds/Offline/`; current verification is POLISH_OFFLINE_AUDIT.md. Device signing
> and profiling remain unverified. Unchecked local-export/modal items below are stale.

Applied by `MobileSettingsBuilder.cs` (menu **Stencil Relay / Apply Mobile Settings**)
and `IconBuilder.cs` (menu **Rebuild Icon**). Re-run after Unity upgrades.

## Player Settings — DONE
- [x] Product Name = `Stencil Relay`
- [x] Company = `Rehman Mughal`
- [x] Bundle identifier (iOS + Android) = `com.rehmanmughal.stencilrelay`
- [x] Version = `1.0.0` · iOS build = `1` · Android versionCode = `1`
- [x] Orientation = Landscape Left + Right only (portrait auto-rotate off)
- [x] iOS scripting backend = IL2CPP
- [x] iOS architecture = ARM64
- [x] iOS managed stripping = Medium
- [x] iOS graphics = default (Metal)
- [x] iOS minimum version = 13.0
- [x] iOS `requiresFullScreen` = true, automatic signing = off
- [x] Android = IL2CPP, ARM64, minSdk 25
- [x] App icon assigned (Default / iOS / Android) — `Assets/Content/AppIcon.png` 1024²,
      opaque, original (three overlapping stencil layers → bright opening)
- [x] `Application.targetFrameRate = 60`, vSync off (GameServices.Awake)
- [x] Build scenes: Bootstrap, LightStudio, ProjectionTable (SceneBuilder sets EditorBuildSettings)

## Permissions — DONE (none requested)
- [x] No camera / microphone / photo / location / contacts / Bluetooth usage strings
- [x] No ATT / tracking (no ads/analytics SDK — see OFFLINE_AUDIT.md)

## Safe area — DONE
- [x] `SafeAreaPanel` on the Projection Table HUD container (insets to `Screen.safeArea`)
- [x] Required controls edge-anchored inside the safe area (layer tabs / rotate cluster /
      PROJECT / moves / pause) — verified by screenshot (m8_iphone_table.png)
- [ ] Modal panels (pause/result/finale) inset to safe area — buttons are centred and
      safe; panel-edge decoration polish deferred

## Performance (§42) — PARTIAL
- [x] `targetFrameRate` 60; realtime lights ≤ 2 (key + projector pool)
- [x] Plate/light meshes rebuilt only on change, not per frame
- [ ] Level 11 / Level 12 profiled on device — NOT DONE (no device)
- [ ] Transparent overdraw budget on full-sheet levels measured — NOT DONE

## Build — see FINAL_AUDIT / M10
- [ ] Xcode project export from Unity — needs iOS Build Support module (M10)
- [ ] Build smoke test / physical device test — EXTERNAL BLOCKER (macOS + signing)

## Responsive UI QA
See UI_QA.md — first pass done at 16:9; per-aspect sweep incomplete (tooling).
