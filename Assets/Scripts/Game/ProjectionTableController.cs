using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using StencilRelay.Core;
using StencilRelay.Levels;
using StencilRelay.Game.View;

namespace StencilRelay.Game
{
    /// <summary>
    /// Owns one level attempt: builds the board / plate / projection views, routes player
    /// actions into a <see cref="LevelManipulation"/>, and runs the PROJECT flow. All puzzle
    /// logic lives in the pure model; this class only coordinates and keeps views in sync.
    /// </summary>
    public sealed class ProjectionTableController : MonoBehaviour
    {
        [Header("Bindings")]
        [SerializeField] BoardView board;
        [SerializeField] Transform plateRoot;
        [SerializeField] Material cellMaterial;
        [SerializeField] Light2D projectorSpot;   // small warm pool over the board on PROJECT
        [SerializeField] Light2D keyLight;        // dims on PROJECT so the pattern reads
        [SerializeField] CameraFeedback cameraFeedback;
        float _keyBase = 0.85f;
        int _lastMoveCount;

        [Header("Fallback (editor testing without GameServices)")]
        [SerializeField] LevelDatabase fallbackDatabase;
        [SerializeField] int fallbackLevelId;

        [Header("Plate tints (cycled per layer) - translucent acrylic on the lit table")]
        [SerializeField] Color[] plateTints =
        {
            new Color(0.90f, 0.93f, 0.93f, 0.20f),
            new Color(0.70f, 0.87f, 0.98f, 0.22f),
            new Color(0.98f, 0.90f, 0.68f, 0.22f),
            new Color(0.88f, 0.76f, 0.98f, 0.22f),
        };

        public LevelManipulation Manipulation { get; private set; }
        public LevelRuntime Runtime => Manipulation?.Runtime;
        public int LevelId { get; private set; }
        public bool HasResult { get; private set; }
        public bool IsPaused { get; set; }
        public ProjectionResult LastResult { get; private set; }

        /// <summary>Fired on any manipulation or projection change.</summary>
        public event Action StateChanged;
        /// <summary>Fired after PROJECT: (result, rating-if-success).</summary>
        public event Action<ProjectionResult, Rating> Projected;

        readonly List<PlateView> _plateViews = new List<PlateView>();
        readonly Dictionary<string, ProjectionView> _projViews = new Dictionary<string, ProjectionView>();
        StencilLevelDefinition _def;

        void Start()
        {
            if (Manipulation == null) LoadFromContextOrFallback();
        }

        void LoadFromContextOrFallback()
        {
            var db = GameServices.I != null ? GameServices.I.Levels
                   : (fallbackDatabase != null ? fallbackDatabase : Resources.Load<LevelDatabase>("LevelDatabase"));
            int id = GameServices.I != null ? GameServices.I.PendingLevelId : fallbackLevelId;
            if (db == null) { Debug.LogError("[ProjectionTable] No level database."); return; }
            var def = db.ById(id) ?? db.ByIndex(id);
            if (def == null) { Debug.LogError($"[ProjectionTable] No level {id}."); return; }
            Load(def);
        }

        /// <summary>Build everything for a level definition. Safe to call from tests.</summary>
        public void Load(StencilLevelDefinition def)
        {
            _def = def;
            LevelId = def.levelId;
            var rt = def.BuildRuntime();
            Manipulation = new LevelManipulation(rt);
            Manipulation.Changed += OnModelChanged;

            EnsureInfrastructure();

            board.width = rt.BoardW;
            board.height = rt.BoardH;
            board.cellSize = 7.2f / Mathf.Max(rt.BoardW, rt.BoardH);
            board.BuildGuide(cellMaterial);
            var authoredGuide = GameObject.Find("BoardGuide");
            if (authoredGuide != null) authoredGuide.SetActive(false);

            BuildPlateViews(rt);
            BuildProjectionViews(rt);

            HasResult = false;
            LastResult = null;
            _lastMoveCount = 0;
            SetProjectorGlow(false);
            RefreshViews();
            StateChanged?.Invoke();
        }

        void EnsureInfrastructure()
        {
            if (board == null) board = GetComponentInChildren<BoardView>();
            if (board == null) board = new GameObject("BoardView").AddComponent<BoardView>();
            if (board.transform.parent == null) board.transform.SetParent(transform, false);
            if (plateRoot == null)
            {
                plateRoot = new GameObject("Plates").transform;
                plateRoot.SetParent(transform, false);
            }
            if (cellMaterial == null)
            {
                var sh = Shader.Find("Sprites/Default") ?? Shader.Find("Universal Render Pipeline/Unlit");
                cellMaterial = new Material(sh) { name = "CellMaterial(runtime)" };
            }
            if (projectorSpot == null)
            {
                var go = GameObject.Find("ProjectorSpot");
                if (go != null) projectorSpot = go.GetComponent<Light2D>();
            }
            if (keyLight == null)
            {
                var go = GameObject.Find("KeyLight");
                if (go != null) keyLight = go.GetComponent<Light2D>();
            }
            if (keyLight != null) _keyBase = keyLight.intensity > 0f ? keyLight.intensity : _keyBase;
            if (cameraFeedback == null && Camera.main != null)
                cameraFeedback = Camera.main.GetComponent<CameraFeedback>();
        }

