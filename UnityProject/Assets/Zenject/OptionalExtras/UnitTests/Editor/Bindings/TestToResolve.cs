using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestToResolve : TestWithContainer
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

            Binder.Bind<Foo>().ToInstance(foo);
            Binder.Bind<IFoo>().ToResolve<Foo>();

            Assert.IsEqual(Resolver.Resolve<IFoo>(), Resolver.Resolve<Foo>());
        }

        [Test]
        public void TestIdentifier()
        {
            var foo = new Foo();

            Binder.Bind<Foo>("foo").ToInstance(foo);
            Binder.Bind<IFoo>().ToResolve<Foo>("foo");

            Resolver.Resolve<IFoo>();

            Assert.IsEqual(Resolver.Resolve<IFoo>(), Resolver.Resolve<Foo>("foo"));
        }

        [Test]
        public void TestMissingIdentifier()
        {
            var foo = new Foo();

            Binder.Bind<Foo>("foo").ToInstance(foo);
            Binder.Bind<IFoo>().ToResolve<Foo>();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<IFoo>(); });
        }
    }
}
