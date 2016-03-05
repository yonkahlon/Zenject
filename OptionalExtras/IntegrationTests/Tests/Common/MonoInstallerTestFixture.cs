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

        protected virtual float Delay
        {
            get
            {
                return 0.1f;
            }
        }

        IEnumerator Run()
        {
            GlobalCompositionRoot.Instance.EnsureIsInitialized();

            var testMethods = this.GetType().GetAllMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.GetCustomAttributes(typeof(InstallerTestAttribute), false).Any()).ToList();

            var wrapper = this.gameObject.AddComponent<InstallerWrapper>();

            foreach (var method in testMethods)
            {
                wrapper.InstallCallback = () =>
                {
                    Container = wrapper.GetContainer();
                    method.Invoke(this, new object[0]);

                    // Might as well validate after install finishes too
                    Container.IsValidating = true;
                    try
                    {
                        var firstError = Container.ValidateAll().FirstOrDefault();

                        if (firstError != null)
                        {
                            Log.Error("Error occurred during validation!");
                            throw firstError;
                        }
                    }
                    finally
                    {
                        Container.IsValidating = false;
                    }
                };

                var settings = new SceneCompositionRoot.StaticSettings()
                {
                    Installers = new MonoInstaller[] { wrapper },
                    ParentNewObjectsUnderRoot = true,
                    OnlyInjectWhenActive = true,
                };

                var oldRootObjects = this.gameObject.scene.GetRootGameObjects();

                var root = SceneCompositionRoot.Instantiate(gameObject, settings);

                // Wait a few frames to have it start up
                yield return new WaitForSeconds(Delay);

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

        class InstallerWrapper : MonoInstaller
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

