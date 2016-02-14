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
            Binder.Bind<Test1>().ToTransient();

            Assert.That(Resolver.ValidateResolve<Test1>().IsEmpty());

            var test1 = Resolver.Resolve<Test1>();
            var test2 = Resolver.Resolve<Test1>();

            Assert.That(test1 != null && test2 != null);
            Assert.That(!ReferenceEquals(test1, test2));
        }

        [Test]
        public void TestTransientTypeUntyped()
        {
            Binder.Bind(typeof(Test1)).ToTransient();

            Assert.That(Resolver.ValidateResolve<Test1>().IsEmpty());

            var test1 = Resolver.Resolve<Test1>();
            var test2 = Resolver.Resolve<Test1>();

            Assert.That(test1 != null && test2 != null);
            Assert.That(!ReferenceEquals(test1, test2));
        }
    }
}


