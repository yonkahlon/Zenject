using System;
using NUnit.Framework;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestFacade : TestWithContainer
    {
        public class FooFacade : Facade
        {
            public static int CreateCount;

            public FooFacade()
            {
                CreateCount++;
            }
        }

        public class MyTickable : ITickable
        {
            public static bool HasTicked;

            public void Tick()
            {
                HasTicked = true;
            }
        }

        public class MyInit : IInitializable
        {
            public static bool HasInitialized;

            public void Initialize()
            {
                HasInitialized = true;
            }
        }

        public class MyDispose : IDisposable
        {
            public static bool HasDisposed;

            public void Dispose()
            {
                HasDisposed = true;
            }
        }

        [Test]
        public void Test()
        {
            MyTickable.HasTicked = false;
            MyInit.HasInitialized = false;
            MyDispose.HasDisposed = false;
            FooFacade.CreateCount = 0;

            Binder.Install<FacadeCommonInstaller>();

            Binder.Bind<ITickable>().ToSingleFacadeMethod<FooFacade>(FacadeInstaller);
            Binder.Bind<IInitializable>().ToSingleFacadeMethod<FooFacade>(FacadeInstaller);
            Binder.Bind<IDisposable>().ToSingleFacadeMethod<FooFacade>(FacadeInstaller);

            AssertValidates();

            Assert.That(!MyInit.HasInitialized);
            Resolver.Resolve<InitializableManager>().Initialize();
            Assert.That(MyInit.HasInitialized);

            Assert.That(!MyTickable.HasTicked);
            Resolver.Resolve<TickableManager>().Update();
            Assert.That(MyTickable.HasTicked);

            Assert.That(!MyDispose.HasDisposed);
            Resolver.Resolve<DisposableManager>().Dispose();
            Assert.That(MyDispose.HasDisposed);

            Assert.IsEqual(FooFacade.CreateCount, 1);
        }

        [Test]
        public void TestAll()
        {
            MyTickable.HasTicked = false;
            MyInit.HasInitialized = false;
            MyDispose.HasDisposed = false;
            FooFacade.CreateCount = 0;

            Binder.Install<FacadeCommonInstaller>();

            Binder.BindAllInterfacesToSingleFacadeMethod<FooFacade>(FacadeInstaller);

            AssertValidates();

            Assert.That(!MyInit.HasInitialized);
            Resolver.Resolve<InitializableManager>().Initialize();
            Assert.That(MyInit.HasInitialized);

            Assert.That(!MyTickable.HasTicked);
            Resolver.Resolve<TickableManager>().Update();
            Assert.That(MyTickable.HasTicked);

            Assert.That(!MyDispose.HasDisposed);
            Resolver.Resolve<DisposableManager>().Dispose();
            Assert.That(MyDispose.HasDisposed);

            Assert.IsEqual(FooFacade.CreateCount, 1);
        }

        void FacadeInstaller(IBinder binder)
        {
            binder.Install<FacadeCommonInstaller>();
            binder.Bind<FooFacade>().ToSingle();

            binder.Bind<IDisposable>().ToSingle<MyDispose>();
            binder.Bind<IInitializable>().ToSingle<MyInit>();
            binder.Bind<ITickable>().ToSingle<MyTickable>();
        }
    }
}

