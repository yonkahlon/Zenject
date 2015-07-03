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
    public class TestCircularDependencies : TestWithContainer
    {
        class Test1
        {
            public static int CreateCount;

            [Inject]
            public Test2 Other = null;

            public Test1()
            {
                CreateCount++;
            }
        }

        class Test2
        {
            public static int CreateCount;

            [Inject]
            public Test1 Other = null;

            public Test2()
            {
                CreateCount++;
            }
        }

        [Test]
        public void TestFields()
        {
            Test2.CreateCount = 0;
            Test1.CreateCount = 0;

            Container.Bind<Test1>().ToSingle();
            Container.Bind<Test2>().ToSingle();

            var test1 = Container.Resolve<Test1>();
            var test2 = Container.Resolve<Test2>();

            Assert.IsEqual(Test2.CreateCount, 1);
            Assert.IsEqual(Test1.CreateCount, 1);
            Assert.IsEqual(test1.Other, test2);
            Assert.IsEqual(test2.Other, test1);
        }

        class Test3
        {
            public static int CreateCount;

            public Test4 Other = null;

            public Test3()
            {
                CreateCount++;
            }

            [PostInject]
            public void Initialize(Test4 other)
            {
                this.Other = other;
            }
        }

        class Test4
        {
            public static int CreateCount;

            public Test3 Other;

            public Test4()
            {
                CreateCount++;
            }

            [PostInject]
            public void Initialize(Test3 other)
            {
                this.Other = other;
            }
        }

        [Test]
        public void TestPostInject()
        {
            Test4.CreateCount = 0;
            Test3.CreateCount = 0;

            Container.Bind<Test3>().ToSingle();
            Container.Bind<Test4>().ToSingle();

            var test1 = Container.Resolve<Test3>();
            var test2 = Container.Resolve<Test4>();

            Assert.IsEqual(Test4.CreateCount, 1);
            Assert.IsEqual(Test3.CreateCount, 1);
            Assert.IsEqual(test1.Other, test2);
            Assert.IsEqual(test2.Other, test1);
        }
    }
}


