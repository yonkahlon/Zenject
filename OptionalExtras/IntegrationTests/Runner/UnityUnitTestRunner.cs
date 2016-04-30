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
    public class UnityUnitTestRunner : MonoBehaviour
    {
        [SerializeField]
        bool _runAll;

        [SerializeField]
        UnityEvent _testMethod;

        public void Awake()
        {
            if (_runAll)
            {
                gameObject.AddComponent<UnityUnitTestMultiRunner>();
            }
            else
            {
                var installer = gameObject.AddComponent<UnityUnitTestSingleRunnerInstaller>();
                installer.TestMethod = _testMethod;

                var compRoot = SceneCompositionRoot.CreateComponent(gameObject);

                compRoot.ParentNewObjectsUnderRoot = true;

                compRoot.Installers = new List<MonoInstaller>()
                {
                    installer
                };

                compRoot.Run();
            }
        }
    }
}

