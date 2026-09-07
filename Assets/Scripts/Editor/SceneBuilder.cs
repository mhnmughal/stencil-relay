using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using StencilRelay.Game;
using StencilRelay.Game.View;
using StencilRelay.Levels;

namespace StencilRelay.Editor
{
    /// <summary>
    /// Reproducibly authors the three production scenes (Bootstrap, LightStudio,
    /// ProjectionTable) as real saved .unity files with a full, hand-editable hierarchy,
    /// wires the serialized references, and composes the optical-studio environment via
    /// <see cref="EnvironmentBuilder"/> (soft-shape sprites + role materials + URP 2D lights).
    ///
    /// Menu: Stencil Relay/Rebuild Scenes  (run Rebuild Materials + Rebuild Sprites first)
    /// </summary>
    public static class SceneBuilder
    {
        const string ScenesDir = "Assets/Scenes";
        static readonly Color Ivory = new Color(0.949f, 0.922f, 0.867f);
        static readonly Color BlueBlack = new Color(0.055f, 0.078f, 0.094f);
        static readonly Color Brass = new Color(0.690f, 0.553f, 0.341f);
        static readonly Color Sage = new Color(0.663f, 0.788f, 0.643f);

        [MenuItem("Stencil Relay/Rebuild Scenes")]
        public static void RebuildAll()
        {
            Directory.CreateDirectory(ScenesDir);
            var db = FindDatabase();

            BuildBootstrap(db);
            BuildLightStudio(db);
            BuildProjectionTable(db);

            // Build settings: Bootstrap, LightStudio, ProjectionTable, in order.
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene($"{ScenesDir}/Bootstrap.unity", true),
                new EditorBuildSettingsScene($"{ScenesDir}/LightStudio.unity", true),
                new EditorBuildSettingsScene($"{ScenesDir}/ProjectionTable.unity", true),
            };

            // Retire the default sample scene.
            var sample = $"{ScenesDir}/SampleScene.unity";
            if (File.Exists(sample)) AssetDatabase.DeleteAsset(sample);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene($"{ScenesDir}/Bootstrap.unity");
            Debug.Log("[SceneBuilder] Rebuilt Bootstrap / LightStudio / ProjectionTable and set Build Settings.");
        }

        static LevelDatabase FindDatabase()
        {
            var guids = AssetDatabase.FindAssets("t:LevelDatabase");
            return guids.Length > 0
                ? AssetDatabase.LoadAssetAtPath<LevelDatabase>(AssetDatabase.GUIDToAssetPath(guids[0]))
                : null;
        }

        static Material CellMaterial()
            => AssetDatabase.LoadAssetAtPath<Material>("Assets/Content/Materials/M_Cell.mat");

        // ---------------------------------------------------------------- Bootstrap

        static void BuildBootstrap(LevelDatabase db)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            AddCamera("Camera", BlueBlack);

            var services = new GameObject("GameServices");
            var audio = services.AddComponent<AudioController>();
            var gs = services.AddComponent<GameServices>();
            var so = new SerializedObject(gs);
            so.FindProperty("levelDatabase").objectReferenceValue = db;
            so.FindProperty("audioController").objectReferenceValue = audio;
            so.ApplyModifiedPropertiesWithoutUndo();

