#if !ZEN_NOT_UNITY3D

using System;
using System.Collections;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEngine;

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public sealed class SceneDecoratorCompositionRoot : MonoBehaviour
    {
        public string SceneName;

        [SerializeField]
        public DecoratorInstaller[] DecoratorInstallers;

        [SerializeField]
        public MonoInstaller[] PreInstallers;

        [SerializeField]
        public MonoInstaller[] PostInstallers;

        Action<DiContainer> _beforeInstallHooks;
        Action<DiContainer> _afterInstallHooks;

        public void Awake()
        {
            StartCoroutine(Init());
        }

        IEnumerator Init()
        {
#pragma warning disable 168
            // Ensure the global comp root is initialized so that it doesn't get parented to us below
            var globalRoot = GlobalCompositionRoot.Instance;
#pragma warning restore 168

            _beforeInstallHooks = SceneCompositionRoot.BeforeInstallHooks;
            SceneCompositionRoot.BeforeInstallHooks = null;

            _afterInstallHooks = SceneCompositionRoot.AfterInstallHooks;
            SceneCompositionRoot.AfterInstallHooks = null;

            // We need to wait one frame before calling GetRootGameObjects in case we are being loaded from another decorator scene
            // Otherwise, the other decorator scene will assume that the second-nested scene objects are part of the first-nested scene and then
            // group them before the second nested scene has a chance to
            yield return null;

            var rootObjectsBeforeLoad = UnityUtil.GetRootGameObjects();

            ZenUtil.LoadSceneAdditive(
                SceneName, AddPreBindings, AddPostBindings);

            // Wait one frame for objects to be added to the scene heirarchy
            yield return null;

            var newlyAddedObjects = UnityUtil.GetRootGameObjects().Except(rootObjectsBeforeLoad);

            var sceneGameObject = new GameObject(SceneName + " (Scene)");

            foreach (var obj in newlyAddedObjects)
            {
                obj.transform.SetParent(sceneGameObject.transform);
            }
        }

        public void AddPreBindings(DiContainer container)
        {
            if (_beforeInstallHooks != null)
            {
                _beforeInstallHooks(container);
                _beforeInstallHooks = null;
            }

            container.Install(PreInstallers);

            ProcessDecoratorInstallers(container, true);
        }

        public void AddPostBindings(DiContainer container)
        {
            container.Install(PostInstallers);

            ProcessDecoratorInstallers(container, false);

            if (_afterInstallHooks != null)
            {
                _afterInstallHooks(container);
                _afterInstallHooks = null;
            }
        }

        void ProcessDecoratorInstallers(DiContainer container, bool isBefore)
        {
            if (DecoratorInstallers == null)
            {
                return;
            }

            foreach (var installer in DecoratorInstallers)
            {
                Assert.IsNotNull(installer, "Found null installer in composition root");

                if (installer.enabled)
                {
                    container.Inject(installer);

                    if (isBefore)
                    {
                        installer.PreInstallBindings();
                    }
                    else
                    {
                        installer.PostInstallBindings();
                    }
                }
            }
        }
    }
}

#endif
