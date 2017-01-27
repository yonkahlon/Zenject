using System.Collections.Generic;
using NUnit.Framework;
using Assert=ModestTree.Assert;

#pragma warning disable 219

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestPooledFactoryTo0 : ZenjectUnitTestFixture
    {
        [Test]
        public void TestFactoryProperties()
        {
            Container.BindPooledFactory<Foo, Foo.Factory>();

            var factory = Container.Resolve<Foo.Factory>();

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumCreated, 0);
            Assert.IsEqual(factory.NumInactive, 0);

            var foo = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumCreated, 1);
            Assert.IsEqual(factory.NumInactive, 0);
            Assert.IsEqual(foo.ResetCount, 1);

            factory.Despawn(foo);

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumCreated, 1);
            Assert.IsEqual(factory.NumInactive, 1);
            Assert.IsEqual(foo.ResetCount, 1);

            foo = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumCreated, 1);
            Assert.IsEqual(factory.NumInactive, 0);
            Assert.IsEqual(foo.ResetCount, 2);

            var foo2 = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 2);
            Assert.IsEqual(factory.NumCreated, 2);
            Assert.IsEqual(factory.NumInactive, 0);
            Assert.IsEqual(foo2.ResetCount, 1);

            factory.Despawn(foo);

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumCreated, 2);
            Assert.IsEqual(factory.NumInactive, 1);
            Assert.IsEqual(foo.ResetCount, 2);

            factory.Despawn(foo2);

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumCreated, 2);
            Assert.IsEqual(factory.NumInactive, 2);
        }

        [Test]
        public void TestExpandDouble()
        {
            Container.BindPooledFactory<Foo, Foo.Factory>().ExpandByDoubling();

            var factory = Container.Resolve<Foo.Factory>();

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumCreated, 0);
            Assert.IsEqual(factory.NumInactive, 0);

            var foo = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumCreated, 1);
            Assert.IsEqual(factory.NumInactive, 0);

            var foo2 = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 2);
            Assert.IsEqual(factory.NumCreated, 2);
            Assert.IsEqual(factory.NumInactive, 0);

            var foo3 = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 3);
            Assert.IsEqual(factory.NumCreated, 4);
            Assert.IsEqual(factory.NumInactive, 1);

            factory.Despawn(foo2);

            Assert.IsEqual(factory.NumActive, 2);
            Assert.IsEqual(factory.NumCreated, 4);
            Assert.IsEqual(factory.NumInactive, 2);

            var foo4 = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 3);
            Assert.IsEqual(factory.NumCreated, 4);
            Assert.IsEqual(factory.NumInactive, 1);
        }

        [Test]
        public void TestFixedSize()
        {
            Container.BindPooledFactory<Foo, Foo.Factory>().WithFixedSize(2);

            var factory = Container.Resolve<Foo.Factory>();

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumCreated, 2);
            Assert.IsEqual(factory.NumInactive, 2);

            var foo = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumCreated, 2);
            Assert.IsEqual(factory.NumInactive, 1);

            var foo2 = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 2);
            Assert.IsEqual(factory.NumCreated, 2);
            Assert.IsEqual(factory.NumInactive, 0);

            Assert.Throws<PoolExceededFixedSizeException>(() => factory.Spawn());
        }

        [Test]
        public void TestInitialSize()
        {
            Container.BindPooledFactory<Foo, Foo.Factory>().WithInitialSize(5);

            var factory = Container.Resolve<Foo.Factory>();

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumCreated, 5);
            Assert.IsEqual(factory.NumInactive, 5);
        }

        class Bar
        {
            public Bar()
            {
            }

            public class Factory : Factory<Bar>
            {
            }
        }

        class Foo : IPoolable
        {
            public Foo()
            {
            }

            public int ResetCount
            {
                get; private set;
            }

            public void OnDespawned()
            {
            }

            public void OnSpawned()
            {
                ResetCount++;
            }

            public class Factory : PooledFactory<Foo>
            {
            }
        }

        [Test]
        public void TestSubContainers()
        {
            Container.BindPooledFactory<Qux, Qux.Factory>().FromSubContainerResolve().ByMethod(InstallQux).NonLazy();

            var factory = Container.Resolve<Qux.Factory>();
            var qux = factory.Spawn();
        }

        void InstallQux(DiContainer subContainer)
        {
            subContainer.Bind<Qux>().AsSingle();
        }

        class Qux : IPoolable
        {
            public void OnDespawned()
            {
            }

            public void OnSpawned()
            {
            }

            public class Factory : PooledFactory<Qux>
            {
            }
        }
    }
}

