using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using StencilRelay.Core;
using StencilRelay.Levels;
using StencilRelay.Game;

namespace StencilRelay.Tests
{
    /// <summary>
    /// Smoke test for the playable slice: load the Projection Table scene, drive the
    /// controller through a solve, and confirm PROJECT reports success with the right rating.
    /// </summary>
    public class VerticalSliceSmokeTests
    {
        [UnityTest]
        public IEnumerator ProjectionTable_Loads_Level1_And_SolvesToPerfectRegister()
        {
            if (GameServices.I != null) GameServices.I.PendingLevelId = 1;
            yield return SceneManager.LoadSceneAsync("ProjectionTable", LoadSceneMode.Single);
            yield return null; // Start()
            yield return null;

            var controller = Object.FindFirstObjectByType<ProjectionTableController>();
            Assert.IsNotNull(controller, "ProjectionTableController missing from scene");
            Assert.IsNotNull(controller.Manipulation, "Controller did not load a level");
            Assert.AreEqual(1, controller.LevelId, "Should load level 1");

            var rt = controller.Runtime;
            Assert.AreEqual(1, rt.Plates.Count);

            // Level 1 optimal: slide plate A to (2,2).
            controller.SelectPlate(0);
            bool moved = controller.SlideSelectedTo(new Vector2Int(2, 2));
            Assert.IsTrue(moved, "Slide to optimal offset rejected");
            yield return null;

            var result = controller.Project();
            Assert.IsTrue(result.Success, "Projection should succeed at the optimal configuration");
            Assert.AreEqual(Rating.PerfectRegister, controller.Manipulation.RateSuccess());

            yield return null;
        }

        [UnityTest]
        public IEnumerator ProjectionTable_WrongConfiguration_ReportsMisalignedReason()
        {
            yield return SceneManager.LoadSceneAsync("ProjectionTable", LoadSceneMode.Single);
            yield return null;
            yield return null;

            var controller = Object.FindFirstObjectByType<ProjectionTableController>();
            Assert.IsNotNull(controller.Manipulation);

            var result = controller.Project(); // initial state - does not solve
            Assert.IsFalse(result.Success);
            CollectionAssert.IsNotEmpty(new System.Collections.Generic.List<string>(result.FailureReasons));

            controller.AdjustLayers();
            Assert.IsFalse(controller.Manipulation.Locked, "AdjustLayers should unlock manipulation");

            yield return null;
        }

        [UnityTest]
        public IEnumerator Level0_Calibration_IsTutorial_And_Solves_Via_SlideThenRotate()
        {
            if (GameServices.I != null) GameServices.I.PendingLevelId = 0;
            yield return SceneManager.LoadSceneAsync("ProjectionTable", LoadSceneMode.Single);
            yield return null;
            yield return null;

            var controller = Object.FindFirstObjectByType<ProjectionTableController>();
            Assert.AreEqual(0, controller.LevelId);
            Assert.IsTrue(controller.Runtime.IsTutorial, "Level 0 must be a tutorial");
            Assert.GreaterOrEqual(controller.Runtime.TutorialInstructions.Count, 3, "Tutorial steps missing from runtime");

            controller.SelectPlate(0);
            Assert.IsTrue(controller.SlideSelectedTo(new Vector2Int(2, 2)), "slide to (2,2) rejected");
            Assert.IsTrue(controller.RotateSelected(), "rotate rejected");   // -> CW90
            yield return null;

            var result = controller.Project();
            Assert.IsTrue(result.Success, "Calibration should solve after slide + one rotate");
            Assert.AreEqual(Rating.Aligned, controller.Manipulation.RateSuccess(), "tutorial rating is always Aligned");

            yield return null;
        }

        [UnityTest]
        public IEnumerator SaveSystem_RecordsTutorial_UnlocksLevel1()
        {
            yield return SceneManager.LoadSceneAsync("Bootstrap", LoadSceneMode.Single);
            yield return null;
            Assert.IsNotNull(GameServices.I);
            GameServices.I.Save.ResetProgress();
            Assert.IsFalse(GameServices.I.Save.Data.IsUnlocked(1));

            GameServices.I.Save.RecordCompletion(0, Rating.Aligned, 2);
            Assert.IsTrue(GameServices.I.Save.Data.tutorialDone);
            Assert.IsTrue(GameServices.I.Save.Data.IsUnlocked(1));
            Assert.AreEqual(1, GameServices.I.NextLevelId(0));

            GameServices.I.Save.ResetProgress();
            yield return null;
        }

