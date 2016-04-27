using System;
using NUnit.Framework;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestInheritInSubContainers : TestWithContainer
    {
        [Test]
        public void TestFromNew1()
        {
            Container.Bind<Foo>().AsSingle();

            var sub1 = Container.CreateSubContainer();

            Assert.IsEqual(sub1.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestFromNew2()
        {
            Container.Bind<Foo>().AsSingle().InheritInSubContainers();

            var sub1 = Container.CreateSubContainer();

            Assert.IsNotEqual(sub1.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestFromResolve()
        {
            Container.Bind<IBar>().To<Bar>().FromResolve().InheritInSubContainers();
            Container.Bind<Bar>().AsSingle();

            var sub1 = Container.CreateSubContainer();

            Assert.IsEqual(Container.ResolveAll<IBar>().Count, 1);
            Assert.IsEqual(sub1.ResolveAll<IBar>().Count, 2);
        }

        public interface IBar
        {
        }

        public class Foo
        {
        }

        public class Bar : IBar
        {
        }
    }
}
