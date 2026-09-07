# ASSET_PROVENANCE.md — Stencil Relay

Every font, texture, sprite, mesh, icon, audio and third-party asset. Only
project-generated originals, properly licensed, or verified open-source.

## Fonts
| Asset | Source | License | Used for |
|---|---|---|---|
| LiberationSans SDF (+ fallback) | Unity TMP Essential Resources (`Window/TextMeshPro/Import TMP Essential Resources`), from `com.unity.ugui` | **SIL Open Font License 1.1** (Liberation fonts, Red Hat / Google) — redistribution + embedding permitted | All UI text (TextMeshPro default) |

No custom fonts bundled. The heading/body pairing is achieved with weight/size/tracking
on the one OFL family — deliberately not the Misprint/Punchcard typography.

## Sprites / textures (project-generated, procedural)
| Asset | Method | Used for |
|---|---|---|
| `Assets/Content/Resources/Sprites/spr_pixel,spr_soft_rect,spr_disc,spr_ring,spr_glow,spr_ticks,spr_beam` | `ProceduralSpriteBuilder.cs` — soft-edge shape rasterisation in-editor | environment composition, UI button backing, projection markers/beam |
| `Assets/Content/AppIcon.png` (1024²) | `IconBuilder.cs` — three rotated translucent plates + common-hole bright opening | app icon (Default / iOS / Android) |

Source for all: **Project-generated original. External dependency: None.** Deterministic.

## Materials
`Assets/Content/Materials/M_*.mat` (26) — `MaterialLibraryBuilder.cs`, all shader
`Sprites/Default` (Unity built-in), tinted. No external material assets.

## Meshes
Runtime-generated quad meshes only (`CellMeshBuilder`, plate / light / grid). No
imported models.

## Audio
See `AUDIO_PROVENANCE.md` — 19 clips + mixer, all project-generated originals.

## Third-party
None at asset level. Packages: see `PACKAGE_AUDIT.md`. The Unity engine, URP, TMP
default resources and Input System are Unity-provided under the Unity license /
bundled OFL as noted.
