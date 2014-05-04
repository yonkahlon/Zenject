using System;

namespace ModestTree.Zenject
{
    public static class InstallerUtil
    {
        // Install basic functionality for most unity apps
        public static void InstallUnityStandard(DiContainer container)
        {
            container.Bind<UnityKernel>().AsSingleGameObject();

            container.Bind<StandardKernel>().AsSingle();
            // Uncomment this once you remove dependency in PlayerSandboxWrapper
            //container.Bind<StandardKernel>().AsTransient().WhenInjectedInto<UnityKernel>();

            container.Bind<InitializableHandler>().AsSingle();
            container.Bind<ITickable>().AsLookup<UnityEventManager>();
        }
    }
}


