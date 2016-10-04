using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEditor;
using UnityEngine;
using Zenject;
using Zenject.TestFramework;

namespace SceneTester
{
    public class StSceneStatusMonitor : ITickable, IInitializable
    {
        readonly StSceneRunner _sceneRunner;
        readonly StModel _model;

        DateTime _startTime;
        bool _hitError;
        bool _hasDisplayedError;
        States _state = States.InProgress;

        public StSceneStatusMonitor(
            StModel model,
            StSceneRunner sceneRunner)
        {
            _sceneRunner = sceneRunner;
            _model = model;
        }

        public void Initialize()
        {
            Assert.That(_model.IsRunningTest);

            _startTime = DateTime.UtcNow;

            ListenOnErrors();
        }

        bool ShouldIgnoreErrors()
        {
            return EditorPrefs.GetBool(ZenjectIntegrationTestMultiRunner.EditorPrefsKeyIgnoreError, false);
        }

        public void OnReceivedLog(string condition, string stackTrace, LogType type)
        {
            if (_state == States.InProgress
                && (type == LogType.Assert || type == LogType.Error || type == LogType.Exception))
            {
                if (ShouldIgnoreErrors())
                {
                    // This error is expected so can be ignored!
                }
                else
                {
                    _state = States.HitError;
                }
            }
        }

        void UnlistenOnErrors()
        {
            Application.logMessageReceived -= OnReceivedLog;
        }

        void ListenOnErrors()
        {
            Application.logMessageReceived += OnReceivedLog;
        }

        void StartShutdown()
        {
            UnlistenOnErrors();
            EditorApplication.isPlaying = false;
        }

        public void Tick()
        {
            switch (_state)
            {
                case States.InProgress:
                {
                    if (!EditorApplication.isPlaying)
                    {
                        StartShutdown();

                        // Need to wait one frame for it to shutdown
                        // before starting another scene
                        _state = States.ShuttingDownPassed;
                    }
                    else
                    {
                        // Wait 10 seconds
                        // This might not be enough for some scenes but good enough for now
                        if ((DateTime.UtcNow - _startTime).TotalSeconds > 10)
                        {
                            // Note here that we intentionally do not change state here
                            // so that we can catch any errors that occur during the frame in
                            // which we shut down
                            // (Next frame it will enter the if statement above)
                            EditorApplication.isPlaying = false;
                        }
                    }

                    break;
                }
                case States.ShuttingDownPassed:
                {
                    _state = States.Done;

                    Log.Info("Scene Tester: Scene '{0}' Succeeded", _model.CurrentScene);

                    if (_sceneRunner.HasNextScene)
                    {
                        _sceneRunner.StartNextScene();
                    }
                    else
                    {
                        _model.IsRunningTest = false;
                        Log.Info("Scene Tester: Done!  All Scenes Run Successfully");

                        if (_model.ExitAfter)
                        {
                            EditorApplication.Exit(0);
                        }
                    }
                    break;
                }
                case States.HitError:
                {
                    _model.IsRunningTest = false;
                    Log.Error("Scene Tester: Scene '{0}' failed!", _model.CurrentScene);

                    StartShutdown();
                    _state = States.Done;

                    if (_model.ExitAfter)
                    {
                        EditorApplication.Exit(1);
                    }

                    break;
                }
                case States.Done:
                {
                    // Do nothing
                    break;
                }
                default:
                {
                    throw Assert.CreateException();
                }
            }
        }

        enum States
        {
            InProgress,
            Done,
            HitError,
            ShuttingDownPassed,
        }
    }
}
