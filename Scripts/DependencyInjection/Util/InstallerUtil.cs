using System;

namespace ModestTree.Zenject
{
    public static class InstallerUtil
    {
        // Install basic functionality for most unity apps
        public static void InstallUnityStandard(DiContainer container)
        {
            container.Bind<UnityKernel>().AsSingleGameObject();
            container.Bind<StandardKernel>().AsTransient().WhenInjectedInto<UnityKernel>();

            container.Bind<EntryPointInitializer>().AsSingle();
            container.Bind<ITickable>().AsLookup<UnityEventManager>();
        }
    }
}


