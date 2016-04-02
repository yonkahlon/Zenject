using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestToMethod : TestWithContainer
    {
        [Test]
        public void TestSingle()
        {
            var foo = new Foo();

            Container.Bind<Foo>().ToMethod((ctx) => foo).AsSingle();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>(), foo);
        }

        [Test]
        public void TestTransient()
        {
            var foo = new Foo();

            Container.Bind<Foo>().ToMethod((ctx) => foo).AsTransient();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>(), foo);
        }

        [Test]
        public void TestCached()
        {
            var foo = new Foo();

            Container.Bind<Foo>().ToMethod((ctx) => foo).AsCached();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>(), foo);
        }

        [Test]
        [ExpectedException]
        public void TestSingleConflict()
        {
            Container.Bind<Foo>().ToMethod((ctx) => new Foo()).AsSingle();
            Container.Bind<Foo>().ToSelf().AsSingle();

            AssertValidates();

            Container.ResolveAll<Foo>();
        }

        [Test]
        public void TestSingle2()
        {
            var foo = new Foo();
            Func<InjectContext, Foo> method = (ctx) => foo;

            Container.Bind<Foo>().ToMethod(method).AsSingle();
            Container.Bind<IFoo>().ToMethod(method).AsSingle();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<Foo>(), foo);
        }

        [Test]
        public void TestSingle3()
        {
            Container.Bind<Foo>().ToMethodSelf(CreateFoo).AsSingle();
            Container.Bind<IFoo>().ToMethod<Foo>(CreateFoo).AsSingle();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
        }

        Foo CreateFoo(InjectContext ctx)
        {
            return new Foo();
        }

        [Test]
        public void TestSingle4()
        {
            int numCalls = 0;

            Func<InjectContext, Foo> method = (ctx) =>
                {
                    numCalls++;
                    return null;
                };

            Container.Bind<Foo>().ToMethod(method).AsSingle();
            Container.Bind<IFoo>().ToMethod(method).AsSingle();

            AssertValidates();

            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<IFoo>();

            Assert.IsEqual(numCalls, 1);
        }

        [Test]
        public void TestTransient2()
        {
            int numCalls = 0;

            Func<InjectContext, Foo> method = (ctx) =>
            {
                numCalls++;
                return null;
            };

            Container.Bind<Foo>().ToMethod(method).AsTransient();
            Container.Bind<IFoo>().ToMethod(method).AsTransient();

            AssertValidates();

            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<IFoo>();

            Assert.IsEqual(numCalls, 4);
        }

        [Test]
        public void TestCached2()
        {
            Container.Bind(typeof(Foo), typeof(IFoo)).ToMethod((ctx) => new Foo()).AsCached();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
        }

        interface IFoo
        {
        }

        class Foo : IFoo
        {
        }
    }
}

