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
            var nestedContainer = new DiContainer();

            Assert.Throws<ZenjectResolveException>(() => nestedContainer.Resolve<IFoo>());
            Assert.Throws<ZenjectResolveException>(() => Container.Resolve<IFoo>());

            Container.Bind<IFoo>().ToSingle<Foo>();

            Assert.Throws<ZenjectResolveException>(() => nestedContainer.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<IFoo>().GetBar(), 0);

            nestedContainer.FallbackProvider = new DiContainerProvider(Container);

            Assert.IsEqual(nestedContainer.Resolve<IFoo>().GetBar(), 0);
            Assert.IsEqual(Container.Resolve<IFoo>().GetBar(), 0);

            nestedContainer.Bind<IFoo>().ToSingle<Foo2>();

            Assert.IsEqual(nestedContainer.Resolve<IFoo>().GetBar(), 1);
            Assert.IsEqual(Container.Resolve<IFoo>().GetBar(), 0);
        }
    }
}
