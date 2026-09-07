using System.Collections.Generic;
using UnityEngine;

namespace StencilRelay.Core
{
    /// <summary>
    /// 90-degree rotation state. Clockwise, origin bottom-left.
    /// </summary>
    public enum Rot90
    {
        None = 0,
        CW90 = 1,
        CW180 = 2,
        CW270 = 3
    }

    /// <summary>
    /// Dense boolean mask over a width x height integer cell grid. Origin bottom-left.
    /// The whole puzzle is evaluated on these; rendering reads the same data so visuals
    /// can never change logic. (Master prompt section 17 - Determinism.)
    /// ponytail: bool[] backing; swap for a ulong bitset only if profiling shows mask ops hot.
    /// </summary>
    public sealed class CellGrid
    {
        public readonly int Width;
        public readonly int Height;
        readonly bool[] _cells;

        public CellGrid(int width, int height)
        {
            Width = Mathf.Max(0, width);
            Height = Mathf.Max(0, height);
            _cells = new bool[Width * Height];
        }

        public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

        public bool Get(int x, int y) => InBounds(x, y) && _cells[y * Width + x];

        public void Set(int x, int y, bool value)
        {
            if (InBounds(x, y)) _cells[y * Width + x] = value;
        }

        public void SetAll(bool value)
        {
            for (int i = 0; i < _cells.Length; i++) _cells[i] = value;
        }

        public CellGrid Clone()
        {
            var copy = new CellGrid(Width, Height);
            System.Array.Copy(_cells, copy._cells, _cells.Length);
            return copy;
        }

        /// <summary>this = this AND other (dimensions must match).</summary>
        public void And(CellGrid other)
        {
            RequireSameSize(other);
            for (int i = 0; i < _cells.Length; i++) _cells[i] &= other._cells[i];
        }

        /// <summary>this = this AND NOT other (dimensions must match).</summary>
        public void AndNot(CellGrid other)
        {
            RequireSameSize(other);
            for (int i = 0; i < _cells.Length; i++) _cells[i] &= !other._cells[i];
        }

        /// <summary>this = this OR other (dimensions must match).</summary>
        public void Or(CellGrid other)
        {
            RequireSameSize(other);
            for (int i = 0; i < _cells.Length; i++) _cells[i] |= other._cells[i];
        }

        public bool Any()
        {
            for (int i = 0; i < _cells.Length; i++) if (_cells[i]) return true;
            return false;
        }

        public int Count()
        {
            int n = 0;
            for (int i = 0; i < _cells.Length; i++) if (_cells[i]) n++;
            return n;
        }

        public bool ContentEquals(CellGrid other)
        {
            if (other == null || other.Width != Width || other.Height != Height) return false;
            for (int i = 0; i < _cells.Length; i++) if (_cells[i] != other._cells[i]) return false;
            return true;
        }

        public IEnumerable<Vector2Int> LitCells()
        {
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                if (_cells[y * Width + x]) yield return new Vector2Int(x, y);
        }

        public static CellGrid FromCells(int width, int height, IEnumerable<Vector2Int> cells)
        {
            var g = new CellGrid(width, height);
            if (cells != null)
                foreach (var c in cells) g.Set(c.x, c.y, true);
            return g;
        }

        void RequireSameSize(CellGrid other)
        {
            if (other == null || other.Width != Width || other.Height != Height)
                throw new System.ArgumentException(
                    $"CellGrid size mismatch: {Width}x{Height} vs {other?.Width}x{other?.Height}");
        }

        /// <summary>
        /// Map a local cell inside a footprint of size (w,h) through a 90-degree rotation.
        /// Returns the cell in the rotated footprint (whose size is (h,w) for CW90/CW270).
        /// </summary>
        public static Vector2Int RotateCell(Vector2Int c, Rot90 rot, int w, int h)
        {
            switch (rot)
            {
                case Rot90.CW90:  return new Vector2Int(c.y, (w - 1) - c.x);
                case Rot90.CW180: return new Vector2Int((w - 1) - c.x, (h - 1) - c.y);
                case Rot90.CW270: return new Vector2Int((h - 1) - c.y, c.x);
                default:          return c;
            }
        }

        /// <summary>Footprint size after applying a rotation to a (w,h) footprint.</summary>
        public static Vector2Int RotatedFootprint(Rot90 rot, int w, int h)
            => (rot == Rot90.CW90 || rot == Rot90.CW270) ? new Vector2Int(h, w) : new Vector2Int(w, h);

        public static Rot90 Inverse(Rot90 rot) => rot switch
        {
            Rot90.CW90 => Rot90.CW270,
            Rot90.CW270 => Rot90.CW90,
            _ => rot
        };

        /// <summary>
        /// Inverse of <see cref="RotateCell"/>: given a cell in the rotated footprint, return
        /// the original local cell. <paramref name="w"/>,<paramref name="h"/> are the ORIGINAL
        /// (pre-rotation) footprint dimensions.
        /// </summary>
        public static Vector2Int UnrotateCell(Vector2Int c, Rot90 rot, int w, int h)
        {
            var fp = RotatedFootprint(rot, w, h);
            return RotateCell(c, Inverse(rot), fp.x, fp.y);
        }
    }
}