        void SetProjectorGlow(bool on)
        {
            // On PROJECT the room dims and a small warm pool lights the table, so the
            // cyan projected pattern (additive overlay) is the dominant signal (section 18).
            if (projectorSpot != null) projectorSpot.intensity = on ? 0.5f : 0f;
            if (keyLight != null) keyLight.intensity = on ? _keyBase * 0.4f : _keyBase;
        }

        void BuildPlateViews(LevelRuntime rt)
        {
            foreach (var pv in _plateViews) if (pv) Destroy(pv.gameObject);
            _plateViews.Clear();

            for (int i = 0; i < rt.Plates.Count; i++)
            {
                var go = new GameObject($"Plate_{rt.Plates[i].Id}");
                go.transform.SetParent(plateRoot, false);
                var view = go.AddComponent<PlateView>();
                var tint = plateTints.Length > 0 ? plateTints[i % plateTints.Length] : Color.gray;
                view.Bind(rt.Plates[i], board, cellMaterial, tint, sortingOrder: 10 + i);
                _plateViews.Add(view);
            }
        }

        void BuildProjectionViews(LevelRuntime rt)
        {
            foreach (var kv in _projViews) if (kv.Value) Destroy(kv.Value.gameObject);
            _projViews.Clear();

            foreach (var proj in rt.Projectors)
            {
                var go = new GameObject($"Projection_{proj.Id}");
                go.transform.SetParent(transform, false);
                var view = go.AddComponent<ProjectionView>();
                view.Init(board, cellMaterial);
                var sensorsForProj = rt.Sensors.FindAll(s => s.ProjectorId == proj.Id);
                view.BuildMarkers(sensorsForProj);
                _projViews[proj.Id] = view;
            }
        }

        // ---- picking / drag (called by input) ----

        public BoardView Board => board;

        /// <summary>Topmost plate whose footprint covers a board cell, or -1.</summary>
        public int PickPlateAt(Vector2Int cell)
        {
            if (Runtime == null) return -1;
            int selected = Manipulation.SelectedIndex;
            if (selected >= 0 && CoversCell(selected, cell)) return selected;
            for (int i = Runtime.Plates.Count - 1; i >= 0; i--)
            {
                if (CoversCell(i, cell)) return i;
            }
            return -1;
        }

        bool CoversCell(int i, Vector2Int cell)
        {
            var p = Runtime.Plates[i];
            return p.ResolveOpaque(Runtime.BoardW, Runtime.BoardH).Get(cell.x, cell.y) ||
                p.ResolveOpenOnBoard(Runtime.BoardW, Runtime.BoardH).Get(cell.x, cell.y);
        }

        public void CancelDrag()
        {
            foreach (var view in _plateViews) if (view) view.ClearDragOffset();
        }

        /// <summary>Move a plate's view for a live (uncommitted) drag, in grid-offset units.</summary>
        public void PreviewDrag(int plateIndex, Vector2 offsetGrid)
        {
            if (plateIndex < 0 || plateIndex >= _plateViews.Count || !_plateViews[plateIndex]) return;
            _plateViews[plateIndex].SetDragWorldOffset(
                new Vector3((offsetGrid.x - Manipulation.Plate(plateIndex).Transform.Offset.x) * board.cellSize,
                    (offsetGrid.y - Manipulation.Plate(plateIndex).Transform.Offset.y) * board.cellSize, -0.15f));
        }

        /// <summary>End a drag: snap to the nearest allowed offset and commit.</summary>
        public bool EndDrag(int plateIndex, Vector2 offsetGrid)
        {
            if (plateIndex < 0 || Manipulation == null) return false;
            _plateViews[plateIndex].ClearDragOffset();
            Manipulation.Select(plateIndex);
            bool moved = Manipulation.TrySlideToNearest(plateIndex, offsetGrid);
            if (moved) { Sfx?.Play("sfx_plate_slide", 0.6f); Sfx?.Play("sfx_plate_snap", 0.7f); Sfx?.Haptic(); }
            RefreshViews();
            return moved;
        }

        // ---- player actions (called by input / UI) ----

        static AudioController Sfx => GameServices.I != null ? GameServices.I.Audio : null;

