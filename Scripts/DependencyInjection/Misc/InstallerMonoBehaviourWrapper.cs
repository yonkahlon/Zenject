using ModestTree.Zenject;
using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    // In some cases it's useful to not use a monobehaviour for an installer
    // for example, if you're running unit tests outside of unity, or if you
    // want to re-use an existing installer in another installer, etc.
    // So for these cases you can use a normal C# class that implements IInstaller,
    // and then wrap it up in this class for use in dragging and dropping it into
    // unity scenes as well
    public class InstallerMonoBehaviourWrapper<TImpl> : MonoBehaviour, IInstaller where TImpl : IInstaller
    {
        public TImpl Impl;

        public void RegisterBindings(DiContainer container)
        {
            Impl.RegisterBindings(container);
        }
    }
}
