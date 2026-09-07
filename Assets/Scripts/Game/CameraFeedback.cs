using UnityEngine;

namespace StencilRelay.Game
{
    /// <summary>
    /// Restrained camera feedback for the Projection Table: a tiny orthographic-size dip on
    /// a plate snap, a slightly longer settle on PERFECT REGISTER. No shake, no rotation.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class CameraFeedback : MonoBehaviour
    {
        Camera _cam;
        float _baseSize;
        float _kick;      // current dip amount
        float _decay = 8f;

        void Awake()
        {
            _cam = GetComponent<Camera>();
            _baseSize = _cam.orthographicSize;
        }

        void LateUpdate()
        {
            if (_kick > 0.0001f)
            {
                _kick = Mathf.Lerp(_kick, 0f, 1f - Mathf.Exp(-_decay * Time.unscaledDeltaTime));
                _cam.orthographicSize = _baseSize - _kick;
            }
            else if (!Mathf.Approximately(_cam.orthographicSize, _baseSize))
            {
                _cam.orthographicSize = _baseSize;
            }
        }

        public void Snap() { _kick = Mathf.Max(_kick, 0.06f); _decay = 10f; }
        public void Settle() { _kick = Mathf.Max(_kick, 0.14f); _decay = 4f; }
    }
}
