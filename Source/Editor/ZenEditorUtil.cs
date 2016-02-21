#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModestTree.Util;
using UnityEditor;
using UnityEngine;
using ModestTree;
using Zenject.Internal;

#if UNITY_5_3
using UnityEditor.SceneManagement;
#endif

namespace Zenject
{
    public static class ZenEditorUtil
    {
        public static SceneDecoratorCompositionRoot TryGetSceneDecoratorCompositionRoot()
        {
            return GameObject.FindObjectsOfType<SceneDecoratorCompositionRoot>().OnlyOrDefault();
        }

        public static SceneCompositionRoot TryGetSceneCompositionRoot()
        {
            return GameObject.FindObjectsOfType<SceneCompositionRoot>().OnlyOrDefault();
        }

        // Returns true if we should continue
        static bool CheckForExistingCompositionRoot()
        {
            if (TryGetSceneCompositionRoot() != null)
            {
                var shouldContinue = EditorUtility.DisplayDialog("Error", "There already exists a SceneCompositionRoot in the scene.  Are you sure you want to add another?", "Yes", "Cancel");
                return shouldContinue;
            }

            return true;
        }

        [MenuItem("GameObject/Zenject/Scene Composition Root", false, 9)]
        public static void CreateSceneCompositionRoot(MenuCommand menuCommand)
        {
            if (CheckForExistingCompositionRoot())
            {
                var root = new GameObject("SceneCompositionRoot").AddComponent<SceneCompositionRoot>();
                Selection.activeGameObject = root.gameObject;
            }
        }

        [MenuItem("GameObject/Zenject/Decorator Composition Root", false, 9)]
        public static void CreateDecoratorCompositionRoot(MenuCommand menuCommand)
        {
            if (CheckForExistingCompositionRoot())
            {
                var root = new GameObject("DecoratorCompositionRoot").AddComponent<SceneDecoratorCompositionRoot>();
                Selection.activeGameObject = root.gameObject;
            }
        }

        [MenuItem("GameObject/Zenject/Facade Composition Root", false, 9)]
        public static void CreateFacadeCompositionRoot(MenuCommand menuCommand)
        {
            var root = new GameObject("FacadeCompositionRoot").AddComponent<FacadeCompositionRoot>();
            Selection.activeGameObject = root.gameObject;
        }

        [MenuItem("Assets/Create/Zenject/Global Composition Root")]
        public static void AddGlobalCompositionRoot()
        {
            var dir = UnityEditorUtil.TryGetCurrentDirectoryInProjectsTab();

            if (dir == null)
            {
                EditorUtility.DisplayDialog("Error",
                    "Could not find directory to place the '{0}.prefab' asset.  Please try again by right clicking in the desired folder within the projects pane."
                    .Fmt(GlobalCompositionRoot.GlobalCompRootResourcePath), "Ok");
                return;
            }

            var parentFolderName = Path.GetFileName(dir);

            if (parentFolderName != "Resources")
            {
                EditorUtility.DisplayDialog("Error",
                    "'{0}.prefab' must be placed inside a directory named 'Resources'.  Please try again by right clicking within the Project pane in a valid Resources folder."
                    .Fmt(GlobalCompositionRoot.GlobalCompRootResourcePath), "Ok");
                return;
            }

            var prefabPath = (Path.Combine(dir, GlobalCompositionRoot.GlobalCompRootResourcePath) + ".prefab").Replace("\\", "/");
            var emptyPrefab = PrefabUtility.CreateEmptyPrefab(prefabPath);

            var gameObject = new GameObject();

            try
            {
                gameObject.AddComponent<GlobalCompositionRoot>();
                PrefabUtility.ReplacePrefab(gameObject, emptyPrefab);
            }
            finally
            {
                GameObject.DestroyImmediate(gameObject);
            }

            Debug.Log("Created new GlobalCompositionRoot at '{0}'".Fmt(prefabPath));
        }

        public static void OpenScene(string scenePath)
        {
#if UNITY_5_3
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
#else
            EditorApplication.OpenScene(scenePath);
#endif
        }

        public static void OpenSceneAdditive(string scenePath)
        {
#if UNITY_5_3
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
#else
            EditorApplication.OpenSceneAdditive(scenePath);
#endif
        }
    }
}
#endif
