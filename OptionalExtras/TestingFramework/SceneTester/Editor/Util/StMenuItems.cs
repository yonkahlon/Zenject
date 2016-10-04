using System.IO;
using ModestTree;
using ModestTree.Util;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Zenject;

namespace SceneTester
{
    public static class StMenuItems
    {
        [MenuItem("Assets/Test Scenes Underneath Folder")]
        public static void TestScenesUnderneathFolder()
        {
            var allFolderPaths = ZenUnityEditorUtil.GetSelectedFolderPathsInProjectsTab().Select(relativePath =>
                    Path.Combine(
                        Directory.GetParent(Application.dataPath).FullName, relativePath)).ToHashSet().ToList();

            var allSceneNames = StUtil.GetAllSceneNamesUnderneathFolders(allFolderPaths);

            StWindow.Instance.RunScenes(allSceneNames, false, false);
        }

        [MenuItem("Assets/Test Scenes Underneath Folder", true)]
        public static bool ValidateTestScenesUnderneathFolder()
        {
            return !ZenUnityEditorUtil.GetSelectedFolderPathsInProjectsTab().IsEmpty();
        }
    }
}
