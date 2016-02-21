using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestGenericContract : TestWithContainer
    {
        class Test1<T>
        {
            public T Data;
        }

        class Test2
        {
        }

        [Test]
        public void TestToSingle()
        {
            Binder.Bind(typeof(Test1<>)).ToSingle();

            AssertValidates();

            var test1 = Resolver.Resolve<Test1<int>>();
            Assert.That(test1.Data == 0);
            test1.Data = 5;

            var test2 = Resolver.Resolve<Test1<int>>();

            Assert.That(test2 == test1);
            Assert.That(test1.Data == 5);
        }

        [Test]
        public void TestToTransient()
        {
            Binder.Bind(typeof(Test1<>)).ToTransient();

            AssertValidates();

            var test1 = Resolver.Resolve<Test1<int>>();
            Assert.That(test1.Data == 0);

            var test2 = Resolver.Resolve<Test1<int>>();
            Assert.That(test2.Data == 0);
            Assert.That(test2 != test1);

            Resolver.Resolve<Test1<string>>();
            Resolver.Resolve<Test1<List<int>>>();
            Resolver.Resolve<Test1<Test2>>();
        }
    }
}
