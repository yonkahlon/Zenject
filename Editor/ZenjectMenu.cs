using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Debug=UnityEngine.Debug;
using Fasterflect;

namespace ModestTree.Zenject
{
    public static class ZenjectMenu
    {
        [MenuItem("Assets/Zenject/Validate Current Scene #%v")]
        public static void ValidateCurrentScene()
        {
            var compRoots = GameObject.FindObjectsOfType<CompositionRoot>();

            if (compRoots.HasMoreThan(1))
            {
                Debug.LogError("Found multiple composition roots when only one was expected while validating current scene");
                return;
            }

            if (compRoots.IsEmpty())
            {
                Debug.LogError("Could not find composition root while validating current scene");
                return;
            }

            var compRoot = compRoots.Single();

            var sceneInstallers = compRoot.GetComponents<MonoBehaviour>().Where(x => x.GetType().DerivesFrom<ISceneInstaller>()).Cast<ISceneInstaller>();

            if (sceneInstallers.HasMoreThan(1))
            {
                Debug.LogError("Found multiple scene installers when only one was expected while validating current scene");
                return;
            }

            if (sceneInstallers.IsEmpty())
            {
                Debug.LogError("Could not find scene installer while validating current scene");
                return;
            }

            var installer = sceneInstallers.Single();

            var resolveErrors = ZenUtil.ValidateInstaller(installer, false, compRoot).Take(10);

            // Only show a few to avoid spamming the log too much
            foreach (var error in resolveErrors)
            {
                Debug.LogException(error);
            }

            if (resolveErrors.Any())
            {
                Debug.LogError("Validation Completed With Errors");
            }
            else
            {
                Debug.Log("Validation Completed Successfully");
            }
        }

        [MenuItem("Assets/Zenject/Output Object Graph For Current Scene")]
        public static void OutputObjectGraphForScene()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogError("Zenject error: Must be in play mode to generate object graph.  Hit Play button and try again.");
                return;
            }

            DiContainer container;
            try
            {
                container = ZenEditorUtil.GetContainerForCurrentScene();
            }
            catch (ZenjectException e)
            {
                Debug.LogError("Unable to find container in current scene. " + e.GetFullMessage());
                return;
            }

            var ignoreTypes = Enumerable.Empty<Type>();
            var types = container.AllConcreteTypes;

            ZenEditorUtil.OutputObjectGraphForCurrentScene(container, ignoreTypes, types);
        }
    }
}
