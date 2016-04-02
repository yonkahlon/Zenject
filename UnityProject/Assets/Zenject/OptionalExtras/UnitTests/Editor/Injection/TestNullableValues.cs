using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Injection
{
    [TestFixture]
    public class TestNullableValues : TestWithContainer
    {
        class Test2
        {
            public int? val;

            public Test2(int? val)
            {
                this.val = val;
            }
        }

        [Test]
        public void RunTest1()
        {
            Container.Bind<Test2>().ToSelf().AsSingle();
            Container.Bind<int>().ToInstance(1);

            AssertValidates();

            Assert.That(Container.ValidateResolve<Test2>().IsEmpty());
            var test1 = Container.Resolve<Test2>();
            Assert.IsEqual(test1.val, 1);
        }
    }
}
