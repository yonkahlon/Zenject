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
    public class TestToInstance : TestWithContainer
    {
        [Test]
        public void TestTransient()
        {
            var foo = new Foo();

            Container.Bind<IFoo>().ToInstance(foo);
            Container.Bind<Foo>().ToInstance(foo);

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<Foo>(), foo);
        }

        [Test]
        [ExpectedException]
        public void TestSingle()
        {
            Container.Bind<Foo>().ToInstance(new Foo()).AsSingle();
            Container.Bind<Foo>().ToSelf().AsSingle();

            AssertValidates();

            Container.Resolve<Foo>();
        }

        // There's really no good reason to do this but it is part of the api
        [Test]
        public void TestCached()
        {
            var foo = new Foo();

            Container.Bind<IFoo>().ToInstance(foo).AsCached();
            Container.Bind<Foo>().ToInstance(foo).AsCached();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<Foo>(), foo);
        }

        interface IFoo
        {
        }

        class Foo : IFoo
        {
        }
    }
}

