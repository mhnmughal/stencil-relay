# AUDIO_PROVENANCE.md — Stencil Relay

Every audio asset in the project, with provenance. **All clips are project-generated
originals — no sample libraries, no external recordings, no third-party assets.**

Generation: synthesised offline by `Assets/Scripts/Editor/AudioSynthBuilder.cs`
(menu **Stencil Relay / Rebuild Audio**) — additive sine / detuned-sine bells,
filtered white noise, linear/exponential envelopes, pitch sweeps, seamless-loop
crossfade. Written as 16-bit PCM mono WAV @ 44.1 kHz, imported Vorbis q0.7
(loops streamed). Deterministic (fixed RNG seeds) — re-running produces identical files.

```
Source: Project-generated original
External dependency: None
Creation method: offline DSP synthesis (see AudioSynthBuilder.cs)
License: n/a (original work, part of this project)
```

## Clips (`Assets/Content/Resources/Audio/`)

| File | Purpose | Character |
|------|---------|-----------|
| sfx_plate_pickup | select / lift a stencil plate | soft two-tone acrylic tap |
| sfx_plate_slide | plate sliding | filtered-noise swish, falling |
| sfx_plate_snap | plate snaps to a grid position | short bright click + tick |
| sfx_pin_click | registration pin engages | tight high double-click |
| sfx_rotate | 90° rotation | low mechanical sweep + noise |
| sfx_flip | plate flip | wider sweep, longer |
| sfx_table_touch | tapping the table frame | dull low thud |
| sfx_projector_switch | PROJECT switch thrown | low clunk + air |
| sfx_projector_startup | projector lamp warms up | rising sweep + hum swell |
| sfx_light_hum | steady lamp hum (short) | 110 Hz + harmonic + air |
| sfx_target_activate | a required target lights | bell, warm |
| sfx_forbidden | forbidden sensor exposed | low beating two-tone, uneasy |
| sfx_success | PATTERN LOCKED | ascending major bell chord |
| sfx_misalign | MISALIGNED | soft low falling two-tone |
| sfx_rack_slide | Pattern Rack / level select | noise swish + low tick |
| sfx_pause | pause overlay | descending sweep, muted |
| sfx_settings | settings control / small UI | tiny high click |
| sfx_final_complete | MASTER ALIGNMENT COMPLETE | long six-note rising bell wash |
| amb_room_tone | studio ambience (loop) | 55/82 Hz drone + low air + slow breath |

## Mixer
`Assets/Content/Resources/StencilRelayMixer.mixer` — Master → Music, SFX, UI groups.
Exposed params `MasterVol` / `MusicVol` / `SFXVol` / `UIVol` (dB). The settings
sliders drive `MusicVol` and `SFXVol`/`UIVol` via `AudioController.ApplyVolumes`
(linear 0–1 → 20·log10 dB). Ambience routes to Music; gameplay SFX to SFX; menu to UI.

## Not the Punchcard Operator identity
No shared clips, no shared synthesis, no industrial/mechanical console palette — this
set is glass/acrylic/optical and calm (master prompt §22).
