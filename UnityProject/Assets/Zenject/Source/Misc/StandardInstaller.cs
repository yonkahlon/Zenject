using System;
using System.Linq;

namespace Zenject
{
    // This is automatically installed on the scene composition root,
    // global composition root, facades, and monofacades
    public class StandardInstaller : Installer
    {
        public override void InstallBindings()
        {
            Binder.Bind<TickableManager>().ToSingle();
            Binder.Bind<InitializableManager>().ToSingle();
            Binder.Bind<DisposableManager>().ToSingle();
        }
    }
}
