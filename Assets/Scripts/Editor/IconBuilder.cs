using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace StencilRelay.Editor
{
    /// <summary>
    /// Generates the original Stencil Relay app icon: three overlapping translucent
    /// stencil layers whose common overlap forms one bright geometric opening. 1024x1024,
    /// opaque (no App Store alpha), readable small, distinct from any prior icon.
    /// Project-generated (procedural) - see ASSET_PROVENANCE.md.
    ///
    /// Menu: Stencil Relay/Rebuild Icon
    /// </summary>
    public static class IconBuilder
    {
        const string Path1024 = "Assets/Content/AppIcon.png";
        const int N = 1024;

        static readonly Color Bg = new Color(0.043f, 0.063f, 0.078f);
        static readonly Color LayerA = new Color(0.690f, 0.553f, 0.341f); // brass
        static readonly Color LayerB = new Color(0.663f, 0.788f, 0.643f); // sage
        static readonly Color LayerC = new Color(0.55f, 0.72f, 0.80f);    // glass blue
        static readonly Color Core = new Color(0.78f, 0.96f, 1.0f);       // bright cyan opening

        [MenuItem("Stencil Relay/Rebuild Icon")]
        public static void Rebuild()
        {
            var tex = new Texture2D(N, N, TextureFormat.RGBA32, false);
            var px = new Color[N * N];

            Vector2 c = new Vector2(N / 2f, N / 2f);
            float plate = N * 0.30f;   // half-size of each plate square
            float hole = N * 0.135f;   // half-size of each plate's square hole

            // three plates: rotation, centre offset
            var plates = new[]
            {
                (rot:  8f  * Mathf.Deg2Rad, off: new Vector2(-N*0.055f,  N*0.045f), tint: LayerA),
                (rot: -18f * Mathf.Deg2Rad, off: new Vector2( N*0.06f,   N*0.02f ), tint: LayerB),
                (rot:  32f * Mathf.Deg2Rad, off: new Vector2( 0f,       -N*0.07f ), tint: LayerC),
            };

            for (int y = 0; y < N; y++)
            for (int x = 0; x < N; x++)
            {
                var p = new Vector2(x + 0.5f, y + 0.5f);
                var col = Bg;

                int openCount = 0;   // how many plates have their hole here
                foreach (var pl in plates)
                {
                    var local = Rotate(p - c - pl.off, -pl.rot);
                    bool inPlate = Mathf.Abs(local.x) < plate && Mathf.Abs(local.y) < plate;
                    bool inHole = Mathf.Abs(local.x) < hole && Mathf.Abs(local.y) < hole;
                    if (inPlate && !inHole)
                    {
                        // translucent acrylic plate body over what's underneath
                        col = Lerp(col, pl.tint, 0.34f);
                        // faint bright edge
                        float edge = Mathf.Min(plate - Mathf.Abs(local.x), plate - Mathf.Abs(local.y));
                        if (edge < N * 0.006f) col = Lerp(col, Color.white, 0.25f);
                    }
                    if (inHole) openCount++;
                }

                // where every plate is open -> the projected opening
                if (openCount == 3)
                {
                    col = Core;
                }
                else
                {
                    // soft glow bleeding out of the opening
                    float d = DistToCommonHole(p, c, plates, hole);
                    if (d < N * 0.10f) col = Lerp(col, Core, Mathf.Clamp01(1f - d / (N * 0.10f)) * 0.5f);
                }

                col.a = 1f;
                px[y * N + x] = col;
            }

            tex.SetPixels(px);
            tex.Apply();
            File.WriteAllBytes(Path1024, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.Refresh();
            var imp = AssetImporter.GetAtPath(Path1024) as TextureImporter;
            if (imp != null)
            {
                imp.textureType = TextureImporterType.Default;
                imp.alphaIsTransparency = false;
                imp.mipmapEnabled = false;
                imp.maxTextureSize = 1024;
                imp.textureCompression = TextureImporterCompression.Uncompressed; // icon quality
                imp.sRGBTexture = true;
                imp.SaveAndReimport();
            }

            AssignIcon();
            Debug.Log("[IconBuilder] Rebuilt " + Path1024 + " and assigned to Default + iOS + Android icons.");
        }

        static void AssignIcon()
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(Path1024);
            if (tex == null) return;

            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new[] { tex });
            SetPlatformIcons(NamedBuildTarget.iOS, tex);
            SetPlatformIcons(NamedBuildTarget.Android, tex);
            AssetDatabase.SaveAssets();
        }

        static void SetPlatformIcons(NamedBuildTarget target, Texture2D tex)
        {
            try
            {
                var kinds = PlayerSettings.GetSupportedIconKinds(target);
                foreach (var kind in kinds)
                {
                    var icons = PlayerSettings.GetPlatformIcons(target, kind);
                    for (int i = 0; i < icons.Length; i++) icons[i].SetTexture(tex);
                    PlayerSettings.SetPlatformIcons(target, kind, icons);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[IconBuilder] icon assign for {target} partial: {e.Message}");
            }
        }

        static Vector2 Rotate(Vector2 v, float rad)
        {
            float cs = Mathf.Cos(rad), sn = Mathf.Sin(rad);
            return new Vector2(v.x * cs - v.y * sn, v.x * sn + v.y * cs);
        }

        static Color Lerp(Color a, Color b, float t) => Color.Lerp(a, b, t);

        static float DistToCommonHole(Vector2 p, Vector2 c,
            (float rot, Vector2 off, Color tint)[] plates, float hole)
        {
            // approximate: max over plates of signed distance outside that plate's hole
            float worst = 0f;
            foreach (var pl in plates)
            {
                var local = Rotate(p - c - pl.off, -pl.rot);
                float dx = Mathf.Abs(local.x) - hole;
                float dy = Mathf.Abs(local.y) - hole;
                worst = Mathf.Max(worst, Mathf.Max(dx, dy));
            }
            return Mathf.Max(0f, worst);
        }
    }
}
