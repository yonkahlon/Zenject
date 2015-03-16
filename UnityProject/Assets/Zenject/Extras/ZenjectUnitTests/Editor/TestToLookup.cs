using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestToLookup : TestWithContainer
    {
        interface IFoo
        {
        }

        class Foo : IFoo
        {
        }

        [Test]
        public void Test1()
        {
            var foo = new Foo();

            Container.Bind<Foo>().To(foo);
            Container.Bind<IFoo>().ToLookup<Foo>();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestIdentifier()
        {
            var foo = new Foo();

            Container.Bind<Foo>("foo").To(foo);
            Container.Bind<IFoo>().ToLookup<Foo>("foo");

            Container.Resolve<IFoo>();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>("foo"));
        }

        [Test]
        public void TestMissingIdentifier()
        {
            var foo = new Foo();

            Container.Bind<Foo>("foo").To(foo);
            Container.Bind<IFoo>().ToLookup<Foo>();

            Assert.Throws<ZenjectResolveException>(
                delegate { Container.Resolve<IFoo>(); });
        }
    }
}
