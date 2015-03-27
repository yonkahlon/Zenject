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
    public class TestSingleton : TestWithContainer
    {
        private interface IFoo
        {
            int ReturnValue();
        }

        private class Foo : IFoo
        {
            public int ReturnValue()
            {
                return 5;
            }
        }

        [Test]
        public void TestClassRegistration()
        {
            Container.Bind<Foo>().ToSingle();

            Assert.That(Container.ValidateResolve<Foo>().IsEmpty());
            Assert.That(Container.Resolve<Foo>().ReturnValue() == 5);
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
            Assert.That(Container.Resolve<IFoo>().ReturnValue() == 5);
        }

        [Test]
        public void TestInterfaceBoundToImplementationRegistrationUntyped()
        {
            Container.Bind(typeof(IFoo)).ToSingle(typeof(Foo));

            Assert.That(Container.ValidateResolve<IFoo>().IsEmpty());
            Assert.That(Container.Resolve<IFoo>().ReturnValue() == 5);
        }

        [Test]
        public void TestInterfaceBoundToInstanceRegistration()
        {
            IFoo instance = new Foo();

            Container.Bind<IFoo>().ToInstance(instance);

            Assert.That(Container.ValidateResolve<IFoo>().IsEmpty());
            var builtInstance = Container.Resolve<IFoo>();

            Assert.That(ReferenceEquals(builtInstance, instance));
            Assert.That(builtInstance.ReturnValue() == 5);
        }

        [Test]
        public void TestInterfaceBoundToInstanceRegistrationUntyped()
        {
            IFoo instance = new Foo();

            Container.Bind(typeof(IFoo)).ToInstance(instance);

            Assert.That(Container.ValidateResolve<IFoo>().IsEmpty());
            var builtInstance = Container.Resolve<IFoo>();

            Assert.That(ReferenceEquals(builtInstance, instance));
            Assert.That(builtInstance.ReturnValue() == 5);
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

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestToSingleWithInstance()
        {
            Container.Bind<Foo>().ToSingleInstance(new Foo());
            Container.Bind<Foo>().ToSingleInstance(new Foo());
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestToSingleWithInstanceUntyped()
        {
            Container.Bind(typeof(Foo)).ToSingleInstance(new Foo());
            Container.Bind(typeof(Foo)).ToSingleInstance(new Foo());
        }

        [Test]
        public void TestToSingleWithInstanceIsUnique()
        {
            var foo = new Foo();

            Container.Bind<Foo>().ToSingleInstance(foo);
            Container.Bind<IFoo>().ToSingle<Foo>();

            Assert.That(
                ReferenceEquals(Container.Resolve<IFoo>(), Container.Resolve<Foo>()));
        }

        [Test]
        public void TestToSingleWithInstanceIsUniqueUntyped()
        {
            var foo = new Foo();

            Container.Bind(typeof(Foo)).ToSingleInstance(foo);
            Container.Bind(typeof(IFoo)).ToSingle<Foo>();

            Assert.That(
                ReferenceEquals(Container.Resolve<IFoo>(), Container.Resolve<Foo>()));
        }

        [Test]
        public void TestToSingleWithInstance2()
        {
            var foo = new Foo();

            Container.Bind<Foo>().ToInstance(foo);
            Container.Bind<IFoo>().ToSingle<Foo>();

            Assert.That(
                !ReferenceEquals(Container.Resolve<IFoo>(), Container.Resolve<Foo>()));
        }

        [Test]
        public void TestToSingleWithInstance2Untyped()
        {
            var foo = new Foo();

            Container.Bind(typeof(Foo)).ToInstance(foo);
            Container.Bind(typeof(IFoo)).ToSingle<Foo>();

            Assert.That(
                !ReferenceEquals(Container.Resolve<IFoo>(), Container.Resolve<Foo>()));
        }

        [Test]
        public void TestToSingleMethod()
        {
            var foo = new Foo();

            Container.Bind(typeof(Foo)).ToSingleMethod((container) => foo);
            Container.Bind(typeof(IFoo)).ToSingle<Foo>();

            Assert.That(ReferenceEquals(Container.Resolve<Foo>(), foo));
            Assert.That(ReferenceEquals(Container.Resolve<Foo>(), Container.Resolve<IFoo>()));
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestToSingleMethod2()
        {
            var foo = new Foo();
            var foo2 = new Foo();

            Container.Bind(typeof(Foo)).ToSingleMethod((container) => foo);
            Container.Bind(typeof(IFoo)).ToSingleMethod((container) => foo2);
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestToSingleMethod3()
        {
            Container.Bind<Foo>().ToSingle();
            Container.Bind(typeof(IFoo)).ToSingleMethod((container) => new Foo());
        }
    }
}


