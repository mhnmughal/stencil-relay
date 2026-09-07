using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Rendering;

namespace StencilRelay.Editor
{
    /// <summary>
    /// Applies the production Player Settings for Stencil Relay (iOS primary, Android
    /// compatible). Reproducible: Stencil Relay/Apply Mobile Settings.
    /// </summary>
    public static class MobileSettingsBuilder
    {
        [MenuItem("Stencil Relay/Apply Mobile Settings")]
        public static void Apply()
        {
            var log = new StringBuilder();

            PlayerSettings.productName = "Stencil Relay";
            PlayerSettings.companyName = "Rehman Mughal";
            PlayerSettings.bundleVersion = "1.0.0";

            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, "com.rehmanmughal.stencilrelay");
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.rehmanmughal.stencilrelay");
            PlayerSettings.iOS.buildNumber = "1";
            PlayerSettings.Android.bundleVersionCode = 1;

            // Landscape only
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.useAnimatedAutorotation = true;

            // iOS backend / architecture / stripping / graphics
            try
            {
                PlayerSettings.SetScriptingBackend(NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
                PlayerSettings.SetArchitecture(NamedBuildTarget.iOS, 1); // 1 = ARM64
                PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.iOS, ManagedStrippingLevel.Medium);
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, true); // Metal by default on iOS
                log.AppendLine("iOS: IL2CPP, ARM64, Medium strip, default (Metal) graphics.");
            }
            catch (System.Exception e) { log.AppendLine("iOS settings partial (module missing?): " + e.Message); }

            // iOS: no sensitive permissions / usage strings; manual signing
            PlayerSettings.iOS.locationUsageDescription = "";
            PlayerSettings.iOS.cameraUsageDescription = "";
            PlayerSettings.iOS.microphoneUsageDescription = "";
            PlayerSettings.iOS.targetOSVersionString = "13.0";
            PlayerSettings.iOS.appleEnableAutomaticSigning = false;
            PlayerSettings.iOS.requiresFullScreen = true;

            // Android
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            try { PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP); } catch { }
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
            PlayerSettings.Android.forceInternetPermission = false;

            // Quality: keep vSync on, target 60 handled at runtime via Application.targetFrameRate
            PlayerSettings.MTRendering = true;

            AssetDatabase.SaveAssets();

            log.AppendLine($"productName={PlayerSettings.productName}");
            log.AppendLine($"iOS id={PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.iOS)} build={PlayerSettings.iOS.buildNumber}");
            log.AppendLine($"version={PlayerSettings.bundleVersion} orientation={PlayerSettings.defaultInterfaceOrientation}");
            log.AppendLine($"iOS backend={PlayerSettings.GetScriptingBackend(NamedBuildTarget.iOS)}");
            Debug.Log("[MobileSettings] " + log);
        }
    }
}
