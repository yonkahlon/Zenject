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
    public class TestToFactory : TestWithContainer
    {
        static Foo StaticFoo = new Foo();

        [Test]
        public void TestSelfSingle()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<Foo>().ToFactorySelf<FooFactory>().AsSingle();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>(), StaticFoo);

            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<Foo>();

            Assert.IsEqual(FooFactory.InstanceCount, 1);
        }

        [Test]
        public void TestConcreteSingle()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<IFoo>().ToFactory<Foo, FooFactory>().AsSingle();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFoo>(), StaticFoo);

            Container.Resolve<IFoo>();
            Container.Resolve<IFoo>();
            Container.Resolve<IFoo>();

            Assert.IsEqual(FooFactory.InstanceCount, 1);
        }

        [Test]
        public void TestSelfAndConcreteSingle()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<Foo>().ToFactorySelf<FooFactory>().AsSingle();
            Container.Bind<IFoo>().ToFactory<Foo, FooFactory>().AsSingle();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFoo>(), StaticFoo);
            Assert.IsEqual(Container.Resolve<Foo>(), StaticFoo);

            Container.Resolve<IFoo>();
            Container.Resolve<IFoo>();
            Container.Resolve<IFoo>();

            Assert.IsEqual(FooFactory.InstanceCount, 1);
        }

        [Test]
        public void TestSelfTransient()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<Foo>().ToFactorySelf<FooFactory>().AsTransient();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>(), StaticFoo);

            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<Foo>();

            Assert.IsEqual(FooFactory.InstanceCount, 4);
        }

        [Test]
        public void TestConcreteTransient()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<IFoo>().ToFactory<Foo, FooFactory>().AsTransient();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFoo>(), StaticFoo);

            Container.Resolve<IFoo>();
            Container.Resolve<IFoo>();
            Container.Resolve<IFoo>();

            Assert.IsEqual(FooFactory.InstanceCount, 4);
        }

        [Test]
        public void TestSelfCached()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<Foo>().ToFactorySelf<FooFactory>().AsCached();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>(), StaticFoo);

            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<Foo>();

            Assert.IsEqual(FooFactory.InstanceCount, 1);
        }

        [Test]
        public void TestConcreteCached()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<IFoo>().ToFactory<Foo, FooFactory>().AsCached();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFoo>(), StaticFoo);

            Container.Resolve<IFoo>();
            Container.Resolve<IFoo>();
            Container.Resolve<IFoo>();

            Assert.IsEqual(FooFactory.InstanceCount, 1);
        }

        class FooFactory : IFactory<Foo>
        {
            public static int InstanceCount = 0;

            public FooFactory()
            {
                InstanceCount++;
            }

            public Foo Create()
            {
                return StaticFoo;
            }
        }

        interface IFoo
        {
        }

        class Foo : IFoo
        {
        }
    }
}

