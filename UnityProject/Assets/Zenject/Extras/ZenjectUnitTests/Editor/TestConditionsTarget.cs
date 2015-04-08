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
    public class TestConditionsTarget : TestWithContainer
    {
        private class Test0
        {
        }

        private class Test1
        {
            public Test1(Test0 test)
            {
            }
        }

        private class Test2
        {
            public Test2(Test0 test)
            {
            }
        }

        public override void Setup()
        {
            base.Setup();
            Container.Bind<Test0>().ToSingle().When(r => r.ObjectType == typeof(Test2));
        }

        [Test]
        public void TestTargetConditionError()
        {
            Container.Bind<Test1>().ToSingle();

            Assert.Throws<ZenjectResolveException>(
                delegate { Container.Resolve<Test1>(); });

            Assert.That(Container.ValidateResolve<Test1>().Any());
        }

        [Test]
        public void TestTargetConditionSuccess()
        {
            Container.Bind<Test2>().ToSingle();
            Assert.That(Container.ValidateResolve<Test2>().IsEmpty());
            var test2 = Container.Resolve<Test2>();

            Assert.That(test2 != null);
        }
    }
}


