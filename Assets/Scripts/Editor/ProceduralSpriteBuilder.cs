using System.IO;
using UnityEditor;
using UnityEngine;

namespace StencilRelay.Editor
{
    /// <summary>
    /// Generates the crafted shape sprites the environment and UI are composed from:
    /// bevelled panels, brushed-metal strips, glass with a highlight streak, brass pins
    /// and dials, a lens, a lamp globe, a projector beam cone, tick rulers, corner
    /// brackets, a registration seal, dust motes. All project-generated (procedural),
    /// white/grey with baked lighting cues, tinted per use. See ASSET_PROVENANCE.md.
    ///
    /// Menu: Stencil Relay/Rebuild Sprites
    /// </summary>
    public static class ProceduralSpriteBuilder
    {
        const string Dir = "Assets/Content/Resources/Sprites";

        [MenuItem("Stencil Relay/Rebuild Sprites")]
        public static void Rebuild()
        {
            Directory.CreateDirectory(Dir);

            Save("spr_pixel", Solid(8));
            Save("spr_soft_rect", RoundedRect(256, 256, 44, 9));
            Save("spr_disc", Disc(256, 3));
            Save("spr_ring", Ring(256, 0.62f, 0.92f, 3));
            Save("spr_glow", RadialGlow(256, 2.2f));
            Save("spr_ticks", TickStrip(512, 40, 12));
            Save("spr_beam", BeamCone(192, 320));

            // crafted / lit
            Save("spr_panel", BevelPanel(320, 320, 40, top: 0.16f, bottom: -0.22f, rim: 0.05f));
            Save("spr_panel_inset", BevelPanel(320, 320, 40, top: -0.20f, bottom: 0.14f, rim: 0.05f));
            Save("spr_metal_strip", BrushedStrip(512, 96));
            Save("spr_glass", GlassPanel(320, 320, 40));
            Save("spr_pin", DomePin(160));
            Save("spr_dial", BrassDial(256));
            Save("spr_lens", Lens(256));
            Save("spr_globe", LampGlobe(256));
            Save("spr_bracket", CornerBracket(192, 34));
            Save("spr_seal", Seal(256));
            Save("spr_switch", RockerSwitch(256, 160));
            Save("spr_vignette", Vignette(512));

            AssetDatabase.Refresh();
            foreach (var f in Directory.GetFiles(Dir, "*.png"))
            {
                if (AssetImporter.GetAtPath(f.Replace('\\', '/')) is not TextureImporter importer) continue;
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 256;
                importer.filterMode = FilterMode.Bilinear;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                if (Path.GetFileNameWithoutExtension(f) == "spr_soft_rect") importer.spriteBorder = new Vector4(44, 44, 44, 44);
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }
            Debug.Log("[SpriteBuilder] Rebuilt " + Directory.GetFiles(Dir, "*.png").Length + " sprites in " + Dir);
        }

        // ---------------------------------------------------------------- helpers

        static void Save(string name, Texture2D tex)
        {
            File.WriteAllBytes($"{Dir}/{name}.png", tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
        }

        static Texture2D New(int w, int h)
        {
            var t = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var px = new Color[w * h];
            for (int i = 0; i < px.Length; i++) px[i] = new Color(1, 1, 1, 0);
            t.SetPixels(px);
            return t;
        }

        static float RoundBox(float px, float py, float w, float h, float r)
        {
            float dx = Mathf.Max(0, Mathf.Max(r - px, px - (w - 1 - r)));
            float dy = Mathf.Max(0, Mathf.Max(r - py, py - (h - 1 - r)));
            return Mathf.Sqrt(dx * dx + dy * dy) - r; // <0 inside
        }

        static Texture2D Solid(int s)
        {
            var t = New(s, s);
            for (int y = 0; y < s; y++) for (int x = 0; x < s; x++) t.SetPixel(x, y, Color.white);
            t.Apply();
            return t;
        }

        static Texture2D RoundedRect(int w, int h, float radius, float feather)
        {
            var t = New(w, h);
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float d = RoundBox(x, y, w, h, radius);
                t.SetPixel(x, y, new Color(1, 1, 1, Mathf.Clamp01(0.5f - d / feather)));
            }
            t.Apply();
            return t;
        }

