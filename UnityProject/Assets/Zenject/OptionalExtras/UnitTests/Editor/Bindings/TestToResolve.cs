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

            AssertValidates();

            Assert.IsEqual(Resolver.Resolve<IFoo>(), Resolver.Resolve<Foo>());
        }

        [Test]
        public void TestIdentifier()
        {
            var foo = new Foo();

            Binder.Bind<Foo>("foo").ToInstance(foo);
            Binder.Bind<IFoo>().ToResolve<Foo>("foo");

            AssertValidates();

            Resolver.Resolve<IFoo>();

            Assert.IsEqual(Resolver.Resolve<IFoo>(), Resolver.Resolve<Foo>("foo"));
        }

        [Test]
        public void TestMissingIdentifier()
        {
            var foo = new Foo();

            Binder.Bind<Foo>("foo").ToInstance(foo);
            Binder.Bind<IFoo>().ToResolve<Foo>();

            // TODO: Validation should fail but doesn't
            // ToResolve needs to stop using ToMethod and instead have its own provider
            //AssertValidationFails();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<IFoo>(); });
        }
    }
}
