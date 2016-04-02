using System;
using NUnit.Framework;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestInheritInSubContainers : TestWithContainer
    {
        public class Foo
        {
        }

        [Test]
        public void Test1()
        {
            Container.Bind<Foo>().ToSelf().AsSingle();

            var sub1 = Container.CreateSubContainer();

            Assert.IsEqual(sub1.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void Test2()
        {
            Container.Bind<Foo>().ToSelf().AsSingle().InheritInSubContainers();

            var sub1 = Container.CreateSubContainer();

            Assert.IsNotEqual(sub1.Resolve<Foo>(), Container.Resolve<Foo>());
        }
    }
}