        static Texture2D Disc(int s, float feather)
        {
            var t = New(s, s);
            float c = (s - 1) * 0.5f, r = s * 0.5f - 2;
            for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
            {
                float d = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c)) - r;
                t.SetPixel(x, y, new Color(1, 1, 1, Mathf.Clamp01(0.5f - d / feather)));
            }
            t.Apply();
            return t;
        }

        static Texture2D Ring(int s, float inner, float outer, float feather)
        {
            var t = New(s, s);
            float c = (s - 1) * 0.5f, R = s * 0.5f;
            for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
            {
                float d = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c)) / R;
                float a = Mathf.Clamp01(0.5f - (inner - d) / (feather / R)) *
                          Mathf.Clamp01(0.5f - (d - outer) / (feather / R));
                t.SetPixel(x, y, new Color(1, 1, 1, a));
            }
            t.Apply();
            return t;
        }

        static Texture2D RadialGlow(int s, float falloff)
        {
            var t = New(s, s);
            float c = (s - 1) * 0.5f, R = s * 0.5f;
            for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
            {
                float d = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c)) / R;
                t.SetPixel(x, y, new Color(1, 1, 1, Mathf.Clamp01(Mathf.Pow(1f - d, falloff))));
            }
            t.Apply();
            return t;
        }

        static Texture2D Vignette(int s)
        {
            var t = New(s, s);
            float c = (s - 1) * 0.5f, R = s * 0.5f;
            for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
            {
                float d = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c)) / R;
                float a = Mathf.Clamp01((d - 0.55f) / 0.45f);
                t.SetPixel(x, y, new Color(1, 1, 1, a * a * 0.9f));
            }
            t.Apply();
            return t;
        }

        static Texture2D TickStrip(int w, int h, int count)
        {
            var t = New(w, h);
            for (int i = 0; i <= count; i++)
            {
                int x = Mathf.RoundToInt((float)i / count * (w - 4)) + 2;
                int len = (i % 4 == 0) ? h - 6 : h / 2;
                for (int y = 3; y < 3 + len; y++)
                for (int dx = 0; dx < 2; dx++)
                    t.SetPixel(Mathf.Min(w - 1, x + dx), y, new Color(1, 1, 1, 0.9f));
            }
            // faint baseline
            for (int x = 0; x < w; x++) t.SetPixel(x, 2, new Color(1, 1, 1, 0.35f));
            t.Apply();
            return t;
        }

        static Texture2D BeamCone(int w, int h)
        {
            var t = New(w, h);
            float cx = (w - 1) * 0.5f;
            for (int y = 0; y < h; y++)
            {
                float v = y / (float)h;                 // 0 top (narrow) .. 1 bottom (wide)
                float halfW = Mathf.Lerp(w * 0.06f, w * 0.5f, v);
                float vert = Mathf.Lerp(0.9f, 0.08f, v);
                for (int x = 0; x < w; x++)
                {
                    float horiz = 1f - Mathf.Clamp01(Mathf.Abs(x - cx) / halfW);
                    t.SetPixel(x, y, new Color(1, 1, 1, Mathf.Clamp01(horiz * horiz * vert)));
                }
            }
            t.Apply();
            return t;
        }

        static Texture2D BevelPanel(int w, int h, float radius, float top, float bottom, float rim)
        {
            var t = New(w, h);
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float d = RoundBox(x, y, w, h, radius);
                float a = Mathf.Clamp01(0.5f - d / 4f);
                if (a <= 0f) { t.SetPixel(x, y, new Color(0, 0, 0, 0)); continue; }

                float v = y / (float)h;                       // 0 bottom .. 1 top
                float shade = 1f + Mathf.Lerp(bottom, top, v); // vertical light gradient
                // rim light near the rounded edge
                float edge = Mathf.Clamp01(1f - Mathf.Abs(d) / 10f);
                shade += edge * rim;
                shade = Mathf.Clamp(shade, 0.15f, 1.6f);
                t.SetPixel(x, y, new Color(shade, shade, shade, a));
            }
            t.Apply();
            return t;
        }

        static Texture2D BrushedStrip(int w, int h)
        {
            var t = New(w, h);
            var rnd = new System.Random(4242);
            for (int y = 0; y < h; y++)
            {
                float band = 0.85f + 0.15f * Mathf.Sin(y / (float)h * Mathf.PI);
                for (int x = 0; x < w; x++)
                {
                    float streak = 0.94f + 0.06f * Mathf.Sin(x * 0.35f + y * 0.02f)
                                   + (float)(rnd.NextDouble() - 0.5) * 0.05f;
                    float edge = Mathf.Clamp01(Mathf.Min(y, h - 1 - y) / 3f);
                    float s = Mathf.Clamp01(band * streak);
                    t.SetPixel(x, y, new Color(s, s, s, edge));
                }
            }
            t.Apply();
            return t;
        }

        static Texture2D GlassPanel(int w, int h, float radius)
        {
            var t = New(w, h);
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float d = RoundBox(x, y, w, h, radius);
                float a = Mathf.Clamp01(0.5f - d / 4f);
                if (a <= 0f) { t.SetPixel(x, y, new Color(0, 0, 0, 0)); continue; }
                float body = 0.55f;
                // diagonal highlight streak
                float diag = (x + (h - y)) / (float)(w + h);
                float streak = Mathf.Exp(-Mathf.Pow((diag - 0.32f) / 0.06f, 2f)) * 0.5f
                             + Mathf.Exp(-Mathf.Pow((diag - 0.62f) / 0.03f, 2f)) * 0.3f;
                float edge = Mathf.Clamp01(1f - Mathf.Abs(d) / 8f) * 0.35f;
                float s = Mathf.Clamp01(body + streak + edge);
                t.SetPixel(x, y, new Color(s, s, s, a * (0.32f + streak)));
            }
            t.Apply();
            return t;
        }

        static Texture2D DomePin(int s)
        {
            var t = New(s, s);
            float c = (s - 1) * 0.5f, R = s * 0.42f;
            for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
            {
                float dx = (x - c) / R, dy = (y - c) / R;
                float rr = dx * dx + dy * dy;
                if (rr > 1f) { t.SetPixel(x, y, new Color(0, 0, 0, 0)); continue; }
                float z = Mathf.Sqrt(1f - rr);
                // light from upper-left
                float lit = Mathf.Clamp01(0.35f + 0.65f * (-dx * 0.5f + dy * 0.5f + z * 0.7f));
                float spec = Mathf.Pow(Mathf.Clamp01(-dx * 0.4f + dy * 0.5f + z * 0.75f), 12f);
                float shade = Mathf.Clamp01(lit + spec * 0.8f);
                float a = Mathf.Clamp01((1f - rr) / 0.08f);
                t.SetPixel(x, y, new Color(shade, shade, shade, a));
            }
            t.Apply();
            return t;
        }

        static Texture2D BrassDial(int s)
        {
            var t = New(s, s);
            float c = (s - 1) * 0.5f, R = s * 0.5f;
            for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
            {
                float dx = x - c, dy = y - c;
                float d = Mathf.Sqrt(dx * dx + dy * dy) / R;
                if (d > 1f) { t.SetPixel(x, y, new Color(0, 0, 0, 0)); continue; }
                float ang = Mathf.Atan2(dy, dx);
                float knurl = 0.9f + 0.1f * Mathf.Sin(ang * 24f);
                float ringLight = 0.75f + 0.25f * Mathf.Sin(ang - 2.2f);   // directional sheen
                float face = d < 0.62f ? 1.05f : knurl;
                float inner = d < 0.62f ? (0.85f + 0.2f * (dy / R)) : 1f;  // domed face
                float a = Mathf.Clamp01((1f - d) / 0.06f);
                float shade = Mathf.Clamp01(face * ringLight * inner);
                // pointer notch
                if (d > 0.55f && d < 0.95f && Mathf.Abs(Mathf.DeltaAngle(ang * Mathf.Rad2Deg, 90f)) < 6f)
                    shade = Mathf.Min(shade, 0.25f);
                t.SetPixel(x, y, new Color(shade, shade, shade, a));
            }
            t.Apply();
            return t;
        }

        static Texture2D Lens(int s)
        {
            var t = New(s, s);
            float c = (s - 1) * 0.5f, R = s * 0.5f;
            for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
            {
                float dx = x - c, dy = y - c;
                float d = Mathf.Sqrt(dx * dx + dy * dy) / R;
                if (d > 1f) { t.SetPixel(x, y, new Color(0, 0, 0, 0)); continue; }
                float rim = Mathf.Clamp01(1f - Mathf.Abs(d - 0.88f) / 0.10f);         // metal rim
                float glass = d < 0.82f ? 0.28f : 0f;
                float refl = d < 0.82f ? Mathf.Exp(-Mathf.Pow((d - 0.35f) / 0.12f, 2f)) * 0.4f : 0f;
                float sweep = d < 0.82f ? Mathf.Clamp01(0.2f + 0.3f * (dx + dy) / R) * 0.25f : 0f;
                float a = Mathf.Clamp01((1f - d) / 0.05f) * (rim > 0.02f ? 1f : (glass + refl > 0.02f ? 0.9f : 0f));
                float shade = Mathf.Clamp01(rim * 0.9f + glass + refl + sweep);
                t.SetPixel(x, y, new Color(shade, shade, shade, Mathf.Max(a, rim)));
            }
            t.Apply();
            return t;
        }

        static Texture2D LampGlobe(int s)
        {
            var t = New(s, s);
            float c = (s - 1) * 0.5f, R = s * 0.46f;
            for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
            {
                float dx = (x - c) / R, dy = (y - c) / R;
                float rr = dx * dx + dy * dy;
                if (rr > 1f) { t.SetPixel(x, y, new Color(0, 0, 0, 0)); continue; }
                float hot = Mathf.Exp(-Mathf.Pow((Mathf.Sqrt((dx + 0.2f) * (dx + 0.2f) + (dy - 0.2f) * (dy - 0.2f))) / 0.35f, 2f));
                float body = Mathf.Lerp(0.55f, 1f, 1f - rr);
                float shade = Mathf.Clamp01(body + hot * 0.8f);
                float a = Mathf.Clamp01((1f - rr) / 0.12f);
                t.SetPixel(x, y, new Color(shade, shade, shade, a));
            }
            t.Apply();
            return t;
        }

        static Texture2D CornerBracket(int s, int thick)
        {
            var t = New(s, s);
            int arm = (int)(s * 0.66f);
            for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
            {
                bool on = (x < thick && y < arm) || (y < thick && x < arm);
                if (!on) { t.SetPixel(x, y, new Color(0, 0, 0, 0)); continue; }
                float lit = 0.8f + 0.2f * ((x < thick ? (1f - x / (float)thick) : 0f) + (y < thick ? (1f - y / (float)thick) : 0f));
                t.SetPixel(x, y, new Color(lit, lit, lit, 1f));
            }
            t.Apply();
            return t;
        }

        static Texture2D Seal(int s)
        {
            var t = New(s, s);
            float c = (s - 1) * 0.5f, R = s * 0.5f;
            for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
            {
                float dx = x - c, dy = y - c;
                float d = Mathf.Sqrt(dx * dx + dy * dy) / R;
                float ang = Mathf.Atan2(dy, dx);
                float scallop = 0.80f + 0.06f * Mathf.Sin(ang * 20f);
                float outer = Mathf.Clamp01((scallop - d) / 0.03f);
                float innerRing = Mathf.Clamp01(1f - Mathf.Abs(d - 0.6f) / 0.04f);
                float star = 0f;
                for (int k = 0; k < 6; k++)
                {
                    float a2 = k * Mathf.PI / 3f + Mathf.PI / 6f;
                    float px = Mathf.Cos(a2) * 0.32f * R + c, py = Mathf.Sin(a2) * 0.32f * R + c;
                    star = Mathf.Max(star, Mathf.Clamp01(1f - Mathf.Sqrt((x - px) * (x - px) + (y - py) * (y - py)) / (0.05f * R)));
                }
                float a = Mathf.Max(Mathf.Max(outer * 0.9f, innerRing), star);
                float shade = 0.6f + 0.4f * (dy / R);
                t.SetPixel(x, y, new Color(shade, shade, shade, a));
            }
            t.Apply();
            return t;
        }

        static Texture2D RockerSwitch(int w, int h)
        {
            var t = New(w, h);
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float d = RoundBox(x, y, w, h, h * 0.42f);
                float a = Mathf.Clamp01(0.5f - d / 3f);
                if (a <= 0f) { t.SetPixel(x, y, new Color(0, 0, 0, 0)); continue; }
                float v = y / (float)h;
                // pressed-in lower half glows, upper is a lit lip
                float shade = v > 0.5f ? Mathf.Lerp(1.15f, 0.75f, (v - 0.5f) * 2f)
                                       : Mathf.Lerp(0.4f, 0.7f, v * 2f);
                t.SetPixel(x, y, new Color(shade, shade, shade, a));
            }
            t.Apply();
            return t;
        }
    }
}
