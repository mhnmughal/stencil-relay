using UnityEngine;
using UnityEngine.SceneManagement;
using StencilRelay.Levels;
using StencilRelay.Progression;

namespace StencilRelay.Game
{
    /// <summary>
    /// Persistent service container. Created in the Bootstrap scene, survives scene loads.
    /// Holds the save system, audio, and the level database, and carries the "which level
    /// to load next" hand-off between the Pattern Rack and the Projection Table.
    /// </summary>
    public sealed class GameServices : MonoBehaviour
    {
        public static GameServices I { get; private set; }

        [SerializeField] LevelDatabase levelDatabase;
        [SerializeField] AudioController audioController;

        /// <summary>Level database - serialized override, else the one in Resources/.</summary>
        public LevelDatabase Levels => levelDatabase != null
            ? levelDatabase
            : (levelDatabase = Resources.Load<LevelDatabase>("LevelDatabase"));
        public AudioController Audio => audioController;
        public SaveSystem Save { get; private set; }

        /// <summary>Level id the Projection Table should load on its next start.</summary>
        public int PendingLevelId { get; set; }

        /// <summary>Set true when Level 12 is completed so the Light Studio plays the finale once.</summary>
        public bool JustCompletedFinale { get; set; }
        public bool OpenRackOnArrival { get; set; }

        public const string SceneBootstrap = "Bootstrap";
        public const string SceneLightStudio = "LightStudio";
        public const string SceneProjectionTable = "ProjectionTable";

        void Awake()
        {
            if (I != null && I != this) { Destroy(gameObject); return; }
            I = this;
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            Save = new SaveSystem();

            if (audioController == null) audioController = GetComponentInChildren<AudioController>();
            if (audioController == null) audioController = gameObject.AddComponent<AudioController>();
            audioController.ApplyVolumes(Save.Data.musicVolume, Save.Data.sfxVolume);
            audioController.StartAmbience();
        }

        // ---- navigation ----

        public void GoToStudio()
        {
            Time.timeScale = 1f;
            if (Save.Data.GetRecord(SaveSystem.LastLevelId)?.completed == true && !Save.Data.finaleShown)
                JustCompletedFinale = true;
            SceneManager.LoadScene(SceneLightStudio);
        }

        public void GoToLevel(int levelId)
        {
            Time.timeScale = 1f;
            PendingLevelId = levelId;
            SceneManager.LoadScene(SceneProjectionTable);
        }

        /// <summary>Next level after the one just finished, or -1 if none / not unlocked.</summary>
        public int NextLevelId(int finishedId)
        {
            int next = finishedId + 1;
            if (next > SaveSystem.LastLevelId) return -1;
            return Save.IsUnlocked(next) ? next : -1;
        }
    }
}
