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
    public class TestFactory : TestWithContainer
    {
        class Test0
        {
        }

        class Test1
        {
        }

        class Test2
        {
            [Inject]
            public Test1 test1 = null;

            [Inject]
            public Test0 test0 = null;

            [Inject]
            public int value = 0;

            // Test1 should be provided from container
            public class Factory : Factory<int, Test1, Test2>
            {
            }
        }

        [Test]
        public void Test()
        {
            Container.Bind<Test0>().ToSingle();
            Container.Bind<Test2.Factory>().ToSingle();

            AssertValidates();

            var factory = Container.Resolve<Test2.Factory>();
            var test = factory.Create(5, new Test1());

            Assert.That(test.value == 5);
        }
    }
}


