using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Zenject;

namespace SceneTester
{
    public class StSceneRunner : ITickable
    {
        readonly StCommands.SceneOpener _sceneOpener;
        readonly StModel _model;
        readonly StListView _listView;

        int? _playSceneNextFrameCountdown;

        public StSceneRunner(
            StListView listView,
            StModel model,
            StCommands.SceneOpener sceneOpener)
        {
            _sceneOpener = sceneOpener;
            _model = model;
            _listView = listView;
        }

        public bool HasNextScene
        {
            get
            {
                return !_model.QueuedScenes.IsEmpty();
            }
        }

        public void StartScenes(List<string> sceneNames, bool exitAfter, bool isValidating)
        {
            ValidateSceneNames(sceneNames);

            _model.IsRunningTest = true;
            _model.QueuedScenes = sceneNames;
            _model.ExitAfter = exitAfter;
            _model.IsValidating = isValidating;

            Log.Trace("Queued {0} scenes", _model.QueuedScenes.Count());

            StartNextScene();
        }

        void ValidateSceneNames(List<string> sceneNames)
        {
            var allSceneNames = _model.List.SceneInfos.Select(x => x.Name);

            foreach (var name in sceneNames)
            {
                Assert.That(allSceneNames.Contains(name), "Could not find scene with name '{0}'", name);
            }
        }

        public void ValidateScenes()
        {
            var selectedScenes = _listView.GetSelectedNames();

            if (!selectedScenes.IsEmpty())
            {
                StartScenes(selectedScenes, false, true);
            }
        }

        public void StartScenes()
        {
            var selectedScenes = _listView.GetSelectedNames();

            if (!selectedScenes.IsEmpty())
            {
                StartScenes(selectedScenes, false, false);
            }
        }

        public void StartNextScene()
        {
            Assert.That(_model.IsRunningTest);
            Assert.That(!_model.QueuedScenes.IsEmpty());

            var sceneName = _model.DequeueScene();

            _model.CurrentScene = sceneName;

            _sceneOpener.Execute(sceneName);

            // This is hacky but necessary when using Unity's new multi-scene editting stuff
            // Unity needs a few frames to properly load every scene that is added as secondary
            // scenes within the main scene
            _playSceneNextFrameCountdown = 5;
        }

        public void Tick()
        {
            if (_playSceneNextFrameCountdown.HasValue)
            {
                if (_playSceneNextFrameCountdown <= 0)
                {
                    _playSceneNextFrameCountdown = null;

                    ProjectContext.PersistentIsValidating = _model.IsValidating;

                    EditorApplication.isPlaying = true;
                }
                else
                {
                    _playSceneNextFrameCountdown--;
                }
            }
        }
    }
}
