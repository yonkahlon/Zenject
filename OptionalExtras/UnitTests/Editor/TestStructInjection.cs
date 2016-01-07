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
        private struct Test1
        {
        }

        private class Test2
        {
            public Test2(Test1 t1)
            {
            }
        }

        [Test]
        public void TestCase1()
        {
            Container.Bind<Test1>().ToInstance(new Test1());
            Container.Bind<Test2>().ToSingle();

            Assert.That(Container.ValidateResolve<Test2>().IsEmpty());
            var t2 = Container.Resolve<Test2>();

            Assert.That(t2 != null);
        }
    }
}

