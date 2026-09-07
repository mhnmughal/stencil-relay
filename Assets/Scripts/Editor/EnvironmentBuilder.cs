using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using StencilRelay.Game.View;

namespace StencilRelay.Editor
{
    /// <summary>
    /// Composes the scene-authored optical-studio environment for the Light Studio and
    /// Projection Table scenes from the crafted procedural sprite set + the role material
    /// set + URP 2D lights. Called by <see cref="SceneBuilder"/>.
    ///
    /// The camera is a clean orthographic top-down of the light table - the "elevated
    /// drafting angle" feel comes from the projector unit, lens, lighting, framing and a
    /// vignette, not a tilted camera (a tilt fights grid readability and pointer mapping).
    /// </summary>
    public static class EnvironmentBuilder
    {
        const string S = "Assets/Content/Resources/Sprites/";
        const string M = "Assets/Content/Materials/";

        static Sprite Spr(string n) => AssetDatabase.LoadAssetAtPath<Sprite>($"{S}{n}.png");
        static Material Add => AssetDatabase.LoadAssetAtPath<Material>($"{M}M_ProjectedLight.mat");
        static Material Lit => AssetDatabase.LoadAssetAtPath<Material>($"{M}M_Cell.mat");

        // palette
        static readonly Color Ivory      = new Color(0.949f, 0.922f, 0.867f);
        static readonly Color Brass      = new Color(0.760f, 0.605f, 0.372f);
        static readonly Color BrassDark  = new Color(0.52f, 0.40f, 0.24f);
        static readonly Color Cyan       = new Color(0.624f, 0.910f, 0.941f);
        static readonly Color Alu        = new Color(0.60f, 0.63f, 0.66f);
        static readonly Color AluDark    = new Color(0.34f, 0.37f, 0.40f);
        static readonly Color BlueBlack  = new Color(0.043f, 0.063f, 0.078f);
        static readonly Color Drafting   = new Color(0.90f, 0.87f, 0.79f);
        static readonly Color Glass      = new Color(0.74f, 0.85f, 0.88f);

        // -------------------------------------------------------------- Projection Table

