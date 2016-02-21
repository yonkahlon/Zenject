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
    public class TestParameters : TestWithContainer
    {
        class Test1
        {
            public int f1;
            public int f2;

            public Test1(int f1, int f2)
            {
                this.f1 = f1;
                this.f2 = f2;
            }
        }

        [Test]
        public void TestExtraParametersSameType()
        {
            var test1 = Container.Instantiator.Instantiate<Test1>(5, 10);

            Assert.That(test1 != null);
            Assert.That(test1.f1 == 5 && test1.f2 == 10);

            var test2 = Container.Instantiator.Instantiate<Test1>(10, 5);

            Assert.That(test2 != null);
            Assert.That(test2.f1 == 10 && test2.f2 == 5);
        }

        [Test]
        public void TestMissingParameterThrows()
        {
            Binder.Bind<Test1>().ToTransient();

            AssertValidationFails();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test1>(); });

            Assert.That(Resolver.ValidateResolve<Test1>().Any());
        }
    }
}


