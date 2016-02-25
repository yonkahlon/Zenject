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
            Container.Bind<Foo>().ToSingleFactory<FooFactory>();
            Container.Bind<IFoo>().ToSingleFactory<FooFactory, Foo>();

            FooFactory.WasCalled = false;

            AssertValidates();

            var foo = Container.Resolve<Foo>();
            Assert.That(FooFactory.WasCalled);

            FooFactory.WasCalled = false;
            var ifoo = Container.Resolve<IFoo>();

            Assert.That(!FooFactory.WasCalled);

            var foo2 = Container.Resolve<Foo>();

            Assert.That(!FooFactory.WasCalled);

            Assert.IsEqual(foo, foo2);
            Assert.IsEqual(ifoo, foo2);
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestCase2()
        {
            // Cannot bind different singleton providers
            Container.Bind<Foo>().ToSingleFactory<FooFactory>();
            Container.Bind<Foo>().ToSingle();
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

