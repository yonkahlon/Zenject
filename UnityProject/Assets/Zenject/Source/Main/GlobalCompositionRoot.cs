#if !ZEN_NOT_UNITY3D

#pragma warning disable 414
using ModestTree;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zenject
{
    public class GlobalCompositionRoot : CompositionRoot
    {
        public const string GlobalInstallersResourceName = "ZenjectGlobalInstallers";

        static GlobalCompositionRoot _instance;

        DiContainer _container;
        IFacade _rootFacade;
        bool _hasInitialized;

        public override DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        public override IFacade RootFacade
        {
            get
            {
                return _rootFacade;
            }
        }

        public static GlobalCompositionRoot Instance
        {
            get
            {
                Assert.IsNotNull(_instance);
                return _instance;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void GlobalMainEntryPoint()
        {
            _instance = new GameObject("Global Composition Root")
                .AddComponent<GlobalCompositionRoot>();
            _instance.Initialize();
        }

        void Initialize()
        {
            Log.Debug("Initializing GlobalCompositionRoot");

            Assert.IsNull(Container);
            Assert.IsNull(RootFacade);

            DontDestroyOnLoad(gameObject);

            _container = CreateContainer(false, this);
            _rootFacade = _container.Resolve<IFacade>();

            Assert.IsNotNull(Container);
            Assert.IsNotNull(RootFacade);
        }

        public void InitializeRootIfNecessary()
        {
            if (!_hasInitialized)
            {
                _hasInitialized = true;
                _rootFacade.Initialize();
            }
        }

        public static DiContainer CreateContainer(bool isValidating, GlobalCompositionRoot root)
        {
            Assert.That(isValidating || root != null);

            var container = new DiContainer();

            container.IsValidating = isValidating;

            container.Bind<CompositionRoot>().ToInstance(root);
            container.Bind<GlobalCompositionRoot>().ToInstance(root);

            container.Install<StandardInstaller>();
            container.Install(GetGlobalInstallers());

            return container;
        }

        static IEnumerable<IInstaller> GetGlobalInstallers()
        {
            // For backwards compatibility include the old name
            var installerConfigs1 = Resources.LoadAll("ZenjectGlobalCompositionRoot", typeof(GlobalInstallerConfig));

            var installerConfigs2 = Resources.LoadAll(GlobalInstallersResourceName, typeof(GlobalInstallerConfig));

            return installerConfigs1.Concat(installerConfigs2).Cast<GlobalInstallerConfig>().SelectMany(x => x.Installers).Cast<IInstaller>();
        }
    }
}

#endif
