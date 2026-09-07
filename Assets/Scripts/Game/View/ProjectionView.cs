using System.Collections.Generic;
using UnityEngine;
using StencilRelay.Core;

namespace StencilRelay.Game.View
{
    /// <summary>
    /// Draws the projected-light overlay and the target / forbidden-sensor markers for one
    /// projector, from a <see cref="ProjectionResult"/>. Purely presentational - reads the
    /// evaluator's output, never recomputes. Targets are discs, forbidden sensors are rings
    /// (shape, not just colour - accessibility). On PROJECT the light mask reveals, a beam
    /// drops from the projector, lit targets bloom, exposed forbidden sensors pulse.
    /// </summary>
    public sealed class ProjectionView : MonoBehaviour
    {
        public Color lightColor = new Color(0.62f, 0.91f, 0.94f, 0.9f);
        public Color targetLit = new Color(0.66f, 0.79f, 0.64f, 1f);
        public Color targetUnlit = new Color(0.43f, 0.47f, 0.49f, 0.85f);
        public Color forbiddenExposed = new Color(0.89f, 0.47f, 0.37f, 1f);
        public Color forbiddenDark = new Color(0.43f, 0.47f, 0.49f, 0.85f);

        BoardView _board;
        Material _sharedMat;
        MeshRenderer _lightRenderer;
        Mesh _lightMesh;
        SpriteRenderer _beam;

        sealed class Marker
        {
            public SpriteRenderer Sr;
            public bool IsTarget;
            public string Label;
            public string ProjectorId;
            public Color Target;
            public float Pop;     // decays to 1
            public bool Pulse;
        }

        readonly List<Marker> _markers = new List<Marker>();
        float _reveal;            // 0..1 light-mask reveal
        float _beamT;             // 0..1 beam drop
        bool _showing;
        MaterialPropertyBlock _lightBlock;

        void Awake() => _lightBlock = new MaterialPropertyBlock();

        static Sprite Spr(string n) => Resources.Load<Sprite>("Sprites/" + n);

        public void Init(BoardView board, Material sharedUnlitTransparent)
        {
            _board = board;
            _sharedMat = sharedUnlitTransparent;

            var lightGo = new GameObject("ProjectedLight");
            lightGo.transform.SetParent(transform, false);
            lightGo.AddComponent<MeshFilter>().sharedMesh = _lightMesh = new Mesh { name = "LightMesh" };
            _lightRenderer = lightGo.AddComponent<MeshRenderer>();
            _lightRenderer.sharedMaterial = _sharedMat;
            _lightRenderer.sortingOrder = 50;
            lightGo.transform.position = _board.Origin;
            lightGo.transform.localScale = new Vector3(_board.cellSize, _board.cellSize, 1f);

            var beamGo = new GameObject("Beam");
            beamGo.transform.SetParent(transform, false);
            _beam = beamGo.AddComponent<SpriteRenderer>();
            _beam.sprite = Spr("spr_beam");
            _beam.sharedMaterial = _sharedMat;
            _beam.color = new Color(lightColor.r, lightColor.g, lightColor.b, 0f);
            _beam.sortingOrder = 48;

            SetLight(null, "");
        }

        public void ClearLight()
        {
            _showing = false;
            foreach (var m in _markers) { m.Pop = 1f; m.Pulse = false; m.Target = m.IsTarget ? targetUnlit : forbiddenDark; }
        }

        /// <summary>Show the composited light for a projector (call after PROJECT).</summary>
        public void SetLight(CellGrid light, string _)
        {
            if (_lightMesh == null) return;
            _lightMesh.Clear();
            if (light != null && light.Any())
            {
                CellMeshBuilder.Build(light.LitCells(), _lightMesh);
                _showing = true;
                _reveal = 0f;
                _beamT = 0f;
            }
            else
            {
                _showing = false;
            }
        }

