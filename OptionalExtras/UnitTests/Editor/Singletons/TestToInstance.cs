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
    public class TestToInstance : TestWithContainer
    {
        interface IFoo
        {
        }

        class Foo : IFoo
        {
        }

        [Test]
        public void TestInterfaceBoundToInstanceRegistration()
        {
            IFoo instance = new Foo();

            Container.Bind<IFoo>().ToInstance(instance);

            Assert.That(Container.ValidateResolve<IFoo>().IsEmpty());
            var builtInstance = Container.Resolve<IFoo>();

            Assert.That(ReferenceEquals(builtInstance, instance));
        }

        [Test]
        public void TestInterfaceBoundToInstanceRegistrationUntyped()
        {
            IFoo instance = new Foo();

            Container.Bind(typeof(IFoo)).ToInstance(instance);

            Assert.That(Container.ValidateResolve<IFoo>().IsEmpty());
            var builtInstance = Container.Resolve<IFoo>();

            Assert.That(ReferenceEquals(builtInstance, instance));
        }

        [Test]
        public void TestDuplicateInstanceBindingsFail()
        {
            var instance = new Foo();

            Container.Bind<Foo>().ToInstance(instance);
            Container.Bind<Foo>().ToInstance(instance);

            Assert.That(Container.ValidateResolve<Foo>().Any());

            Assert.Throws<ZenjectResolveException>(
                delegate { Container.Resolve<Foo>(); });
        }

        [Test]
        public void TestDuplicateInstanceBindingsFailUntyped()
        {
            var instance = new Foo();

            Container.Bind(typeof(Foo)).ToInstance(instance);
            Container.Bind(typeof(Foo)).ToInstance(instance);

            Assert.That(Container.ValidateResolve<Foo>().Any());

            Assert.Throws<ZenjectResolveException>(
                delegate { Container.Resolve<Foo>(); });
        }
    }
}

