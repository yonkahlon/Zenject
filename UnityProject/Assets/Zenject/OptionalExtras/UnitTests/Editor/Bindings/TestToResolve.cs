using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestToResolve : TestWithContainer
    {
        [Test]
        public void TestTransient()
        {
            var foo = new Foo();

            Container.BindInstance(foo);
            Container.Bind<IFoo>().ToResolve<Foo>();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), foo);
        }

        [Test]
        public void TestIdentifier()
        {
            var foo = new Foo();

            Container.Bind<Foo>("foo").ToInstance(foo);
            Container.Bind<IFoo>().ToResolve<Foo>("foo");

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>("foo"));
            Assert.IsEqual(Container.Resolve<IFoo>(), foo);
        }

        [Test]
        public void TestCached()
        {
            Container.Bind<Foo>().ToSelf().AsTransient();

            Container.Bind<IFoo>().ToResolve<Foo>().AsCached();

            AssertValidates();

            Assert.IsNotEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestSingle()
        {
            Container.Bind<Foo>().ToInstance(new Foo());

            Container.Bind<IFoo>().ToResolve<Foo>().AsSingle();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
        }

        [Test]
        [ExpectedException]
        public void TestSingleFailure()
        {
            Container.Bind<Foo>().ToSelf().AsSingle();
            Container.Bind<IFoo>().ToResolve<Foo>().AsSingle();

            AssertValidates();

            Container.Resolve<IFoo>();
        }

        [Test]
        public void TestInfiniteLoop()
        {
            Container.Bind<IFoo>().ToResolve<IFoo>().AsSingle();

            AssertValidationFails();

            Assert.Throws(() => Container.Resolve<IFoo>());
        }

        [Test]
        public void TestResolveManyTransient()
        {
            Container.Bind<Foo>().ToSelf();
            Container.Bind<Foo>().ToInstance(new Foo());

            Container.Bind<IFoo>().ToResolve<Foo>();

            AssertValidates();

            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 2);
        }

        [Test]
        public void TestResolveManyTransient2()
        {
            Container.Bind<Foo>().ToSelf();
            Container.Bind<Foo>().ToInstance(new Foo());

            Container.Bind(typeof(IFoo), typeof(IBar)).ToResolve<Foo>();

            AssertValidates();

            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 2);
            Assert.IsEqual(Container.ResolveAll<IBar>().Count, 2);
        }

        [Test]
        public void TestResolveManyCached()
        {
            Container.Bind<Foo>().ToSelf();
            Container.Bind<Foo>().ToSelf();

            Container.Bind<IFoo>().ToResolve<Foo>().AsCached();

            AssertValidates();

            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 2);
            Assert.That(Enumerable.SequenceEqual(Container.ResolveAll<IFoo>(), Container.ResolveAll<IFoo>()));
        }

        [Test]
        public void TestResolveManyCached2()
        {
            Container.Bind<Foo>().ToSelf();
            Container.Bind<Foo>().ToSelf();

            Container.Bind(typeof(IFoo), typeof(IBar)).ToResolve<Foo>().AsCached();

            AssertValidates();

            Assert.IsEqual(Container.ResolveAll<IBar>().Count, 2);
            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 2);
            Assert.That(Enumerable.SequenceEqual(Container.ResolveAll<IFoo>().Cast<object>(), Container.ResolveAll<IBar>().Cast<object>()));
        }

        [Test]
        public void TestResolveManyCached3()
        {
            Container.Bind<Foo>().ToSelf();
            Container.Bind<Foo>().ToSelf();

            Container.Bind<IFoo>().ToResolve<Foo>().AsCached();
            Container.Bind<IBar>().ToResolve<Foo>().AsCached();

            AssertValidates();

            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 2);
            Assert.IsEqual(Container.ResolveAll<IBar>().Count, 2);
            Assert.That(!Enumerable.SequenceEqual(Container.ResolveAll<IFoo>().Cast<object>(), Container.ResolveAll<IBar>().Cast<object>()));
        }

        [Test]
        public void TestResolveManySingle()
        {
            Container.Bind<Foo>().ToSelf();
            Container.Bind<Foo>().ToSelf();

            // This is a bit weird since it's a singleton but matching multiple... but valid
            Container.Bind<IFoo>().ToResolve<Foo>().AsSingle();
            Container.Bind<IBar>().ToResolve<Foo>().AsSingle();

            AssertValidates();

            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 2);
            Assert.IsEqual(Container.ResolveAll<IBar>().Count, 2);
            Assert.That(Enumerable.SequenceEqual(Container.ResolveAll<IFoo>().Cast<object>(), Container.ResolveAll<IBar>().Cast<object>()));
        }

        interface IBar
        {
        }

        interface IFoo
        {
        }

        class Foo : IFoo, IBar
        {
        }
    }
}
