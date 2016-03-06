using System;
using NUnit.Framework;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestFacadeFactory : TestWithContainer
    {
        public class FooFacade : Facade
        {
            public class Factory : FacadeFactory<FooFacade>
            {
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
        public void TestMethod()
        {
            MyTickable.HasTicked = false;
            MyInit.HasInitialized = false;
            MyDispose.HasDisposed = false;

            Container.Bind<TickableManager>().ToTransient();
            Container.Bind<InitializableManager>().ToTransient();
            Container.Bind<DisposableManager>().ToTransient();

            Container.BindFacadeFactoryMethod<FooFacade.Factory>(FacadeInstaller);

            AssertValidates();

            var facadeFactory = Container.Resolve<FooFacade.Factory>();

            Assert.That(!MyInit.HasInitialized);
            var facade = facadeFactory.Create();
            facade.Initialize();
            Assert.That(MyInit.HasInitialized);

            Assert.That(!MyTickable.HasTicked);
            facade.Tick();
            Assert.That(MyTickable.HasTicked);

            Assert.That(!MyDispose.HasDisposed);
            facade.Dispose();
            Assert.That(MyDispose.HasDisposed);
        }

        [Test]
        public void TestInstaller()
        {
            MyTickable.HasTicked = false;
            MyInit.HasInitialized = false;
            MyDispose.HasDisposed = false;

            Container.Bind<TickableManager>().ToTransient();
            Container.Bind<InitializableManager>().ToTransient();
            Container.Bind<DisposableManager>().ToTransient();

            Container.BindFacadeFactoryInstaller<FooFacade.Factory, FooInstaller>();

            AssertValidates();

            var facadeFactory = Container.Resolve<FooFacade.Factory>();

            Assert.That(!MyInit.HasInitialized);
            var facade = facadeFactory.Create();
            facade.Initialize();
            Assert.That(MyInit.HasInitialized);

            Assert.That(!MyTickable.HasTicked);
            facade.Tick();
            Assert.That(MyTickable.HasTicked);

            Assert.That(!MyDispose.HasDisposed);
            facade.Dispose();
            Assert.That(MyDispose.HasDisposed);
        }

        void FacadeInstaller(DiContainer container)
        {
            container.Bind<FooFacade>().ToSingle();

            container.Bind<IDisposable>().ToSingle<MyDispose>();
            container.Bind<IInitializable>().ToSingle<MyInit>();
            container.Bind<ITickable>().ToSingle<MyTickable>();
        }

        class FooInstaller : Installer
        {
            public override void InstallBindings()
            {
                Container.Bind<FooFacade>().ToSingle();

                Container.Bind<IDisposable>().ToSingle<MyDispose>();
                Container.Bind<IInitializable>().ToSingle<MyInit>();
                Container.Bind<ITickable>().ToSingle<MyTickable>();
            }
        }
    }
}
