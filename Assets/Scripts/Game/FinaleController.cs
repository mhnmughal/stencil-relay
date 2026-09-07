using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace StencilRelay.Game
{
    /// <summary>
    /// Plays MASTER ALIGNMENT COMPLETE once, when the Light Studio loads straight after
    /// Level 12 is completed: the three rack trays illuminate, a composed figure lights on
    /// the table, the calibration ruler glows, an achievement mark appears. Persisted via
    /// SaveSystem.MarkFinaleShown so later launches acknowledge completion without replaying.
    /// </summary>
    public sealed class FinaleController : MonoBehaviour
    {
        [SerializeField] LightStudioController studio;
        [SerializeField] GameObject panel;
        [SerializeField] CanvasGroup panelGroup;
        [SerializeField] TMP_Text title;
        [SerializeField] TMP_Text body;
        [SerializeField] TMP_Text mark;
        [SerializeField] Button reviewButton;
        [SerializeField] Button returnButton;

        void Awake()
        {
            if (studio == null) studio = FindFirstObjectByType<LightStudioController>();
            if (reviewButton) reviewButton.onClick.AddListener(() => { Dismiss(); if (studio) studio.OpenPatternRack(); });
            if (returnButton) returnButton.onClick.AddListener(Dismiss);
        }

        void Start()
        {
            bool play = GameServices.I != null && GameServices.I.JustCompletedFinale;
            if (!play)
            {
                if (panel) panel.SetActive(false);
                return;
            }
            GameServices.I.JustCompletedFinale = false;
            StartCoroutine(Play());
        }

        IEnumerator Play()
        {
            GameServices.I?.Audio?.Play("sfx_final_complete", 0.9f);
            if (studio) studio.SetMenuInteractable(false);
            if (panel) panel.SetActive(true);
            if (panelGroup) panelGroup.alpha = 0f;
            if (title) title.text = "";
            if (body) body.text = "";
            if (mark) mark.text = "";

            // fade the frosted overlay in
            for (float t = 0; t < 1f; t += Time.unscaledDeltaTime / 0.4f)
            {
                if (panelGroup) panelGroup.alpha = Mathf.Min(1f, t);
                yield return null;
            }
            if (panelGroup) panelGroup.alpha = 1f;

            // illuminate the rack sheets in sequence
            var rack = GameObject.Find("StencilRack");
            var sheets = rack != null
                ? rack.GetComponentsInChildren<SpriteRenderer>().Where(s => s.name.StartsWith("RackSheet")).ToArray()
                : new SpriteRenderer[0];
            var lit = new Color(0.62f, 0.91f, 0.94f, 0.55f);
            foreach (var s in sheets)
            {
                StartCoroutine(Glow(s, lit, 0.5f));
                yield return new WaitForSecondsRealtime(0.06f);
            }

            // ruler + table composed figure
            var ruler = GameObject.Find("BrassRuler")?.GetComponent<SpriteRenderer>();
            if (ruler != null) StartCoroutine(Glow(ruler, new Color(1f, 0.9f, 0.6f, 1f), 0.4f));

            yield return new WaitForSecondsRealtime(0.35f);
            if (title) title.text = "MASTER ALIGNMENT COMPLETE";
            yield return new WaitForSecondsRealtime(0.25f);
            if (body) body.text = "Every pattern registered. The studio is aligned.";
            yield return new WaitForSecondsRealtime(0.3f);
            if (mark) mark.text = "—  MASTER REGISTER  —";

            GameServices.I?.Save.MarkFinaleShown();
        }

        static IEnumerator Glow(SpriteRenderer sr, Color target, float dur)
        {
            var from = sr.color;
            for (float t = 0; t < dur; t += Time.unscaledDeltaTime)
            {
                sr.color = Color.Lerp(from, target, t / dur);
                yield return null;
            }
            sr.color = target;
        }

        void Dismiss()
        {
            if (panel) panel.SetActive(false);
            if (studio)
            {
                studio.SetMenuInteractable(true);
                studio.RefreshMenu();
            }
        }
    }
}
