using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestBaseClassPropertyInjection : TestWithContainer
    {
        class Test0
        {
        }

        class Test3
        {
        }

        class Test1 : Test3
        {
            [Inject] protected Test0 val = null;

            public Test0 GetVal()
            {
                return val;
            }
        }

        class Test2 : Test1
        {
        }

        [Test]
        public void TestCaseBaseClassPropertyInjection()
        {
            Binder.Bind<Test0>().ToSingle();
            Binder.Bind<Test2>().ToSingle();

            AssertValidates();

            Assert.That(Resolver.ValidateResolve<Test2>().IsEmpty());
            var test1 = Resolver.Resolve<Test2>();

            Assert.That(test1.GetVal() != null);
        }
    }
}


