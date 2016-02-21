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
    public class TestToSingleFactory : TestWithContainer
    {
        interface IFoo
        {
        }

        class Foo : IFoo
        {
        }

        [Test]
        public void TestCase1()
        {
            Binder.Bind<Foo>().ToSingleFactory<FooFactory>();
            Binder.Bind<IFoo>().ToSingleFactory<FooFactory, Foo>();

            FooFactory.WasCalled = false;

            AssertValidates();

            var foo = Resolver.Resolve<Foo>();
            Assert.That(FooFactory.WasCalled);

            FooFactory.WasCalled = false;
            var ifoo = Resolver.Resolve<IFoo>();

            Assert.That(!FooFactory.WasCalled);

            var foo2 = Resolver.Resolve<Foo>();

            Assert.That(!FooFactory.WasCalled);

            Assert.IsEqual(foo, foo2);
            Assert.IsEqual(ifoo, foo2);
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestCase2()
        {
            // Cannot bind different singleton providers
            Binder.Bind<Foo>().ToSingleFactory<FooFactory>();
            Binder.Bind<Foo>().ToSingle();
        }

        class FooFactory : IFactory<Foo>
        {
            public static bool WasCalled;

            public Foo Create()
            {
                WasCalled = true;
                return new Foo();
            }
        }
    }
}

