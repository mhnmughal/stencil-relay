using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using StencilRelay.Levels;

namespace StencilRelay.Editor
{
    // Also callable from Unity batch mode after the MCP development package is removed.
    public static class ReleaseVerification
    {
        public static void BuildWindows() => Build(BuildTarget.StandaloneWindows64, "Builds/Offline/Win/StencilRelay.exe");
        public static void BuildIos() => Build(BuildTarget.iOS, "Builds/Offline/iOS");

        static void Build(BuildTarget target, string path)
        {
            PlayerSettings.SetManagedStrippingLevel(UnityEditor.Build.NamedBuildTarget.Standalone, ManagedStrippingLevel.Medium);
            PlayerSettings.stripEngineCode = true;
            AssetDatabase.SaveAssets();
            var validation = LevelValidation.Validate(Resources.Load<LevelDatabase>("LevelDatabase"));
            if (!validation.AllValid) throw new Exception(validation.Headline);
            Directory.CreateDirectory("AuditEvidence");
            File.WriteAllText("AuditEvidence/level-validation.txt", validation.Headline + "\n" + string.Join("\n", validation.Issues));
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                locationPathName = path, target = target, options = BuildOptions.None
            });
            var s = report.summary;
            File.WriteAllText($"AuditEvidence/build-{target}.txt", $"{s.result}\nErrors: {s.totalErrors}\nWarnings: {s.totalWarnings}\nBytes: {s.totalSize}\nDuration: {s.totalTime}\nOutput: {s.outputPath}\n");
            if (s.result != BuildResult.Succeeded) throw new Exception($"{target} build failed: {s.totalErrors} errors");
        }
    }
}
