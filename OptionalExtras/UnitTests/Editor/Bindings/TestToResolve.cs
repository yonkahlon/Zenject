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

            Container.Bind<Foo>().ToInstance(foo);
            Container.Bind<IFoo>().ToResolve<Foo>();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestIdentifier()
        {
            var foo = new Foo();

            Container.Bind<Foo>("foo").ToInstance(foo);
            Container.Bind<IFoo>().ToResolve<Foo>("foo");

            AssertValidates();

            Container.Resolve<IFoo>();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>("foo"));
        }

        [Test]
        public void TestMissingIdentifier()
        {
            var foo = new Foo();

            Container.Bind<Foo>("foo").ToInstance(foo);
            Container.Bind<IFoo>().ToResolve<Foo>();

            // TODO: Validation should fail but doesn't
            // ToResolve needs to stop using ToMethod and instead have its own provider
            //AssertValidationFails();

            Assert.Throws<ZenjectResolveException>(
                delegate { Container.Resolve<IFoo>(); });
        }
    }
}