        [UnityTest]
        public IEnumerator Finale_PlaysOnce_AfterLevel12_AndPersists()
        {
            yield return SceneManager.LoadSceneAsync("Bootstrap", LoadSceneMode.Single);
            yield return null;
            Assert.IsNotNull(GameServices.I);
            GameServices.I.Save.ResetProgress();
            Assert.IsFalse(GameServices.I.Save.Data.finaleShown);

            GameServices.I.JustCompletedFinale = true;
            yield return SceneManager.LoadSceneAsync("LightStudio", LoadSceneMode.Single);
            yield return null;

            var finale = Object.FindFirstObjectByType<FinaleController>();
            Assert.IsNotNull(finale, "FinaleController missing from LightStudio");

            // let the finale coroutine run (uses realtime waits, ~2s total)
            float timeout = 6f;
            while (!GameServices.I.Save.Data.finaleShown && timeout > 0f)
            {
                timeout -= Time.unscaledDeltaTime;
                yield return null;
            }
            Assert.IsTrue(GameServices.I.Save.Data.finaleShown, "Finale did not persist finaleShown");
            Assert.IsFalse(GameServices.I.JustCompletedFinale, "JustCompletedFinale flag not cleared");

            // reload: finale must NOT replay
            yield return SceneManager.LoadSceneAsync("LightStudio", LoadSceneMode.Single);
            yield return null;
            yield return null;
            var panel = GameObject.Find("FinalePanel");
            Assert.IsTrue(panel == null || !panel.activeInHierarchy, "Finale replayed on a later launch");

            GameServices.I.Save.ResetProgress();
            yield return null;
        }

        [UnityTest]
        public IEnumerator Audio_Mixer_And_Clips_Load_And_SettingsDriveMixer()
        {
            yield return SceneManager.LoadSceneAsync("Bootstrap", LoadSceneMode.Single);
            yield return null;
            Assert.IsNotNull(GameServices.I.Audio, "AudioController missing");

            var mixer = Resources.Load<UnityEngine.Audio.AudioMixer>("StencilRelayMixer");
            Assert.IsNotNull(mixer, "AudioMixer not in Resources");
            Assert.AreEqual(4, mixer.FindMatchingGroups("").Length, "expected Master + Music/SFX/UI groups");

            var snap = Resources.Load<AudioClip>("Audio/sfx_plate_snap");
            var amb = Resources.Load<AudioClip>("Audio/amb_room_tone");
            Assert.IsNotNull(snap, "sfx clip missing");
            Assert.IsNotNull(amb, "ambience clip missing");

            GameServices.I.Save.SetAudio(0.25f, 0.5f);
            GameServices.I.Audio.ApplyVolumes(0.25f, 0.5f);
            yield return null;
            Assert.IsTrue(mixer.GetFloat("MusicVol", out var mv));
            Assert.Less(mv, 0f, "MusicVol should be negative dB for a 0.25 linear level");
            Assert.IsTrue(mixer.GetFloat("SFXVol", out _));

            // firing a one-shot must not throw
            GameServices.I.Audio.Play("sfx_plate_snap");
            GameServices.I.Audio.PlayUi("sfx_settings");
            yield return null;

            GameServices.I.Save.ResetProgress();
            yield return null;
        }

        [UnityTest]
        public IEnumerator Bootstrap_Boots_Into_LightStudio_With_Services()
        {
            yield return SceneManager.LoadSceneAsync("Bootstrap", LoadSceneMode.Single);
            // BootstrapRunner waits then loads LightStudio
            float timeout = 5f;
            while (SceneManager.GetActiveScene().name != "LightStudio" && timeout > 0f)
            {
                timeout -= Time.unscaledDeltaTime;
                yield return null;
            }
            Assert.AreEqual("LightStudio", SceneManager.GetActiveScene().name, "Bootstrap did not route to LightStudio");
            Assert.IsNotNull(GameServices.I, "GameServices should persist");
            Assert.IsNotNull(GameServices.I.Save, "SaveSystem should be initialised");
            Assert.IsNotNull(GameServices.I.Levels, "LevelDatabase should be wired");

            yield return null;
            foreach (var m in new[] { "RackPanel", "SettingsPanel", "ResetConfirm", "FinalePanel" })
            {
                var go = GameObject.Find(m);
                Assert.IsTrue(go == null || !go.activeInHierarchy, $"{m} should be hidden on Light Studio start");
            }

            yield return null;
        }
    }
}
