using System.Collections.Generic;
using UnityEngine;
using StencilRelay.Core;

namespace StencilRelay.Game.View
{
    /// <summary>
    /// Builds a single merged quad mesh from a set of grid cells, in a local space where
    /// one cell = 1 unit and cell (0,0)'s corner is at local origin. Used for plate bodies
    /// (opaque footprint minus holes) and for the projected-light overlay.
    /// ponytail: rebuilt only when the cell set changes (flip / new level), not per frame.
    /// </summary>
    public static class CellMeshBuilder
    {
        public static Mesh Build(IEnumerable<Vector2Int> cells, Mesh reuse = null)
        {
            var mesh = reuse != null ? reuse : new Mesh { name = "CellMesh" };
            mesh.Clear();

            var verts = new List<Vector3>();
            var tris = new List<int>();
            var uvs = new List<Vector2>();

            foreach (var c in cells)
            {
                int b = verts.Count;
                verts.Add(new Vector3(c.x, c.y, 0));
                verts.Add(new Vector3(c.x + 1, c.y, 0));
                verts.Add(new Vector3(c.x + 1, c.y + 1, 0));
                verts.Add(new Vector3(c.x, c.y + 1, 0));
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(0, 1));
                tris.Add(b); tris.Add(b + 2); tris.Add(b + 1);
                tris.Add(b); tris.Add(b + 3); tris.Add(b + 2);
            }

            mesh.SetVertices(verts);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary>Cells inside a footprint that are NOT holes (the opaque body), in local footprint coords.</summary>
        public static IEnumerable<Vector2Int> OpaqueFootprintCells(StencilShape shape, bool flipped)
        {
            var holes = new HashSet<Vector2Int>(shape.OpenCells(flipped));
            for (int y = 0; y < shape.FootprintH; y++)
            for (int x = 0; x < shape.FootprintW; x++)
            {
                var cell = new Vector2Int(x, y);
                if (!holes.Contains(cell)) yield return cell;
            }
        }
    }
}
