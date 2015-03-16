using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ModestTree;

namespace Zenject
{
    public static class ZenEditorUtil
    {
        public static DiContainer GetContainerForCurrentScene()
        {
            var compRoot = GameObject.FindObjectsOfType<CompositionRoot>().OnlyOrDefault();

            if (compRoot == null)
            {
                throw new ZenjectException(
                    "Unable to find CompositionRoot in current scene.");
            }

            return compRoot.Container;
        }

        public static List<ZenjectResolveException> ValidateAllActiveScenes(int maxErrors)
        {
            var activeScenes = UnityEditor.EditorBuildSettings.scenes.Where(x => x.enabled).Select(x => x.ToString()).ToList();
            return ValidateScenes(activeScenes, maxErrors);
        }

        // This can be called by build scripts using batch mode unity for continuous integration testing
        public static void ValidateAllScenesFromScript()
        {
            var activeScenes = UnityEditor.EditorBuildSettings.scenes.Where(x => x.enabled).Select(x => x.ToString()).ToList();
            ValidateScenesThenExit(activeScenes, 25);
        }

        public static void ValidateScenesThenExit(List<string> sceneNames, int maxErrors)
        {
            var errors = ValidateScenes(sceneNames, maxErrors);

            if (errors.IsEmpty())
            {
                // 0 = no errors
                EditorApplication.Exit(0);
            }
            else
            {
                Log.Error("Found {0} validation errors!", errors.Count == maxErrors ? ("over " + maxErrors.ToString()) : errors.Count.ToString());

                foreach (var err in errors)
                {
                    Log.ErrorException(err);
                }

                // 1 = errors occurred
                EditorApplication.Exit(1);
            }
        }

        public static List<ZenjectResolveException> ValidateScenes(List<string> sceneNames, int maxErrors)
        {
            var errors = new List<ZenjectResolveException>();
            var activeScenes = sceneNames
                .Select(x => new { Name = x, Path = GetScenePath(x) }).ToList();

            foreach (var sceneInfo in activeScenes)
            {
                Log.Trace("Validating Scene '{0}'", sceneInfo.Path);
                EditorApplication.OpenScene(sceneInfo.Path);

                errors.AddRange(ValidateCurrentScene().Take(maxErrors - errors.Count));

                if (errors.Count >= maxErrors)
                {
                    break;
                }
            }

            if (errors.IsEmpty())
            {
                Log.Trace("Successfully validated all {0} scenes", activeScenes.Count);
            }
            else
            {
                Log.Error("Zenject Validation failed!  Found {0} errors.", errors.Count);

                foreach (var err in errors)
                {
                    Log.ErrorException(err);
                }
            }

            return errors;
        }

        static string GetScenePath(string sceneName)
        {
            var namesToPaths = UnityEditor.EditorBuildSettings.scenes.ToDictionary(
                x => Path.GetFileNameWithoutExtension(x.path), x => x.path);

            if (!namesToPaths.ContainsKey(sceneName))
            {
                throw new Exception(
                    "Could not find scene with name '" + sceneName + "'");
            }

            return namesToPaths[sceneName];
        }

        public static IEnumerable<ZenjectResolveException> ValidateCurrentScene()
        {
            var compRoot = GameObject.FindObjectsOfType<CompositionRoot>().OnlyOrDefault();

            if (compRoot == null || compRoot.Installers.IsEmpty())
            {
                return Enumerable.Empty<ZenjectResolveException>();
            }

            return ZenEditorUtil.ValidateInstallers(compRoot);
        }

        public static IEnumerable<ZenjectResolveException> ValidateInstallers(CompositionRoot compRoot)
        {
            var globalContainer = GlobalCompositionRoot.CreateContainer(true, null);
            var container = compRoot.CreateContainer(true, globalContainer, new List<IInstaller>());

            foreach (var error in container.ValidateResolve(new InjectContext(container, typeof(IDependencyRoot), null)))
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
    }
}
