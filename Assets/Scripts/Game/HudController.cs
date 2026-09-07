using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StencilRelay.Core;
using StencilRelay.Levels;
using StencilRelay.Progression;

namespace StencilRelay.Game
{
    /// <summary>
    /// Binds the scene-authored Projection Table HUD to the controller. Creates nothing -
    /// every widget is a serialized reference to an object that exists in the scene.
    /// </summary>
    public sealed class HudController : MonoBehaviour
    {
        [SerializeField] ProjectionTableController controller;

        [Header("Frame controls")]
        [SerializeField] Button[] layerTabs = new Button[4];
        [SerializeField] Button rotateButton;
        [SerializeField] Button flipButton;
        [SerializeField] Button resetButton;
        [SerializeField] Button projectButton;
        [SerializeField] TMP_Text movesText;
        [SerializeField] TMP_Text tutorialBanner;
        [SerializeField] TMP_Text levelTitle;
        [SerializeField] TMP_Text plateStatus;
        [SerializeField] Button inspectButton;
        [SerializeField] Button reconfigureButton;
        Coroutine _resultRoutine;

        [Header("Pause")]
        [SerializeField] Button pauseButton;
        [SerializeField] GameObject pausePanel;
        [SerializeField] Button resumeButton;
        [SerializeField] Button restartButton;
        [SerializeField] Button pauseGuideButton;
        [SerializeField] Button pauseStudioButton;

        [Header("Result")]
        [SerializeField] GameObject resultPanel;
        [SerializeField] TMP_Text resultTitle;
        [SerializeField] TMP_Text resultBody;
        [SerializeField] Button resultPrimaryButton;   // Adjust Layers / Next Pattern
        [SerializeField] TMP_Text resultPrimaryLabel;
        [SerializeField] Button resultRackButton;

        void Awake()
        {
            if (controller == null) controller = FindFirstObjectByType<ProjectionTableController>();

            for (int i = 0; i < layerTabs.Length; i++)
            {
                int idx = i;
                if (layerTabs[i] != null) layerTabs[i].onClick.AddListener(() => controller.SelectPlate(idx));
            }
            if (rotateButton) rotateButton.onClick.AddListener(() => controller.RotateSelected());
            if (flipButton) flipButton.onClick.AddListener(() => controller.FlipSelected());
            if (resetButton) resetButton.onClick.AddListener(() => controller.ResetSelected());
            if (projectButton) projectButton.onClick.AddListener(OnProject);

            if (pauseButton) pauseButton.onClick.AddListener(() => SetPause(true));
            if (resumeButton) resumeButton.onClick.AddListener(() => SetPause(false));
            if (restartButton) restartButton.onClick.AddListener(OnRestart);
            if (pauseGuideButton) pauseGuideButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                GameServices.I?.GoToLevel(0);
            });
            if (pauseStudioButton) pauseStudioButton.onClick.AddListener(GoStudio);

