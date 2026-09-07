using System.Linq;
using UnityEditor;
using UnityEngine;
using StencilRelay.Levels;

namespace StencilRelay.Editor
{
    /// <summary>
    /// Editor entry points for level validation. Menu: Stencil Relay/Validate Levels.
    /// The actual checks live in StencilRelay.Levels.LevelValidation (shared with EditMode tests).
    /// </summary>
    public sealed class LevelValidatorWindow : EditorWindow
    {
        LevelDatabase _db;
        ValidationReport _report;
        Vector2 _scroll;

        [MenuItem("Stencil Relay/Validate Levels %#v")]
        public static void Open()
        {
            var w = GetWindow<LevelValidatorWindow>("Level Validator");
            w._db = FindDatabase();
            w.Run();
            w.Show();
        }

        [MenuItem("Stencil Relay/Validate Levels (log only)")]
        public static void ValidateToConsole()
        {
            var db = FindDatabase();
            var report = LevelValidation.Validate(db);
            LogReport(report);
        }

        static LevelDatabase FindDatabase()
        {
            var guids = AssetDatabase.FindAssets("t:LevelDatabase");
            return guids.Length > 0
                ? AssetDatabase.LoadAssetAtPath<LevelDatabase>(AssetDatabase.GUIDToAssetPath(guids[0]))
                : null;
        }

        static void LogReport(ValidationReport report)
        {
            Debug.Log($"[Stencil Relay] Level Validation: {report.Headline}");
            foreach (var issue in report.Issues.OrderByDescending(i => i.IsError).ThenBy(i => i.LevelId))
            {
                if (issue.IsError) Debug.LogError(issue.ToString());
                else Debug.LogWarning(issue.ToString());
            }
            if (report.AllValid) Debug.Log("[Stencil Relay] 13 / 13 VALID");
        }

        void Run()
        {
            _report = LevelValidation.Validate(_db);
            LogReport(_report);
        }

        void OnGUI()
        {
            EditorGUILayout.Space();
            _db = (LevelDatabase)EditorGUILayout.ObjectField("Level Database", _db, typeof(LevelDatabase), false);
            if (GUILayout.Button("Validate", GUILayout.Height(28))) Run();

            if (_report == null) return;

            EditorGUILayout.Space();
            var style = new GUIStyle(EditorStyles.boldLabel) { fontSize = 14 };
            style.normal.textColor = _report.AllValid ? new Color(0.4f, 0.8f, 0.4f) : new Color(0.9f, 0.5f, 0.4f);
            EditorGUILayout.LabelField(_report.Headline, style);

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (var issue in _report.Issues.OrderByDescending(i => i.IsError).ThenBy(i => i.LevelId))
            {
                var c = GUI.color;
                GUI.color = issue.IsError ? new Color(1f, 0.6f, 0.6f) : new Color(1f, 0.9f, 0.6f);
                EditorGUILayout.LabelField(issue.ToString(), EditorStyles.wordWrappedLabel);
                GUI.color = c;
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
