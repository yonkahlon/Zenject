using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace ModestTree
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InstallerTestAttribute : Attribute
    {
    }

    public abstract class MonoInstallerTestFixture : MonoBehaviour
    {
        protected DiContainer Container
        {
            get;
            private set;
        }

        public void Start()
        {
            StartCoroutine(Run());
        }

        IEnumerator Run()
        {
            GlobalCompositionRoot.Instance.EnsureIsInitialized();

            var testMethods = this.GetType().GetAllInstanceMethods()
                .Where(x => x.GetCustomAttributes(typeof(InstallerTestAttribute), false).Any()).ToList();

            foreach (var method in testMethods)
            {
                var wrapper = new InstallerWrapper();
                wrapper.InstallCallback = () =>
                {
                    Container = wrapper.GetContainer();
                    method.Invoke(this, new object[0]);
                };

                var settings = new SceneCompositionRoot.StaticSettings()
                {
                    Installers = new List<IInstaller>() { wrapper },
                    ParentNewObjectsUnderRoot = true,
                    OnlyInjectWhenActive = true,
                };

                var oldRootObjects = this.gameObject.scene.GetRootGameObjects();

                var root = SceneCompositionRoot.Instantiate(gameObject, settings);

                // Wait a few frames to have it start up
                yield return null;
                yield return null;
                yield return null;
                yield return null;

                GameObject.Destroy(root.gameObject);

                foreach (Transform childTransform in this.gameObject.transform)
                {
                    GameObject.Destroy(childTransform.gameObject);
                }

                foreach (var obj in this.gameObject.scene.GetRootGameObjects().Except(oldRootObjects))
                {
                    GameObject.Destroy(obj);
                }

                yield return null;

                Log.Trace("Installer Test '{0}' passed successfully", method.Name);
            }

            Log.Trace("All Installer Tests passed successfully");
            IntegrationTest.Pass();
        }

        class InstallerWrapper : Installer
        {
            public Action InstallCallback;

            public DiContainer GetContainer()
            {
                return Container;
            }

            public override void InstallBindings()
            {
                InstallCallback();
            }
        }
    }
}
