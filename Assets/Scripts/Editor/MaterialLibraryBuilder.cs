using System.IO;
using UnityEditor;
using UnityEngine;

namespace StencilRelay.Editor
{
    /// <summary>
    /// Builds the Stencil Relay material set as real .mat assets under Assets/Content/Materials.
    /// URP-2D-safe: everything uses `Sprites/Default` (renders under Renderer2D and tints
    /// per-renderer via MaterialPropertyBlock `_Color`). Additive variants for light/glow.
    /// Palette matches Assets/Documentation/UI_UX_SYSTEM.md.
    ///
    /// Menu: Stencil Relay/Rebuild Materials
    /// </summary>
    public static class MaterialLibraryBuilder
    {
        const string Dir = "Assets/Content/Materials";

        // palette
        static readonly Color Ivory      = new Color(0.949f, 0.922f, 0.867f);
        static readonly Color GlassNeutral = new Color(0.788f, 0.824f, 0.816f);
        static readonly Color BlueBlack   = new Color(0.055f, 0.078f, 0.094f);
        static readonly Color Cyan        = new Color(0.624f, 0.910f, 0.941f);
        static readonly Color Brass       = new Color(0.690f, 0.553f, 0.341f);
        static readonly Color Coral       = new Color(0.894f, 0.471f, 0.373f);
        static readonly Color Sage        = new Color(0.663f, 0.788f, 0.643f);
        static readonly Color GrayShadow  = new Color(0.431f, 0.463f, 0.475f);

        [MenuItem("Stencil Relay/Rebuild Materials")]
        public static void Rebuild()
        {
            Directory.CreateDirectory(Dir);

            // structural / environment
            Make("M_Backdrop", BlueBlack, 1f);
            Make("M_Drafting", new Color(0.96f, 0.93f, 0.86f), 1f);            // warm table surface
            Make("M_TableGlow", new Color(1f, 0.97f, 0.90f), 0.55f, additive: true);
            Make("M_Aluminium", new Color(0.62f, 0.64f, 0.66f), 1f);
            Make("M_Brass", Brass, 1f);
            Make("M_MaskBlack", new Color(0.06f, 0.07f, 0.08f), 1f);
            Make("M_Glass", new Color(0.85f, 0.90f, 0.92f), 0.18f);
            Make("M_Frost", new Color(0.80f, 0.85f, 0.87f), 0.86f);           // pause overlay

            // stencil plates (translucent acrylic tints)
            Make("M_Plate_A", new Color(0.79f, 0.82f, 0.82f), 0.42f);
            Make("M_Plate_B", new Color(0.72f, 0.80f, 0.86f), 0.42f);
            Make("M_Plate_C", new Color(0.86f, 0.82f, 0.74f), 0.42f);
            Make("M_Plate_D", new Color(0.80f, 0.76f, 0.84f), 0.42f);
            Make("M_PlateEdgeGlow", Cyan, 0.7f, additive: true);
            Make("M_PlateShadow", GrayShadow, 0.35f);

            // projection
            Make("M_LightBeam", Cyan, 0.5f, additive: true);
            Make("M_ProjectedLight", Cyan, 0.85f, additive: true);
            Make("M_TargetUnlit", GrayShadow, 1f);
            Make("M_TargetLit", Sage, 1f, additive: true);
            Make("M_ForbiddenDark", GrayShadow, 1f);
            Make("M_ForbiddenExposed", Coral, 1f, additive: true);

            // keep the legacy generic material working (controller fallback / older refs)
            Make("M_Cell", Color.white, 1f);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MaterialLibrary] Rebuilt Stencil Relay materials in " + Dir);
        }

        static void Make(string name, Color color, float alpha, bool additive = false)
        {
            var path = $"{Dir}/{name}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            var shader = Shader.Find("Sprites/Default");
            if (mat == null)
            {
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.shader = shader;
            var c = color; c.a = Mathf.Clamp01(alpha);
            mat.color = c;
            mat.SetColor("_Color", c);

            if (additive)
            {
                mat.SetFloat("_BlendMode", 1f);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.EnableKeyword("_ALPHABLEND_ON");
            }
            mat.renderQueue = 3000;
            EditorUtility.SetDirty(mat);
        }
    }
}