            if (resultPrimaryButton) resultPrimaryButton.onClick.AddListener(OnResultPrimary);
            if (resultRackButton) resultRackButton.onClick.AddListener(() =>
            {
                if (GameServices.I != null) GameServices.I.OpenRackOnArrival = true;
                GoStudio();
            });
            if (inspectButton) inspectButton.onClick.AddListener(() => resultPanel.SetActive(false));
            if (reconfigureButton) reconfigureButton.onClick.AddListener(() => controller.AdjustLayers());
        }

        void OnEnable()
        {
            if (controller != null)
            {
                controller.StateChanged += Redraw;
                controller.Projected += OnProjected;
            }
        }

        void OnDisable()
        {
            if (controller != null)
            {
                controller.StateChanged -= Redraw;
                controller.Projected -= OnProjected;
            }
        }

        void Start()
        {
            if (pausePanel) pausePanel.SetActive(false);
            if (resultPanel) resultPanel.SetActive(false);
            Redraw();
        }

        void OnProject()
        {
            if (controller.Manipulation == null || controller.Manipulation.Locked) return;
            controller.Project();
        }

        void OnProjected(ProjectionResult result, Rating rating)
        {
            if (resultPanel == null) return;
            if (_resultRoutine != null) StopCoroutine(_resultRoutine);
            _resultRoutine = StartCoroutine(ShowResult(result, rating));
        }

        System.Collections.IEnumerator ShowResult(ProjectionResult result, Rating rating)
        {
            yield return new WaitForSeconds(0.65f); // let the player read the light and sensor response
            if (!controller.HasResult) yield break;
            resultPanel.SetActive(true);
            StartCoroutine(FadeIn(resultPanel));
            if (resultTitle) resultTitle.color = result.Success
                ? new Color(0.66f, 0.79f, 0.64f) : new Color(0.89f, 0.47f, 0.37f);

            if (result.Success)
            {
                bool tutorial = controller.Runtime.IsTutorial;
                if (resultTitle) resultTitle.text = tutorial ? "CALIBRATION COMPLETE" : "PATTERN LOCKED";
                if (resultBody) resultBody.text = tutorial ? "Light registered. Your first pattern is ready."
                    : $"{RatingLabel(rating)}\n{controller.Manipulation.MoveCount} moves  /  par {controller.Runtime.MovePar}";
                if (resultPrimaryLabel)
                    resultPrimaryLabel.text = tutorial ? "Begin Pattern 01"
                        : NextExists() ? "Next Pattern" : "Return to Studio";
            }
            else
            {
                if (resultTitle) resultTitle.text = "MISALIGNED";
                if (resultBody) resultBody.text = string.Join("\n", result.FailureReasons.DefaultIfEmpty("Projection Incomplete"));
                if (resultPrimaryLabel) resultPrimaryLabel.text = "Adjust Layers";
            }
        }

        void OnResultPrimary()
        {
            if (resultPanel) resultPanel.SetActive(false);

            if (controller.LastResult != null && controller.LastResult.Success)
            {
                RecordAndAdvance();
            }
            else
            {
                controller.AdjustLayers();
            }
        }

        void RecordAndAdvance()
        {
            var svc = GameServices.I;

            if (svc != null && controller.LevelId == SaveSystem.LastLevelId && !svc.Save.Data.finaleShown)
            {
                svc.JustCompletedFinale = true;
                GoStudio();
                return;
            }

            int next = svc != null ? svc.NextLevelId(controller.LevelId) : -1;
            if (next >= 0) svc.GoToLevel(next);
            else GoStudio();
        }

        bool NextExists()
            => GameServices.I != null && GameServices.I.NextLevelId(controller.LevelId) >= 0;

        void OnRestart()
        {
            SetPause(false);
            if (resultPanel) resultPanel.SetActive(false);
            controller.AdjustLayers();
            controller.ResetAll();   // Redraw() → UpdateTutorial() resets the banner
        }

        void GoStudio()
        {
            Time.timeScale = 1f;
            if (GameServices.I != null) GameServices.I.GoToStudio();
        }

        void SetPause(bool paused)
        {
            controller.IsPaused = paused;
            controller.CancelDrag();
            if (pausePanel) pausePanel.SetActive(paused);
            Time.timeScale = paused ? 0f : 1f;
            GameServices.I?.Audio?.PlayUi("sfx_pause", 0.7f);
        }

        void Redraw()
        {
            var m = controller != null ? controller.Manipulation : null;
            if (m == null) return;

            if (movesText != null)
                movesText.text = m.Runtime.IsTutorial ? "CALIBRATION" : $"MOVES {m.MoveCount:00}  /  PAR {m.Runtime.MovePar:00}";
            if (levelTitle) levelTitle.text = $"{controller.LevelId:00}  /  {m.Runtime.Title.ToUpperInvariant()}";
            if (plateStatus) plateStatus.text = m.SelectedIndex < 0 ? "SELECT A LAYER"
                : $"LAYER {m.Plate(m.SelectedIndex).Id}  /  {(m.Plate(m.SelectedIndex).Transform.Flipped ? "BACK" : "FRONT")}"
                    + (m.Runtime.ParallaxPerDepth != 0 ? $"  /  SHIFT +{m.Runtime.ParallaxShiftFor(m.SelectedIndex).x}" : "");
            if (reconfigureButton) reconfigureButton.gameObject.SetActive(m.Locked);
            if (!controller.HasResult && resultPanel) resultPanel.SetActive(false);

            bool interactable = !m.Locked;
            if (rotateButton) rotateButton.interactable = interactable && m.SelectedIndex >= 0
                && m.Permissions(m.SelectedIndex).AllowedRotations.Count > 1;
            if (flipButton) flipButton.interactable = interactable && m.SelectedIndex >= 0
                && m.Permissions(m.SelectedIndex).FlipAllowed;
            if (resetButton) resetButton.interactable = interactable && m.SelectedIndex >= 0;
            if (projectButton) { projectButton.interactable = interactable; projectButton.gameObject.SetActive(interactable); }
            foreach (var b in new[] { rotateButton, flipButton, resetButton })
                if (b) b.GetComponentInChildren<TMP_Text>().color = b.interactable
                    ? new Color(0.95f, 0.92f, 0.87f) : new Color(0.47f, 0.53f, 0.54f);

            int plateCount = m.Runtime.Plates.Count;
            for (int i = 0; i < layerTabs.Length; i++)
            {
                if (layerTabs[i] == null) continue;
                layerTabs[i].gameObject.SetActive(i < plateCount);
                layerTabs[i].interactable = interactable;
                var label = layerTabs[i].GetComponentInChildren<TMP_Text>();
                if (label && i < plateCount) label.text = $"{m.Plate(i).Id}\n<size=14>{(m.Plate(i).Transform.Flipped ? "BACK" : "FRONT")}</size>";
                var colors = layerTabs[i].colors;
                colors.normalColor = (m.SelectedIndex == i)
                    ? new Color(0.66f, 0.91f, 0.94f, 1f)
                    : new Color(0.85f, 0.85f, 0.82f, 0.5f);
                layerTabs[i].colors = colors;
            }

            UpdateTutorial(m);
        }

        /// <summary>
        /// Tutorial banner. Steps are authored in action order (select, slide, rotate,
        /// project); progress = 0 before a plate is picked, then 1 + committed moves.
        /// Works for the single Level 0 tutorial without per-action bookkeeping.
        /// </summary>
        void UpdateTutorial(LevelManipulation m)
        {
            if (tutorialBanner == null) return;
            var rt = m.Runtime;
            if (rt == null || !rt.IsTutorial || rt.TutorialInstructions.Count == 0)
            {
                tutorialBanner.text = rt.Sensors.Any(s => s.RequiredFilter > 0)
                    ? "Match numbered filter openings to their numbered targets."
                    : rt.Projectors.Count > 1 ? "One stack, two projectors. Light every target; keep rings dark."
                    : rt.ParallaxPerDepth != 0 ? "Depth shifts the openings. Check each layer's shift before projecting."
                    : rt.Sensors.Any(s => s.Kind == ConditionKind.ForbiddenSensor)
                        ? "Light the solid targets. Keep the outlined rings dark."
                        : "Align the openings to light every solid target.";
                return;
            }

            if (controller.HasResult && controller.LastResult != null && controller.LastResult.Success)
            {
                tutorialBanner.text = "CALIBRATION COMPLETE";
                return;
            }

            var current = m.Plate(0).Transform;
            int progress = m.SelectedIndex < 0 ? 0 : current.Offset != rt.Optimal[0].Offset ? 1
                : current.Rotation != rt.Optimal[0].Rotation ? 2 : 3;
            int idx = Mathf.Clamp(progress, 0, rt.TutorialInstructions.Count - 1);
            tutorialBanner.text = rt.TutorialInstructions[idx];
        }

        static System.Collections.IEnumerator FadeIn(GameObject go)
        {
            var cg = go.GetComponent<CanvasGroup>();
            if (cg == null) cg = go.AddComponent<CanvasGroup>();
            var rt = go.transform as RectTransform;
            for (float t = 0; t < 1f; t += Time.unscaledDeltaTime / 0.18f)
            {
                cg.alpha = Mathf.Min(1f, t);
                yield return null;
            }
            cg.alpha = 1f;
            if (rt) rt.localScale = Vector3.one;
        }

        static string RatingLabel(Rating r) => r switch
        {
            Rating.PerfectRegister => "PERFECT REGISTER",
            Rating.Precise => "PRECISE",
            Rating.Aligned => "ALIGNED",
            _ => ""
        };
    }
}
