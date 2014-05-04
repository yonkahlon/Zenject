using System;

namespace ModestTree.Zenject
{
    public class StandardUnityInstaller : Installer
    {
        // Install basic functionality for most unity apps
        public override void RegisterBindings()
        {
            _container.Bind<UnityKernel>().AsSingleGameObject();

            _container.Bind<StandardKernel>().AsSingle();
            // Uncomment this once you remove dependency in PlayerSandboxWrapper
            //_container.Bind<StandardKernel>().AsTransient().WhenInjectedInto<UnityKernel>();

            _container.Bind<InitializableHandler>().AsSingle();
            _container.Bind<ITickable>().AsLookup<UnityEventManager>();
        }
    }
}


