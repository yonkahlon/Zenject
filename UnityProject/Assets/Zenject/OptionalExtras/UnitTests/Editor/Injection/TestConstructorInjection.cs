using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Injection
{
    [TestFixture]
    public class TestConstructorInjection : TestWithContainer
    {
        class Test1
        {
        }

        class Test2
        {
            public Test1 val;

            public Test2(Test1 val)
            {
                this.val = val;
            }
        }

        [Test]
        public void TestCase1()
        {
            Container.Bind<Test2>().ToSelf().AsSingle();
            Container.Bind<Test1>().ToSelf().AsSingle();

            AssertValidates();

            Assert.That(Container.ValidateResolve<Test2>().IsEmpty());
            var test1 = Container.Resolve<Test2>();

            Assert.That(test1.val != null);
        }

        [Test]
        public void TestConstructByFactory()
        {
            Container.Bind<Test2>().ToSelf().AsSingle();

            var val = new Test1();
            var test1 = Container.Instantiate<Test2>(new object[] { val });

            Assert.That(test1.val == val);
        }
    }
}


