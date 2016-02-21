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

            Binder.BindFacadeFactoryMethod<FooFacade, FooFacade.Factory>(FacadeInstaller);

            AssertValidates();

            var facadeFactory = Resolver.Resolve<FooFacade.Factory>();

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

            Binder.BindFacadeFactoryInstaller<FooFacade, FooFacade.Factory, FooInstaller>();

            AssertValidates();

            var facadeFactory = Resolver.Resolve<FooFacade.Factory>();

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

        void FacadeInstaller(IBinder binder)
        {
            binder.Install<FacadeCommonInstaller>();
            binder.Bind<FooFacade>().ToSingle();

            binder.Bind<IDisposable>().ToSingle<MyDispose>();
            binder.Bind<IInitializable>().ToSingle<MyInit>();
            binder.Bind<ITickable>().ToSingle<MyTickable>();
        }

        class FooInstaller : Installer
        {
            public override void InstallBindings()
            {
                Binder.Install<FacadeCommonInstaller>();
                Binder.Bind<FooFacade>().ToSingle();

                Binder.Bind<IDisposable>().ToSingle<MyDispose>();
                Binder.Bind<IInitializable>().ToSingle<MyInit>();
                Binder.Bind<ITickable>().ToSingle<MyTickable>();
            }
        }
    }
}
