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

            Binder.Bind<Test1>().ToTransient();
            Binder.Bind<Test2>().ToTransient();
            Binder.Bind<Test3>().ToTransient();
            Binder.Bind<Test4>().ToTransient();
        }

        [Test]
        public void TestUnspecifiedNameConstructorInjection()
        {
            Binder.Bind<Test0>().ToTransient();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test1>(); });

            Assert.That(Resolver.ValidateResolve<Test1>().Any());
        }

        [Test]
        public void TestUnspecifiedNameFieldInjection()
        {
            Binder.Bind<Test0>().ToTransient();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test2>(); });

            Assert.That(Resolver.ValidateResolve<Test2>().Any());
        }

        [Test]
        public void TestSuccessConstructorInjectionString()
        {
            Binder.Bind<Test0>().ToInstance(new Test0());
            Binder.Bind<Test0>("foo").ToInstance(new Test0());

            // Should not throw exceptions
            Resolver.Resolve<Test1>();

            Assert.IsNotNull(Resolver.Resolve<Test1>());
        }

        [Test]
        public void TestSuccessFieldInjectionString()
        {
            Binder.Bind<Test0>().ToInstance(new Test0());
            Binder.Bind<Test0>("foo").ToInstance(new Test0());

            Assert.That(Resolver.ValidateResolve<Test2>().IsEmpty());
            Assert.IsNotNull(Resolver.Resolve<Test2>());
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
            Binder.Bind<Test0>().ToInstance(new Test0());
            Binder.Bind<Test0>("TestValue1").ToInstance(new Test0());

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test3>(); });

            Assert.That(Resolver.ValidateResolve<Test1>().Any());
        }

        [Test]
        public void TestSuccessConstructorInjectionEnum()
        {
            Binder.Bind<Test0>().ToInstance(new Test0());
            Binder.Bind<Test0>("TestValue2").ToInstance(new Test0());

            // No exceptions
            Resolver.Resolve<Test3>();

            Assert.IsNotNull(Resolver.Resolve<Test3>());
        }

        [Test]
        public void TestFailFieldInjectionEnum()
        {
            Binder.Bind<Test0>().ToInstance(new Test0());
            Binder.Bind<Test0>("TestValue1").ToInstance(new Test0());

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test3>(); });

            Assert.That(Resolver.ValidateResolve<Test3>().Any());
        }

        [Test]
        public void TestSuccessFieldInjectionEnum()
        {
            Binder.Bind<Test0>().ToInstance(new Test0());
            Binder.Bind<Test0>("TestValue3").ToInstance(new Test0());

            Assert.That(Resolver.ValidateResolve<Test4>().IsEmpty());
            Assert.IsNotNull(Resolver.Resolve<Test4>());
        }
    }
}