        public static void DecorateProjectionTable(Transform sceneParent, float boardW, float boardH)
        {
            var env = new GameObject("StudioEnvironment").transform;
            env.SetParent(sceneParent, false);
            float w = boardW, h = boardH;
            float halfMax = Mathf.Max(w, h) * 0.5f;

            // --- architectural backdrop ---
            Rect("Backdrop", env, "spr_pixel", Lit, BlueBlack, new Vector3(0, 0, 8), new Vector2(48, 30), -120);
            Rect("BackWall", env, "spr_panel", Lit, new Color(0.075f, 0.10f, 0.12f, 1f), new Vector3(0, 1.5f, 7.4f), new Vector2(34, 20), -115);
            Glow("BackWash", env, new Color(0.12f, 0.18f, 0.21f, 0.5f), new Vector3(-1.5f, 2f, 7f), 26, -112);

            // --- drafting table: aluminium frame -> warm surface -> recessed board well ---
            Rect("TableFrame", env, "spr_metal_strip", Lit, Alu, new Vector3(0, 0, 3.2f), new Vector2(w + 3.0f, h + 3.0f), -40);
            Rect("TableFrameInner", env, "spr_panel_inset", Lit, AluDark, new Vector3(0, 0, 3.1f), new Vector2(w + 2.2f, h + 2.2f), -38);
            Rect("TableSurface", env, "spr_panel", Lit, Drafting, new Vector3(0, 0, 2.9f), new Vector2(w + 1.7f, h + 1.7f), -30);
            Rect("WellShadow", env, "spr_soft_rect", Lit, new Color(0, 0, 0, 0.35f), new Vector3(0.05f, -0.05f, 2.75f), new Vector2(w + 0.5f, h + 0.5f), -26);
            Rect("BoardWell", env, "spr_panel_inset", Lit, new Color(0.14f, 0.17f, 0.19f, 1f), new Vector3(0, 0, 2.7f), new Vector2(w + 0.22f, h + 0.22f), -24);
            Glow("WellGlow", env, new Color(0.55f, 0.78f, 0.82f, 0.07f), new Vector3(0, 0, 2.6f), halfMax * 2f + 2f, -22);

            // grid guide
            var grid = new GameObject("BoardGuide").transform; grid.SetParent(env, false);
            grid.localPosition = new Vector3(0, 0, 2.4f);
            int cols = Mathf.RoundToInt(w / 1.2f), rows = Mathf.RoundToInt(h / 1.2f);
            for (int i = 0; i <= cols; i++)
                Rect($"gx{i}", grid, "spr_pixel", Lit, new Color(0.60f, 0.78f, 0.82f, 0.12f), new Vector3(-w / 2 + i * 1.2f, 0, 0), new Vector2(0.02f, h), -6);
            for (int j = 0; j <= rows; j++)
                Rect($"gy{j}", grid, "spr_pixel", Lit, new Color(0.60f, 0.78f, 0.82f, 0.12f), new Vector3(0, -h / 2 + j * 1.2f, 0), new Vector2(w, 0.02f), -6);

            // --- brass registration rails + ticks + corner brackets + pins ---
            for (int sgn = -1; sgn <= 1; sgn += 2)
            {
                float y = sgn * (h / 2 + 0.42f);
                Rect($"Rail_{sgn}", env, "spr_metal_strip", Lit, Brass, new Vector3(0, y, 2.0f), new Vector2(w + 0.9f, 0.46f), -14);
                Rect($"RailTicks_{sgn}", env, "spr_ticks", Lit, new Color(0.28f, 0.20f, 0.11f, 0.8f), new Vector3(0, y, 1.9f), new Vector2(w + 0.4f, 0.34f), -13);
            }
            foreach (var (sx, sy, rot) in new[] { (-1, -1, 0f), (1, -1, 90f), (1, 1, 180f), (-1, 1, 270f) })
            {
                var b = Rect($"Bracket_{sx}_{sy}", env, "spr_bracket", Lit, Brass,
                    new Vector3(sx * (w / 2 + 0.14f), sy * (h / 2 + 0.14f), 2.05f), new Vector2(0.9f, 0.9f), -12);
                b.transform.localRotation = Quaternion.Euler(0, 0, rot);
                Sprite("Pin_" + sx + "_" + sy, env, "spr_pin", Lit, Brass,
                    new Vector3(sx * (w / 2 - 0.35f), sy * (h / 2 - 0.35f), 1.85f), 0.34f, 4);
            }

            // --- projector unit: seated above the frame, clear of the top-centre HUD counter ---
            var proj = new GameObject("ProjectorUnit").transform; proj.SetParent(env, false);
            proj.localPosition = new Vector3(0, h / 2 + 3.5f, 1.5f);
            Rect("ProjMount", proj, "spr_metal_strip", Lit, AluDark, new Vector3(0, 0.75f, 0.3f), new Vector2(0.3f, 1.5f), 30);
            Rect("ProjHousing", proj, "spr_panel", Lit, new Color(0.26f, 0.29f, 0.31f, 1f), new Vector3(0, 0, 0.2f), new Vector2(1.9f, 1.1f), 32);
            Rect("ProjVent", proj, "spr_ticks", Lit, new Color(0, 0, 0, 0.5f), new Vector3(0, 0.28f, 0.1f), new Vector2(1.4f, 0.3f), 33);
            Sprite("ProjLens", proj, "spr_lens", Lit, new Color(0.82f, 0.9f, 0.94f, 1f), new Vector3(0, -0.5f, 0f), 0.85f, 34);
            var beam = Sprite("ProjBeam", proj, "spr_beam", Add, new Color(Cyan.r, Cyan.g, Cyan.b, 0.05f), new Vector3(0, -(h / 2 + 2f), 0.4f), 1f, 20);
            beam.transform.localScale = new Vector3(w * 0.9f / (beam.sprite.rect.size.x / beam.sprite.pixelsPerUnit),
                                                    (h + 3.0f) / (beam.sprite.rect.size.y / beam.sprite.pixelsPerUnit), 1f);
            beam.name = "ProjBeam";

            // --- warm halo behind the PROJECT control ---
            Glow("Ctrl_ProjectGlow", env, new Color(1f, 0.68f, 0.42f, 0.26f), new Vector3(0, -h / 2 - 0.95f, 1.5f), 2.6f, 39);

            // --- vignette on top of everything world-space ---
            Sprite("Vignette", env, "spr_vignette", Lit, new Color(0.02f, 0.03f, 0.04f, 0.85f), new Vector3(0, 0, -3f), Mathf.Max(w, h) * 3.4f, 200);

            // --- lighting ---
            var lights = new GameObject("Lighting").transform; lights.SetParent(sceneParent, false);
            GlobalLight2D("KeyLight", lights, new Color(1f, 0.96f, 0.9f), 0.62f);
            PointLight2D("ProjectorSpot", lights, new Vector3(0, 0, -1f), new Color(1f, 0.95f, 0.86f), 0f,
                Mathf.Min(w, h) * 0.35f, halfMax * 1.5f);
        }

        // -------------------------------------------------------------- Light Studio

