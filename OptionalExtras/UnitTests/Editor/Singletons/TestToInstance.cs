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

            Binder.Bind<IFoo>().ToInstance(instance);

            Assert.That(Resolver.ValidateResolve<IFoo>().IsEmpty());
            var builtInstance = Resolver.Resolve<IFoo>();

            Assert.That(ReferenceEquals(builtInstance, instance));
        }

        [Test]
        public void TestInterfaceBoundToInstanceRegistrationUntyped()
        {
            IFoo instance = new Foo();

            Binder.Bind(typeof(IFoo)).ToInstance(instance);

            Assert.That(Resolver.ValidateResolve<IFoo>().IsEmpty());
            var builtInstance = Resolver.Resolve<IFoo>();

            Assert.That(ReferenceEquals(builtInstance, instance));
        }

        [Test]
        public void TestDuplicateInstanceBindingsFail()
        {
            var instance = new Foo();

            Binder.Bind<Foo>().ToInstance(instance);
            Binder.Bind<Foo>().ToInstance(instance);

            Assert.That(Resolver.ValidateResolve<Foo>().Any());

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Foo>(); });
        }

        [Test]
        public void TestDuplicateInstanceBindingsFailUntyped()
        {
            var instance = new Foo();

            Binder.Bind(typeof(Foo)).ToInstance(instance);
            Binder.Bind(typeof(Foo)).ToInstance(instance);

            Assert.That(Resolver.ValidateResolve<Foo>().Any());

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Foo>(); });
        }
    }
}

