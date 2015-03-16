using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestPropertyInjection : TestWithContainer
    {
        class Test1
        {
        }

        class Test2
        {
            [Inject]
            public Test1 val2 { get; private set; }

            [Inject]
            Test1 val4 { get; set; }

            public Test1 GetVal4()
            {
                return val4;
            }
        }

        [Test]
        public void TestPublicPrivate()
        {
            var test1 = new Test1();

            Container.Bind<Test2>().ToSingle();
            Container.Bind<Test1>().To(test1);

            Assert.That(Container.ValidateResolve<Test2>().IsEmpty());
            var test2 = Container.Resolve<Test2>();

            Assert.IsEqual(test2.val2, test1);
            Assert.IsEqual(test2.GetVal4(), test1);
        }

        [Test]
        public void TestCase2()
        {
        }
    }
}


