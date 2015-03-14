#pragma warning disable 414
using ModestTree;

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Zenject
{
    // Define this class as a component of a top-level game object of your scene heirarchy
    // Then any children will get injected during resolve stage
    public sealed class CompositionRoot : MonoBehaviour
    {
        public static Action<DiContainer> BeforeInstallHooks;
        public static Action<DiContainer> AfterInstallHooks;

        public bool OnlyInjectWhenActive = true;

        [SerializeField]
        public MonoInstaller[] Installers = new MonoInstaller[0];

        DiContainer _container;
        IDependencyRoot _dependencyRoot = null;

        static List<IInstaller> _staticInstallers = new List<IInstaller>();

        public DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        // This method is used for cases where you need to create the CompositionRoot entirely in code
        // Necessary because the Awake() method is called immediately after AddComponent<CompositionRoot>
        // so there's no other way to add installers to it
        public static CompositionRoot AddComponent(
            GameObject gameObject, IInstaller rootInstaller)
        {
            return AddComponent(gameObject, new List<IInstaller>() { rootInstaller });
        }

        public static CompositionRoot AddComponent(
            GameObject gameObject, List<IInstaller> installers)
        {
            Assert.That(_staticInstallers.IsEmpty());
            _staticInstallers.AddRange(installers);
            return gameObject.AddComponent<CompositionRoot>();
        }

        public void Awake()
        {
            Log.Debug("Zenject Started");

            _container = CreateContainer(
                false, GlobalCompositionRoot.Instance.Container, _staticInstallers);
            _staticInstallers.Clear();

            InjectionHelper.InjectChildGameObjects(_container, gameObject, !OnlyInjectWhenActive);
            _dependencyRoot = _container.Resolve<IDependencyRoot>();
        }

        public DiContainer CreateContainer(
            bool allowNullBindings, DiContainer parentContainer, List<IInstaller> extraInstallers)
        {
            var container = new DiContainer();
            container.AllowNullBindings = allowNullBindings;
            container.FallbackProvider = new DiContainerProvider(parentContainer);
            container.Bind<CompositionRoot>().To(this);

            if (BeforeInstallHooks != null)
            {
                BeforeInstallHooks(container);
                // Reset extra bindings for next time we change scenes
                BeforeInstallHooks = null;
            }

            CompositionRootHelper.InstallStandardInstaller(container, this.gameObject);

            var allInstallers = extraInstallers.Concat(Installers).ToList();

            if (allInstallers.Where(x => x != null).IsEmpty())
            {
                Log.Warn("No installers found while initializing CompositionRoot");
            }
            else
            {
                CompositionRootHelper.InstallSceneInstallers(container, allInstallers);
            }

            if (AfterInstallHooks != null)
            {
                AfterInstallHooks(container);
                // Reset extra bindings for next time we change scenes
                AfterInstallHooks = null;
            }

            return container;
        }
    }
}
