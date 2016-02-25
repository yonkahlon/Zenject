using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestTransientInjection : TestWithContainer
    {
        class Test1
        {
        }

        [Test]
        public void TestTransientType()
        {
            Container.Bind<Test1>().ToTransient();

            Assert.That(Container.ValidateResolve<Test1>().IsEmpty());

            AssertValidates();

            var test1 = Container.Resolve<Test1>();
            var test2 = Container.Resolve<Test1>();

            Assert.That(test1 != null && test2 != null);
            Assert.That(!ReferenceEquals(test1, test2));
        }

        [Test]
        public void TestTransientTypeUntyped()
        {
            Container.Bind(typeof(Test1)).ToTransient();

            AssertValidates();

            Assert.That(Container.ValidateResolve<Test1>().IsEmpty());

            var test1 = Container.Resolve<Test1>();
            var test2 = Container.Resolve<Test1>();

            Assert.That(test1 != null && test2 != null);
            Assert.That(!ReferenceEquals(test1, test2));
        }
    }
}


