using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestPostInjectParameters : TestWithContainer
    {
        class Test0
        {
        }

        class Test1
        {
        }

        class Test2
        {
        }

        class Test3
        {
            public bool HasInitialized;

            public Test0 test0 = null;

            [Inject]
            public Test1 test1 = null;

            [PostInject]
            public void Init(
                Test0 test0,
                [InjectOptional]
                Test2 test2)
            {
                Assert.That(!HasInitialized);
                Assert.IsNotNull(test1);
                Assert.IsNull(test2);
                Assert.IsNull(this.test0);
                this.test0 = test0;
                HasInitialized = true;
            }
        }

        [Test]
        public void Test()
        {
            Container.Bind<Test1>().ToSingle();
            Container.Bind<Test3>().ToSingle();

            Assert.That(!Container.ValidateResolve<Test3>().IsEmpty());

            Container.Bind<Test0>().ToSingle();

            Assert.That(Container.ValidateResolve<Test3>().IsEmpty());

            AssertValidates();

            var test3 = Container.Resolve<Test3>();

            Assert.That(test3.HasInitialized);
            Assert.IsNotNull(test3.test0);
        }
    }
}
