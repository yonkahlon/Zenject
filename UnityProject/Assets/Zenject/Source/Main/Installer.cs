using System;
using System.Collections.Generic;
using System.Linq;

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public abstract class Installer : IInstaller
    {
        [Inject]
        IBinder _binder = null;

        protected IBinder Binder
        {
            get
            {
                return _binder;
            }
        }

        public virtual bool IsEnabled
        {
            get
            {
                return true;
            }
        }

        public abstract void InstallBindings();
    }
}