        public void BuildMarkers(IEnumerable<SensorSpec> sensors)
        {
            foreach (var m in _markers) if (m.Sr) DestroyImmediate(m.Sr.gameObject);
            _markers.Clear();

            foreach (var s in sensors)
            foreach (var cell in s.Cells)
            {
                bool target = s.Kind == ConditionKind.RequiredTarget;
                var go = new GameObject($"{s.Kind}_{s.Label}_{cell.x}_{cell.y}");
                go.transform.SetParent(transform, false);
                go.transform.position = _board.CellCenter(cell.x, cell.y) + new Vector3(0, 0, -0.05f);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = Spr(target ? "spr_disc" : "spr_ring");
                sr.sharedMaterial = _sharedMat;
                sr.sortingOrder = 62;
                float d = _board.cellSize * (target ? 0.55f : 0.7f);
                var native = sr.sprite.rect.size / sr.sprite.pixelsPerUnit;
                go.transform.localScale = new Vector3(d / native.x, d / native.y, 1f);
                sr.color = target ? targetUnlit : forbiddenDark;

                // Dynamic sensor identity follows the same data as the evaluator.
                var labelGo = new GameObject("SensorIdentity");
                labelGo.transform.SetParent(transform, false);
                labelGo.transform.position = _board.CellCenter(cell.x, cell.y) + new Vector3(0, 0, -0.12f);
                var text = labelGo.AddComponent<TMPro.TextMeshPro>();
                text.fontSize = 3;
                text.alignment = TMPro.TextAlignmentOptions.Center;
                text.color = new Color(0.98f, 0.97f, 0.9f);
                text.rectTransform.sizeDelta = new Vector2(_board.cellSize, _board.cellSize * 0.6f);
                text.text = s.RequiredFilter > 0 ? s.RequiredFilter.ToString()
                    : s.ProjectorId == "P" ? s.Label : $"{s.Label}\n<size=70%>{s.ProjectorId}</size>";
                text.GetComponent<MeshRenderer>().sortingOrder = 66;

                _markers.Add(new Marker
                {
                    Sr = sr, IsTarget = target, Label = s.Label, ProjectorId = s.ProjectorId,
                    Target = target ? targetUnlit : forbiddenDark, Pop = 1f
                });
            }
        }

        public void ApplyResult(ProjectionResult result, IReadOnlyList<SensorSpec> sensors)
        {
            foreach (var m in _markers)
            {
                var cond = result.Conditions.Find(c => c.Label == m.Label && c.ProjectorId == m.ProjectorId);
                bool passed = cond?.Passed ?? true;
                if (m.IsTarget)
                {
                    m.Target = passed ? targetLit : targetUnlit;
                    if (passed) m.Pop = 1.6f;
                }
                else
                {
                    m.Target = passed ? forbiddenDark : forbiddenExposed;
                    m.Pulse = !passed;
                }
            }
        }

        void Update()
        {
            float dt = Time.deltaTime;

            float revealTarget = _showing ? 1f : 0f;
            _reveal = Mathf.MoveTowards(_reveal, revealTarget, dt / 0.28f);
            if (_showing) _beamT = Mathf.Min(1f, _beamT + dt / 0.3f);
            else _beamT = Mathf.MoveTowards(_beamT, 0f, dt / 0.2f);

            if (_lightRenderer != null)
            {
                var c = lightColor; c.a = lightColor.a * _reveal;
                var b = _lightBlock;
                _lightRenderer.GetPropertyBlock(b);
                b.SetColor("_BaseColor", c); b.SetColor("_Color", c);
                _lightRenderer.SetPropertyBlock(b);
            }

            if (_beam != null)
            {
                float bw = _board.BoardWorldSize.x * 0.9f;
                float bh = _board.BoardWorldSize.y * (0.15f + 0.85f * _beamT);
                _beam.transform.position = _board.transform.position + new Vector3(0, _board.BoardWorldSize.y * 0.5f, 0.1f);
                var native = _beam.sprite.rect.size / _beam.sprite.pixelsPerUnit;
                _beam.transform.localScale = new Vector3(bw / native.x, -bh / native.y, 1f);
                var bc = lightColor; bc.a = 0.4f * _beamT * (_showing ? 1f : 0f);
                _beam.color = bc;
            }

            float t = Time.time;
            foreach (var m in _markers)
            {
                if (!m.Sr) continue;
                m.Pop = Mathf.Lerp(m.Pop, 1f, 1f - Mathf.Exp(-9f * dt));
                var col = m.Target;
                if (m.Pulse) col = Color.Lerp(forbiddenDark, forbiddenExposed, 0.5f + 0.5f * Mathf.Sin(t * 8f));
                m.Sr.color = Color.Lerp(m.Sr.color, col, 1f - Mathf.Exp(-10f * dt));
                float baseD = m.IsTarget ? 0.55f : 0.7f;
                var native = m.Sr.sprite.rect.size / m.Sr.sprite.pixelsPerUnit;
                float d = _board.cellSize * baseD * m.Pop;
                m.Sr.transform.localScale = new Vector3(d / native.x, d / native.y, 1f);
            }
        }

        void OnDestroy() { if (_lightMesh != null) Destroy(_lightMesh); }
    }
}
