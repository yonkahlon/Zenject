using System.Collections.Generic;

namespace ModestTree.Zenject
{
    public class DependencyRootStandard : IDependencyRoot
    {
        // Usually we pass dependencies via contructor injection
        // but since we define a root so often (eg. unit tests)
        // just use [Inject] for the root classes

        [Inject]
        UnityKernel _unityKernel = null;

        [Inject]
        InitializableHandler _initializer = null;

        public virtual void Start()
        {
            _initializer.Initialize();
        }
    }

    public class DependencyRootStandard<TRoot> : DependencyRootStandard
        where TRoot : class
    {
        [Inject]
        TRoot _root = null;
    }
}
