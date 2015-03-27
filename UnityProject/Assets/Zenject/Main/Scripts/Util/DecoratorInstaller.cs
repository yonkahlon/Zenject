#if !ZEN_NOT_UNITY3D

using System;
using UnityEngine;

namespace Zenject
{
    public abstract class DecoratorInstaller : MonoBehaviour
    {
        public DiContainer Container
        {
            get;
            set;
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
