using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestListInjection : TestWithContainer
    {
        class Test1
        {
            public Test1(List<int> values)
            {
            }
        }

        class Test2
        {
            public Test2( 
                [InjectOptional] List<int> values)
            {
            }
        }

        class Test3
        {
            [Inject]
            public List<int> values = null;
        }

        class Test4
        {
            [InjectOptional]
            public List<int> values = null;
        }

        [Test]
        [ExpectedException]
        public void TestCase1()
        {
            Binder.Bind<Test1>().ToSingle();

            Resolver.ResolveAll<Test1>();
        }

        [Test]
        public void TestCase2()
        {
            Binder.Bind<Test2>().ToSingle();

            Assert.That(Resolver.ValidateResolve<Test2>().IsEmpty());
            var result = Resolver.ResolveAll<Test2>();

            Assert.That(result != null);
        }

        [Test]
        [ExpectedException(typeof(ZenjectResolveException))]
        public void TestCase3()
        {
            Binder.Bind<Test3>().ToSingle();

            Resolver.ResolveAll<Test3>();
        }

        [Test]
        public void TestCase4()
        {
            Binder.Bind<Test4>().ToSingle();

            var result = Resolver.ResolveAll<Test4>();

            Assert.That(result != null);
        }
    }
}