        public void SelectPlate(int index)
        {
            if (Manipulation == null) return;
            int prev = Manipulation.SelectedIndex;
            Manipulation.Select(index);
            if (Manipulation.SelectedIndex != prev && Manipulation.SelectedIndex >= 0)
            {
                Sfx?.Play("sfx_plate_pickup", 0.7f);
                Sfx?.Haptic();
            }
        }

        public bool SlideSelectedToGrid(Vector2 gridCoord)
        {
            if (Manipulation == null || Manipulation.SelectedIndex < 0) return false;
            return Manipulation.TrySlideToNearest(Manipulation.SelectedIndex, gridCoord);
        }

        public bool SlideSelectedTo(Vector2Int offset)
        {
            if (Manipulation == null || Manipulation.SelectedIndex < 0) return false;
            return Manipulation.TrySlide(Manipulation.SelectedIndex, offset);
        }

        public bool RotateSelected()
        {
            bool ok = Manipulation != null && Manipulation.SelectedIndex >= 0 &&
                      Manipulation.RotateStep(Manipulation.SelectedIndex);
            if (ok) { Sfx?.Play("sfx_rotate", 0.8f); Sfx?.Play("sfx_pin_click", 0.5f); Sfx?.Haptic(); }
            return ok;
        }

        public bool FlipSelected()
        {
            bool ok = Manipulation != null && Manipulation.SelectedIndex >= 0 &&
                      Manipulation.Flip(Manipulation.SelectedIndex);
            if (ok) { Sfx?.Play("sfx_flip", 0.8f); Sfx?.Haptic(); }
            return ok;
        }

        public void ResetSelected()
        {
            if (Manipulation != null && Manipulation.SelectedIndex >= 0)
                Manipulation.ResetPlate(Manipulation.SelectedIndex);
        }

        public void ResetAll() => Manipulation?.ResetAll();

        public ProjectionResult Project()
        {
            if (Manipulation == null) return null;
            if (Manipulation.Locked || IsPaused) return LastResult;
            CancelDrag();
            var result = Manipulation.Project();
            LastResult = result;
            HasResult = true;

            Sfx?.Play("sfx_projector_switch", 0.8f);
            Sfx?.Play("sfx_projector_startup", 0.6f);
            Sfx?.Haptic();

            foreach (var proj in Runtime.Projectors)
                if (_projViews.TryGetValue(proj.Id, out var view))
                {
                    result.LightByProjector.TryGetValue(proj.Id, out var light);
                    view.SetLight(light, proj.Id);
                    view.ApplyResult(result, Runtime.Sensors.FindAll(s => s.ProjectorId == proj.Id));
                }

            SetProjectorGlow(true);
            var rating = result.Success ? Manipulation.RateSuccess() : Rating.None;

            if (result.Success)
            {
                GameServices.I?.Save.RecordCompletion(LevelId, rating, Manipulation.MoveCount);
                Sfx?.Play("sfx_target_activate", 0.7f);
                Sfx?.Play("sfx_success", 0.8f);
                if (rating == Rating.PerfectRegister)
                {
                    cameraFeedback?.Settle();
                    Sfx?.Play("sfx_pin_click", 0.6f);
                }
            }
            else
            {
                if (result.Conditions.Exists(c => !c.Passed && c.Kind == ConditionKind.ForbiddenSensor))
                    Sfx?.Play("sfx_forbidden", 0.8f);
                Sfx?.Play("sfx_misalign", 0.7f);
            }

            StateChanged?.Invoke();
            Projected?.Invoke(result, rating);
            return result;
        }

        /// <summary>Return to manipulation after a MISALIGNED result.</summary>
        public void AdjustLayers()
        {
            Manipulation?.Unlock();
            HasResult = false;
            LastResult = null;
            SetProjectorGlow(false);
            foreach (var v in _projViews.Values) if (v) v.ClearLight();
            RefreshViews();
            StateChanged?.Invoke();
        }

        // ---- view sync ----

        void OnModelChanged()
        {
            if (Manipulation != null && Manipulation.MoveCount > _lastMoveCount && cameraFeedback != null)
                cameraFeedback.Snap();
            if (Manipulation != null) _lastMoveCount = Manipulation.MoveCount;
            RefreshViews();
        }

        void RefreshViews()
        {
            for (int i = 0; i < _plateViews.Count; i++)
            {
                if (!_plateViews[i]) continue;
                _plateViews[i].Refresh();
                _plateViews[i].SetSelected(Manipulation != null && Manipulation.SelectedIndex == i);
            }
            if (Manipulation != null && !Manipulation.Locked)
                foreach (var v in _projViews.Values) if (v) v.ClearLight();
            StateChanged?.Invoke();
        }
    }
}
