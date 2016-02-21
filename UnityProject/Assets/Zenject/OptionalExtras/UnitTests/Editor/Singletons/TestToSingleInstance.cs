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
    public class TestToSingleInstance : TestWithContainer
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
            var foo = new Foo();

            Binder.Bind<Foo>().ToSingleInstance(foo);
            Binder.Bind<IFoo>().ToSingleInstance(foo);

            AssertValidates();

            Assert.ReferenceEquals(Resolver.Resolve<IFoo>(), foo);
            Assert.ReferenceEquals(Resolver.Resolve<Foo>(), foo);
            Assert.ReferenceEquals(Resolver.Resolve<IFoo>(), Container.Resolver.Resolve<Foo>());
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestCaseDuplicates()
        {
            Binder.Bind<Foo>().ToSingleInstance(new Foo());
            Binder.Bind<Foo>().ToSingleInstance(new Foo());
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestCaseDuplicatesUntyped()
        {
            Binder.Bind(typeof(Foo)).ToSingleInstance(new Foo());
            Binder.Bind(typeof(Foo)).ToSingleInstance(new Foo());
        }
    }
}

