using System;
using System.Linq;

namespace Zenject
{
    // This is automatically installed on the scene composition root,
    // and the global composition root
    // However, for plain facades you need to install this yourself
    // if you want to inherit from Facade
    public class FacadeCommonInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<TickableManager>().ToSingle();
            Container.Bind<InitializableManager>().ToSingle();
            Container.Bind<DisposableManager>().ToSingle();
        }
    }
}
