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
            Binder.Bind<IBar>().ToSingle<Foo0>("foo");
            Binder.Bind<IFoo>().ToSingle<Foo0>("foo");
            Binder.Bind<Foo0>().ToSingle("foo");

            AssertValidates();

            Assert.IsEqual(Resolver.Resolve<Foo0>(), Container.Resolver.Resolve<IFoo>());
            Assert.IsEqual(Resolver.Resolve<Foo0>(), Container.Resolver.Resolve<IBar>());
        }

        [Test]
        public void TestDifferentIdsDifferentInstances()
        {
            Binder.Bind<IFoo>().ToSingle<Foo0>("foo");
            Binder.Bind<Foo0>().ToSingle("bar");

            AssertValidates();

            Assert.IsNotEqual(Resolver.Resolve<Foo0>(), Container.Resolver.Resolve<IFoo>());
        }

        [Test]
        public void TestNoIdDifferentInstances()
        {
            Binder.Bind<IFoo>().ToSingle<Foo0>();
            Binder.Bind<Foo0>().ToSingle("bar");

            AssertValidates();

            Assert.IsNotEqual(Resolver.Resolve<Foo0>(), Container.Resolver.Resolve<IFoo>());
        }

        [Test]
        public void TestManyInstances()
        {
            Binder.Bind<IFoo>().ToSingle<Foo0>("foo1");
            Binder.Bind<IFoo>().ToSingle<Foo0>("foo2");
            Binder.Bind<IFoo>().ToSingle<Foo0>("foo3");
            Binder.Bind<IFoo>().ToSingle<Foo0>("foo4");

            Assert.IsEqual(Resolver.ResolveAll<IFoo>().Count, 4);
        }
    }
}

