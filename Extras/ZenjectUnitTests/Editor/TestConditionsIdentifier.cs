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
    public class TestConditionsIdentifier : TestWithContainer
    {
        class Test0
        {
        }

        class Test1
        {
            public Test1(
                [Inject("foo")]
                Test0 name1)
            {
            }
        }

        class Test2
        {
            [Inject("foo")]
            public Test0 name2 = null;
        }

        public override void Setup()
        {
            base.Setup();

            Container.Bind<Test1>().ToTransient();
            Container.Bind<Test2>().ToTransient();
            Container.Bind<Test3>().ToTransient();
            Container.Bind<Test4>().ToTransient();
        }

        [Test]
        public void TestUnspecifiedNameConstructorInjection()
        {
            Container.Bind<Test0>().ToTransient();

            Assert.Throws<ZenjectResolveException>(
                delegate { Container.Resolve<Test1>(); });

            Assert.That(Container.ValidateResolve<Test1>().Any());
        }

        [Test]
        public void TestUnspecifiedNameFieldInjection()
        {
            Container.Bind<Test0>().ToTransient();

            Assert.Throws<ZenjectResolveException>(
                delegate { Container.Resolve<Test2>(); });

            Assert.That(Container.ValidateResolve<Test2>().Any());
        }

        [Test]
        public void TestSuccessConstructorInjectionString()
        {
            Container.Bind<Test0>().To(new Test0());
            Container.Bind<Test0>("foo").To(new Test0());

            // Should not throw exceptions
            Container.Resolve<Test1>();

            Assert.IsNotNull(Container.Resolve<Test1>());
        }

        [Test]
        public void TestSuccessFieldInjectionString()
        {
            Container.Bind<Test0>().To(new Test0());
            Container.Bind<Test0>("foo").To(new Test0());

            Assert.That(Container.ValidateResolve<Test2>().IsEmpty());
            Assert.IsNotNull(Container.Resolve<Test2>());
        }

        class Test3
        {
            public Test3(
                [Inject("TestValue2")]
                Test0 test0)
            {
            }
        }

        class Test4
        {

        }

        [Test]
        public void TestFailConstructorInjectionEnum()
        {
            Container.Bind<Test0>().To(new Test0());
            Container.Bind<Test0>("TestValue1").To(new Test0());

            Assert.Throws<ZenjectResolveException>(
                delegate { Container.Resolve<Test3>(); });

            Assert.That(Container.ValidateResolve<Test1>().Any());
        }

        [Test]
        public void TestSuccessConstructorInjectionEnum()
        {
            Container.Bind<Test0>().To(new Test0());
            Container.Bind<Test0>("TestValue2").To(new Test0());

            // No exceptions
            Container.Resolve<Test3>();

            Assert.IsNotNull(Container.Resolve<Test3>());
        }

        [Test]
        public void TestFailFieldInjectionEnum()
        {
            Container.Bind<Test0>().To(new Test0());
            Container.Bind<Test0>("TestValue1").To(new Test0());

            Assert.Throws<ZenjectResolveException>(
                delegate { Container.Resolve<Test3>(); });

            Assert.That(Container.ValidateResolve<Test3>().Any());
        }

        [Test]
        public void TestSuccessFieldInjectionEnum()
        {
            Container.Bind<Test0>().To(new Test0());
            Container.Bind<Test0>("TestValue3").To(new Test0());

            Assert.That(Container.ValidateResolve<Test4>().IsEmpty());
            Assert.IsNotNull(Container.Resolve<Test4>());
        }
    }
}
