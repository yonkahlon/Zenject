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
            Container.Bind<Foo>().ToSingle();

            Assert.That(Container.ValidateResolve<Foo>().IsEmpty());
            Assert.IsNotNull(Container.Resolve<Foo>());
        }

        [Test]
        public void TestSingletonOneInstance()
        {
            Container.Bind<Foo>().ToSingle();

            Assert.That(Container.ValidateResolve<Foo>().IsEmpty());
            var test1 = Container.Resolve<Foo>();
            Assert.That(Container.ValidateResolve<Foo>().IsEmpty());
            var test2 = Container.Resolve<Foo>();

            Assert.That(test1 != null && test2 != null);
            Assert.That(ReferenceEquals(test1, test2));
        }

        [Test]
        public void TestSingletonOneInstanceUntyped()
        {
            Container.Bind(typeof(Foo)).ToSingle();

            Assert.That(Container.ValidateResolve<Foo>().IsEmpty());
            var test1 = Container.Resolve<Foo>();
            Assert.That(Container.ValidateResolve<Foo>().IsEmpty());
            var test2 = Container.Resolve<Foo>();

            Assert.That(test1 != null && test2 != null);
            Assert.That(ReferenceEquals(test1, test2));
        }

        [Test]
        public void TestInterfaceBoundToImplementationRegistration()
        {
            Container.Bind<IFoo>().ToSingle<Foo>();

            Assert.That(Container.ValidateResolve<IFoo>().IsEmpty());
            Assert.IsNotNull(Container.Resolve<IFoo>());
        }

        [Test]
        public void TestInterfaceBoundToImplementationRegistrationUntyped()
        {
            Container.Bind(typeof(IFoo)).ToSingle(typeof(Foo));

            Assert.That(Container.ValidateResolve<IFoo>().IsEmpty());
            Assert.IsNotNull(Container.Resolve<IFoo>());
        }

        [Test]
        public void TestDuplicateBindings()
        {
            // Note: does not error out until a request for Foo is made
            Container.Bind<Foo>().ToSingle();
            Container.Bind<Foo>().ToSingle();
        }

        [Test]
        public void TestDuplicateBindingsFail()
        {
            Container.Bind<Foo>().ToSingle();
            Container.Bind<Foo>().ToSingle();

            Assert.Throws<ZenjectResolveException>(
                delegate { Container.Resolve<Foo>(); });

            Assert.That(Container.ValidateResolve<Foo>().Any());
        }

        [Test]
        public void TestDuplicateBindingsFailUntyped()
        {
            Container.Bind(typeof(Foo)).ToSingle();
            Container.Bind(typeof(Foo)).ToSingle();

            Assert.Throws<ZenjectResolveException>(
                delegate { Container.Resolve<Foo>(); });

            Assert.That(Container.ValidateResolve<Foo>().Any());
        }
    }
}
