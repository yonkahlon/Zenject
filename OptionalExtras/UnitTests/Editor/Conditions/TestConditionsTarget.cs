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
        class Test0
        {
        }

        class Test1
        {
            public Test1(Test0 test)
            {
            }
        }

        class Test2
        {
            public Test2(Test0 test)
            {
            }
        }

        public override void Setup()
        {
            base.Setup();
            Binder.Bind<Test0>().ToSingle().When(r => r.ObjectType == typeof(Test2));
        }

        [Test]
        public void TestTargetConditionError()
        {
            Binder.Bind<Test1>().ToSingle();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test1>(); });

            Assert.That(Resolver.ValidateResolve<Test1>().Any());
        }

        [Test]
        public void TestTargetConditionSuccess()
        {
            Binder.Bind<Test2>().ToSingle();
            Assert.That(Resolver.ValidateResolve<Test2>().IsEmpty());
            var test2 = Resolver.Resolve<Test2>();

            Assert.That(test2 != null);
        }
    }
}


