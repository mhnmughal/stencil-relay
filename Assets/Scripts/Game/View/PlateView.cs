using UnityEngine;
using StencilRelay.Core;

namespace StencilRelay.Game.View
{
    /// <summary>
    /// Visual for one <see cref="StencilPlate"/>. The mesh is always rebuilt from the
    /// plate's resolved board cells, so what you see equals what the evaluator computes.
    /// Manipulation flourishes are transient transform effects layered on top: a short
    /// slide-in on a committed move, a squash on rotate, an edge-flip on flip, an eased
    /// lift on select. Precise and brief - no bounce.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class PlateView : MonoBehaviour
    {
        public string PlateId { get; private set; }

        StencilPlate _plate;
        BoardView _board;
        MeshFilter _mf;
        MeshRenderer _mr;
        Mesh _mesh;
        MeshRenderer _shadow;
        readonly System.Collections.Generic.List<TMPro.TextMeshPro> _filterLabels = new();

        Color _baseTint = new Color(0.79f, 0.82f, 0.82f, 0.5f);
        readonly Color _selectedTint = new Color(0.66f, 0.91f, 0.94f, 0.30f);
        bool _selected;

        PlateTransform _last;
        bool _hasLast;
        Vector3 _slideOffset;     // decaying world offset for slide-in
        float _squash = 1f;       // decaying toward 1 after a rotate
        float _flipT = 1f;        // 0..1 flip progress (1 = done)
        float _dragOffsetActive;  // when >0, drag controls position

        void Awake()
        {
            _mf = GetComponent<MeshFilter>();
            _mr = GetComponent<MeshRenderer>();
            _mesh = new Mesh { name = "PlateMesh" };
            _mf.sharedMesh = _mesh;
        }

        public void Bind(StencilPlate plate, BoardView board, Material sharedMaterial, Color baseTint, int sortingOrder)
        {
            _plate = plate;
            _board = board;
            PlateId = plate.Id;
            _baseTint = baseTint;
            _hasLast = false;

            if (sharedMaterial != null)
            {
                _mr.sharedMaterial = sharedMaterial;
                _mr.sortingOrder = sortingOrder;

                var sgo = new GameObject("Shadow");
                sgo.transform.SetParent(transform, false);
                sgo.transform.localPosition = new Vector3(0.12f, -0.12f, 0.02f);
                sgo.AddComponent<MeshFilter>().sharedMesh = _mesh;
                _shadow = sgo.AddComponent<MeshRenderer>();
                _shadow.sharedMaterial = sharedMaterial;
                _shadow.sortingOrder = sortingOrder - 1;
                SetBlock(_shadow, new Color(0.06f, 0.08f, 0.09f, 0.28f));
            }
            Refresh();
        }

        public void SetSelected(bool selected)
        {
            _selected = selected;
            ApplyTint();
        }

        public void SetDragWorldOffset(Vector3 worldOffset)
        {
            _dragOffsetActive = 1f;
            transform.position = _board.Origin + worldOffset;
        }

        public void ClearDragOffset()
        {
            _dragOffsetActive = 0f;
            transform.position = _board.Origin;
        }

        /// <summary>Rebuild the mesh and register any flourish for what changed.</summary>
        public void Refresh()
        {
            if (_plate == null || _board == null) return;

            var t = _plate.Transform;
            if (_hasLast)
            {
                if (t.Offset != _last.Offset)
                    _slideOffset = new Vector3(
                        (_last.Offset.x - t.Offset.x) * _board.cellSize,
                        (_last.Offset.y - t.Offset.y) * _board.cellSize, 0f) * 0.6f;
                if (t.Rotation != _last.Rotation) _squash = 0.84f;
                if (t.Flipped != _last.Flipped) _flipT = 0f;
            }
            _last = t;
            _hasLast = true;

            var opaque = _plate.ResolveOpaque(_board.width, _board.height);
            CellMeshBuilder.Build(opaque.LitCells(), _mesh);
            int labelIndex = 0;
            foreach (var cell in _plate.ResolveOpenOnBoard(_board.width, _board.height).LitCells())
            {
                int filter = _plate.OpenFilterAtBoardCell(cell, _board.width, _board.height);
                if (filter <= 0) continue;
                if (labelIndex == _filterLabels.Count)
                {
                    var go = new GameObject("FilterNumber");
                    go.transform.SetParent(transform, false);
                    var text = go.AddComponent<TMPro.TextMeshPro>();
                    text.fontSize = 3;
                    text.alignment = TMPro.TextAlignmentOptions.Center;
                    text.color = new Color(0.1f, 0.2f, 0.22f);
                    text.GetComponent<MeshRenderer>().sortingOrder = 65;
                    text.rectTransform.sizeDelta = new Vector2(0.45f, 0.4f);
                    _filterLabels.Add(text);
                }
                var label = _filterLabels[labelIndex++];
                label.gameObject.SetActive(true);
                label.text = filter.ToString();
                label.transform.localPosition = new Vector3(cell.x + 0.22f, cell.y + 0.78f, -0.1f);
            }
            for (int i = labelIndex; i < _filterLabels.Count; i++) _filterLabels[i].gameObject.SetActive(false);
            ApplyTint();
        }

        void Update()
        {
            if (_board == null) return;
            float dt = Time.deltaTime;

            _slideOffset = Vector3.Lerp(_slideOffset, Vector3.zero, 1f - Mathf.Exp(-16f * dt));
            _squash = Mathf.Lerp(_squash, 1f, 1f - Mathf.Exp(-14f * dt));
            if (_flipT < 1f) _flipT = Mathf.Min(1f, _flipT + dt / 0.22f);

            float lift = _selected ? -0.15f : 0f;
            if (_dragOffsetActive <= 0f)
                transform.position = _board.Origin + _slideOffset + new Vector3(0, 0, lift);

            // flip = plate turns on its vertical edge (scale.x dips to ~0 and back)
            float flipScaleX = Mathf.Lerp(0.04f, 1f, Mathf.Abs(_flipT * 2f - 1f));
            float s = _board.cellSize;
            transform.localScale = new Vector3(s * _squash * flipScaleX, s * _squash, 1f);

            if (_shadow != null)
            {
                var target = _selected ? new Vector3(0.22f, -0.22f, 0.02f) : new Vector3(0.12f, -0.12f, 0.02f);
                _shadow.transform.localPosition = Vector3.Lerp(_shadow.transform.localPosition, target, 1f - Mathf.Exp(-12f * dt));
            }
        }

        void ApplyTint()
        {
            if (_mr == null) return;
            SetBlock(_mr, _selected ? _selectedTint : _baseTint);
        }

        void OnDestroy() { if (_mesh != null) Destroy(_mesh); }

        static void SetBlock(MeshRenderer mr, Color c)
        {
            var block = new MaterialPropertyBlock();
            mr.GetPropertyBlock(block);
            block.SetColor("_BaseColor", c);
            block.SetColor("_Color", c);
            mr.SetPropertyBlock(block);
        }
    }
}
