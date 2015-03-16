using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestConstructorInjection : TestWithContainer
    {
        private class Test1
        {
        }

        private class Test2
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
            Container.Bind<Test2>().ToSingle();
            Container.Bind<Test1>().ToSingle();

            Assert.That(Container.ValidateResolve<Test2>().IsEmpty());
            var test1 = Container.Resolve<Test2>();

            Assert.That(test1.val != null);
        }

        [Test]
        public void TestConstructByFactory()
        {
            Container.Bind<Test2>().ToSingle();

            var val = new Test1();
            var factory = new FactoryUntyped<Test2>(Container);
            var test1 = factory.Create(val);

            Assert.That(test1.val == val);
        }
    }
}


