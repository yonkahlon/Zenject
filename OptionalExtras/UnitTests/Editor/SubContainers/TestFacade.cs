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

            Container.Bind<TickableManager>().ToTransient();
            Container.Bind<InitializableManager>().ToTransient();
            Container.Bind<DisposableManager>().ToTransient();

            Container.Bind<ITickable>().ToSingleFacadeMethod<FooFacade>(FacadeInstaller);
            Container.Bind<IInitializable>().ToSingleFacadeMethod<FooFacade>(FacadeInstaller);
            Container.Bind<IDisposable>().ToSingleFacadeMethod<FooFacade>(FacadeInstaller);

            AssertValidates();

            Assert.That(!MyInit.HasInitialized);
            Container.Resolve<InitializableManager>().Initialize();
            Assert.That(MyInit.HasInitialized);

            Assert.That(!MyTickable.HasTicked);
            Container.Resolve<TickableManager>().Update();
            Assert.That(MyTickable.HasTicked);

            Assert.That(!MyDispose.HasDisposed);
            Container.Resolve<DisposableManager>().Dispose();
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

            Container.Bind<TickableManager>().ToTransient();
            Container.Bind<InitializableManager>().ToTransient();
            Container.Bind<DisposableManager>().ToTransient();

            Container.BindAllInterfaces<FooFacade>().ToSingleFacadeMethod<FooFacade>(FacadeInstaller);

            AssertValidates();

            Assert.That(!MyInit.HasInitialized);
            Container.Resolve<InitializableManager>().Initialize();
            Assert.That(MyInit.HasInitialized);

            Assert.That(!MyTickable.HasTicked);
            Container.Resolve<TickableManager>().Update();
            Assert.That(MyTickable.HasTicked);

            Assert.That(!MyDispose.HasDisposed);
            Container.Resolve<DisposableManager>().Dispose();
            Assert.That(MyDispose.HasDisposed);

            Assert.IsEqual(FooFacade.CreateCount, 1);
        }

        void FacadeInstaller(DiContainer container)
        {
            container.Bind<FooFacade>().ToSingle();

            container.Bind<IDisposable>().ToSingle<MyDispose>();
            container.Bind<IInitializable>().ToSingle<MyInit>();
            container.Bind<ITickable>().ToSingle<MyTickable>();
        }
    }
}

