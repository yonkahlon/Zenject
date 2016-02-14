#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public abstract class MonoInstaller : MonoBehaviour, IInstaller
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
                return this.enabled;
            }
        }

        public virtual void Start()
        {
            // Define this method so we expose the enabled check box
        }

        public abstract void InstallBindings();
    }
}

#endif
