using System;
using UnityEngine;

namespace ModestTree.Zenject
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
