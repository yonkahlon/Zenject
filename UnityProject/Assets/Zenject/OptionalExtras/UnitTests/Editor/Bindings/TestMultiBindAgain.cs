using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestMultiBindAgain : TestWithContainer
    {
        class Test0
        {
        }

        class Test3 : Test0
        {
        }

        class Test4 : Test0
        {
        }

        class Test2
        {
            public Test0 test;

            public Test2(Test0 test)
            {
                this.test = test;
            }
        }

        class Test1
        {
            public List<Test0> test;

            public Test1(List<Test0> test)
            {
                this.test = test;
            }
        }

        [Test]
        [ExpectedException]
        public void TestMultiBind2()
        {
            // Multi-binds should not map to single-binds
            Binder.Bind<Test0>().ToSingle<Test3>();
            Binder.Bind<Test0>().ToSingle<Test4>();
            Binder.Bind<Test2>().ToSingle();

            AssertValidates();

            Assert.That(Resolver.ValidateResolve<Test2>().IsEmpty());
            Resolver.Resolve<Test2>();
        }
    }
}


