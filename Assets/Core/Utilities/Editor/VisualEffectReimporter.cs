using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameSystemsCookbook
{
    /// <summary>
    /// This forces a one-time reimport of the project's Visual Effect Graph assets.
    /// </summary>
    ///
    // A .unitypackage carries only the contents of the Assets folder, so importing this project
    // into a template without the Visual Effect Graph package lands the .vfx files before the
    // importer that understands them exists. Unity imports them as unrecognized assets, and
    // installing the package afterwards does not send them back through the importer, so the
    // effects stay empty. Nothing is logged and nothing looks broken -- the only symptom is that
    // no effects appear.
    //
    // Reimporting once, after the package is present, resolves it. That ordering is the whole
    // point: a reimport that runs while the package is still missing does nothing at all.

    [InitializeOnLoad]
    public static class VisualEffectReimporter
    {
        // These appear as menu names
        private const string k_ReimportMenu = "Tools/GameSystems/Reimport Visual Effects";

        // Tracked per Editor session rather than persisted, so a run that finds nothing to do
        // cannot disable the reimport permanently. SessionState resets when the Editor closes, so
        // the worst case is one redundant reimport per session, which is cheap and harmless.
        private const string k_DoneKey = "GameSystemsCookbook.VfxReimported";

        // Presence of this assembly is what tells us the Visual Effect Graph package is installed.
        // Checking for it by name avoids a hard reference, so this compiles either way.
        private const string k_VfxEditorAssembly = "Unity.VisualEffectGraph.Editor";

        static VisualEffectReimporter()
        {
            EditorApplication.update += WaitUntilIdle;
        }

        // Polls until the asset database is no longer busy, then runs once and detaches
        private static void WaitUntilIdle()
        {
            if (EditorApplication.isUpdating || EditorApplication.isCompiling)
                return;

            EditorApplication.update -= WaitUntilIdle;
            RunOnce();
        }

        // Skips when this project has already been through it
        private static void RunOnce()
        {
            if (SessionState.GetBool(k_DoneKey, false))
                return;

            // Nothing can be reimported until the importer exists. This is the ordering that
            // matters: on a first import the package is often still missing at this point, and
            // PackageDependencyChecker has yet to offer to install it. Return without recording
            // success -- installing the package reloads the domain, which brings us back here with
            // the importer in place.
            if (!IsVisualEffectGraphInstalled())
                return;

            List<string> paths = FindVisualEffectAssets();

            // The graphs may not have finished importing yet, in which case there is nothing to
            // reimport. Return without recording success so the next domain reload tries again --
            // recording it here would mean never retrying, and the effects would stay broken.
            if (paths.Count == 0)
                return;

            Reimport(paths);

            SessionState.SetBool(k_DoneKey, true);
        }

        // Is the Visual Effect Graph package present? Its editor assembly is loaded whenever it is,
        // and a package install reloads the domain, so this is accurate by the time it is consulted.
        private static bool IsVisualEffectGraphInstalled()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == k_VfxEditorAssembly)
                    return true;
            }

            return false;
        }

        // Reimports on demand, ignoring the once-per-project record
        [MenuItem(k_ReimportMenu)]
        private static void ReimportFromMenu()
        {
            if (!IsVisualEffectGraphInstalled())
            {
                EditorUtility.DisplayDialog("Reimport Visual Effects",
                    "The Visual Effect Graph package is not installed, so there is no importer to reimport these assets with.\n\n" +
                    "Install it first, from Tools > GameSystems > Check Required Packages.", "OK");
                return;
            }

            List<string> paths = FindVisualEffectAssets();

            if (paths.Count == 0)
            {
                EditorUtility.DisplayDialog("Reimport Visual Effects", "This project has no Visual Effect Graph assets.", "OK");
                return;
            }

            Reimport(paths);
        }

        // Collects the .vfx assets by path. Matching on the extension avoids a hard reference to
        // the Visual Effect Graph assembly, so this compiles whether or not the package is present.
        private static List<string> FindVisualEffectAssets()
        {
            return AssetDatabase.GetAllAssetPaths()
                .Where(path => path.StartsWith("Assets/") && path.EndsWith(".vfx"))
                .ToList();
        }

        private static void Reimport(List<string> paths)
        {
            try
            {
                AssetDatabase.StartAssetEditing();

                foreach (string path in paths)
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
            finally
            {
                // In a finally block so a failure part-way through cannot leave the asset
                // database stuck in a batched-editing state
                AssetDatabase.StopAssetEditing();
            }

            Debug.Log($"Reimported {paths.Count} Visual Effect Graph asset(s) so their shaders compile. " +
                $"Run it again any time from <b>{k_ReimportMenu}</b>.");
        }
    }
}
