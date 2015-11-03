using System;
using System.Linq;

namespace Zenject
{
    public class StandardInstaller<TRoot> : Installer
        where TRoot : IDependencyRoot
    {
        // Install basic functionality for most unity apps
        public override void InstallBindings()
        {
            Container.Bind<IDependencyRoot>().ToSingle<TRoot>();
            Container.Bind<TRoot>().ToSingle();

            Container.Bind<TickableManager>().ToSingle();
            Container.Bind<InitializableManager>().ToSingle();
            Container.Bind<DisposableManager>().ToSingle();
        }
    }

    public class StandardInstaller : StandardInstaller<DependencyRoot>
    {
    }
}
