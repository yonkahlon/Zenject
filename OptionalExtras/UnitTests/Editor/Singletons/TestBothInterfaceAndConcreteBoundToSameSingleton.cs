using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestBothInterfaceAndConcreteBoundToSameSingleton : TestWithContainer
    {
        abstract class Test0
        {
        }

        class Test1 : Test0
        {
        }

        [Test]
        public void TestCaseBothInterfaceAndConcreteBoundToSameSingleton()
        {
            Binder.Bind<Test0>().ToSingle<Test1>();
            Binder.Bind<Test1>().ToSingle();

            Assert.That(Resolver.ValidateResolve<Test0>().IsEmpty());
            var test1 = Resolver.Resolve<Test0>();

            Assert.That(Resolver.ValidateResolve<Test1>().IsEmpty());
            var test2 = Resolver.Resolve<Test1>();

            Assert.That(ReferenceEquals(test1, test2));
        }
    }
}


