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

            Container.Bind<Foo>().ToSingleInstance(foo);
            Container.Bind<IFoo>().ToSingleInstance(foo);

            AssertValidates();

            Assert.ReferenceEquals(Container.Resolve<IFoo>(), foo);
            Assert.ReferenceEquals(Container.Resolve<Foo>(), foo);
            Assert.ReferenceEquals(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestCaseDuplicates()
        {
            Container.Bind<Foo>().ToSingleInstance(new Foo());
            Container.Bind<Foo>().ToSingleInstance(new Foo());
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestCaseDuplicatesUntyped()
        {
            Container.Bind(typeof(Foo)).ToSingleInstance(new Foo());
            Container.Bind(typeof(Foo)).ToSingleInstance(new Foo());
        }
    }
}

