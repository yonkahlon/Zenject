using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Debug=UnityEngine.Debug;

namespace ModestTree.Zenject
{
    public static class ZenjectMenu
    {
        public static void ValidateCurrentSceneThenPlay()
        {
            if (ValidateCurrentScene())
            {
                EditorApplication.isPlaying = true;
            }
        }

        [MenuItem("Assets/Create/Zenject Global Installer Config")]
        public static void CreateProjectConfig()
        {
            var asset = ScriptableObject.CreateInstance<GlobalInstallerConfig>();

            var path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/ZenjectGlobalInstallers.asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Edit/Zenject/Validate Current Scene #%v")]
        public static bool ValidateCurrentScene()
        {
            var compRoots = GameObject.FindObjectsOfType<CompositionRoot>();

            if (compRoots.HasMoreThan(1))
            {
                Log.Error("Found multiple composition roots when only one was expected while validating current scene");
                return false;
            }

            if (compRoots.IsEmpty())
            {
                // Return true to allow playing in this case
                Log.Error("Could not find composition root while validating current scene");
                return true;
            }

            var compRoot = compRoots.Single();

            if (compRoot.Installers.IsEmpty())
            {
                Log.Warn("Could not find installers while validating current scene");
                // Return true to allow playing in this case
                return true;
            }

            // Only show a few to avoid spamming the log too much
            var resolveErrors = ValidateInstallers(compRoot).Take(10).ToList();

            foreach (var error in resolveErrors)
            {
                Log.ErrorException(error);
            }

            if (resolveErrors.Any())
            {
                Log.Error("Validation Completed With Errors");
                return false;
            }

            Log.Info("Validation Completed Successfully");
            return true;
        }

        static IEnumerable<ZenjectResolveException> ValidateInstallers(CompositionRoot compRoot)
        {
            var globalContainer = GlobalCompositionRoot.CreateContainer(true, null);
            var container = compRoot.CreateContainer(true, globalContainer);

            foreach (var error in container.ValidateResolve<IDependencyRoot>())
            {
                yield return error;
            }

            // Also make sure we can fill in all the dependencies in the built-in scene
            foreach (var curTransform in compRoot.GetComponentsInChildren<Transform>())
            {
                foreach (var monoBehaviour in curTransform.GetComponents<MonoBehaviour>())
                {
                    if (monoBehaviour == null)
                    {
                        Log.Warn("Found null MonoBehaviour on " + curTransform.name);
                        continue;
                    }

                    foreach (var error in container.ValidateObjectGraph(monoBehaviour.GetType()))
                    {
                        yield return error;
                    }
                }
            }

            foreach (var installer in globalContainer.InstalledInstallers.Concat(container.InstalledInstallers))
            {
                if (installer is IValidatable)
                {
                    foreach (var error in ((IValidatable)installer).Validate())
                    {
                        yield return error;
                    }
                }
            }

            foreach (var error in container.ValidateValidatables())
            {
                yield return error;
            }
        }

        [MenuItem("Edit/Zenject/Output Object Graph For Current Scene")]
        public static void OutputObjectGraphForScene()
        {
            if (!EditorApplication.isPlaying)
            {
                Log.Error("Zenject error: Must be in play mode to generate object graph.  Hit Play button and try again.");
                return;
            }

            DiContainer container;
            try
            {
                container = ZenEditorUtil.GetContainerForCurrentScene();
            }
            catch (ZenjectException e)
            {
                Log.Error("Unable to find container in current scene. " + e.Message);
                return;
            }

            var ignoreTypes = Enumerable.Empty<Type>();
            var types = container.AllConcreteTypes;

            ZenEditorUtil.OutputObjectGraphForCurrentScene(container, ignoreTypes, types);
        }
    }
}

