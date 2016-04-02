using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Injection
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
            Container.Bind<Test0>().ToSelf().AsSingle();
            Container.Bind<Test0>().ToSelf().AsSingle();

            Container.Bind<Test1>().ToSelf().AsSingle();

            AssertValidationFails();

            Assert.Throws(
                delegate { Container.Resolve<Test1>(); });

            Assert.That(Container.ValidateResolve<Test1>().Any());
        }
    }
}


