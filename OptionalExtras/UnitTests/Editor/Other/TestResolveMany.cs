using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestResolveMany : TestWithContainer
    {
        class Test0
        {
        }

        class Test1 : Test0
        {
        }

        class Test2 : Test0
        {
        }

        [Test]
        public void TestCase1()
        {
            Container.Bind<Test0>().ToSingle<Test1>();
            Container.Bind<Test0>().ToSingle<Test2>();

            List<Test0> many = Container.ResolveAll<Test0>();

            Assert.That(many.Count == 2);
        }

        [Test]
        public void TestOptional()
        {
            List<Test0> many = Container.ResolveAll<Test0>(true);
            Assert.That(many.Count == 0);
        }
    }
}


