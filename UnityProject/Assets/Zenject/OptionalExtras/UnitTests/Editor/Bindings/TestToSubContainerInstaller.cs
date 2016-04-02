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
    public class TestToSubContainerInstaller : TestWithContainer
    {
        [Test]
        public void TestInstallerSelfSingle()
        {
            Container.Bind<Foo>().ToSubContainerSelf<FooInstaller>().AsSingle();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Foo>().Bar);
        }

        [Test]
        public void TestInstallerSelfTransient()
        {
            Container.Bind<Foo>().ToSubContainerSelf<FooInstaller>().AsTransient();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Foo>().Bar);
        }

        [Test]
        public void TestInstallerSelfCached()
        {
            Container.Bind<Foo>().ToSubContainerSelf<FooInstaller>().AsCached();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Foo>().Bar);
        }

        [Test]
        public void TestInstallerSelfSingleMultipleContracts()
        {
            Container.Bind<Foo>().ToSubContainerSelf<FooInstaller>().AsSingle();
            Container.Bind<Bar>().ToSubContainerSelf<FooInstaller>().AsSingle();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>().Bar, Container.Resolve<Bar>());
        }

        [Test]
        public void TestInstallerSelfCachedMultipleContracts()
        {
            Container.Bind(typeof(Foo), typeof(IFoo)).ToSubContainer<Foo, FooInstaller>().AsCached();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestInstallerSelfSingleMultipleMatches()
        {
            Container.Bind<Qux>().ToSubContainerSelf<FooInstaller>().AsSingle();

            AssertValidates();

            Assert.IsEqual(Container.ResolveAll<Qux>().Count, 2);
        }

        [Test]
        public void TestInstallerSelfIdentifiersFails()
        {
            Container.Bind<Gorp>().ToSubContainerSelf<FooInstaller>().AsSingle();

            AssertValidationFails();

            Assert.Throws(() => Container.Resolve<Gorp>());
        }

        [Test]
        public void TestInstallerSelfIdentifiers()
        {
            Container.Bind<Gorp>().ToSubContainerSelf<FooInstaller>("gorp").AsSingle();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Gorp>());
        }

        public class Gorp
        {
        }

        public class Qux
        {
        }

        public class Bar
        {
        }

        public interface IFoo
        {
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

        class FooInstaller : Installer
        {
            public override void InstallBindings()
            {
                Container.Bind<Foo>().ToSelf().AsSingle();
                Container.Bind<Bar>().ToSelf().AsSingle();

                Container.Bind<Qux>().ToSelf();
                Container.Bind<Qux>().ToInstance(new Qux());

                Container.Bind<Gorp>("gorp").ToSelf();
            }
        }
    }
}
