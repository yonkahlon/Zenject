
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModestTree.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Zenject;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModestTree.UnityUnitTester
{
    public class UnityUnitTestMultiRunner : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("_waitTime")]
        public float WaitTime = 0.5f;

        bool _hitAnyError;
        bool _suppressAllErrors;
        bool _hasFailed;

        public void Start()
        {
            StartCoroutine(Run());
            ListenOnAllErrors();

            Debug.Log("Starting UnityUnitTestMultiRunner...");
        }

        IEnumerator Run()
        {
            foreach (var fixture in GameObject.FindObjectsOfType<MonoTestFixture>())
            {
                var testMethods = fixture.GetType().GetAllInstanceMethods()
                    .Where(x => x.GetCustomAttributes(typeof(TestAttribute), false).Any());

                foreach (var methodInfo in testMethods)
                {
                    yield return StartCoroutine(RunFixture(fixture, methodInfo, true));

                    if (_hasFailed)
                    {
                        break;
                    }

                    yield return StartCoroutine(RunFixture(fixture, methodInfo, false));

                    if (_hasFailed)
                    {
                        break;
                    }
                }

                if (_hasFailed)
                {
                    break;
                }
            }

            if (_hasFailed)
            {
                Log.Error("Unity Unit Tests failed with errors");
            }
            else
            {
                Log.Info("Unity Unit Tests passed successfully");
            }

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }

        IEnumerator RunFixture(MonoTestFixture fixture, MethodInfo methodInfo, bool validateOnly)
        {
            // These should be reset each time
            Assert.That(!_suppressAllErrors, "_suppressAllErrors is false");

            bool isExpectingErrorDuringTest;

            if (validateOnly)
            {
                isExpectingErrorDuringTest = methodInfo.HasAttribute<ExpectedValidationExceptionAttribute>();
            }
            else
            {
                isExpectingErrorDuringTest = methodInfo.HasAttribute<ExpectedExceptionAttribute>();
            }

            var testName = "{0}.{1}".Fmt(fixture.GetType().Name(), methodInfo.Name);

            Log.Trace("{0} test '{1}'{2}", validateOnly ? "Validating" : "Running", testName, isExpectingErrorDuringTest ? "(expecting error)" : "");

            var compRoot = SceneCompositionRoot.Create();

            // Put under ourself otherwise it disables us during validation
            compRoot.transform.parent = this.transform;

            compRoot.IsValidating = validateOnly;
            compRoot.ValidateShutDownAfterwards = false;

            compRoot.NormalInstallers = new Installer[]
            {
                new ActionInstaller((container) =>
                    {
                        fixture.Container = container;
                        methodInfo.Invoke(fixture, new object[0]);
                        container.FlushBindings();
                    })
            };

            compRoot.ParentNewObjectsUnderRoot = true;

            _hitAnyError = false;
            _suppressAllErrors = isExpectingErrorDuringTest;

            try
            {
                compRoot.Run();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            if (!validateOnly)
            {
                yield return new WaitForSeconds(WaitTime);
            }

            _suppressAllErrors = false;
            bool hitErrorDuringTest = _hitAnyError;

            GameObject.Destroy(compRoot.gameObject);

            yield return null;

            if (isExpectingErrorDuringTest)
            {
                if (hitErrorDuringTest)
                {
                    Log.Trace("Hit expected error during test '{0}'. Ignoring.", testName);
                }
                else
                {
                    Log.Error("Expected to hit error during test '{0}' but none was found!", testName);
                    _hasFailed = true;
                }
            }
            else
            {
                if (hitErrorDuringTest)
                {
                    _hasFailed = true;
                }
            }
        }

        void ListenOnAllErrors()
        {
            Application.logMessageReceived += OnLogCallback;
        }

        public void OnLogCallback(string message, string stackTrace, LogType logType)
        {
            if (logType == LogType.Assert || logType == LogType.Error || logType == LogType.Exception)
            {
                _hitAnyError = true;
            }
        }

        public class ActionInstaller : Installer
        {
            readonly Action<DiContainer> _installMethod;

            public ActionInstaller(Action<DiContainer> installMethod)
            {
                _installMethod = installMethod;
            }

            public override void InstallBindings()
            {
                _installMethod(Container);
            }
        }
    }
}

