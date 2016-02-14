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
            Binder.BindIFactoryUntyped<Test4>().ToInstance(test4);

            Assert.IsNotNull(ReferenceEquals(test4, Resolver.Resolve<IFactoryUntyped<Test4>>().Create()));
        }

        [Test]
        public void TestToMethod()
        {
            var test3 = new Test3();

            Binder.BindIFactoryUntyped<ITest>().ToMethod((c, args) => test3);

            Assert.That(ReferenceEquals(test3, Resolver.Resolve<IFactoryUntyped<ITest>>().Create()));
        }

        [Test]
        public void TestToFactory()
        {
            Binder.BindIFactoryUntyped<Test4>().ToFactory();

            Assert.IsNotNull(Resolver.Resolve<IFactoryUntyped<Test4>>().Create());
        }

        [Test]
        public void TestToDerivedFactory()
        {
            Binder.BindIFactoryUntyped<ITest>().ToFactory<Test2>();

            Assert.IsNotNull(Resolver.Resolve<IFactoryUntyped<ITest>>().Create() is Test2);
        }

        [Test]
        public void TestToCustomFactory()
        {
            Binder.BindIFactoryUntyped<ITest>().ToCustomFactory<Test2.Factory>();

            Assert.IsNotNull(Resolver.Resolve<IFactoryUntyped<ITest>>().Create() is Test2);
        }
    }
}

