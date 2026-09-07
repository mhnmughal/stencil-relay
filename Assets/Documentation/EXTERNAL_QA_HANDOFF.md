# EXTERNAL_QA_HANDOFF.md — Stencil Relay

> Superseded 2026-09-07: section A is optional, explicitly skipped by the owner.
> MCP has been removed. Latest exports: `Builds/Offline/iOS/`. See
> POLISH_OFFLINE_AUDIT.md for current verification. Signing/device profiling remain
> unverified; the older package-retention recommendation below no longer applies.

The local development phase is complete (see `RELEASE_CANDIDATE.md` / `FINAL_AUDIT.md`).
Everything below needs a person, a Mac, or a device — the model cannot and must not
mark these PASS. Record results back into `LEVEL_QA.md` and this file.

---

## A. HUMAN 13 / 13 TOUCH-PLAY QA   — `NOT VERIFIED — HUMAN REQUIRED`

Play the shipped build (Windows now, iOS after step B). For **each** stage
00 01 02 03 04 05 06 07 08 09 10 11 12 verify:

- [ ] loads to the Projection Table with the right board / plates / targets
- [ ] tap selects a plate (tab or the plate itself); selected plate lifts + tab lights
- [ ] drag slides the plate; release snaps to a grid position predictably
- [ ] ROTATE steps 90° and feels controlled (levels 3, 4, 8, 12)
- [ ] FLIP reveals the other face clearly (levels 7, 8, 12)
- [ ] PROJECT locks input, the room dims, the pattern + targets respond, result shows
- [ ] a wrong solution shows an understandable reason (e.g. "Target Unlit (A)",
      "Forbidden Sensor Lit", "Filter Mismatch", "Target Unlit (T, P2)")
- [ ] the intended solution reaches PATTERN LOCKED with a rating
- [ ] Adjust Layers / Reconfigure returns to manipulation with state intact
- [ ] Pause → Resume / Restart / Calibration Guide / Return to Studio all work
- [ ] rating shown (Aligned / Precise / Perfect Register); replaying never lowers it
- [ ] completing the level unlocks the next in the Pattern Rack
- [ ] Level 0 only: the tutorial guidance advances (select → slide → rotate → project →
      CALIBRATION COMPLETE) and, after completing once, is replayable from the Guide
- [ ] Level 12 only: completion triggers MASTER ALIGNMENT COMPLETE (rack illumination,
      achievement mark, Review Patterns / Return to Studio), and a later launch
      acknowledges completion without replaying the finale

Subjective (write observations, not pass/fail):
readability · responsiveness · accidental taps · confusing interactions · animation
delay · audio annoyance · visual clutter.

---

## B. iOS DEVICE HANDOFF   — `NOT VERIFIED — MACOS / XCODE / CREDENTIALS REQUIRED`

The Unity-side export is done and clean:
`I:/UnityProject/StencilRelayBuilds/iOS/Unity-iPhone.xcodeproj` (0 errors, 0
release-blocking warnings). On a Mac:

1. Copy the `StencilRelayBuilds/iOS/` folder to the Mac; open `Unity-iPhone.xcodeproj`
   in Xcode (matching the Unity 6000.3.19f1 iOS support version).
2. Target **Unity-iPhone** → Signing & Capabilities → set your Team; automatic signing.
3. Confirm: Bundle Identifier `com.rehmanmughal.stencilrelay`; Display Name
   `Stencil Relay`; Deployment Target iOS 13.0; Device Orientation = Landscape Left +
   Landscape Right only; no added capabilities; Info.plist has **no** camera / mic /
   photo / location / contacts / Bluetooth / tracking usage keys.
4. Product → Archive (Release). Resolve any signing-only issues.
5. Install on a physical iPhone and an iPad (landscape).
6. Launch from a fresh install (delete any prior install first — exercises first-launch
   save path).
7. Play a representative set of levels including 0, 6 (forbidden), 7 (flip), 11 (dual),
   12 (finale).
8. Background the app mid-manipulation and mid-projection; resume — no stuck input lock,
   state intact.
9. Toggle Music / Sound sliders and Haptics in Light Settings — confirm they take effect
   and persist across a relaunch. Reset Pattern History → confirm dialog → progress clears.
10. Safe areas: notch / Dynamic Island / rounded corners / home indicator — all required
    controls and text clear of them; decorative studio art may bleed.
11. Orientation: rotating the device keeps landscape; portrait is not offered.
12. No permission prompts appear at any point.
13. Airplane mode ON for a full session — the game plays identically (offline).
14. Level 11 dual projection reads (both boards legible, failure names the projector).
15. Level 12 Master Plate solvable on device; MASTER ALIGNMENT COMPLETE plays.
16. Watch for frame hitches / thermals over ~15 min of play.

---

## C. DEVICE PERFORMANCE PROFILING   — `NOT VERIFIED — PHYSICAL DEVICE REQUIRED`

Use Xcode Instruments / Unity Profiler (device build).

### Level 11 — Dual Projection
- [ ] FPS / frame time (target 60, acceptable ≥ 55 sustained)
- [ ] GPU frame time; transparent overdraw over the board
- [ ] the projection effect cost when both projectors evaluate
- [ ] managed allocations per frame during manipulation and on PROJECT (target ~0)
- [ ] thermal state after 5 min

### Level 12 — Master Plate  (+ the MASTER ALIGNMENT COMPLETE finale)
- [ ] repeat the above
- [ ] the finale's rack-illumination coroutine + text reveal — no hitch

If overdraw is high on full-sheet-plate levels, lower the plate tint alpha further or
switch the plate material to a cheaper unlit path (all plate rendering is in
`PlateView` + `Assets/Content/Materials/M_Cell.mat`).

---

## D. BEFORE THE ABSOLUTE FINAL STORE BUILD
- The `com.anklebreaker.unity-mcp` dev package is **kept** in `Packages/manifest.json`
  (Editor-only, build-inert — evidence in `PACKAGE_AUDIT.md`). It does not ship. If you
  prefer a manifest with zero non-Unity packages for submission, delete that one line
  and re-resolve — the build is identical (verified once: recompiles clean, 0 errors).
- Re-run `Stencil Relay/Apply Mobile Settings` and `Rebuild Icon` if Unity is upgraded.
