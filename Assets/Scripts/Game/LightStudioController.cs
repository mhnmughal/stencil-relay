using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StencilRelay.Levels;

namespace StencilRelay.Game
{
    /// <summary>
    /// Light Studio: the menu + Pattern Rack. Widgets are scene-authored; this only wires
    /// them. Rack tray buttons unlock in sequence from the save data.
    /// </summary>
    public sealed class LightStudioController : MonoBehaviour
    {
        [Header("Menu")]
        [SerializeField] Button beginButton;
        [SerializeField] TMP_Text beginLabel;
        [SerializeField] Button rackButton;
        [SerializeField] Button guideButton;
        [SerializeField] Button settingsButton;
        [SerializeField] TMP_Text completionMark;

        [Header("Pattern Rack")]
        [SerializeField] GameObject rackPanel;
        [SerializeField] Button rackCloseButton;
        [SerializeField] Button[] levelButtons = new Button[12];   // ids 1..12
        [SerializeField] TMP_Text[] levelLabels = new TMP_Text[12];

        [Header("Settings")]
        [SerializeField] GameObject settingsPanel;
        [SerializeField] Button settingsCloseButton;
        [SerializeField] Slider musicSlider;
        [SerializeField] Slider sfxSlider;
        [SerializeField] Toggle hapticsToggle;
        [SerializeField] TMP_Text musicValue;
        [SerializeField] TMP_Text sfxValue;
        [SerializeField] Button resetButton;
        [SerializeField] GameObject resetConfirmPanel;
        [SerializeField] Button resetConfirmYes;
        [SerializeField] Button resetConfirmNo;

        void Awake()
        {
            if (beginButton) beginButton.onClick.AddListener(OnBegin);
            if (rackButton) rackButton.onClick.AddListener(OpenPatternRack);
            if (rackCloseButton) rackCloseButton.onClick.AddListener(() => { Toggle(rackPanel, false); Beep(); });
            if (guideButton) guideButton.onClick.AddListener(() => Go(0));
            if (settingsButton) settingsButton.onClick.AddListener(() => { Toggle(settingsPanel, true); Beep(); });
            if (settingsCloseButton) settingsCloseButton.onClick.AddListener(() => { Toggle(settingsPanel, false); Beep(); });

            for (int i = 0; i < levelButtons.Length; i++)
            {
                int levelId = i + 1;
                if (levelButtons[i]) levelButtons[i].onClick.AddListener(() => Go(levelId));
            }

            if (musicSlider) musicSlider.onValueChanged.AddListener(_ => PushAudio());
            if (sfxSlider) sfxSlider.onValueChanged.AddListener(_ => { PushAudio(); Beep(); });
            if (hapticsToggle) hapticsToggle.onValueChanged.AddListener(v => { GameServices.I?.Save.SetHaptics(v); GameServices.I?.Audio?.Haptic(); });
            if (resetButton) resetButton.onClick.AddListener(() => Toggle(resetConfirmPanel, true));
            if (resetConfirmNo) resetConfirmNo.onClick.AddListener(() => Toggle(resetConfirmPanel, false));
            if (resetConfirmYes) resetConfirmYes.onClick.AddListener(OnResetConfirmed);
        }

        void Start()
        {
            Toggle(rackPanel, false);
            Toggle(settingsPanel, false);
            Toggle(resetConfirmPanel, false);
            RefreshFromSave();
            if (GameServices.I != null && GameServices.I.OpenRackOnArrival)
            {
                GameServices.I.OpenRackOnArrival = false;
                if (!GameServices.I.JustCompletedFinale) OpenPatternRack();
            }
        }

        static void Beep() => GameServices.I?.Audio?.PlayUi("sfx_settings", 0.6f);

        // ---- API for FinaleController ----
        public void OpenPatternRack()
        {
            Toggle(rackPanel, true);
            GameServices.I?.Audio?.PlayUi("sfx_rack_slide", 0.7f);
        }
        public void RefreshMenu() => RefreshFromSave();
        public void SetMenuInteractable(bool on)
        {
            foreach (var b in new[] { beginButton, rackButton, guideButton, settingsButton })
                if (b) b.interactable = on;
        }

        void RefreshFromSave()
        {
            var svc = GameServices.I;
            var data = svc != null ? svc.Save.Data : null;
            var db = svc != null ? svc.Levels : null;

            bool tutorialDone = data?.tutorialDone ?? false;
            if (beginLabel) beginLabel.text = tutorialDone ? $"Continue Pattern {data.highestUnlockedId:00}" : "Begin Calibration";
            if (completionMark) completionMark.text = (data?.finaleShown ?? false) ? "MASTER ALIGNMENT COMPLETE" : "";

            for (int i = 0; i < levelButtons.Length; i++)
            {
                int levelId = i + 1;
                bool unlocked = data != null && data.IsUnlocked(levelId);
                if (levelButtons[i]) levelButtons[i].interactable = unlocked;
                if (levelLabels[i])
                {
                    var def = db?.ById(levelId);
                    var rec = data?.GetRecord(levelId);
                    string title = def != null ? def.displayTitle : $"Pattern {levelId:00}";
                    string state = rec != null && rec.completed ? RatingLabel(rec.BestRating)
                        : unlocked ? "READY TO ALIGN" : "LOCKED";
                    levelLabels[i].text = $"<size=28>{levelId:00}</size>\n{title}\n<size=14>{state}</size>";
                }
            }

            if (musicSlider && data != null) musicSlider.SetValueWithoutNotify(data.musicVolume);
            if (sfxSlider && data != null) sfxSlider.SetValueWithoutNotify(data.sfxVolume);
            if (hapticsToggle && data != null) hapticsToggle.SetIsOnWithoutNotify(data.haptics);
            RefreshAudioLabels();
        }

        void OnBegin()
        {
            var svc = GameServices.I;
            if (svc == null) { Go(0); return; }
            if (!svc.Save.Data.tutorialDone) Go(0);
            else Go(Mathf.Max(1, svc.Save.Data.highestUnlockedId));
        }

        void Go(int levelId)
        {
            GameServices.I?.Audio?.PlayUi("sfx_rack_slide", 0.7f);
            GameServices.I?.GoToLevel(levelId);
        }

        void PushAudio()
        {
            var svc = GameServices.I;
            if (svc == null) return;
            float m = musicSlider ? musicSlider.value : svc.Save.Data.musicVolume;
            float s = sfxSlider ? sfxSlider.value : svc.Save.Data.sfxVolume;
            svc.Save.SetAudio(m, s);
            svc.Audio.ApplyVolumes(m, s);
            RefreshAudioLabels();
        }

        void RefreshAudioLabels()
        {
            if (musicValue && musicSlider) musicValue.text = $"{Mathf.RoundToInt(musicSlider.value * 100)}%";
            if (sfxValue && sfxSlider) sfxValue.text = $"{Mathf.RoundToInt(sfxSlider.value * 100)}%";
        }

        void OnResetConfirmed()
        {
            GameServices.I?.Save.ResetProgress();
            Toggle(resetConfirmPanel, false);
            Toggle(settingsPanel, false);
            RefreshFromSave();
            var svc = GameServices.I;
            if (svc != null) svc.Audio.ApplyVolumes(svc.Save.Data.musicVolume, svc.Save.Data.sfxVolume);
        }

        static void Toggle(GameObject go, bool on) { if (go) go.SetActive(on); }

        static string RatingLabel(Rating r) => r switch
        {
            Rating.PerfectRegister => "Perfect Register",
            Rating.Precise => "Precise",
            Rating.Aligned => "Aligned",
            _ => ""
        };
    }
}
