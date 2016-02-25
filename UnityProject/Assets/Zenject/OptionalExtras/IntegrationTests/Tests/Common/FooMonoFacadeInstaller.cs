using System;
using Zenject;

namespace ModestTree
{
    public class FooMonoFacadeInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindAllInterfaces<FooMonoFacadeRunner>().ToSingle<FooMonoFacadeRunner>();
            Container.Bind<FooMonoFacadeRunner>().ToSingle();
        }
    }

    public class FooMonoFacadeRunner : ITickable, IInitializable, IDisposable
    {
        public bool HasDisposed
        {
            get;
            private set;
        }

        public bool HasInitialized
        {
            get;
            private set;
        }

        public bool HasTicked
        {
            get;
            private set;
        }

        public void Initialize()
        {
            HasInitialized = true;
        }

        public void Tick()
        {
            HasTicked = true;
        }

        public void Dispose()
        {
            HasDisposed = true;
        }
    }
}
