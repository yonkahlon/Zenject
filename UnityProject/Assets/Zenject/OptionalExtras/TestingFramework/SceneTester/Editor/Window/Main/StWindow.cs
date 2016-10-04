using System;
using System.IO;
using ModestTree;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using ModestTree.Util;

namespace SceneTester
{
    public class StWindow : ZenjectEditorWindow
    {
        StModel.Data _serializedData;

        public static StWindow Instance
        {
            get
            {
                var window = EditorWindow.GetWindow<StWindow>();
                window.titleContent = new GUIContent("Scene Tester");
                return window;
            }
        }

        [MenuItem("Window/Scene Tester", false, 1)]
        static void OpenWindow()
        {
            Instance.EnsureIsOpen();
        }

        public void EnsureIsOpen()
        {
            // Do nothing
        }

        public void RunScenes(List<string> sceneNames, bool exitAfter, bool isValidating)
        {
            Container.Resolve<StSceneRunner>().StartScenes(sceneNames, exitAfter, isValidating);
        }

        public static void BatchModeValidate()
        {
            BatchModeExecInternal(true);
        }

        public static void BatchModeRun()
        {
            BatchModeExecInternal(false);
        }

        static void BatchModeExecInternal(bool isValidating)
        {
            try
            {
                var sceneNameArg = UnityCustomCommandLineHandler.TryGetArgument("scenes");
                var sceneNames = sceneNameArg == null ? new List<string>() : sceneNameArg.Split(',').ToList();

                var sceneDirArg = UnityCustomCommandLineHandler.TryGetArgument("sceneDirs");
                var dirNames = sceneDirArg == null ? new List<string>() : sceneDirArg.Split(',').ToList();

                Assert.That(!sceneNames.IsEmpty() || !dirNames.IsEmpty());

                var allSceneNames = sceneNames.Concat(
                    StUtil.GetAllSceneNamesUnderneathFolders(dirNames)).ToList();

                Log.Trace("Scene Tester:  Testing {0} scenes: '{1}'",
                allSceneNames.Count, allSceneNames.Join(", "));

                StWindow.Instance.RunScenes(allSceneNames, true, isValidating);
            }
            catch (Exception e)
            {
                Log.ErrorException(
                    "Failed when {0}!".Fmt(isValidating ? "running zenject validation" : "running startup tests"), e);

                EditorApplication.Exit(1);
            }
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_serializedData).WhenInjectedInto<StModel>();
            Container.BindInstance(_serializedData.List).WhenInjectedInto<StListModel>();

            if (_serializedData.IsRunningTest)
            {
                Container.BindAllInterfacesAndSelf<StSceneStatusMonitor>().To<StSceneStatusMonitor>().AsSingle();
            }

            Container.Bind<EditorWindow>().FromInstance(this);

            StWindowInstaller.InstallFromResource(Container);
        }

        public override void OnEnable()
        {
            if (_serializedData == null || _serializedData.List == null)
            {
                _serializedData = new StModel.Data()
                {
                    IsRunningTest = false,
                    QueuedScenes = new List<string>(),
                    List = new StListModel.Data()
                    {
                        ScrollPos = Vector2.zero,
                        SearchFilter = "",

                        SortMethod = 0,
                        SortDescending = true,

                        SceneInfos = new List<SceneData>(),
                    },
                };
            }

            base.OnEnable();
        }
    }
}