        public static void DecorateLightStudio(Transform sceneParent)
        {
            var env = new GameObject("StudioEnvironment").transform;
            env.SetParent(sceneParent, false);

            Rect("Backdrop", env, "spr_pixel", Lit, BlueBlack, new Vector3(0, 0, 8), new Vector2(52, 30), -120);
            Rect("BackWall", env, "spr_panel", Lit, new Color(0.075f, 0.10f, 0.12f, 1f), new Vector3(-1f, 1f, 7.4f), new Vector2(40, 22), -115);
            Glow("RoomWash", env, new Color(0.12f, 0.18f, 0.21f, 0.5f), new Vector3(-2f, 1.5f, 7f), 34, -112);

            // --- large drafting table (centre-right so the left menu plaque sits on it) ---
            Rect("TableFrame", env, "spr_metal_strip", Lit, Alu, new Vector3(-1.2f, -0.6f, 3.2f), new Vector2(15.2f, 9.2f), -40);
            Rect("TableInner", env, "spr_panel_inset", Lit, AluDark, new Vector3(-1.2f, -0.6f, 3.1f), new Vector2(14.4f, 8.4f), -38);
            Rect("Table", env, "spr_panel", Lit, Drafting, new Vector3(-1.2f, -0.6f, 2.8f), new Vector2(13.6f, 7.8f), -30);
            Glow("TableGlow", env, new Color(1f, 0.97f, 0.9f, 0.18f), new Vector3(-1.2f, -0.6f, 2.6f), 13f, -24);

            // --- brass T-square rail with ticks across the upper table ---
            Rect("BrassRuler", env, "spr_metal_strip", Lit, Brass, new Vector3(-1.2f, 2.7f, 2f), new Vector2(11.4f, 0.44f), -14);
            Ticks("RulerTicks", env, new Vector3(-1.2f, 2.7f, 1.9f), new Vector2(11.0f, 0.38f), 0, -13);

            // --- magnifier loupe + a couple of glass set-squares on the table ---
            var lens = Sprite("MagnifyingLens", env, "spr_lens", Lit, new Color(0.86f, 0.92f, 0.95f, 1f), new Vector3(3.4f, -1.7f, 1.5f), 1.35f, 12);
            var mag = Rect("MagHandle", env, "spr_metal_strip", Lit, BrassDark, new Vector3(4.25f, -2.5f, 1.55f), new Vector2(1.5f, 0.2f), 11);
            mag.transform.localRotation = Quaternion.Euler(0, 0, -38f);
            var sq = Sprite("TemplateA", env, "spr_glass", Lit, new Color(Glass.r, Glass.g, Glass.b, 0.34f), new Vector3(2.2f, -1.9f, 1.62f), 1.6f, 8);
            sq.transform.localRotation = Quaternion.Euler(0, 0, 14f);

            // --- calibration pendant lamp: stem drops from above the frame, globe over the table ---
            Rect("LampArm", env, "spr_metal_strip", Lit, AluDark, new Vector3(-1.2f, 4.6f, 1.7f), new Vector2(0.24f, 2.4f), 12);
            Sprite("LampShade", env, "spr_disc", Lit, new Color(0.30f, 0.34f, 0.36f, 1f), new Vector3(-1.2f, 3.95f, 1.7f), 1.0f, 12);
            Sprite("LampGlobe", env, "spr_globe", Lit, new Color(1f, 0.96f, 0.86f, 1f), new Vector3(-1.2f, 3.65f, 1.6f), 0.95f, 14);
            Glow("LampGlow", env, new Color(1f, 0.9f, 0.7f, 0.4f), new Vector3(-1.2f, 3.5f, 1.7f), 3.2f, 11);

            // --- translucent stencil rack (the Pattern Rack) tucked against the right edge ---
            var rack = new GameObject("StencilRack").transform; rack.SetParent(env, false);
            rack.localPosition = new Vector3(7.7f, 0.2f, 2f);
            Rect("RackFrame", rack, "spr_panel", Lit, AluDark, new Vector3(0, 0, 0.4f), new Vector2(3.3f, 7.2f), -18);
            Rect("RackFace", rack, "spr_panel_inset", Lit, new Color(0.22f, 0.25f, 0.27f, 1f), new Vector3(0, 0, 0.3f), new Vector2(2.8f, 6.6f), -16);
            for (int i = 0; i < 8; i++)
            {
                float y = 2.7f - i * 0.74f;
                Rect($"Groove{i:00}", rack, "spr_pixel", Lit, new Color(0, 0, 0, 0.4f), new Vector3(0, y - 0.32f, 0.25f), new Vector2(2.5f, 0.05f), -15);
                var sheet = Sprite($"RackSheet{i:00}", rack, "spr_glass", Lit,
                    new Color(0.78f, 0.86f, 0.88f, 0.18f + (i % 3) * 0.04f),
                    new Vector3(0.1f + (i % 3) * 0.05f, y, 0.2f), 1f, -12 + (i % 3));
                sheet.transform.localScale = new Vector3(2.4f / (sheet.sprite.rect.size.x / sheet.sprite.pixelsPerUnit),
                                                         0.52f / (sheet.sprite.rect.size.y / sheet.sprite.pixelsPerUnit), 1f);
            }
            Glow("RackGlow", rack, new Color(0.62f, 0.91f, 0.94f, 0.16f), new Vector3(0, 0, 0.15f), 5f, -13);

            // --- optical control strip: brass knobs along the lower table edge ---
            for (int i = 0; i < 4; i++)
                Sprite($"Knob{i}", env, "spr_dial", Lit, Brass, new Vector3(1.6f + i * 1.0f, -4.05f, 1.5f), 0.58f, 14);
            Rect("ControlStrip", env, "spr_metal_strip", Lit, AluDark, new Vector3(2.7f, -4.05f, 1.6f), new Vector2(4.6f, 0.9f), 13);

            Sprite("Vignette", env, "spr_vignette", Lit, new Color(0.02f, 0.03f, 0.04f, 0.8f), new Vector3(-1f, 0, -3f), 40f, 200);

            var lights = new GameObject("Lighting").transform; lights.SetParent(sceneParent, false);
            GlobalLight2D("RoomLight", lights, new Color(0.95f, 0.93f, 0.9f), 0.66f);
            PointLight2D("TableLamp2D", lights, new Vector3(-1.2f, 1.5f, -1f), new Color(1f, 0.95f, 0.85f), 0.85f, 2f, 13f);
            PointLight2D("RackLamp2D", lights, new Vector3(7.7f, 0.2f, -1f), Cyan, 0.35f, 1f, 6f);
        }

