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

            var modulesContainer = new DiContainer();
            installer.InstallModules(modulesContainer);

            modulesContainer.ValidateResolve<List<Module>>();

            var container = new DiContainer();

            var allModules = modulesContainer.ResolveMany<Module>();

            foreach (var module in allModules)
            {
                module.Container = container;
                module.AddBindings();
            }

            container.ValidateResolve<IDependencyRoot>();

            // Also make sure we can fill in all the dependencies in the built-in scene
            foreach (var monoBehaviour in compRoot.GetComponentsInChildren<MonoBehaviour>())
            {
                container.ValidateObjectGraph(monoBehaviour.GetType());
            }

            foreach (var module in allModules)
            {
                module.ValidateSubGraphs();
            }

            UnityEngine.Debug.Log("Validation succeeded for object graph given by '" + installer.GetType().Name() + "'");
        }

        [MenuItem("Assets/Zenject/Output Object Graph For Current Scene")]
        public static void OutputObjectGraphForScene()
        {
            if (EditorApplication.isPlaying)
            {
                var roots = GameObject.FindObjectsOfType<CompositionRoot>();

                if (roots.IsEmpty())
                {
                    Debug.LogError("Zenject error: Unable to find CompositionRoot in current scene.");
                }
                else if (roots.Length > 1)
                {
                    Debug.LogError("Zenject error: Found multiple CompositionRoot objects.  Not sure which one to use");
                }
                else
                {
                    string dotFilePath = EditorUtility.SaveFilePanel("Choose the path to export the object graph", "", "ObjectGraph", "dot");

                    if (!dotFilePath.IsEmpty())
                    {
                        ObjectGraphVisualizer.OutputObjectGraphToFile(roots[0].Container, dotFilePath);

                        var dotExecPath = EditorPrefs.GetString("Zenject.GraphVizDotExePath", "");

                        if (dotExecPath.IsEmpty() || !File.Exists(dotExecPath))
                        {
                            EditorUtility.DisplayDialog(
                                "GraphViz", "Unable to locate GraphViz.  Please select the graphviz 'dot.exe' file which can be found at [GraphVizInstallDirectory]/bin/dot.exe.  If you do not have GraphViz you can download it at http://www.graphviz.org", "Ok");

                            dotExecPath = EditorUtility.OpenFilePanel("Please select dot.exe from GraphViz bin directory", "", "exe");

                            EditorPrefs.SetString("Zenject.GraphVizDotExePath", dotExecPath);
                        }

                        if (!dotExecPath.IsEmpty())
                        {
                            RunDotExe(dotExecPath, dotFilePath);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Zenject error: Must be running a zenject scene to generate object graph");
            }
        }

        static void RunDotExe(string dotExePath, string dotFileInputPath)
        {
            var outputDir = Path.GetDirectoryName(dotFileInputPath);
            var fileBaseName = Path.GetFileNameWithoutExtension(dotFileInputPath);

            var proc = new System.Diagnostics.Process();

            proc.StartInfo.FileName = dotExePath;
            proc.StartInfo.Arguments = String.Format("-Tpng {0}.dot -o{0}.png", fileBaseName);
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.WorkingDirectory = outputDir;

            proc.Start();
            proc.WaitForExit();

            var errorMessage = proc.StandardError.ReadToEnd();
            proc.WaitForExit();

            if (errorMessage.IsEmpty())
            {
                EditorUtility.DisplayDialog(
                    "Success!", String.Format("Successfully created files {0}.dot and {0}.png", fileBaseName), "Ok");
            }
            else
            {
                Debug.LogError("Zenject error: Failure during object graph creation: " + errorMessage);

                // Do we care about STDOUT?
                //var outputMessage = proc.StandardOutput.ReadToEnd();
                //Debug.Log("outputMessage = " + outputMessage);
            }

        }
    }
}
