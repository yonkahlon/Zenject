#if !ZEN_NOT_UNITY3D

using System;
using UnityEngine;

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public abstract class DecoratorInstaller : MonoBehaviour
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

        public virtual void PreInstallBindings()
        {
        }

        public virtual void PostInstallBindings()
        {
        }
    }
}

#endif
