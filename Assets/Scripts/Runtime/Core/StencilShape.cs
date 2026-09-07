using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StencilRelay.Core
{
    /// <summary>
    /// Immutable geometry of one stencil plate.
    ///
    /// A plate is an opaque rectangle of size <see cref="FootprintW"/> x <see cref="FootprintH"/>.
    /// Cells listed in <see cref="FrontOpen"/> are holes (light passes); every other cell in
    /// the footprint blocks light. <see cref="BackOpen"/> is the geometry seen after a flip,
    /// authored in final (post-flip) coordinates. If BackOpen is null, flipping mirrors the
    /// front horizontally.
    ///
    /// Optional <see cref="FilterTags"/> maps an open cell -> filter category id (0 = clear).
    /// Filter evaluation is added at Level 10; the base model ignores tags.
    /// </summary>
    public sealed class StencilShape
    {
        public readonly int FootprintW;
        public readonly int FootprintH;
        readonly Vector2Int[] _frontOpen;
        readonly Vector2Int[] _backOpen;   // may be null
        readonly Dictionary<Vector2Int, int> _frontFilter;
        readonly Dictionary<Vector2Int, int> _backFilter;

        public IReadOnlyList<Vector2Int> FrontOpen => _frontOpen;
        public IReadOnlyList<Vector2Int> BackOpen => _backOpen;
        public bool HasDistinctBack => _backOpen != null;

        public StencilShape(
            int footprintW, int footprintH,
            IEnumerable<Vector2Int> frontOpen,
            IEnumerable<Vector2Int> backOpen = null,
            IDictionary<Vector2Int, int> frontFilter = null,
            IDictionary<Vector2Int, int> backFilter = null)
        {
            FootprintW = Mathf.Max(1, footprintW);
            FootprintH = Mathf.Max(1, footprintH);
            _frontOpen = Dedup(frontOpen);
            _backOpen = backOpen == null ? null : Dedup(backOpen);
            _frontFilter = frontFilter == null ? null : new Dictionary<Vector2Int, int>(frontFilter);
            _backFilter = backFilter == null ? null : new Dictionary<Vector2Int, int>(backFilter);
        }

        Vector2Int[] Dedup(IEnumerable<Vector2Int> cells)
        {
            if (cells == null) return System.Array.Empty<Vector2Int>();
            return cells
                .Where(c => c.x >= 0 && c.y >= 0 && c.x < FootprintW && c.y < FootprintH)
                .Distinct()
                .ToArray();
        }

        /// <summary>
        /// Open cells for the given face, in local footprint coordinates (before rotation/offset).
        /// </summary>
        public IEnumerable<Vector2Int> OpenCells(bool flipped)
        {
            if (!flipped) return _frontOpen;
            if (_backOpen != null) return _backOpen;
            return _frontOpen.Select(c => new Vector2Int((FootprintW - 1) - c.x, c.y));
        }

        /// <summary>Filter category for a local open cell (0 = clear). Level 10 use.</summary>
        public int FilterAt(bool flipped, Vector2Int localCell)
        {
            var map = flipped && _backOpen != null ? _backFilter : _frontFilter;
            if (map != null && map.TryGetValue(localCell, out var tag)) return tag;
            return 0;
        }
    }
}
