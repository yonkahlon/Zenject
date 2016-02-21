using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestMultipleInterfaceSameSingle : TestWithContainer
    {
        interface ITest1
        {
        }

        interface ITest2
        {
        }

        class Test1 : ITest1, ITest2
        {
        }

        [Test]
        public void TestCase1()
        {
            Binder.Bind<ITest1>().ToSingle<Test1>();
            Binder.Bind<ITest2>().ToSingle<Test1>();

            AssertValidates();

            Assert.That(Resolver.ValidateResolve<ITest1>().IsEmpty());
            var test1 = Resolver.Resolve<ITest1>();
            Assert.That(Resolver.ValidateResolve<ITest2>().IsEmpty());
            var test2 = Resolver.Resolve<ITest2>();

            Assert.That(ReferenceEquals(test1, test2));
        }
    }
}


