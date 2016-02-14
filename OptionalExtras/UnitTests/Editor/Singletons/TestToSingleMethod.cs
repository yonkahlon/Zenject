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

            Binder.Bind(typeof(Foo)).ToSingleMethod((container) => foo);

            Assert.That(ReferenceEquals(Resolver.Resolve<Foo>(), foo));
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestDuplicates()
        {
            Binder.Bind<Foo>().ToSingleMethod((container) => new Foo());
            Binder.Bind<IFoo>().ToSingleMethod<Foo>((container) => new Foo());
        }

        [Test]
        public void TestDuplicates2()
        {
            Func<InjectContext, Foo> method = (ctx) => new Foo();

            Binder.Bind<Foo>().ToSingleMethod(method);
            Binder.Bind<IFoo>().ToSingleMethod(method);

            Assert.ReferenceEquals(Resolver.Resolve<Foo>(), Container.Resolver.Resolve<IFoo>());
        }

        [Test]
        public void TestDuplicates3()
        {
            Binder.Bind<Foo>().ToSingleMethod<Foo>(CreateFoo);
            Binder.Bind<IFoo>().ToSingleMethod<Foo>(CreateFoo);

            Assert.ReferenceEquals(Resolver.Resolve<Foo>(), Container.Resolver.Resolve<IFoo>());
        }

        Foo CreateFoo(InjectContext ctx)
        {
            return new Foo();
        }
    }
}

