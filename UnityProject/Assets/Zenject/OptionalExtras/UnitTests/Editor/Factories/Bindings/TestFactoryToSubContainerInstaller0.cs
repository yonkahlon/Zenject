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
    public class TestFactoryToSubContainerInstaller0 : TestWithContainer
    {
        [Test]
        public void TestSelf()
        {
            Container.BindFactory<Foo, Foo.Factory>().ToSubContainerSelf<FooInstaller>();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo.Factory>().Create(), FooInstaller.Foo);
        }

        [Test]
        public void TestConcrete()
        {
            Container.BindFactory<IFoo, IFooFactory>().ToSubContainer<Foo, FooInstaller>();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFooFactory>().Create(), FooInstaller.Foo);
        }

        class FooInstaller : Installer
        {
            public static Foo Foo = new Foo();

            public override void InstallBindings()
            {
                Container.Bind<Foo>().ToInstance(Foo);
            }
        }

        interface IFoo
        {
        }

        class IFooFactory : Factory<IFoo>
        {
        }

        class Foo : IFoo
        {
            public class Factory : Factory<Foo>
            {
            }
        }
    }
}



