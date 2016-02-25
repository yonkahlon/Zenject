using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestIFactoryFacade : TestWithContainer
    {
        [Test]
        public void TestToFacadeFactoryMethod1()
        {
            bool wasCalled = false;

            Container.BindIFactory<FooFacade>().ToFacadeFactoryMethod<FooFacade.Factory>((c) =>
                {
                    wasCalled = true;

                    c.Bind<FooFacade>().ToSingle();
                });

            AssertValidates();

            Assert.That(!wasCalled);
            Assert.IsNotNull(Container.Resolve<IFactory<FooFacade>>().Create() is FooFacade);
            Assert.That(wasCalled);
        }

        [Test]
        public void TestToFacadeFactoryMethod2()
        {
            bool wasCalled = false;

            Container.BindIFactory<FooFacade>().ToFacadeFactoryMethod<FooFacade, FooFacade.Factory>((c) =>
                {
                    wasCalled = true;
                    c.Bind<FooFacade>().ToSingle();
                });

            AssertValidates();

            Assert.That(!wasCalled);
            Assert.IsNotNull(Container.Resolve<IFactory<FooFacade>>().Create());
            Assert.That(wasCalled);
        }

        [Test]
        public void TestToFacadeFactoryInstallerParam1()
        {
            FooInstaller.WasCalled = false;

            Container.BindIFactory<FooFacade>().ToFacadeFactoryInstaller<FooFacade.Factory, FooInstaller>();

            AssertValidates();

            Assert.That(!FooInstaller.WasCalled);
            Assert.IsNotNull(Container.Resolve<IFactory<FooFacade>>().Create() is FooFacade);
            Assert.That(FooInstaller.WasCalled);
        }

        [Test]
        public void TestToFacadeFactoryInstallerParam2()
        {
            FooInstaller.WasCalled = false;

            Container.BindIFactory<IFoo>().ToFacadeFactoryInstaller<FooFacade, FooFacade.Factory, FooInstaller>();

            AssertValidates();

            Assert.That(!FooInstaller.WasCalled);
            Assert.IsNotNull(Container.Resolve<IFactory<IFoo>>().Create());
            Assert.That(FooInstaller.WasCalled);
        }

        public interface IFoo
        {
        }

        public class FooFacade : IFoo
        {
            public class Factory : FacadeFactory<FooFacade>
            {
            }
        }

        public class FooInstaller : Installer
        {
            public static bool WasCalled;

            public override void InstallBindings()
            {
                Container.Bind<FooFacade>().ToSingle();
                WasCalled = true;
            }
        }
    }
}

