using System;

namespace ModestTree.Zenject
{
    public static class InstallerUtil
    {
        // Install basic functionality for most unity apps
        public static void InstallUnityStandard(DiContainer container)
        {
            container.Bind<IKernel>().AsSingleGameObject<UnityKernel>("Kernel");
            container.Bind<EntryPointInitializer>().AsSingle();
            container.Bind<IEntryPoint>().AsSingle<KernelInitializer>();
            container.Bind<ITickable>().AsLookup<UnityEventManager>();
        }
    }
}


