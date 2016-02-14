using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
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
            Binder.Bind<Test2>().ToSingle();
            Binder.Bind<int>().ToInstance(1);

            Assert.That(Resolver.ValidateResolve<Test2>().IsEmpty());
            var test1 = Resolver.Resolve<Test2>();
            Assert.IsEqual(test1.val, 1);
        }
    }
}
