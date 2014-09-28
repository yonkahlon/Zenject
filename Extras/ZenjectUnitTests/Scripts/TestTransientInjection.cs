using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestTransientInjection : TestWithContainer
    {
        private class Test1
        {
        }

        [Test]
        public void TestTransientType()
        {
            Container.Bind<Test1>().ToTransient();

            TestAssert.That(Container.ValidateResolve<Test1>().IsEmpty());

            var test1 = Container.Resolve<Test1>();
            var test2 = Container.Resolve<Test1>();

            TestAssert.That(test1 != null && test2 != null);
            TestAssert.That(!ReferenceEquals(test1, test2));
        }

        [Test]
        public void TestTransientTypeUntyped()
        {
            Container.Bind(typeof(Test1)).ToTransient();

            TestAssert.That(Container.ValidateResolve<Test1>().IsEmpty());

            var test1 = Container.Resolve<Test1>();
            var test2 = Container.Resolve<Test1>();

            TestAssert.That(test1 != null && test2 != null);
            TestAssert.That(!ReferenceEquals(test1, test2));
        }
    }
}


