using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StencilRelay.Core
{
    /// <summary>Discrete placement of a plate: grid offset, 90-degree rotation, flipped face.</summary>
    public struct PlateTransform
    {
        public Vector2Int Offset;
        public Rot90 Rotation;
        public bool Flipped;

        public PlateTransform(Vector2Int offset, Rot90 rotation = Rot90.None, bool flipped = false)
        {
            Offset = offset;
            Rotation = rotation;
            Flipped = flipped;
        }

        public override string ToString() => $"({Offset.x},{Offset.y}) {Rotation}{(Flipped ? " F" : "")}";
    }

    /// <summary>
    /// A stencil plate with mutable placement. Pure logic - no MonoBehaviour, no scene deps.
    /// Resolves its current placement to board-space masks used by <see cref="StencilLayerStack"/>.
    /// </summary>
    public sealed class StencilPlate
    {
        public readonly string Id;
        public readonly StencilShape Shape;
        public PlateTransform Transform;

        /// <summary>Parallax / depth-plane hook (Level 9). Added to Offset when resolving. Usually zero.</summary>
        public Vector2Int ExtraShift;

        public StencilPlate(string id, StencilShape shape, PlateTransform initial)
        {
            Id = id;
            Shape = shape;
            Transform = initial;
        }

        /// <summary>Board cells this plate blocks light at, for its current placement.</summary>
        public CellGrid ResolveOpaque(int boardW, int boardH) => Resolve(boardW, boardH, opaque: true);

        /// <summary>Board cells that are holes over the board, for its current placement.</summary>
        public CellGrid ResolveOpenOnBoard(int boardW, int boardH) => Resolve(boardW, boardH, opaque: false);

        /// <summary>
        /// Filter category of this plate's OPEN cell at a board cell for the current
        /// placement: the tag (0 = clear glass) if the plate has a hole there, else -1
        /// (the plate is opaque there, or does not cover it). Used for Level 10 filters.
        /// </summary>
        public int OpenFilterAtBoardCell(Vector2Int boardCell, int boardW, int boardH)
        {
            int w = Shape.FootprintW;
            int h = Shape.FootprintH;
            var origin = Transform.Offset + ExtraShift;
            var fpCell = boardCell - origin;
            var fp = CellGrid.RotatedFootprint(Transform.Rotation, w, h);
            if (fpCell.x < 0 || fpCell.y < 0 || fpCell.x >= fp.x || fpCell.y >= fp.y) return -1;

            var local = CellGrid.UnrotateCell(fpCell, Transform.Rotation, w, h);
            foreach (var oc in Shape.OpenCells(Transform.Flipped))
                if (oc == local)
                    return Shape.FilterAt(Transform.Flipped, local);
            return -1;
        }

        CellGrid Resolve(int boardW, int boardH, bool opaque)
        {
            var result = new CellGrid(boardW, boardH);

            int w = Shape.FootprintW;
            int h = Shape.FootprintH;
            var fp = CellGrid.RotatedFootprint(Transform.Rotation, w, h);

            // Open cells rotated into footprint space.
            var openRotated = new HashSet<Vector2Int>();
            foreach (var c in Shape.OpenCells(Transform.Flipped))
                openRotated.Add(CellGrid.RotateCell(c, Transform.Rotation, w, h));

            Vector2Int origin = Transform.Offset + ExtraShift;

            for (int y = 0; y < fp.y; y++)
            for (int x = 0; x < fp.x; x++)
            {
                bool isOpen = openRotated.Contains(new Vector2Int(x, y));
                bool want = opaque ? !isOpen : isOpen;
                if (want)
                    result.Set(origin.x + x, origin.y + y, true);
            }

            return result;
        }
    }

    /// <summary>Which discrete manipulations a level allows for a plate (used by the controller & validator).</summary>
    public sealed class PlatePermissions
    {
        /// <summary>Allowed slide offsets. Null/empty = the initial offset only.</summary>
        public List<Vector2Int> AllowedOffsets = new List<Vector2Int>();
        /// <summary>Allowed rotation states. Null/empty = rotation locked to the initial value.</summary>
        public List<Rot90> AllowedRotations = new List<Rot90>();
        public bool FlipAllowed;

        public bool CanPlace(PlateTransform t, PlateTransform initial)
        {
            bool offsetOk = (AllowedOffsets == null || AllowedOffsets.Count == 0)
                ? t.Offset == initial.Offset
                : AllowedOffsets.Contains(t.Offset);
            bool rotOk = (AllowedRotations == null || AllowedRotations.Count == 0)
                ? t.Rotation == initial.Rotation
                : AllowedRotations.Contains(t.Rotation);
            bool flipOk = FlipAllowed || t.Flipped == initial.Flipped;
            return offsetOk && rotOk && flipOk;
        }

        /// <summary>All placements this permission set permits, given the initial transform.</summary>
        public IEnumerable<PlateTransform> EnumeratePlacements(PlateTransform initial)
        {
            var offsets = (AllowedOffsets != null && AllowedOffsets.Count > 0)
                ? AllowedOffsets : new List<Vector2Int> { initial.Offset };
            var rots = (AllowedRotations != null && AllowedRotations.Count > 0)
                ? AllowedRotations : new List<Rot90> { initial.Rotation };
            var flips = FlipAllowed ? new[] { false, true } : new[] { initial.Flipped };

            foreach (var o in offsets)
            foreach (var r in rots)
            foreach (var f in flips)
                yield return new PlateTransform(o, r, f);
        }
    }
}
