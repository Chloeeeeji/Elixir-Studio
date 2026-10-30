using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace GameSystemsCookbook
{
    /// <summary>
    /// This checks that the packages this project needs are actually installed, and offers to
    /// add any that are missing.
    /// </summary>
    ///
    // An Asset Store package ships only the contents of the Assets folder, so Packages/manifest.json
    // does not travel with it. Importing into a template that lacks these packages (the 3D URP
    // template, for example) leaves assets broken with no obvious explanation, so ask up front
    // rather than letting the user discover it.

    [InitializeOnLoad]
    public static class PackageDependencyChecker
    {
        // Packages this project cannot function correctly without, and what breaks when they are absent
        private static readonly Dictionary<string, string> k_RequiredPackages = new Dictionary<string, string>
        {
            { "com.unity.2d.sprite", "Sprite textures do not import as sprites, so SpriteRenderers appear empty." },
            { "com.unity.visualeffectgraph", "Visual Effect Graph assets do not import, so the ball trail and the Flyweight and Event Channels demo effects are missing." }
        };

        // These appear as menu names
        private const string k_CheckMenu = "Tools/GameSystems/Check Required Packages";

        // Use these keys for Editor preferences
        private const string k_DontAskKey = "GameSystemsCookbook.SkipPackageCheck";
        private const string k_AskedThisSessionKey = "GameSystemsCookbook.PackageCheckAsked";

        // Pending Package Manager queries. Only one of each runs at a time.
        private static ListRequest s_ListRequest;
        private static AddAndRemoveRequest s_AddRequest;

        // Was the check started from the menu? If so, always report the result.
        private static bool s_IsManualCheck;

        // Runs the check once per session, after the Editor settles
        static PackageDependencyChecker()
        {
            EditorApplication.delayCall += RunAutomaticCheck;
        }

        // Skips the check when the user has opted out or has already been asked this session
        private static void RunAutomaticCheck()
        {
            if (EditorPrefs.GetBool(k_DontAskKey, false))
                return;

            if (SessionState.GetBool(k_AskedThisSessionKey, false))
                return;

            SessionState.SetBool(k_AskedThisSessionKey, true);
            BeginCheck(false);
        }

        // Checks on demand, ignoring the opt-out so the user can always re-run it
        [MenuItem(k_CheckMenu)]
        private static void CheckFromMenu()
        {
            BeginCheck(true);
        }

        // Disables the menu item while a query is already in flight
        [MenuItem(k_CheckMenu, true)]
        private static bool ValidateCheckFromMenu()
        {
            return s_ListRequest == null && s_AddRequest == null;
        }

        // Asks the Package Manager what is installed. Offline mode keeps this off the network:
        // the answer comes from the resolved manifest, which is all we need.
        private static void BeginCheck(bool isManualCheck)
        {
            if (s_ListRequest != null || s_AddRequest != null)
                return;

            s_IsManualCheck = isManualCheck;
            s_ListRequest = Client.List(true, true);
            EditorApplication.update += PollListRequest;
        }

        // Package Manager requests complete asynchronously, so poll until one resolves
        private static void PollListRequest()
        {
            if (s_ListRequest == null || !s_ListRequest.IsCompleted)
                return;

            EditorApplication.update -= PollListRequest;

            ListRequest request = s_ListRequest;
            s_ListRequest = null;

            if (request.Status != StatusCode.Success)
            {
                // A failed query is not worth interrupting the user over unless they asked for it
                if (s_IsManualCheck)
                    Debug.LogWarning($"Could not check installed packages: {request.Error?.message}");

                return;
            }

            List<string> missingPackages = new List<string>();

            foreach (KeyValuePair<string, string> required in k_RequiredPackages)
            {
                if (!IsInstalled(request.Result, required.Key))
                    missingPackages.Add(required.Key);
            }

            ReportResult(missingPackages);
        }

        // Is the package present, either as a direct or an indirect dependency?
        private static bool IsInstalled(PackageCollection packages, string packageName)
        {
            foreach (UnityEditor.PackageManager.PackageInfo package in packages)
            {
                if (package.name == packageName)
                    return true;
            }

            return false;
        }

        // Prompts to install anything missing, and confirms success only for a manual check
        private static void ReportResult(List<string> missingPackages)
        {
            if (missingPackages.Count == 0)
            {
                if (s_IsManualCheck)
                    EditorUtility.DisplayDialog("Required Packages", "All required packages are installed.", "OK");

                return;
            }

            string message = "This project needs the following package(s):\n\n";

            foreach (string packageName in missingPackages)
                message += $"• {packageName}\n    {k_RequiredPackages[packageName]}\n\n";

            message += "Install them now?";

            // Offer the opt-out only on the automatic check; a manual check is an explicit request
            int choice = s_IsManualCheck
                ? (EditorUtility.DisplayDialog("Missing Required Packages", message, "Install", "Not Now") ? 0 : 1)
                : EditorUtility.DisplayDialogComplex("Missing Required Packages", message, "Install", "Not Now", "Don't Ask Again");

            switch (choice)
            {
                case 0:
                    InstallPackages(missingPackages);
                    break;

                case 2:
                    EditorPrefs.SetBool(k_DontAskKey, true);
                    Debug.Log($"Skipping the package check from now on. Re-run it any time from <b>{k_CheckMenu}</b>.");
                    break;
            }
        }

        // Adds every missing package in a single resolve, rather than one reload per package
        private static void InstallPackages(List<string> packageNames)
        {
            s_AddRequest = Client.AddAndRemove(packageNames.ToArray());
            EditorApplication.update += PollAddRequest;
        }

        private static void PollAddRequest()
        {
            if (s_AddRequest == null || !s_AddRequest.IsCompleted)
                return;

            EditorApplication.update -= PollAddRequest;

            AddAndRemoveRequest request = s_AddRequest;
            s_AddRequest = null;

            if (request.Status == StatusCode.Success)
                Debug.Log("Installed the required package(s). Unity will reimport any affected assets.");
            else
                Debug.LogError($"Could not install the required package(s): {request.Error?.message}");
        }
    }
}