            var runner = new GameObject("BootstrapRunner");
            runner.AddComponent<BootstrapRunner>();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, $"{ScenesDir}/Bootstrap.unity");
        }

        // ---------------------------------------------------------------- Light Studio

        static void BuildLightStudio(LevelDatabase db)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            AddCamera("Camera", BlueBlack);
            EnsureEventSystem();

            EnvironmentBuilder.DecorateLightStudio(null);

            var canvas = MakeCanvas("StudioCanvas");
            var studioGo = new GameObject("LightStudioController");
            var studio = studioGo.AddComponent<LightStudioController>();
            var so = new SerializedObject(studio);

            // Menu: an engraved plaque along the left of the light table (near the lamp/rack)
            var menuSafe = NewUi("MenuSafe", canvas.transform);
            Stretch(menuSafe);
            menuSafe.AddComponent<StencilRelay.Game.SafeAreaPanel>();

            var plaque = NewUi("MenuPlaque", menuSafe.transform);
            Anchor(RT(plaque, new Vector2(60, 0), new Vector2(360, 560)).gameObject, new Vector2(0f, 0.5f));
            var plImg = plaque.AddComponent<Image>();
            var softMenu = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Content/Resources/Sprites/spr_soft_rect.png");
            if (softMenu != null) { plImg.sprite = softMenu; plImg.type = Image.Type.Sliced; }
            plImg.color = new Color(0.055f, 0.078f, 0.094f, 0.86f);

            // An authored arrangement of optical sheets fills the drafting surface.
            var specimen = NewUi("StudioSpecimen", menuSafe.transform);
            RT(specimen, new Vector2(0, 140), new Vector2(480, 430));
            for (int i = 0; i < 3; i++)
            {
                var sheet = UiPanel(specimen, $"StencilSheet{i}", new Vector2((i - 1) * 65, (i - 1) * 35), new Vector2(290, 290));
                sheet.transform.localRotation = Quaternion.Euler(0, 0, -18 + i * 18);
                var image = sheet.GetComponent<Image>();
                image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Content/Resources/Sprites/spr_glass.png");
                image.color = i == 1 ? new Color(0.4f, 0.66f, 0.69f, 0.7f) : new Color(0.25f, 0.34f, 0.35f, 0.65f);
                image.raycastTarget = false;
                var opening = UiPanel(sheet, "Opening", Vector2.zero, new Vector2(88, 88));
                opening.GetComponent<Image>().color = new Color(0.93f, 0.96f, 0.83f, 0.8f);
                opening.GetComponent<Image>().raycastTarget = false;
            }
            UiText(specimen, "SpecimenCaption", "THREE LAYERS. ONE CLEAR PATTERN.", new Vector2(0, -250), new Vector2(660, 44), 22, TextAlignmentOptions.Center, BlueBlack);
            UiText(specimen, "SpecimenActions", "SLIDE   /   ROTATE   /   FLIP   /   PROJECT", new Vector2(0, -292), new Vector2(660, 36), 16, TextAlignmentOptions.Center, BlueBlack);

            UiText(plaque, "Title", "STENCIL\nRELAY", new Vector2(0, 210), new Vector2(320, 120), 40, TextAlignmentOptions.Center, Ivory);
            var begin = UiButton(plaque, "BeginButton", "Calibration", new Vector2(0, 70), new Vector2(300, 66));
            var beginLabel = begin.GetComponentInChildren<TMP_Text>();
            ((Image)begin.targetGraphic).color = new Color(0.38f, 0.55f, 0.53f);
            var rack = UiButton(plaque, "RackButton", "Pattern Rack", new Vector2(0, -6), new Vector2(300, 66));
            var guide = UiButton(plaque, "GuideButton", "Calibration Guide", new Vector2(0, -82), new Vector2(300, 66));
            var settings = UiButton(plaque, "SettingsButton", "Light Settings", new Vector2(0, -158), new Vector2(300, 66));
            var completion = UiText(plaque, "CompletionMark", "", new Vector2(0, -232), new Vector2(320, 44), 18, TextAlignmentOptions.Center, Sage);

            // ---- Pattern Rack modal ----
            var (rackPanel, rackC) = UiModal(canvas, "RackPanel", new Vector2(1200, 760), frosted: false);
            UiText(rackC, "RackTitle", "PATTERN RACK", new Vector2(0, 300), new Vector2(1000, 60), 36, TextAlignmentOptions.Center, Ivory);
            var rackClose = UiButton(rackC, "RackClose", "Close", new Vector2(500, 310), new Vector2(150, 72));
            var levelButtons = new Button[12];
            var levelLabels = new TMP_Text[12];
            string[] trays = { "TRAY A  ·  ALIGNMENT", "TRAY B  ·  OCCLUSION", "TRAY C  ·  PROJECTION" };
            for (int tray = 0; tray < 3; tray++)
            {
                float x = -390 + tray * 390;
                var trayPanel = UiPanel(rackC, $"Tray{tray}", new Vector2(x, -25), new Vector2(360, 590));
                trayPanel.GetComponent<Image>().color = new Color(0.12f, 0.17f, 0.18f);
                UiText(trayPanel, "TrayTitle", trays[tray], new Vector2(0, 254), new Vector2(340, 40), 18, TextAlignmentOptions.Center, Brass);
                for (int col = 0; col < 4; col++)
                {
                    int id = tray * 4 + col + 1;
                    var b = UiButton(trayPanel, $"Level{id:00}", $"{id:00}", new Vector2(0, 160 - col * 120), new Vector2(320, 108));
                    ((Image)b.targetGraphic).sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Content/Resources/Sprites/spr_glass.png");
                    ((Image)b.targetGraphic).color = new Color(0.30f, 0.43f, 0.44f, 0.95f);
                    levelButtons[id - 1] = b;
                    levelLabels[id - 1] = b.GetComponentInChildren<TMP_Text>();
                    levelLabels[id - 1].fontSize = 18;
                }
            }

            // ---- Settings modal ----
            var (setPanel, setC) = UiModal(canvas, "SettingsPanel", new Vector2(880, 620), frosted: false);
            UiText(setC, "SetTitle", "LIGHT SETTINGS", new Vector2(-80, 250), new Vector2(550, 50), 30, TextAlignmentOptions.Center, Ivory);
            var setClose = UiButton(setC, "SettingsClose", "Close", new Vector2(340, 250), new Vector2(130, 72));
            UiText(setC, "MusicLbl", "Music", new Vector2(-290, 130), new Vector2(220, 40), 22, TextAlignmentOptions.Left, Ivory);
            var music = UiSlider(setC, "MusicSlider", new Vector2(80, 130), new Vector2(400, 30));
            UiText(setC, "SfxLbl", "Sound", new Vector2(-290, 55), new Vector2(220, 40), 22, TextAlignmentOptions.Left, Ivory);
            var sfx = UiSlider(setC, "SfxSlider", new Vector2(80, 55), new Vector2(400, 30));
            var musicValue = UiText(setC, "MusicValue", "80%", new Vector2(350, 130), new Vector2(90, 44), 22, TextAlignmentOptions.Right, Brass);
            var sfxValue = UiText(setC, "SoundValue", "80%", new Vector2(350, 55), new Vector2(90, 44), 22, TextAlignmentOptions.Right, Brass);
            var haptics = UiToggle(setC, "HapticsToggle", "Haptics", new Vector2(-160, -30), new Vector2(400, 40));
            var reset = UiButton(setC, "ResetButton", "Reset Pattern History", new Vector2(0, -130), new Vector2(400, 62));
            UiText(setC, "Credits", "Stencil Relay  /  Rehman Mughal\nAll patterns, sounds and progress stay on this device.", new Vector2(0, -230), new Vector2(760, 64), 18, TextAlignmentOptions.Center, Brass);

            // ---- Reset-confirm modal (top-level, over Settings) ----
            var (confirm, confirmC) = UiModal(canvas, "ResetConfirm", new Vector2(620, 340), frosted: false);
            UiText(confirmC, "ConfirmText", "RESET PATTERN HISTORY?\n\nAll completed patterns and ratings will be removed.", new Vector2(0, 60), new Vector2(560, 160), 20, TextAlignmentOptions.Center, Ivory);
            var yes = UiButton(confirmC, "ConfirmYes", "Reset", new Vector2(-120, -90), new Vector2(200, 58));
            var no = UiButton(confirmC, "ConfirmNo", "Cancel", new Vector2(120, -90), new Vector2(200, 58));

            void Set(string p, Object v) => so.FindProperty(p).objectReferenceValue = v;
            Set("beginButton", begin); Set("beginLabel", beginLabel);
            Set("rackButton", rack); Set("guideButton", guide); Set("settingsButton", settings);
            Set("completionMark", completion);
            Set("rackPanel", rackPanel); Set("rackCloseButton", rackClose);
            Set("settingsPanel", setPanel); Set("settingsCloseButton", setClose);
            Set("musicSlider", music); Set("sfxSlider", sfx); Set("hapticsToggle", haptics);
            Set("musicValue", musicValue); Set("sfxValue", sfxValue);
            Set("resetButton", reset); Set("resetConfirmPanel", confirm);
            Set("resetConfirmYes", yes); Set("resetConfirmNo", no);
            SetArray(so, "levelButtons", levelButtons);
            SetArray(so, "levelLabels", levelLabels);
            so.ApplyModifiedPropertiesWithoutUndo();

            // modals start hidden (no first-frame flash before the controller's Start())
            rackPanel.SetActive(false);
            setPanel.SetActive(false);
            confirm.SetActive(false);

            // ---- Finale overlay (MASTER ALIGNMENT COMPLETE) ----
            var finaleGo = new GameObject("FinaleController");
            var finale = finaleGo.AddComponent<FinaleController>();
            var fso = new SerializedObject(finale);

            var (fPanel, fC) = UiModal(canvas, "FinalePanel", new Vector2(1300, 620), frosted: false);
            var fGroup = fPanel.AddComponent<CanvasGroup>();

            var fTitle = UiText(fC, "FinaleTitle", "", new Vector2(0, 170), new Vector2(1200, 90), 50, TextAlignmentOptions.Center, Sage);
            var fBody = UiText(fC, "FinaleBody", "", new Vector2(0, 50), new Vector2(1080, 90), 24, TextAlignmentOptions.Center, Ivory);
            var fMark = UiText(fC, "FinaleMark", "", new Vector2(0, -50), new Vector2(1000, 60), 28, TextAlignmentOptions.Center, Brass);
            var fReview = UiButton(fC, "FinaleReview", "Review Patterns", new Vector2(-180, -170), new Vector2(320, 66));
            var fReturn = UiButton(fC, "FinaleReturn", "Return to Studio", new Vector2(180, -170), new Vector2(320, 66));

            void F(string p, Object v) => fso.FindProperty(p).objectReferenceValue = v;
            F("studio", studio); F("panel", fPanel); F("panelGroup", fGroup);
            F("title", fTitle); F("body", fBody); F("mark", fMark);
            F("reviewButton", fReview); F("returnButton", fReturn);
            fso.ApplyModifiedPropertiesWithoutUndo();
            fPanel.SetActive(false);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, $"{ScenesDir}/LightStudio.unity");
        }

        // ---------------------------------------------------------------- Projection Table

        static void BuildProjectionTable(LevelDatabase db)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var cam = AddCamera("Camera", BlueBlack);
            cam.orthographic = true;
            cam.orthographicSize = 5.2f;
            cam.transform.position = new Vector3(0, 0, -10);
            cam.gameObject.AddComponent<StencilRelay.Game.CameraFeedback>();
            EnsureEventSystem();

            var tableRoot = new GameObject("ProjectionTable");
            var board = new GameObject("BoardView").AddComponent<BoardView>();
            board.transform.SetParent(tableRoot.transform, false);
            board.width = 6; board.height = 6; board.cellSize = 1.2f;

            EnvironmentBuilder.DecorateProjectionTable(null, board.width * board.cellSize, board.height * board.cellSize);
            var plates = new GameObject("Plates");
            plates.transform.SetParent(tableRoot.transform, false);

            var controller = tableRoot.AddComponent<ProjectionTableController>();
            var cso = new SerializedObject(controller);
            cso.FindProperty("board").objectReferenceValue = board;
            cso.FindProperty("plateRoot").objectReferenceValue = plates.transform;
            cso.FindProperty("cellMaterial").objectReferenceValue = CellMaterial();
            cso.FindProperty("fallbackDatabase").objectReferenceValue = db;
            cso.FindProperty("fallbackLevelId").intValue = 1;
            cso.ApplyModifiedPropertiesWithoutUndo();

            var input = tableRoot.AddComponent<PlateInput>();
            var iso = new SerializedObject(input);
            iso.FindProperty("controller").objectReferenceValue = controller;
            iso.FindProperty("cam").objectReferenceValue = cam;
            iso.ApplyModifiedPropertiesWithoutUndo();

            // ---- HUD ----
            var canvas = MakeCanvas("TableCanvas");
            var hudGo = new GameObject("HudController");
            var hud = hudGo.AddComponent<HudController>();
            var hso = new SerializedObject(hud);

            // safe-area container for all required controls
            var safe = NewUi("SafeArea", canvas.transform);
            var safeRt = safe.GetComponent<RectTransform>();
            safeRt.anchorMin = Vector2.zero; safeRt.anchorMax = Vector2.one;
            safeRt.offsetMin = Vector2.zero; safeRt.offsetMax = Vector2.zero;
            safe.AddComponent<StencilRelay.Game.SafeAreaPanel>();

            var moves = UiText(safe, "MovesText", "MOVES 00", new Vector2(0, -34), new Vector2(400, 44), 26, TextAlignmentOptions.Center, Brass);
            Anchor(moves.gameObject, new Vector2(0.5f, 1f));
            var hintBacking = UiPanel(safe, "HintBacking", new Vector2(0, -108), new Vector2(1100, 50));
            Anchor(hintBacking, new Vector2(0.5f, 1));
            hintBacking.GetComponent<Image>().raycastTarget = false;
            var banner = UiText(safe, "TutorialBanner", "", new Vector2(0, -108), new Vector2(1080, 50), 22, TextAlignmentOptions.Center, Ivory);
            Anchor(banner.gameObject, new Vector2(0.5f, 1f));
            var levelTitle = UiText(safe, "LevelTitle", "", new Vector2(36, -40), new Vector2(500, 48), 22, TextAlignmentOptions.Left, Ivory);
            Anchor(levelTitle.gameObject, new Vector2(0, 1));
            var plateStatus = UiText(safe, "PlateStatus", "SELECT A LAYER", new Vector2(0, 28), new Vector2(700, 36), 18, TextAlignmentOptions.Center, Ivory);
            Anchor(plateStatus.gameObject, new Vector2(0.5f, 0));

            var tabs = new Button[4];
            for (int i = 0; i < 4; i++)
            {
                tabs[i] = UiButton(safe, $"LayerTab{(char)('A' + i)}", ((char)('A' + i)).ToString(),
                    new Vector2(28, 150 - i * 106), new Vector2(92, 96));
                Anchor(tabs[i].gameObject, new Vector2(0f, 0.5f));
            }

            var rotate = UiButton(safe, "RotateButton", "ROTATE", new Vector2(-28, 108), new Vector2(160, 88));
            var flip = UiButton(safe, "FlipButton", "FLIP", new Vector2(-28, 8), new Vector2(160, 88));
            var resetLayer = UiButton(safe, "ResetButton", "RESET LAYER", new Vector2(-28, -92), new Vector2(160, 88));
            foreach (var b in new[] { rotate, flip, resetLayer }) Anchor(b.gameObject, new Vector2(1f, 0.5f));

            var project = UiButton(safe, "ProjectButton", "PROJECT", new Vector2(0, 70), new Vector2(300, 92));
            Anchor(project.gameObject, new Vector2(0.5f, 0f));
            var projImg = project.targetGraphic as Image;
            if (projImg != null) projImg.color = new Color(0.85f, 0.55f, 0.35f, 0.95f);

            var pause = UiButton(safe, "PauseButton", "II", new Vector2(-28, -28), new Vector2(88, 88));
            Anchor(pause.gameObject, new Vector2(1f, 1f));
            var reconfigure = UiButton(safe, "ReconfigureButton", "Adjust Layers", new Vector2(0, 70), new Vector2(300, 92));
            Anchor(reconfigure.gameObject, new Vector2(0.5f, 0));
            reconfigure.gameObject.SetActive(false);

            var (pausePanel, pauseC) = UiModal(canvas, "PausePanel", new Vector2(680, 520), frosted: true);
            UiText(pauseC, "PauseTitle", "CALIBRATION", new Vector2(0, 190), new Vector2(600, 56), 34, TextAlignmentOptions.Center, BlueBlack);
            var resume = UiButton(pauseC, "Resume", "Resume Alignment", new Vector2(0, 92), new Vector2(440, 66));
            var restart = UiButton(pauseC, "Restart", "Restart Pattern", new Vector2(0, 8), new Vector2(440, 66));
            var pauseGuide = UiButton(pauseC, "PauseGuide", "Calibration Guide", new Vector2(0, -76), new Vector2(440, 66));
            var pauseStudio = UiButton(pauseC, "PauseStudio", "Return to Studio", new Vector2(0, -160), new Vector2(440, 66));

            var (resultPanel, resultC) = UiModal(canvas, "ResultPanel", new Vector2(820, 560), frosted: false);
            var resultTitle = UiText(resultC, "ResultTitle", "PATTERN LOCKED", new Vector2(0, 160), new Vector2(760, 64), 38, TextAlignmentOptions.Center, Sage);
            var resultBody = UiText(resultC, "ResultBody", "", new Vector2(0, 46), new Vector2(720, 150), 24, TextAlignmentOptions.Center, Ivory);
            var resultPrimary = UiButton(resultC, "ResultPrimary", "Adjust Layers", new Vector2(-160, -150), new Vector2(300, 66));
            var resultPrimaryLabel = resultPrimary.GetComponentInChildren<TMP_Text>();
            var resultRack = UiButton(resultC, "ResultRack", "Pattern Rack", new Vector2(170, -150), new Vector2(260, 66));
            var inspect = UiButton(resultC, "InspectProjection", "Inspect Projection", new Vector2(0, -64), new Vector2(380, 66));

            void H(string p, Object v) => hso.FindProperty(p).objectReferenceValue = v;
            H("controller", controller);
            SetArray(hso, "layerTabs", tabs);
            H("rotateButton", rotate); H("flipButton", flip); H("resetButton", resetLayer);
            H("projectButton", project); H("movesText", moves); H("tutorialBanner", banner);
            H("levelTitle", levelTitle); H("plateStatus", plateStatus);
            H("inspectButton", inspect); H("reconfigureButton", reconfigure);
            H("pauseButton", pause); H("pausePanel", pausePanel);
            H("resumeButton", resume); H("restartButton", restart);
            H("pauseGuideButton", pauseGuide); H("pauseStudioButton", pauseStudio);
            H("resultPanel", resultPanel); H("resultTitle", resultTitle); H("resultBody", resultBody);
            H("resultPrimaryButton", resultPrimary); H("resultPrimaryLabel", resultPrimaryLabel);
            H("resultRackButton", resultRack);
            hso.ApplyModifiedPropertiesWithoutUndo();

            pausePanel.SetActive(false);
            resultPanel.SetActive(false);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, $"{ScenesDir}/ProjectionTable.unity");
        }

        // ---------------------------------------------------------------- UI helpers

        static Camera AddCamera(string name, Color bg)
        {
            var go = new GameObject(name);
            var cam = go.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = bg;
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            go.transform.position = new Vector3(0, 0, -10);
            go.tag = "MainCamera";
            go.AddComponent<AudioListener>();
            return cam;
        }

        static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null) return;
            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        static GameObject Quad(string name, Vector3 pos, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = name;
            var col = go.GetComponent<Collider>();
            if (col) Object.DestroyImmediate(col);
            go.transform.position = pos;
            go.transform.localScale = scale;
            var mr = go.GetComponent<MeshRenderer>();
            var mat = CellMaterial();
            if (mat) mr.sharedMaterial = mat;
            var block = new MaterialPropertyBlock();
            block.SetColor("_BaseColor", color);
            block.SetColor("_Color", color);
            mr.SetPropertyBlock(block);
            return go;
        }

        static GameObject MakeCanvas(string name)
        {
            var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            return go;
        }

        static GameObject NewUi(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        static RectTransform RT(GameObject go, Vector2 anchoredPos, Vector2 size)
        {
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;
            return rt;
        }

        /// <summary>Re-anchor a widget to an edge/corner (min==max), keeping its size and
        /// its current anchoredPosition as an offset from that edge.</summary>
        static void Anchor(GameObject go, Vector2 anchor)
        {
            var rt = go.GetComponent<RectTransform>();
            var pos = rt.anchoredPosition;
            var size = rt.sizeDelta;
            rt.anchorMin = rt.anchorMax = anchor;
            rt.pivot = anchor;
            rt.sizeDelta = size;
            rt.anchoredPosition = pos;
        }

        static TMP_Text UiText(GameObject parent, string name, string content, Vector2 pos, Vector2 size,
            float fontSize, TextAlignmentOptions align, Color color)
        {
            var go = NewUi(name, parent.transform);
            RT(go, pos, size);
            var t = go.AddComponent<TextMeshProUGUI>();
            t.text = content;
            t.fontSize = fontSize;
            t.alignment = align;
            t.color = color;
            t.raycastTarget = false;
            t.textWrappingMode = TextWrappingModes.Normal;
            return t;
        }

        static Image PanelImage(GameObject go, Color c)
        {
            var img = go.AddComponent<Image>();
            img.color = c;
            return img;
        }

        static GameObject UiPanel(GameObject parent, string name, Vector2 pos, Vector2 size)
        {
            var go = NewUi(name, parent.transform);
            RT(go, pos, size);
            PanelImage(go, new Color(0.06f, 0.09f, 0.11f, 0.94f));
            return go;
        }

        /// <summary>
        /// Full-screen modal: input-blocking scrim + a full-screen backing layer + a
        /// safe-area container holding the content. Returns (root, content). Toggle `root`
        /// active to show/hide. `frosted` = the pale calibration-glass look for Pause;
        /// otherwise a dark blue-black card.
        /// </summary>
        static (GameObject root, GameObject content) UiModal(GameObject canvas, string name,
            Vector2 cardSize, bool frosted)
        {
            var root = NewUi(name, canvas.transform);
            Stretch(root);
            var scrim = root.AddComponent<Image>();
            scrim.color = new Color(0.03f, 0.045f, 0.055f, frosted ? 0.15f : 0.66f);

            var backing = NewUi("Backing", root.transform);
            Stretch(backing);
            var b = backing.AddComponent<Image>();
            var soft = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Content/Resources/Sprites/spr_soft_rect.png");
            if (frosted)
            {
                // a frosted plate that slides over the workspace (not full-screen)
                var brt = backing.GetComponent<RectTransform>();
                brt.anchorMin = new Vector2(0.5f, 0.5f); brt.anchorMax = new Vector2(0.5f, 0.5f);
                brt.sizeDelta = new Vector2(3000, 1400); brt.anchoredPosition = Vector2.zero;
                if (soft != null) { b.sprite = soft; b.type = Image.Type.Sliced; }
                b.color = new Color(0.80f, 0.85f, 0.87f, 0.90f);
            }
            else
            {
                b.color = new Color(0.05f, 0.072f, 0.088f, 0.08f);
            }

            var safe = NewUi("Safe", root.transform);
            Stretch(safe);
            safe.AddComponent<StencilRelay.Game.SafeAreaPanel>();

            var content = NewUi("Content", safe.transform);
            RT(content, Vector2.zero, cardSize);
            var cimg = content.AddComponent<Image>();
            if (soft != null) { cimg.sprite = soft; cimg.type = Image.Type.Sliced; }
            cimg.color = frosted
                ? new Color(0.055f, 0.078f, 0.094f, 0.06f)   // near-invisible; text sits on frost
                : new Color(0.055f, 0.078f, 0.094f, 0.985f);
            if (frosted) cimg.raycastTarget = false;

            return (root, content);
        }

        static void Stretch(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        }

        static Button UiButton(GameObject parent, string name, string label, Vector2 pos, Vector2 size)
        {
            var go = NewUi(name, parent.transform);
            RT(go, pos, size);
            var img = go.AddComponent<Image>();
            var sr = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Content/Resources/Sprites/spr_soft_rect.png");
            if (sr != null) { img.sprite = sr; img.type = Image.Type.Sliced; }
            img.color = new Color(0.10f, 0.13f, 0.15f, 0.82f);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            var cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(1.15f, 1.15f, 1.1f, 1f);
            cb.pressedColor = new Color(0.7f, 0.85f, 0.88f, 1f);
            cb.disabledColor = new Color(0.4f, 0.4f, 0.4f, 0.4f);
            btn.colors = cb;

            var textGo = NewUi("Label", go.transform);
            RT(textGo, Vector2.zero, size);
            var t = textGo.AddComponent<TextMeshProUGUI>();
            t.text = label;
            t.fontSize = 22;
            t.alignment = TextAlignmentOptions.Center;
            t.color = Ivory;
            t.raycastTarget = false;
            t.margin = new Vector4(12, 6, 12, 6);
            return btn;
        }

        static Slider UiSlider(GameObject parent, string name, Vector2 pos, Vector2 size)
        {
            var go = NewUi(name, parent.transform);
            RT(go, pos, new Vector2(size.x, 64));
            var bg = go.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.24f, 0.26f, 0.15f);
            var slider = go.AddComponent<Slider>();

            var fillArea = NewUi("Fill Area", go.transform);
            var fart = RT(fillArea, Vector2.zero, size);
            fart.anchorMin = new Vector2(0, 0.25f);
            fart.anchorMax = new Vector2(1, 0.75f);
            fart.sizeDelta = Vector2.zero;
            var fill = NewUi("Fill", fillArea.transform);
            var fillRt = fill.GetComponent<RectTransform>();
            fillRt.sizeDelta = Vector2.zero;
            var fillImg = fill.AddComponent<Image>();
            fillImg.color = new Color(0.62f, 0.91f, 0.94f, 1f);

            var handleArea = NewUi("Handle Area", go.transform);
            Stretch(handleArea);
            var areaRt = handleArea.GetComponent<RectTransform>();
            areaRt.offsetMin = new Vector2(14, 0); areaRt.offsetMax = new Vector2(-14, 0);
            fart.offsetMin = new Vector2(14, 0); fart.offsetMax = new Vector2(-14, 0);
            var handle = NewUi("Handle", handleArea.transform);
            var handleRt = handle.GetComponent<RectTransform>();
            handleRt.sizeDelta = new Vector2(28, -14);
            var handleImg = handle.AddComponent<Image>();
            handleImg.color = Brass;

            slider.fillRect = fillRt;
            slider.handleRect = handleRt;
            slider.targetGraphic = handleImg;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f; slider.maxValue = 1f; slider.value = 0.8f;
            return slider;
        }

        static Toggle UiToggle(GameObject parent, string name, string label, Vector2 pos, Vector2 size)
        {
            var go = NewUi(name, parent.transform);
            RT(go, pos, new Vector2(size.x, 64));
            var toggle = go.AddComponent<Toggle>();
            go.AddComponent<Image>().color = Color.clear;

            var box = NewUi("Box", go.transform);
            RT(box, new Vector2(-150, 0), new Vector2(34, 34));
            var boxImg = box.AddComponent<Image>();
            boxImg.color = new Color(0.2f, 0.24f, 0.26f, 1f);
            var check = NewUi("Check", box.transform);
            RT(check, Vector2.zero, new Vector2(24, 24));
            var checkImg = check.AddComponent<Image>();
            checkImg.color = Sage;

            var t = NewUi("Label", go.transform);
            RT(t, new Vector2(30, 0), new Vector2(240, 50));
            var tmp = t.AddComponent<TextMeshProUGUI>();
            tmp.text = label; tmp.fontSize = 22; tmp.color = Ivory;
            tmp.alignment = TextAlignmentOptions.Left;

            toggle.targetGraphic = boxImg;
            toggle.graphic = checkImg;
            toggle.isOn = true;
            return toggle;
        }

        static void SetArray(SerializedObject so, string prop, Object[] values)
        {
            var p = so.FindProperty(prop);
            p.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
                p.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }
    }
}
