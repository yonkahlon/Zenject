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
    public class TestToSingleMethod : TestWithContainer
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

            Container.Bind(typeof(Foo)).ToSingleMethod((container) => foo);

            AssertValidates();

            Assert.That(ReferenceEquals(Container.Resolve<Foo>(), foo));
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestDuplicates()
        {
            Container.Bind<Foo>().ToSingleMethod((container) => new Foo());
            Container.Bind<IFoo>().ToSingleMethod<Foo>((container) => new Foo());
        }

        [Test]
        public void TestDuplicates2()
        {
            Func<InjectContext, Foo> method = (ctx) => new Foo();

            Container.Bind<Foo>().ToSingleMethod(method);
            Container.Bind<IFoo>().ToSingleMethod(method);

            AssertValidates();

            Assert.ReferenceEquals(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestDuplicates3()
        {
            Container.Bind<Foo>().ToSingleMethod<Foo>(CreateFoo);
            Container.Bind<IFoo>().ToSingleMethod<Foo>(CreateFoo);

            AssertValidates();

            Assert.ReferenceEquals(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
        }

        Foo CreateFoo(InjectContext ctx)
        {
            return new Foo();
        }
    }
}

