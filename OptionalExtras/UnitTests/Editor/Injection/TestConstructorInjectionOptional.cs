using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestConstructorInjectionOptional : TestWithContainer
    {
        class Test1
        {
        }

        class Test2
        {
            public Test1 val;

            public Test2(Test1 val = null)
            {
                this.val = val;
            }
        }

        class Test3
        {
            public Test1 val;

            public Test3(Test1 val)
            {
                this.val = val;
            }
        }

        [Test]
        public void TestCase1()
        {
            Binder.Bind<Test2>().ToSingle();

            Assert.That(Resolver.ValidateResolve<Test2>().IsEmpty());
            var test1 = Resolver.Resolve<Test2>();

            Assert.That(test1.val == null);
        }

        [Test]
        [ExpectedException(typeof(ZenjectResolveException))]
        public void TestCase2()
        {
            Binder.Bind<Test3>().ToSingle();

            var test1 = Container.Instantiator.Instantiate<Test3>();

            Assert.That(test1.val == null);
        }

        [Test]
        public void TestConstructByFactory()
        {
            Binder.Bind<Test2>().ToSingle();

            var test1 = Container.Instantiator.Instantiate<Test2>();

            Assert.That(test1.val == null);
        }
    }
}


