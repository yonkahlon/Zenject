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
    public class TestIFactoryUntyped : TestWithContainer
    {
        public interface ITest
        {
        }

        class Test2 : ITest
        {
            public class Factory : FactoryUntyped<ITest, Test2>
            {
            }
        }

        class Test3 : ITest
        {
        }

        class Test4
        {
        }

        class Test5 : ITest
        {
            public Test5(string param)
            {
            }

            public class Factory : Factory<string, Test5>
            {
            }
        }

        [Test]
        public void TestToInstance()
        {
            var test4 = new Test4();
            Container.BindIFactoryUntyped<Test4>().ToInstance(test4);

            AssertValidates();

            Assert.IsNotNull(ReferenceEquals(test4, Container.Resolve<IFactoryUntyped<Test4>>().Create()));
        }

        [Test]
        public void TestToMethod()
        {
            var test3 = new Test3();

            Container.BindIFactoryUntyped<ITest>().ToMethod((c, args) => test3);

            AssertValidates();

            Assert.That(ReferenceEquals(test3, Container.Resolve<IFactoryUntyped<ITest>>().Create()));
        }

        [Test]
        public void TestToFactory()
        {
            Container.BindIFactoryUntyped<Test4>().ToFactory();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<IFactoryUntyped<Test4>>().Create());
        }

        [Test]
        public void TestToDerivedFactory()
        {
            Container.BindIFactoryUntyped<ITest>().ToFactory<Test2>();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<IFactoryUntyped<ITest>>().Create() is Test2);
        }

        [Test]
        public void TestToCustomFactory()
        {
            Container.BindIFactoryUntyped<ITest>().ToCustomFactory<Test2.Factory>();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<IFactoryUntyped<ITest>>().Create() is Test2);
        }
    }
}

