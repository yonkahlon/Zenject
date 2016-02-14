using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestStructInjection : TestWithContainer
    {
        struct Test1
        {
        }

        class Test2
        {
            public Test2(Test1 t1)
            {
            }
        }

        [Test]
        public void TestCase1()
        {
            Binder.Bind<Test1>().ToInstance(new Test1());
            Binder.Bind<Test2>().ToSingle();

            Assert.That(Resolver.ValidateResolve<Test2>().IsEmpty());
            var t2 = Resolver.Resolve<Test2>();

            Assert.That(t2 != null);
        }
    }
}

