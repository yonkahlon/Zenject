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
    public class TestSingletonIdentifiers : TestWithContainer
    {
        interface IBar
        {
        }

        interface IFoo
        {
        }

        class Foo0 : IFoo, IBar
        {
        }

        [Test]
        public void TestSameIdsSameInstance()
        {
            Container.Bind<IBar>().ToSingle<Foo0>("foo");
            Container.Bind<IFoo>().ToSingle<Foo0>("foo");
            Container.Bind<Foo0>().ToSingle("foo");

            Assert.IsEqual(Container.Resolve<Foo0>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<Foo0>(), Container.Resolve<IBar>());
        }

        [Test]
        public void TestDifferentIdsDifferentInstances()
        {
            Container.Bind<IFoo>().ToSingle<Foo0>("foo");
            Container.Bind<Foo0>().ToSingle("bar");

            Assert.IsNotEqual(Container.Resolve<Foo0>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestNoIdDifferentInstances()
        {
            Container.Bind<IFoo>().ToSingle<Foo0>();
            Container.Bind<Foo0>().ToSingle("bar");

            Assert.IsNotEqual(Container.Resolve<Foo0>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestManyInstances()
        {
            Container.Bind<IFoo>().ToSingle<Foo0>("foo1");
            Container.Bind<IFoo>().ToSingle<Foo0>("foo2");
            Container.Bind<IFoo>().ToSingle<Foo0>("foo3");
            Container.Bind<IFoo>().ToSingle<Foo0>("foo4");

            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 4);
        }
    }
}

