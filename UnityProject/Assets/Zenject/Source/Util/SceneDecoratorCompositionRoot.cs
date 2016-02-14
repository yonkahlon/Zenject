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

        Action<IBinder> _beforeInstallHooks;
        Action<IBinder> _afterInstallHooks;

        public void Awake()
        {
            // We always want to initialize GlobalCompositionRoot as early as possible
            GlobalCompositionRoot.Instance.EnsureIsInitialized();

            _beforeInstallHooks = SceneCompositionRoot.BeforeInstallHooks;
            SceneCompositionRoot.BeforeInstallHooks = null;

            _afterInstallHooks = SceneCompositionRoot.AfterInstallHooks;
            SceneCompositionRoot.AfterInstallHooks = null;

            SceneCompositionRoot.DecoratedScenes.Add(this.gameObject.scene);

            ZenUtil.LoadSceneAdditive(
                SceneName, AddPreBindings, AddPostBindings);
        }

        public void AddPreBindings(IBinder binder)
        {
            if (_beforeInstallHooks != null)
            {
                _beforeInstallHooks(binder);
                _beforeInstallHooks = null;
            }

            binder.Install(PreInstallers);

            ProcessDecoratorInstallers(binder, true);
        }

        public void AddPostBindings(IBinder binder)
        {
            binder.Install(PostInstallers);

            ProcessDecoratorInstallers(binder, false);

            if (_afterInstallHooks != null)
            {
                _afterInstallHooks(binder);
                _afterInstallHooks = null;
            }
        }

        void ProcessDecoratorInstallers(IBinder binder, bool isBefore)
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
                    binder.Resolver.Inject(installer);

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
