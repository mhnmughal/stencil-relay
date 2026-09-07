using System.Collections;
using System.Linq;
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
    /// Drives every one of the 13 levels through the real Projection Table scene + the
    /// controller + manipulation path: load, confirm it does not start solved, drive each
    /// plate to its authored optimal via the actual slide/rotate/flip verbs, PROJECT,
    /// confirm PATTERN LOCKED, then reset and confirm the initial state returns.
    ///
    /// This is CONTROLLER-PATH verification, not human manual QA (LEVEL_QA.md).
    /// </summary>
    public class AllLevelsPlaythroughTests
    {
        /// <summary>Boot services, then wait for BootstrapRunner to finish routing to LightStudio
        /// so it can't replace a scene we load next.</summary>
        static IEnumerator BootAndSettle()
        {
            yield return SceneManager.LoadSceneAsync("Bootstrap", LoadSceneMode.Single);
            float t = 5f;
            while (SceneManager.GetActiveScene().name != "LightStudio" && t > 0f)
            {
                t -= Time.unscaledDeltaTime;
                yield return null;
            }
            yield return null;
        }

        static IEnumerator LoadLevel(int levelId)
        {
            GameServices.I.PendingLevelId = levelId;
            yield return SceneManager.LoadSceneAsync("ProjectionTable", LoadSceneMode.Single);
            yield return null;
            yield return null;
        }

        static void DriveTo(ProjectionTableController c, int plate, PlateTransform target)
        {
            c.SelectPlate(plate);
            var m = c.Manipulation;
            if (m.Plate(plate).Transform.Offset != target.Offset)
                m.TrySlide(plate, target.Offset);
            int guard = 8;
            while (m.Plate(plate).Transform.Rotation != target.Rotation && guard-- > 0)
                m.RotateStep(plate);
            if (m.Plate(plate).Transform.Flipped != target.Flipped)
                m.Flip(plate);
        }

        [UnityTest]
        public IEnumerator EveryLevel_LoadsSolvesResets_ThroughTheScene()
        {
            yield return BootAndSettle();
            Assert.IsNotNull(GameServices.I);
            GameServices.I.Save.ResetProgress();
            GameServices.I.Save.Data.highestUnlockedId = 12; // allow loading any level

            var db = GameServices.I.Levels;
            Assert.AreEqual(13, db.Ordered.Count(), "expected 13 levels");

            foreach (var def in db.Ordered.ToList())
            {
                yield return LoadLevel(def.levelId);

                var c = Object.FindFirstObjectByType<ProjectionTableController>();
                Assert.IsNotNull(c, $"L{def.levelId}: no controller");
                Assert.IsNotNull(c.Manipulation, $"L{def.levelId}: no manipulation");
                Assert.AreEqual(def.levelId, c.LevelId, $"wrong level loaded");

                // 1. does not start solved
                Assert.IsFalse(c.Manipulation.Runtime.Evaluate().Success,
                    $"L{def.levelId} '{def.displayTitle}' is already solved at load");

                // 2. wrong (initial) PROJECT -> MISALIGNED with a reason, then unlocks
                var bad = c.Project();
                Assert.IsFalse(bad.Success, $"L{def.levelId} initial PROJECT unexpectedly succeeded");
                Assert.IsNotEmpty(bad.FailureReasons.ToList(), $"L{def.levelId} MISALIGNED gave no reason");
                c.AdjustLayers();
                Assert.IsFalse(c.Manipulation.Locked, $"L{def.levelId} still locked after Adjust Layers");
                yield return null;

                // 3. drive every plate to its authored optimal via the real verbs, PROJECT
                for (int i = 0; i < def.plates.Count; i++)
                    DriveTo(c, i, def.plates[i].Optimal);
                yield return null;

                var good = c.Project();
                Assert.IsTrue(good.Success,
                    $"L{def.levelId} '{def.displayTitle}' optimal did not solve through the controller. " +
                    $"Reasons: {string.Join(", ", good.FailureReasons)}");
                Assert.AreNotEqual(Rating.None, c.Manipulation.RateSuccess(), $"L{def.levelId} no rating");
                yield return null;

                // 4. reset returns to the initial (unsolved) state
                c.AdjustLayers();
                c.ResetAll();
                Assert.AreEqual(0, c.Manipulation.MoveCount, $"L{def.levelId} moves not refunded on reset");
                Assert.IsFalse(c.Manipulation.Runtime.Evaluate().Success, $"L{def.levelId} solved after reset");
                yield return null;
            }

            GameServices.I.Save.ResetProgress();
            yield return null;
        }

        [UnityTest]
        public IEnumerator Completion_UnlocksNextLevel_ThroughHud()
        {
            yield return BootAndSettle();
            GameServices.I.Save.ResetProgress();
            GameServices.I.Save.MarkTutorialDone();          // L1 unlocked
            Assert.IsFalse(GameServices.I.Save.Data.IsUnlocked(2));

            yield return LoadLevel(1);

            var c = Object.FindFirstObjectByType<ProjectionTableController>();
            Assert.IsNotNull(c, "controller missing");
            var def = GameServices.I.Levels.ById(1);
            for (int i = 0; i < def.plates.Count; i++) DriveTo(c, i, def.plates[i].Optimal);
            Assert.IsTrue(c.Project().Success);

            // Success must persist before any result button is pressed.
            GameServices.I.Save.Load();
            Assert.IsTrue(GameServices.I.Save.Data.IsUnlocked(2), "completing L1 did not unlock L2");

            GameServices.I.Save.ResetProgress();
            yield return null;
        }

        [UnityTest]
        public IEnumerator SelectedLowerLayer_DragPreview_AndPause_AreStable()
        {
            yield return BootAndSettle();
            yield return LoadLevel(2);
            var c = Object.FindFirstObjectByType<ProjectionTableController>();
            c.SelectPlate(0);
            Assert.AreEqual(0, c.PickPlateAt(new Vector2Int(3, 3)), "selected lower layer must remain draggable");
            var target = c.Runtime.Optimal[0].Offset;
            c.SlideSelectedTo(target);
            c.PreviewDrag(0, target);
            var view = c.GetComponentsInChildren<StencilRelay.Game.View.PlateView>()[0];
            Assert.AreEqual(c.Board.Origin.x, view.transform.position.x, 0.001f, "zero-distance drag must not double the committed offset");
            Assert.AreEqual(c.Board.Origin.y, view.transform.position.y, 0.001f);
            c.IsPaused = true;
            Assert.IsNull(c.Project(), "pause must block projection");
            Assert.IsFalse(c.Manipulation.Locked);
            c.IsPaused = false;
            var first = c.Project();
            Assert.AreSame(first, c.Project(), "PROJECT spam must not re-evaluate or duplicate effects");
            c.AdjustLayers();
            Assert.IsNull(c.LastResult);
            Assert.IsFalse(c.HasResult);
            yield return null;
        }

        [UnityTest]
        public IEnumerator AuthoredUI_ResultRouting_PauseAndAudio_AgreeWithState()
        {
            yield return BootAndSettle();
            GameServices.I.Save.ResetProgress();
            yield return LoadLevel(1);
            var c = Object.FindFirstObjectByType<ProjectionTableController>();
            c.SelectPlate(0);
            Assert.IsFalse(GameObject.Find("RotateButton").GetComponent<UnityEngine.UI.Button>().interactable);
            GameObject.Find("PauseButton").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            yield return null;
            Canvas.ForceUpdateCanvases();
            Assert.IsTrue(c.IsPaused);
            Assert.IsTrue(Object.FindFirstObjectByType<PlateInput>().IsOverUi(new Vector2(Screen.width/2f, Screen.height/2f)));
            GameObject.Find("Resume").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            c.SlideSelectedTo(c.Runtime.Optimal[0].Offset);
            Assert.IsTrue(c.Project().Success);
            Assert.IsNull(GameObject.Find("ProjectButton"), "PROJECT must hide behind the inspect action");
            yield return new WaitForSeconds(0.9f);
            Assert.AreEqual("Next Pattern", GameObject.Find("ResultPrimary").GetComponentInChildren<TMPro.TMP_Text>().text);
            GameObject.Find("ResultRack").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            yield return null; yield return null;
            Assert.IsNotNull(GameObject.Find("RackPanel"), "Pattern Rack must open directly");
            Assert.IsTrue(GameServices.I.Save.Data.IsUnlocked(2));
            GameObject.Find("RackClose").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            GameObject.Find("SettingsButton").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            var slider = GameObject.Find("MusicSlider").GetComponent<UnityEngine.UI.Slider>();
            var mixer = Resources.Load<UnityEngine.Audio.AudioMixer>("StencilRelayMixer");
            foreach (float value in new[] { 0f, 0.25f, 0.5f, 1f })
            {
                slider.value = value;
                yield return null;
                Assert.AreEqual(value, slider.handleRect.anchorMin.x, 0.001f);
                Assert.AreEqual(value, slider.fillRect.anchorMax.x, 0.001f);
                Assert.AreEqual($"{Mathf.RoundToInt(value*100)}%", GameObject.Find("MusicValue").GetComponent<TMPro.TMP_Text>().text);
                Assert.AreEqual(value, GameServices.I.Save.Data.musicVolume);
                Assert.IsTrue(mixer.GetFloat("MusicVol", out float db));
                Assert.AreEqual(value == 0 ? -80f : 20f*Mathf.Log10(value), db, 0.01f);
            }
            GameServices.I.Save.ResetProgress();
        }
    }
}
