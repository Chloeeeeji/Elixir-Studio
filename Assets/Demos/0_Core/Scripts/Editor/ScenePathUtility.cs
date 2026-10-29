using UnityEditor;
using UnityEngine;

namespace GameSystemsCookbook
{
    /// <summary>
    /// Adds a context menu item to copy the path of a selected Scene asset to the clipboard.
    /// Use this for pasting the path into Inspector fields that require a scene path string.
    /// </summary>
    public static class ScenePathUtility
    {
        // Validates that the selected object is a SceneAsset before showing the context menu item
        [MenuItem("Assets/Copy Scene Path", true)]
        static bool ValidateCopyScenePath()
        {
            return Selection.activeObject is SceneAsset;
        }

        [MenuItem("Assets/Copy Scene Path")]
        static void CopyScenePath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            EditorGUIUtility.systemCopyBuffer = path;
            Debug.Log("Scene path copied to clipboard: " + path);
        }
    }
}
