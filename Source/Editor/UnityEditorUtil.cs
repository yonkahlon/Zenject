using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ModestTree.Util
{
    public static class UnityEditorUtil
    {
        // Note that the path is relative to the Assets folder
        public static List<string> TryGetSelectedFolderPathsInProjectsTab()
        {
            var result = new List<string>();

            UnityEngine.Object[] selectedAssets = Selection.GetFiltered(
                typeof(UnityEngine.Object), SelectionMode.Assets);

            foreach (var item in selectedAssets)
            {
                var relativePath = AssetDatabase.GetAssetPath(item);

                if (!string.IsNullOrEmpty(relativePath))
                {
                    var fullPath = Path.GetFullPath(Path.Combine(
                        Application.dataPath, Path.Combine("..", relativePath)));

                    if (Directory.Exists(fullPath))
                    {
                        result.Add(relativePath);
                    }
                }
            }

            return result;
        }

        // Returns the best guess directory in projects pane
        // Useful when adding to Assets -> Create context menu
        // Returns null if it can't find one
        // Note that the path is relative to the Assets folder for use in AssetDatabase.GenerateUniqueAssetPath etc.
        public static string TryGetSelectedFolderPathInProjectsTab()
        {
            return TryGetSelectedFolderPathsInProjectsTab().OnlyOrDefault();
        }

        public static string GetScenePath(string sceneName)
        {
            var scenePath = TryGetScenePath(sceneName);

            if (scenePath == null)
            {
                throw new Exception(
                    "Could not find scene with name '{0}'".Fmt(sceneName));
            }

            return scenePath;
        }

        public static string TryGetScenePath(string sceneName)
        {
            return UnityEditor.EditorBuildSettings.scenes.Select(x => x.path)
                .Where(x => Path.GetFileNameWithoutExtension(x) == sceneName).OnlyOrDefault();
        }

        public static IEnumerable<string> GetAllActiveSceneNames()
        {
            return GetAllActiveScenePaths().Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
        }

        public static List<string> GetAllActiveScenePaths()
        {
            return UnityEditor.EditorBuildSettings.scenes.Where(x => x.enabled)
                .Select(x => x.path).ToList();
        }
    }
}
