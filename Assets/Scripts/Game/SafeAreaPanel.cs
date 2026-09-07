using UnityEngine;

namespace StencilRelay.Game
{
    /// <summary>
    /// Insets a RectTransform to the device safe area (notch / Dynamic Island / rounded
    /// corners / home indicator). Required controls and readable text live under this;
    /// decorative studio art in the scene may extend past it.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaPanel : MonoBehaviour
    {
        RectTransform _rt;
        Rect _last;
        Vector2Int _lastRes;

        void Awake() => _rt = GetComponent<RectTransform>();
        void OnEnable() => Apply();
        void Update()
        {
            if (Screen.safeArea != _last || new Vector2Int(Screen.width, Screen.height) != _lastRes)
                Apply();
        }

        void Apply()
        {
            if (_rt == null) _rt = GetComponent<RectTransform>();
            var sa = Screen.safeArea;
            _last = sa;
            _lastRes = new Vector2Int(Screen.width, Screen.height);
            if (Screen.width <= 0 || Screen.height <= 0) return;

            Vector2 min = sa.position;
            Vector2 max = sa.position + sa.size;
            min.x /= Screen.width; min.y /= Screen.height;
            max.x /= Screen.width; max.y /= Screen.height;

            _rt.anchorMin = min;
            _rt.anchorMax = max;
            _rt.offsetMin = Vector2.zero;
            _rt.offsetMax = Vector2.zero;
        }
    }
}
