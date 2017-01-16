using System.Collections.Generic;
using NUnit.Framework;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestPooledFactoryTo1 : ZenjectUnitTestFixture
    {
        [Test]
        public void TestFactoryProperties()
        {
            Container.BindPooledFactory<Foo, Foo.Factory>();

            var factory = Container.Resolve<Foo.Factory>();

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumCreated, 0);
            Assert.IsEqual(factory.NumInactive, 0);

            var foo = factory.Spawn("asdf");

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumCreated, 1);
            Assert.IsEqual(factory.NumInactive, 0);
            Assert.IsEqual(foo.ResetCount, 1);
            Assert.IsEqual(foo.Value, "asdf");

            factory.Despawn(foo);

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumCreated, 1);
            Assert.IsEqual(factory.NumInactive, 1);
            Assert.IsEqual(foo.ResetCount, 1);

            foo = factory.Spawn("zxcv");

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumCreated, 1);
            Assert.IsEqual(factory.NumInactive, 0);
            Assert.IsEqual(foo.ResetCount, 2);
            Assert.IsEqual(foo.Value, "zxcv");

            var foo2 = factory.Spawn("qwer");

            Assert.IsEqual(factory.NumActive, 2);
            Assert.IsEqual(factory.NumCreated, 2);
            Assert.IsEqual(factory.NumInactive, 0);
            Assert.IsEqual(foo2.ResetCount, 1);
            Assert.IsEqual(foo2.Value, "qwer");

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

        class Foo : IPoolable<string>
        {
            public Foo()
            {
            }

            public string Value
            {
                get;
                private set;
            }

            public int ResetCount
            {
                get; private set;
            }

            public void OnDespawned()
            {
            }

            public void OnSpawned(string value)
            {
                Value = value;
                ResetCount++;
            }

            public class Factory : PooledFactory<string, Foo>
            {
            }
        }
    }
}


