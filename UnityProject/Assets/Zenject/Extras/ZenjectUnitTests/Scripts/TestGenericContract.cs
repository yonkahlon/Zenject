using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;
using System.Linq;

namespace ModestTree.Zenject.Test
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
            _container.Bind(typeof(Test1<>)).ToSingle();

            var test1 = _container.Resolve<Test1<int>>();
            TestAssert.That(test1.Data == 0);
            test1.Data = 5;

            var test2 = _container.Resolve<Test1<int>>();

            TestAssert.That(test2 == test1);
            TestAssert.That(test1.Data == 5);
        }

        [Test]
        public void TestToTransient()
        {
            _container.Bind(typeof(Test1<>)).ToTransient();

            var test1 = _container.Resolve<Test1<int>>();
            TestAssert.That(test1.Data == 0);

            var test2 = _container.Resolve<Test1<int>>();
            TestAssert.That(test2.Data == 0);
            TestAssert.That(test2 != test1);

            _container.Resolve<Test1<string>>();
            _container.Resolve<Test1<List<int>>>();
            _container.Resolve<Test1<Test2>>();
        }
    }
}
