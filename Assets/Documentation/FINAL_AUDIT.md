# FINAL_AUDIT.md — Stencil Relay

> Historical 2026-09-06 snapshot. **Current audit: POLISH_OFFLINE_AUDIT.md (2026-09-07).**
> Human touch-play QA skipped by explicit user request. MCP and UnityWebRequest
> removed; 67-test suite replaces the 65-test snapshot below. Current build evidence
> is under `AuditEvidence/`, outputs under `Builds/Offline/`.

Regenerated 2026-09-06 after the final local closure pass, then re-verified after a
follow-up **environment / UI art polish pass** (owner request; art only — no gameplay,
architecture, feature or level change). Three verification states: `PASS` (real
evidence), `FAIL`, `NOT VERIFIED`. One class is never inferred from another
(§29, §54). See also `RELEASE_CANDIDATE.md`, `UI_QA.md` and `EXTERNAL_QA_HANDOFF.md`.

---

## ORIGINALITY AUDIT (§34)

**vs Punchcard Operator** — no reuse of Control Room, Card Vault, Status Rail, Tool
Dock, punch-card interaction, machine reader, industrial console language, Punchcard
ratings, icon composition, or audio identity. Stencil Relay is an optical drafting
studio; controls embedded in the light-table frame; ratings ALIGNED / PRECISE / PERFECT
REGISTER; audio glass/acrylic/optical. PASS.

**vs Misprint Department** — no reuse of its navigation, screen composition, document
metaphor, correction interaction, UI hierarchy, branding, or icon concept. Navigation
is a physical stencil rack; the workspace is a light table, not a page. PASS.

**vs generic mobile puzzle** — no card grid, no button stack, no dashboard, no
hypercasual bounce. The optical workspace dominates. Core interaction = physical layer
alignment. PASS.

Every one of the 12 main levels has a stated design reason (LEVEL_DESIGN.md); no
filler; independently authored visuals, level layouts, interactions. PASS.

## NO-PLACEHOLDER SCAN (§51)
grep of `Assets/Scripts` for TODO / FIXME / Coming Soon / Lorem Ipsum / PLACEHOLDER /
HACK → **no matches**. `SampleScene` deleted. No dummy levels, dead buttons, or
inaccessible screens. The 4 `ponytail:` comments are documented deliberate
simplifications with named ceilings.

---

## STENCIL RELAY LOCAL RELEASE CANDIDATE

```
Automated Tests:
  65 / 65 PASS   (56 EditMode + 9 PlayMode)

Level Validation:
  13 / 13 VALID   (0 errors, 0 warnings)

Console:
  PASS   (0 compile errors, 0 runtime exceptions, 0 missing references;
          accepted: Unity URP-core APV shader precision hints on d3d11 editor - §30)

Responsive UI:
  PASS   (16:9, 19.5:9, wide-phone, 4:3 - screenshots Assets/Screenshots/qa*, p4*)

Environment / UI Art Polish (post-RC, art only):
  PASS   (both scenes re-composed as a crafted optical drafting studio from the
          19-sprite ProceduralSpriteBuilder set; projector head clear of the HUD;
          no stray geometry / clipping at swept aspects; board readability unchanged.
          Re-verified 65/65 tests, 13/13 validator, console clean, both builds green.
          UI_QA.md "Environment / art polish pass".)

Safe Area:
  PASS   (SafeAreaPanel on the HUD and on every modal overlay; controls edge-anchored)

MCP Production Cleanup:
  PASS   (§33 resolved: com.anklebreaker.unity-mcp KEPT with evidence it is Editor-only
          - both asmdefs ["Editor"], no runtime asmdef references it, ABSENT from both
          build outputs' Managed DLLs. Build-inert. PACKAGE_AUDIT.md)

Offline Audit:
  PASS - NO RUNTIME NETWORK FUNCTIONALITY

Package Audit:
  PASS   (40 -> 23 dependencies; third-party runtime SDK audit PASS - none;
          com.anklebreaker.unity-mcp kept - Editor-only, build-inert)

Windows Production Build:
  PASS   (StandaloneWindows64 - StencilRelayBuilds/Win/StencilRelay.exe, ~108 MB, 0/0)

Windows Smoke Test:
  PASS   (launches; Bootstrap -> LightStudio; Player.log has 0 exceptions/errors)

iOS Unity Export:
  PASS   (Unity-iPhone.xcodeproj at StencilRelayBuilds/iOS/ - 0 errors, 0 release-
          blocking warnings; Product Name / bundle id / landscape / scenes / icon /
          permissions all confirmed in the generated project settings)

Human 13/13 Touch QA:
  NOT VERIFIED - HUMAN REQUIRED
  (13/13 DATA VALID + 13/13 CONTROLLER-PATH PLAYTHROUGH pass; a person must play the
   shipped build - EXTERNAL_QA_HANDOFF.md section A)

iOS Archive / Signing:
  NOT VERIFIED - MACOS / XCODE / CREDENTIALS REQUIRED

Physical iPhone Installation:
  NOT VERIFIED - DEVICE REQUIRED

Level 11 Device Performance:
  NOT VERIFIED - DEVICE REQUIRED

Level 12 Device Performance:
  NOT VERIFIED - DEVICE REQUIRED

Known Locally Fixable Release Blockers:
  NONE

External Blockers:
  - iOS archive / code-signing / physical-device install: macOS + Xcode + Apple
    Developer credentials, unavailable here (§45). Unity-side export complete & clean.
  - Human hands-on manual QA and on-device performance profiling: require a person and
    a device.

Latest Git Commit:
  N/A - not a git repository (git initialization was not authorized). Frozen state =
  working tree + SUMMARY_BANK.md MCP change log MCP-0001 .. MCP-0013.

PROJECT STATUS:
  BLOCKED_EXTERNAL
```

---

## Feature audit detail

| Area | Status |
|---|---|
| Core stencil system / slide / snap / rotation / flip / layer composition | PASS (model + 13/13 controller-path) |
| Projection system / required targets / forbidden sensors / dual projection / filters / parallax | PASS |
| Level 0 Calibration (tutorial steps + persistence) | PASS |
| 12 main levels | PASS (data + sim + controller-path) · human manual NOT VERIFIED |
| Final Level 12 / MASTER ALIGNMENT COMPLETE | PASS (plays once, persists, no replay) |
| Light Studio / Pattern Rack / Projection Table / layer selector / manipulation controls / PROJECT / pause / settings / guide / result states | PASS (scene-authored, wired, screenshot-verified) |
| Environment / materials / lighting | PASS (optical studio; recessed board well; ≤2 lights) |
| Projection VFX / animations | PASS (light-mask reveal, beam, disc-pop/ring-pulse; plate tweens; camera dip) |
| Audio (19 clips) / mixer (Master/Music/SFX/UI) / settings drive dB / provenance | PASS |
| Haptics | PARTIAL (Handheld.Vibrate guarded by setting; device-only, coarse) |
| Save / progression / ratings / reset | PASS (versioned JSON; corrupt/newer/reset handled; tested) |
| Game icon / product settings / iOS settings / permissions | PASS |
| Performance | PARTIAL (60 fps target, ≤2 lights, no per-frame rebuilds; device profiling NOT DONE) |
| Runtime-UI audit (§19/§86) | PASS (all menus/HUD/panels build-time authored) |
| Documentation / Summary Bank | PASS (14 docs current; resumable) |
