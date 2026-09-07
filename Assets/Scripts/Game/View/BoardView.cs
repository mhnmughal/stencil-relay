using UnityEngine;

namespace StencilRelay.Game.View
{
    /// <summary>
    /// Maps the integer puzzle grid to world space on the light table. Origin is the
    /// bottom-left corner of cell (0,0); the board is centered on this transform.
    /// The camera looks down the -Z (or a slight diagonal); the board lies on the XY plane.
    /// </summary>
    public sealed class BoardView : MonoBehaviour
    {
        [Min(2)] public int width = 6;
        [Min(2)] public int height = 6;
        [Min(0.05f)] public float cellSize = 1f;
        Mesh _guide;

        public void BuildGuide(Material material)
        {
            if (_guide == null)
            {
                _guide = new Mesh { name = "LevelGrid" };
                gameObject.AddComponent<MeshFilter>().sharedMesh = _guide;
                var renderer = gameObject.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = material;
                renderer.sortingOrder = -6;
                var block = new MaterialPropertyBlock();
                block.SetColor("_Color", new Color(0.6f, 0.78f, 0.82f, 0.16f));
                renderer.SetPropertyBlock(block);
            }
            var vertices = new System.Collections.Generic.List<Vector3>();
            var triangles = new System.Collections.Generic.List<int>();
            void Rect(float x, float y, float w, float h)
            {
                int start = vertices.Count;
                vertices.Add(new Vector3(x,y,0)); vertices.Add(new Vector3(x+w,y,0));
                vertices.Add(new Vector3(x+w,y+h,0)); vertices.Add(new Vector3(x,y+h,0));
                triangles.AddRange(new[] { start, start+2, start+1, start, start+3, start+2 });
            }
            float left = -BoardWorldSize.x / 2, bottom = -BoardWorldSize.y / 2;
            for (int x = 0; x <= width; x++) Rect(left + x * cellSize - 0.008f, bottom, 0.016f, BoardWorldSize.y);
            for (int y = 0; y <= height; y++) Rect(left, bottom + y * cellSize - 0.008f, BoardWorldSize.x, 0.016f);
            _guide.Clear(); _guide.SetVertices(vertices); _guide.SetTriangles(triangles, 0);
            _guide.SetUVs(0, new System.Collections.Generic.List<Vector2>(new Vector2[vertices.Count]));
            _guide.RecalculateBounds();
        }

        void OnDestroy() { if (_guide != null) Destroy(_guide); }

        public Vector2 BoardWorldSize => new Vector2(width * cellSize, height * cellSize);

        /// <summary>World position of the bottom-left corner of cell (0,0).</summary>
        public Vector3 Origin => transform.position - (Vector3)(BoardWorldSize * 0.5f);

        /// <summary>World position of the centre of cell (cx,cy). Fractional cells allowed (drag).</summary>
        public Vector3 CellCenter(float cx, float cy)
            => Origin + new Vector3((cx + 0.5f) * cellSize, (cy + 0.5f) * cellSize, 0f);

        public Vector3 CellCorner(int cx, int cy)
            => Origin + new Vector3(cx * cellSize, cy * cellSize, 0f);

        /// <summary>Nearest grid cell (may be outside the board) for a world point.</summary>
        public Vector2Int WorldToCell(Vector3 world)
        {
            var local = world - Origin;
            return new Vector2Int(
                Mathf.FloorToInt(local.x / cellSize),
                Mathf.FloorToInt(local.y / cellSize));
        }

        /// <summary>Continuous grid coordinate (cell units) for a world point - used while dragging.</summary>
        public Vector2 WorldToGrid(Vector3 world)
        {
            var local = world - Origin;
            return new Vector2(local.x / cellSize - 0.5f, local.y / cellSize - 0.5f);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.6f, 0.9f, 1f, 0.5f);
            for (int x = 0; x <= width; x++)
                Gizmos.DrawLine(CellCorner(x, 0), CellCorner(x, height));
            for (int y = 0; y <= height; y++)
                Gizmos.DrawLine(CellCorner(0, y), CellCorner(width, y));
        }
    }
}
