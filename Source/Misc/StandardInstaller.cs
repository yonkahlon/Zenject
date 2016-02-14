using System;
using System.Linq;

namespace Zenject
{
    public class StandardInstaller<TRoot> : Installer
        where TRoot : IFacade
    {
        public override void InstallBindings()
        {
            Binder.Bind<IFacade>().ToSingle<TRoot>();
            Binder.Bind<TRoot>().ToSingle();

            Binder.Bind<TickableManager>().ToSingle();
            Binder.Bind<InitializableManager>().ToSingle();
            Binder.Bind<DisposableManager>().ToSingle();
        }
    }

    public class StandardInstaller : StandardInstaller<Facade>
    {
    }
}
