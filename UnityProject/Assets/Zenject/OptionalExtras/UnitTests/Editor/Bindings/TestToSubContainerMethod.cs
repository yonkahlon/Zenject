using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestToSubContainerMethod : TestWithContainer
    {
        [Test]
        public void TestMethodSelfSingle()
        {
            Container.Bind<Foo>().ToSubContainerSelf(InstallFooFacade).AsSingle();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Foo>().Bar);
        }

        [Test]
        public void TestMethodSelfTransient()
        {
            Container.Bind<Foo>().ToSubContainerSelf(InstallFooFacade).AsTransient();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Foo>().Bar);
        }

        [Test]
        public void TestMethodSelfCached()
        {
            Container.Bind<Foo>().ToSubContainerSelf(InstallFooFacade).AsCached();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Foo>().Bar);
        }

        [Test]
        public void TestMethodSelfSingleMultipleContracts()
        {
            Container.Bind<Foo>().ToSubContainerSelf(InstallFooFacade).AsSingle();
            Container.Bind<Bar>().ToSubContainerSelf(InstallFooFacade).AsSingle();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>().Bar, Container.Resolve<Bar>());
        }

        [Test]
        public void TestMethodSelfCachedMultipleContracts()
        {
            Container.Bind(typeof(Foo), typeof(Bar)).ToSubContainerSelf(InstallFooFacade).AsCached();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>().Bar, Container.Resolve<Bar>());
        }

        [Test]
        public void TestMethodConcreteSingle()
        {
            Container.Bind<IFoo>().ToSubContainer<Foo>(InstallFooFacade).AsSingle();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<IFoo>().Bar);
        }

        [Test]
        public void TestMethodConcreteTransient()
        {
            Container.Bind<IFoo>().ToSubContainer<Foo>(InstallFooFacade).AsTransient();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<IFoo>().Bar);
        }

        [Test]
        public void TestMethodConcreteCached()
        {
            Container.Bind<IFoo>().ToSubContainer<Foo>(InstallFooFacade).AsCached();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<IFoo>().Bar);
        }

        [Test]
        public void TestMethodConcreteSingleMultipleContracts()
        {
            Container.Bind<IFoo>().ToSubContainer<Foo>(InstallFooFacade).AsSingle();
            Container.Bind<Bar>().ToSubContainerSelf(InstallFooFacade).AsSingle();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFoo>().Bar, Container.Resolve<Bar>());
        }

        [Test]
        public void TestMethodConcreteCachedMultipleContracts()
        {
            Container.Bind(typeof(Foo), typeof(IFoo)).ToSubContainer<Foo>(InstallFooFacade).AsCached();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestMethodSelfIdentifiersFails()
        {
            Container.Bind<Gorp>().ToSubContainerSelf(InstallFooFacade).AsSingle();

            AssertValidationFails();

            Assert.Throws(() => Container.Resolve<Gorp>());
        }

        [Test]
        public void TestMethodSelfIdentifiers()
        {
            Container.Bind<Gorp>().ToSubContainerSelf(InstallFooFacade, "gorp").AsSingle();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Gorp>());
        }

        public class Gorp
        {
        }

        public class Bar
        {
        }

        public interface IFoo
        {
            Bar Bar
            {
                get;
            }
        }

        public class Foo : IFoo
        {
            public Foo(Bar bar)
            {
                Bar = bar;
            }

            public Bar Bar
            {
                get;
                private set;
            }
        }

        void InstallFooFacade(DiContainer container)
        {
            container.Bind<Foo>().ToSelf().AsSingle();
            container.Bind<Bar>().ToSelf().AsSingle();

            container.Bind<Gorp>("gorp").ToSelf();
        }
    }
}


