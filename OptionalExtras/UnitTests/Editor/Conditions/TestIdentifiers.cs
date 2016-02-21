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
    public class TestIdentifiers : TestWithContainer
    {
        class Test0
        {
        }

        [Test]
        public void TestBasic()
        {
            Binder.Bind<Test0>("foo").ToTransient();

            AssertValidates();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test0>(); });

            Resolver.Resolve<Test0>("foo");
            Assert.That(Resolver.ValidateResolve<Test0>("foo").IsEmpty());
        }

        [Test]
        public void TestBasic2()
        {
            Binder.Bind<Test0>("foo").ToSingle();

            AssertValidates();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test0>(); });

            Resolver.Resolve<Test0>("foo");
            Assert.That(Resolver.ValidateResolve<Test0>("foo").IsEmpty());
        }

        [Test]
        public void TestBasic3()
        {
            Binder.Bind<Test0>("foo").ToMethod((ctx) => new Test0());

            AssertValidates();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test0>(); });

            Resolver.Resolve<Test0>("foo");
            Assert.That(Resolver.ValidateResolve<Test0>("foo").IsEmpty());
        }

        [Test]
        public void TestBasic4()
        {
            Binder.Bind<Test0>("foo").ToTransient();
            Binder.Bind<Test0>("foo").ToTransient();

            AssertValidates();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test0>(); });

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test0>("foo"); });

            Assert.IsEqual(Resolver.ResolveAll<Test0>("foo").Count, 2);
        }

        [Test]
        public void TestToMethodUntyped()
        {
            Binder.Bind(typeof(Test0)).ToMethod((ctx) => new Test0());

            AssertValidates();

            Resolver.Resolve<Test0>();

            Assert.That(Resolver.ValidateResolve<Test0>().IsEmpty());
        }
    }
}
