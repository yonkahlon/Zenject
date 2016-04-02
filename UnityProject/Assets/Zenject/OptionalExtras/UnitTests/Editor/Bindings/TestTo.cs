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
    public class TestTo : TestWithContainer
    {
        [Test]
        public void TestSelfSingle()
        {
            Container.Bind<Foo>().ToSelf().AsSingle();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestSelfTransient()
        {
            Container.Bind<Foo>().ToSelf().AsTransient();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsNotEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestSelfCached()
        {
            Container.Bind<Foo>().ToSelf().AsCached();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestConcreteSingle()
        {
            Container.Bind<Foo>().ToSelf().AsSingle();
            Container.Bind<IFoo>().To<Foo>().AsSingle();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsNotNull(Container.Resolve<IFoo>());

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestConcreteTransient()
        {
            Container.Bind<IFoo>().To<Foo>().AsTransient();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<IFoo>());
            Assert.IsNotEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestConcreteCached()
        {
            Container.Bind<Foo>().ToSelf().AsCached();
            Container.Bind<IFoo>().To<Foo>().AsCached();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsNotNull(Container.Resolve<IFoo>());

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
            Assert.IsNotEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestDuplicateBindingsFail()
        {
            Container.Bind<Foo>().ToSelf().AsSingle();
            Container.Bind<Foo>().ToSelf().AsSingle();

            AssertValidates();

            Assert.Throws(
                delegate { Container.Resolve<Foo>(); });

            Assert.IsEqual(Container.ResolveAll<Foo>().Count, 2);
        }

        [Test]
        public void TestConcreteMultipleTransient()
        {
            Container.Bind<IFoo>().To(new List<Type>() {typeof(Foo), typeof(Bar)}).AsTransient();

            AssertValidates();

            var foos = Container.ResolveAll<IFoo>();

            Assert.IsEqual(foos.Count, 2);
            Assert.That(foos[0] is Foo);
            Assert.That(foos[1] is Bar);

            var foos2 = Container.ResolveAll<IFoo>();

            Assert.IsNotEqual(foos[0], foos2[0]);
            Assert.IsNotEqual(foos[1], foos2[1]);
        }

        [Test]
        public void TestConcreteMultipleSingle()
        {
            Container.Bind<IFoo>().To(new List<Type>() {typeof(Foo), typeof(Bar)}).AsSingle();

            AssertValidates();

            var foos = Container.ResolveAll<IFoo>();

            Assert.IsEqual(foos.Count, 2);
            Assert.That(foos[0] is Foo);
            Assert.That(foos[1] is Bar);

            var foos2 = Container.ResolveAll<IFoo>();

            Assert.IsEqual(foos[0], foos2[0]);
            Assert.IsEqual(foos[1], foos2[1]);
        }

        [Test]
        [ExpectedException]
        public void TestMultipleBindingsSingleFail1()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).ToSelf().AsSingle();
            Container.Resolve<IFoo>();
        }

        [Test]
        [ExpectedException]
        public void TestMultipleBindingsSingleFail2()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To<Qux>().AsSingle();
            Container.Resolve<IFoo>();
        }

        [Test]
        public void TestMultipleBindingsSingle()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To<Foo>().AsSingle();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IBar>());
            Assert.That(Container.Resolve<IFoo>() is Foo);
        }

        [Test]
        public void TestMultipleBindingsTransient()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To<Foo>().AsTransient();

            AssertValidates();

            Assert.That(Container.Resolve<IFoo>() is Foo);
            Assert.That(Container.Resolve<IBar>() is Foo);

            Assert.IsNotEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
            Assert.IsNotEqual(Container.Resolve<IFoo>(), Container.Resolve<IBar>());
        }

        [Test]
        public void TestMultipleBindingsCached()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To<Foo>().AsCached();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IBar>());
        }

        [Test]
        public void TestMultipleBindingsConcreteMultipleSingle()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To(new List<Type>() {typeof(Foo), typeof(Bar)}).AsSingle();
            Container.Bind<Foo>().ToSelf().AsSingle();
            Container.Bind<Bar>().ToSelf().AsSingle();

            AssertValidates();

            var foos = Container.ResolveAll<IFoo>();
            var bars = Container.ResolveAll<IBar>();

            Assert.IsEqual(foos.Count, 2);
            Assert.IsEqual(bars.Count, 2);

            Assert.That(foos[0] is Foo);
            Assert.That(foos[1] is Bar);

            Assert.IsEqual(foos[0], bars[0]);
            Assert.IsEqual(foos[1], bars[1]);

            Assert.IsEqual(foos[0], Container.Resolve<Foo>());
            Assert.IsEqual(foos[1], Container.Resolve<Bar>());
        }

        [Test]
        public void TestMultipleBindingsConcreteMultipleTransient()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To(new List<Type>() {typeof(Foo), typeof(Bar)}).AsTransient();

            AssertValidates();

            var foos = Container.ResolveAll<IFoo>();
            var bars = Container.ResolveAll<IBar>();

            Assert.IsEqual(foos.Count, 2);
            Assert.IsEqual(bars.Count, 2);

            Assert.That(foos[0] is Foo);
            Assert.That(foos[1] is Bar);

            Assert.IsNotEqual(foos[0], bars[0]);
            Assert.IsNotEqual(foos[1], bars[1]);
        }

        [Test]
        public void TestMultipleBindingsConcreteMultipleCached()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To(new List<Type>() {typeof(Foo), typeof(Bar)}).AsCached();
            Container.Bind<Foo>().ToSelf().AsSingle();
            Container.Bind<Bar>().ToSelf().AsSingle();

            AssertValidates();

            var foos = Container.ResolveAll<IFoo>();
            var bars = Container.ResolveAll<IBar>();

            Assert.IsEqual(foos.Count, 2);
            Assert.IsEqual(bars.Count, 2);

            Assert.That(foos[0] is Foo);
            Assert.That(foos[1] is Bar);

            Assert.IsEqual(foos[0], bars[0]);
            Assert.IsEqual(foos[1], bars[1]);

            Assert.IsNotEqual(foos[0], Container.Resolve<Foo>());
            Assert.IsNotEqual(foos[1], Container.Resolve<Bar>());
        }

        [Test]
        public void TestSingletonIdsSameIdsSameInstance()
        {
            Container.Bind<IBar>().To<Foo>().AsSingle("foo");
            Container.Bind<IFoo>().To<Foo>().AsSingle("foo");
            Container.Bind<Foo>().ToSelf().AsSingle("foo");

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<IBar>());
        }

        [Test]
        public void TestSingletonIdsDifferentIdsDifferentInstances()
        {
            Container.Bind<IFoo>().To<Foo>().AsSingle("foo");
            Container.Bind<Foo>().ToSelf().AsSingle("bar");

            AssertValidates();

            Assert.IsNotEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestSingletonIdsNoIdDifferentInstances()
        {
            Container.Bind<IFoo>().To<Foo>().AsSingle();
            Container.Bind<Foo>().ToSelf().AsSingle("bar");

            AssertValidates();

            Assert.IsNotEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestSingletonIdsManyInstances()
        {
            Container.Bind<IFoo>().To<Foo>().AsSingle("foo1");
            Container.Bind<IFoo>().To<Foo>().AsSingle("foo2");
            Container.Bind<IFoo>().To<Foo>().AsSingle("foo3");
            Container.Bind<IFoo>().To<Foo>().AsSingle("foo4");

            AssertValidates();

            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 4);
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

        class Bar : IFoo, IBar
        {
        }

        public class Qux
        {
        }
    }
}
