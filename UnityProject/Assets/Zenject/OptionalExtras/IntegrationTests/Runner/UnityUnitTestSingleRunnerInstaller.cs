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
    public class UnityUnitTestSingleRunnerInstaller : MonoInstaller
    {
        [SerializeField]
        float _waitTime = 0.5f;

        [SerializeField]
        UnityEvent _testMethod;

        public UnityEvent TestMethod
        {
            get
            {
                return _testMethod;
            }
            set
            {
                _testMethod = value;
            }
        }

        public override void InstallBindings()
        {
            foreach (var fixture in GameObject.FindObjectsOfType<MonoTestFixture>())
            {
                fixture.Container = Container;
            }

            _testMethod.Invoke();

            this.StartCoroutine(DelayThenPrintResult());
        }

        IEnumerator DelayThenPrintResult()
        {
            yield return new WaitForSeconds(_waitTime);
            Log.Info("Completed running selected fixture");
        }
    }
}
