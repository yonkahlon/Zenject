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
    public class TestToSingle : TestWithContainer
    {
        interface IFoo
        {
        }

        class Foo : IFoo
        {
        }

        [Test]
        public void TestClassRegistration()
        {
            Binder.Bind<Foo>().ToSingle();

            Assert.That(Resolver.ValidateResolve<Foo>().IsEmpty());
            Assert.IsNotNull(Resolver.Resolve<Foo>());
        }

        [Test]
        public void TestSingletonOneInstance()
        {
            Binder.Bind<Foo>().ToSingle();

            Assert.That(Resolver.ValidateResolve<Foo>().IsEmpty());
            var test1 = Resolver.Resolve<Foo>();
            Assert.That(Resolver.ValidateResolve<Foo>().IsEmpty());
            var test2 = Resolver.Resolve<Foo>();

            Assert.That(test1 != null && test2 != null);
            Assert.That(ReferenceEquals(test1, test2));
        }

        [Test]
        public void TestSingletonOneInstanceUntyped()
        {
            Binder.Bind(typeof(Foo)).ToSingle();

            Assert.That(Resolver.ValidateResolve<Foo>().IsEmpty());
            var test1 = Resolver.Resolve<Foo>();
            Assert.That(Resolver.ValidateResolve<Foo>().IsEmpty());
            var test2 = Resolver.Resolve<Foo>();

            Assert.That(test1 != null && test2 != null);
            Assert.That(ReferenceEquals(test1, test2));
        }

        [Test]
        public void TestInterfaceBoundToImplementationRegistration()
        {
            Binder.Bind<IFoo>().ToSingle<Foo>();

            Assert.That(Resolver.ValidateResolve<IFoo>().IsEmpty());
            Assert.IsNotNull(Resolver.Resolve<IFoo>());
        }

        [Test]
        public void TestInterfaceBoundToImplementationRegistrationUntyped()
        {
            Binder.Bind(typeof(IFoo)).ToSingle(typeof(Foo));

            Assert.That(Resolver.ValidateResolve<IFoo>().IsEmpty());
            Assert.IsNotNull(Resolver.Resolve<IFoo>());
        }

        [Test]
        public void TestDuplicateBindings()
        {
            // Note: does not error out until a request for Foo is made
            Binder.Bind<Foo>().ToSingle();
            Binder.Bind<Foo>().ToSingle();
        }

        [Test]
        public void TestDuplicateBindingsFail()
        {
            Binder.Bind<Foo>().ToSingle();
            Binder.Bind<Foo>().ToSingle();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Foo>(); });

            Assert.That(Resolver.ValidateResolve<Foo>().Any());
        }

        [Test]
        public void TestDuplicateBindingsFailUntyped()
        {
            Binder.Bind(typeof(Foo)).ToSingle();
            Binder.Bind(typeof(Foo)).ToSingle();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Foo>(); });

            Assert.That(Resolver.ValidateResolve<Foo>().Any());
        }
    }
}
