using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StencilRelay.Game
{
    /// <summary>Bootstrap scene entry point: initialises services (via GameServices.Awake)
    /// then routes to the Light Studio.</summary>
    public sealed class BootstrapRunner : MonoBehaviour
    {
        [SerializeField] float minSplashSeconds = 0.15f;

        IEnumerator Start()
        {
            yield return null; // let GameServices.Awake run
            float t = 0f;
            while (t < minSplashSeconds) { t += Time.unscaledDeltaTime; yield return null; }
            SceneManager.LoadScene(GameServices.SceneLightStudio);
        }
    }
}
