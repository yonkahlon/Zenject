using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestFacadeSubContainer : TestWithContainer
    {
        static int NumInstalls;

        [Test]
        public void Test1()
        {
            NumInstalls = 0;
            InitTest.WasRun = false;
            TickTest.WasRun = false;
            DisposeTest.WasRun = false;

            Container.Bind(typeof(TickableManager), typeof(InitializableManager), typeof(DisposableManager))
                .ToSelf().AsSingle().InheritInSubContainers();

            // This is how you add ITickables / etc. within sub containers
            Container.BindAllInterfacesAndSelf<FooFacade>()
                .To<FooFacade>().FromSubContainerResolve().ByMethod(InstallFoo).AsSingle();

            var tickManager = Container.Resolve<TickableManager>();
            var initManager = Container.Resolve<InitializableManager>();
            var disposeManager = Container.Resolve<DisposableManager>();

            Assert.That(!InitTest.WasRun);
            Assert.That(!TickTest.WasRun);
            Assert.That(!DisposeTest.WasRun);

            initManager.Initialize();
            tickManager.Update();
            disposeManager.Dispose();

            Assert.That(InitTest.WasRun);
            Assert.That(TickTest.WasRun);
            Assert.That(DisposeTest.WasRun);
        }

        public void InstallFoo(DiContainer subContainer)
        {
            NumInstalls++;

            subContainer.Bind<FooFacade>().AsSingle();

            subContainer.Bind<IInitializable>().To<InitTest>().AsSingle();
            subContainer.Bind<ITickable>().To<TickTest>().AsSingle();
            subContainer.Bind<IDisposable>().To<DisposeTest>().AsSingle();
        }

        public class FooFacade : Facade
        {
        }

        public class InitTest : IInitializable
        {
            public static bool WasRun;

            public void Initialize()
            {
                WasRun = true;
            }
        }

        public class TickTest : ITickable
        {
            public static bool WasRun;

            public void Tick()
            {
                WasRun = true;
            }
        }

        public class DisposeTest : IDisposable
        {
            public static bool WasRun;

            public void Dispose()
            {
                WasRun = true;
            }
        }
    }
}


