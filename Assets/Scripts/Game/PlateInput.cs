using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using StencilRelay.Game.View;

namespace StencilRelay.Game
{
    /// <summary>
    /// Pointer (touch/mouse) manipulation of stencil plates: tap a plate to select, drag
    /// to slide. Release snaps to the nearest allowed grid offset. Rotate / flip / reset /
    /// PROJECT are UI controls, not gestures. Ignored while a projection result is showing.
    /// </summary>
    public sealed class PlateInput : MonoBehaviour
    {
        [SerializeField] ProjectionTableController controller;
        [SerializeField] Camera cam;

        int _dragPlate = -1;
        Vector2 _grabOffsetGrid;
        readonly List<RaycastResult> _uiHits = new List<RaycastResult>();

        void Reset() => cam = Camera.main;

        void Awake()
        {
            if (controller == null) controller = FindFirstObjectByType<ProjectionTableController>();
            if (cam == null) cam = Camera.main;
        }

        void Update()
        {
            var pointer = Pointer.current;
            if (pointer == null || controller == null || controller.Manipulation == null) return;
            if (controller.Manipulation.Locked || controller.IsPaused || Time.timeScale == 0f)
            { CancelDrag(); return; }

            var board = controller.Board;
            if (board == null) return;

            Vector3 world = ScreenToBoard(pointer.position.ReadValue());
            Vector2 grid = board.WorldToGrid(world);
            Vector2Int cell = new Vector2Int(Mathf.FloorToInt(grid.x + 0.5f), Mathf.FloorToInt(grid.y + 0.5f));

            if (pointer.press.wasPressedThisFrame)
            {
                if (IsOverUi(pointer.position.ReadValue())) return;
                int hit = controller.PickPlateAt(cell);
                if (hit >= 0)
                {
                    controller.SelectPlate(hit);
                    _dragPlate = hit;
                    var t = controller.Manipulation.Plate(hit).Transform;
                    _grabOffsetGrid = grid - new Vector2(t.Offset.x, t.Offset.y);
                }
            }
            else if (pointer.press.isPressed && _dragPlate >= 0)
            {
                controller.PreviewDrag(_dragPlate, grid - _grabOffsetGrid);
            }
            else if (pointer.press.wasReleasedThisFrame && _dragPlate >= 0)
            {
                controller.EndDrag(_dragPlate, grid - _grabOffsetGrid);
                _dragPlate = -1;
            }
        }

        public bool IsOverUi(Vector2 position)
        {
            if (EventSystem.current == null) return false;
            _uiHits.Clear();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = position }, _uiHits);
            return _uiHits.Count > 0;
        }

        void CancelDrag()
        {
            if (_dragPlate >= 0 && controller != null) controller.CancelDrag();
            _dragPlate = -1;
        }

        void OnDisable() => CancelDrag();
        void OnApplicationFocus(bool focused) { if (!focused) CancelDrag(); }

        Vector3 ScreenToBoard(Vector2 screen)
        {
            if (cam == null) return Vector3.zero;
            float dist = cam.orthographic ? Mathf.Abs(cam.transform.position.z) : Mathf.Abs(cam.transform.position.z);
            var w = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, dist));
            w.z = 0f;
            return w;
        }
    }
}
