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
            Binder.Bind<FooFacade>().ToSingleFacadeMethod(FacadeInstaller);

            AssertValidates();

            var foo = Resolver.Resolve<FooFacade>();

            Assert.That(Resolver.TryResolve<Bar>() == null);

            Assert.IsNotNull(foo.Bar);
        }

        [Test]
        public void TestInstaller()
        {
            Binder.Bind<FooFacade>().ToSingleFacadeInstaller<FooInstaller>();

            AssertValidates();

            var foo = Resolver.Resolve<FooFacade>();

            Assert.That(Resolver.TryResolve<Bar>() == null);

            Assert.IsNotNull(foo.Bar);
        }

        void FacadeInstaller(IBinder binder)
        {
            binder.Bind<FooFacade>().ToSingle();
            binder.Bind<Bar>().ToSingle();
        }

        class FooInstaller : Installer
        {
            public override void InstallBindings()
            {
                Binder.Bind<FooFacade>().ToSingle();
                Binder.Bind<Bar>().ToSingle();
            }
        }
    }
}


