using System;
using NUnit.Framework;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestFacade2 : TestWithContainer
    {
        public class Bar
        {
        }

        public class FooFacade
        {
            Bar _bar;

            public FooFacade(Bar bar)
            {
                _bar = bar;
            }

            public Bar Bar
            {
                get
                {
                    return _bar;
                }
            }
        }

        [Test]
        public void TestMethod()
        {
            Container.Bind<FooFacade>().ToSingleFacadeMethod(FacadeInstaller);

            AssertValidates();

            var foo = Container.Resolve<FooFacade>();

            Assert.That(Container.TryResolve<Bar>() == null);

            Assert.IsNotNull(foo.Bar);
        }

        [Test]
        public void TestInstaller()
        {
            Container.Bind<FooFacade>().ToSingleFacadeInstaller<FooInstaller>();

            AssertValidates();

            var foo = Container.Resolve<FooFacade>();

            Assert.That(Container.TryResolve<Bar>() == null);

            Assert.IsNotNull(foo.Bar);
        }

        void FacadeInstaller(DiContainer container)
        {
            container.Bind<FooFacade>().ToSingle();
            container.Bind<Bar>().ToSingle();
        }

        class FooInstaller : Installer
        {
            public override void InstallBindings()
            {
                Container.Bind<FooFacade>().ToSingle();
                Container.Bind<Bar>().ToSingle();
            }
        }
    }
}


