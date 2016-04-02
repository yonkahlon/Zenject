using System;
using System.Collections.Generic;
using System.Reflection;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.BindFeatures
{
    [TestFixture]
    public class TestMultipleContractTypes : TestWithContainer
    {
        class Test1
        {
        }

        class Test2 : Test1
        {
        }

        class Test3 : Test1
        {
        }

        class TestImpl1
        {
            public List<Test1> tests;

            public TestImpl1(List<Test1> tests)
            {
                this.tests = tests;
            }
        }

        class TestImpl2
        {
            [Inject]
            public List<Test1> tests = null;
        }

        [Test]
        public void TestMultiBind1()
        {
            Container.Bind<Test1>().To<Test2>().AsSingle();
            Container.Bind<Test1>().To<Test3>().AsSingle();
            Container.Bind<TestImpl1>().ToSelf().AsSingle();

            AssertValidates();

            Assert.That(Container.ValidateResolve<TestImpl1>().IsEmpty());
            var test1 = Container.Resolve<TestImpl1>();

            Assert.That(test1.tests.Count == 2);
        }

        [Test]
        public void TestMultiBind2()
        {
            Container.Bind<TestImpl1>().ToSelf().AsSingle();

            AssertValidationFails();

            // optional list dependencies should be declared as optional
            Assert.Throws(
                delegate { Container.Resolve<TestImpl1>(); });

            Assert.That(Container.ValidateResolve<TestImpl1>().Any());
        }

        [Test]
        public void TestMultiBind2Validate()
        {
            Container.Bind<TestImpl1>().ToSelf().AsSingle();

            AssertValidationFails();

            Assert.Throws(
                delegate { Container.Resolve<TestImpl1>(); });

            Assert.That(Container.ValidateResolve<Test2>().Any());
        }

        [Test]
        public void TestMultiBindListInjection()
        {
            Container.Bind<Test1>().To<Test2>().AsSingle();
            Container.Bind<Test1>().To<Test3>().AsSingle();
            Container.Bind<TestImpl2>().ToSelf().AsSingle();

            AssertValidates();

            Assert.That(Container.ValidateResolve<TestImpl2>().IsEmpty());
            var test = Container.Resolve<TestImpl2>();
            Assert.That(test.tests.Count == 2);
        }
    }
}

