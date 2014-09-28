using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // Define this class as a component of a top-level game object of your scene heirarchy
    // Then any children will get injected during resolve stage
    public sealed class CompositionRoot : MonoBehaviour
    {
        public static Action<DiContainer> ExtraBindingsLookup;

        DiContainer _container;
        IDependencyRoot _dependencyRoot;

        [SerializeField]
        public MonoInstaller[] Installers;

        public DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        void InstallInstallers()
        {
            if (Installers.Where(x => x != null).IsEmpty())
            {
                Log.Warn("No installers found while initializing CompositionRoot");
                return;
            }

            foreach (var installer in Installers)
            {
                if (installer == null)
                {
                    Log.Warn("Found null installer hooked up to CompositionRoot");
                    continue;
                }

                if (installer.enabled)
                {
                    // The installers that are part of the scene are monobehaviours
                    // and therefore were not created via Zenject and therefore do
                    // not have their members injected
                    // At the very least they will need the container injected but
                    // they might also have some configuration passed from another
                    // scene as well
                    FieldsInjecter.Inject(_container, installer);
                    _container.Bind<IInstaller>().To(installer);

                    // Install this installer and also any other installers that it installs
                    _container.InstallInstallers();

                    Assert.That(!_container.HasBinding<IInstaller>());
                }
            }
        }

        void InitContainer()
        {
            _container = new DiContainer();

            _container.FallbackProvider = new DiContainerProvider(
                GlobalCompositionRoot.Instance.Container);

            _container.Bind<CompositionRoot>().To(this);

            // Install the extra bindings immediately in case they configure the
            // installers used in this scene
            if (ExtraBindingsLookup != null)
            {
                ExtraBindingsLookup(_container);

                // Reset extra bindings for next time we change scenes
                ExtraBindingsLookup = null;
            }

            _container.Bind<IInstaller>().ToSingle<StandardUnityInstaller>();
            _container.Bind<GameObject>().To(this.gameObject).WhenInjectedInto<StandardUnityInstaller>();
            _container.InstallInstallers();
            Assert.That(!_container.HasBinding<IInstaller>());
        }

        public void Awake()
        {
            Log.Debug("Zenject Started");

            InitContainer();
            InstallInstallers();

            InjectionHelper.InjectChildGameObjects(_container, gameObject);
            _dependencyRoot = _container.Resolve<IDependencyRoot>();
        }
    }
}
