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
    public class TestDuplicateInjection : TestWithContainer
    {
        class Test0
        {
        }

        class Test1
        {
            public Test1(Test0 test1)
            {
            }
        }

        [Test]
        public void TestCaseDuplicateInjection()
        {
            Binder.Bind<Test0>().ToSingle();
            Binder.Bind<Test0>().ToSingle();

            Binder.Bind<Test1>().ToSingle();

            AssertValidationFails();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test1>(); });

            Assert.That(Resolver.ValidateResolve<Test1>().Any());
        }
    }
}


