#if !NOT_UNITY3D

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ModestTree;
using UnityEngine.SceneManagement;

namespace Zenject
{
    public static class ZenUnityEditorUtil
    {
        // Don't use this
        public static void ValidateCurrentSceneSetup()
        {
            Assert.That(!ProjectContext.HasInstance);
            ProjectContext.ValidateOnNextRun = true;

            foreach (var sceneContext in GetAllSceneContexts())
            {
                sceneContext.Validate();
            }
        }

        // Don't use this
        public static void RunCurrentSceneSetup()
        {
            Assert.That(!ProjectContext.HasInstance);

            foreach (var sceneContext in GetAllSceneContexts())
            {
                sceneContext.Run();
            }
        }

        static List<SceneContext> GetAllSceneContexts()
        {
            return GetAllScenes().SelectMany(scene =>
                scene
                .GetRootGameObjects()
                .SelectMany(x => x.GetComponentsInChildren<SceneContext>())).ToList();
        }

        static IEnumerable<Scene> GetAllScenes()
        {
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                yield return EditorSceneManager.GetSceneAt(i);
            }
        }

        public static string ConvertFullAbsolutePathToAssetPath(string fullPath)
        {
            return "Assets/" + Path.GetFullPath(fullPath)
                .Remove(0, Path.GetFullPath(Application.dataPath).Length + 1)
                .Replace("\\", "/");
        }

        public static string TryGetSelectedFilePathInProjectsTab()
        {
            return GetSelectedFilePathsInProjectsTab().OnlyOrDefault();
        }

        public static List<string> GetSelectedFilePathsInProjectsTab()
        {
            return GetSelectedPathsInProjectsTab()
                .Where(x => File.Exists(x)).ToList();
        }

        public static List<string> GetSelectedPathsInProjectsTab()
        {
            var paths = new List<string>();

            UnityEngine.Object[] selectedAssets = Selection.GetFiltered(
                typeof(UnityEngine.Object), SelectionMode.Assets);

            foreach (var item in selectedAssets)
            {
                var relativePath = AssetDatabase.GetAssetPath(item);

                if (!string.IsNullOrEmpty(relativePath))
                {
                    var fullPath = Path.GetFullPath(Path.Combine(
                        Application.dataPath, Path.Combine("..", relativePath)));

                    paths.Add(fullPath);
                }
            }

            return paths;
        }

        // Note that the path is relative to the Assets folder
        public static List<string> GetSelectedFolderPathsInProjectsTab()
        {
            return GetSelectedPathsInProjectsTab()
                .Where(x => Directory.Exists(x)).ToList();
        }

        // Returns the best guess directory in projects pane
        // Useful when adding to Assets -> Create context menu
        // Returns null if it can't find one
        // Note that the path is relative to the Assets folder for use in AssetDatabase.GenerateUniqueAssetPath etc.
        public static string TryGetSelectedFolderPathInProjectsTab()
        {
            return GetSelectedFolderPathsInProjectsTab().OnlyOrDefault();
        }
    }
}

#endif