        // -------------------------------------------------------------- primitives

        static SpriteRenderer Base(string name, Transform parent, string sprite, Material mat, Color color,
            Vector3 pos, Vector2 worldSize, int order)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = Spr(sprite);
            if (mat != null) sr.sharedMaterial = mat;
            sr.color = color;
            sr.sortingOrder = order;
            if (sr.sprite != null)
            {
                var native = sr.sprite.rect.size / sr.sprite.pixelsPerUnit;
                go.transform.localScale = new Vector3(worldSize.x / native.x, worldSize.y / native.y, 1f);
            }
            return sr;
        }

        static SpriteRenderer Rect(string n, Transform p, string spr, Material m, Color c, Vector3 pos, Vector2 size, int order)
            => Base(n, p, spr, m, c, pos, size, order);

        static SpriteRenderer Sprite(string n, Transform p, string spr, Material m, Color c, Vector3 pos, float diameter, int order)
            => Base(n, p, spr, m, c, pos, new Vector2(diameter, diameter), order);

        static SpriteRenderer Glow(string n, Transform p, Color c, Vector3 pos, float diameter, int order)
            => Base(n, p, "spr_glow", Add, c, pos, new Vector2(diameter, diameter), order);

        static SpriteRenderer Ticks(string n, Transform p, Vector3 pos, Vector2 size, float zRot, int order)
        {
            var sr = Base(n, p, "spr_ticks", Lit, new Color(0.72f, 0.58f, 0.36f, 0.85f), pos, size, order);
            sr.transform.localRotation = Quaternion.Euler(0, 0, zRot);
            return sr;
        }

        static void GlobalLight2D(string name, Transform parent, Color color, float intensity)
        {
            var go = new GameObject(name); go.transform.SetParent(parent, false);
            var so = new SerializedObject(go.AddComponent<Light2D>());
            so.FindProperty("m_LightType").enumValueIndex = (int)Light2D.LightType.Global;
            so.FindProperty("m_Color").colorValue = color;
            so.FindProperty("m_Intensity").floatValue = intensity;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void PointLight2D(string name, Transform parent, Vector3 pos, Color color, float intensity,
            float innerRadius, float outerRadius)
        {
            var go = new GameObject(name); go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            var so = new SerializedObject(go.AddComponent<Light2D>());
            so.FindProperty("m_LightType").enumValueIndex = (int)Light2D.LightType.Point;
            so.FindProperty("m_Color").colorValue = color;
            so.FindProperty("m_Intensity").floatValue = intensity;
            so.FindProperty("m_PointLightInnerRadius").floatValue = innerRadius;
            so.FindProperty("m_PointLightOuterRadius").floatValue = outerRadius;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
