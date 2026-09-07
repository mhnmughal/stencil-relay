using System.Collections.Generic;
using UnityEngine;
using StencilRelay.Core;

namespace StencilRelay.Levels
{
    /// <summary>Authoring asset for one plate's geometry (front + optional back face).</summary>
    [CreateAssetMenu(menuName = "Stencil Relay/Stencil Shape", fileName = "Shape")]
    public sealed class StencilShapeAsset : ScriptableObject
    {
        [Min(1)] public int footprintW = 3;
        [Min(1)] public int footprintH = 3;
        public List<Vector2Int> frontOpen = new List<Vector2Int>();
        public bool hasDistinctBack;
        public List<Vector2Int> backOpen = new List<Vector2Int>();

        [Header("Filter tags (Level 10) - parallel to *Open lists, 0 = clear")]
        public List<int> frontFilters = new List<int>();
        public List<int> backFilters = new List<int>();

        public StencilShape ToRuntime()
            => new StencilShape(footprintW, footprintH, frontOpen,
                hasDistinctBack ? backOpen : null,
                BuildFilterMap(frontOpen, frontFilters),
                hasDistinctBack ? BuildFilterMap(backOpen, backFilters) : null);

        static Dictionary<Vector2Int, int> BuildFilterMap(List<Vector2Int> cells, List<int> tags)
        {
            if (tags == null || tags.Count == 0) return null;
            var map = new Dictionary<Vector2Int, int>();
            for (int i = 0; i < cells.Count && i < tags.Count; i++)
                if (tags[i] != 0) map[cells[i]] = tags[i];
            return map.Count > 0 ? map : null;
        }
    }
}
