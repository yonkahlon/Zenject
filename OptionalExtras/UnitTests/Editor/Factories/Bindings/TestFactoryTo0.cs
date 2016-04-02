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
    public class TestFactoryTo0 : TestWithContainer
    {
        [Test]
        public void TestSelf()
        {
            Container.BindFactory<Foo, Foo.Factory>().ToSelf();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Foo.Factory>().Create());
        }

        [Test]
        public void TestConcrete()
        {
            Container.BindFactory<IFoo, IFooFactory>().To<Foo>();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<IFooFactory>().Create());

            Assert.That(Container.Resolve<IFooFactory>().Create() is Foo);
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
