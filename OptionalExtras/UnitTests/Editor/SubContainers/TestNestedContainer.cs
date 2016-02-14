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
    public class TestNestedContainer : TestWithContainer
    {
        public interface IFoo
        {
            int GetBar();
        }

        public class Foo : IFoo
        {
            public int GetBar()
            {
                return 0;
            }
        }

        public class Foo2 : IFoo
        {
            public int GetBar()
            {
                return 1;
            }
        }

        [Test]
        public void TestCase1()
        {
            var container1 = new DiContainer();

            Assert.Throws<ZenjectResolveException>(() => container1.Resolver.Resolve<IFoo>());
            Assert.Throws<ZenjectResolveException>(() => Resolver.Resolve<IFoo>());

            Binder.Bind<IFoo>().ToSingle<Foo>();

            Assert.Throws<ZenjectResolveException>(() => container1.Resolver.Resolve<IFoo>());
            Assert.IsEqual(Resolver.Resolve<IFoo>().GetBar(), 0);

            var container2 = new DiContainer(Container);

            Assert.IsEqual(container2.Resolver.Resolve<IFoo>().GetBar(), 0);
            Assert.IsEqual(Resolver.Resolve<IFoo>().GetBar(), 0);

            container2.Binder.Bind<IFoo>().ToSingle<Foo2>();

            Assert.IsEqual(container2.Resolver.Resolve<IFoo>().GetBar(), 1);
            Assert.IsEqual(Resolver.Resolve<IFoo>().GetBar(), 0);
        }
    }
}
