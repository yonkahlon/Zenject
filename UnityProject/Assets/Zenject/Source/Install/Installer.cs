using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    //
    // Derive from this class then install like this:
    //     FooInstaller.Install(Container);
    //
    public abstract class Installer<TDerived> : InstallerBase, IInstallerZeroParams
        where TDerived : Installer<TDerived>
    {
        public static void Install(DiContainer container)
        {
            container.Instantiate<TDerived>().InstallBindings();
        }
    }

    // Use this version to pass parameters to your installer
    public abstract class Installer<TParam1, TDerived> : InstallerBase, IInstallerOneParams
        where TDerived : Installer<TParam1, TDerived>
    {
        public static void Install(DiContainer container, TParam1 p1)
        {
            container.InstantiateExplicit<TDerived>(
                InjectUtil.CreateArgListExplicit(p1)).InstallBindings();
        }
    }
}
