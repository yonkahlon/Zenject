using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;
using System.Linq;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestInjectSources
    {
        class Test0
        {
        }

        class Test1
        {
            public Test0 val;

            public Test1(
                [Inject(InjectSources.Local)]
                Test0 val)
            {
                this.val = val;
            }
        }

        class Test2
        {
            public Test0 val;

            public Test2(
                [Inject(InjectSources.Parent)]
                Test0 val)
            {
                this.val = val;
            }
        }

        class Test3
        {
            public Test0 val;

            public Test3(
                [Inject(InjectSources.AnyParent)]
                Test0 val)
            {
                this.val = val;
            }
        }

        class Test4
        {
            public Test0 val;

            public Test4(
                [Inject(InjectSources.Any)]
                Test0 val)
            {
                this.val = val;
            }
        }

        [Test]
        public void TestAny()
        {
            var rootContainer = new DiContainer();
            var sub1 = rootContainer.CreateSubContainer();

            rootContainer.Binder.Bind<Test0>().ToSingle();
            sub1.Binder.Bind<Test4>().ToSingle();

            Assert.IsNotNull(sub1.Resolver.Resolve<Test4>());
            Assert.That(sub1.Resolver.ValidateResolve<Test4>().IsEmpty());
        }

        [Test]
        public void TestLocal1()
        {
            var rootContainer = new DiContainer();
            var sub1 = rootContainer.CreateSubContainer();

            rootContainer.Binder.Bind<Test0>().ToSingle();
            sub1.Binder.Bind<Test1>().ToSingle();

            Assert.Throws<ZenjectResolveException>(() => sub1.Resolver.Resolve<Test1>());
            Assert.That(sub1.Resolver.ValidateResolve<Test1>().Any());
        }

        [Test]
        public void TestLocal2()
        {
            var rootContainer = new DiContainer();
            var sub1 = rootContainer.CreateSubContainer();

            sub1.Binder.Bind<Test0>().ToSingle();
            sub1.Binder.Bind<Test1>().ToSingle();

            Assert.IsNotNull(sub1.Resolver.Resolve<Test1>());
            Assert.That(sub1.Resolver.ValidateResolve<Test1>().IsEmpty());
        }

        [Test]
        public void TestParent1()
        {
            var rootContainer = new DiContainer();
            var sub1 = rootContainer.CreateSubContainer();

            rootContainer.Binder.Bind<Test0>().ToSingle();
            sub1.Binder.Bind<Test2>().ToSingle();

            Assert.IsNotNull(sub1.Resolver.Resolve<Test2>());
            Assert.That(sub1.Resolver.ValidateResolve<Test2>().IsEmpty());
        }

        [Test]
        public void TestParent2()
        {
            var rootContainer = new DiContainer();
            var sub1 = rootContainer.CreateSubContainer();
            var sub2 = sub1.CreateSubContainer();

            rootContainer.Binder.Bind<Test0>().ToSingle();
            sub2.Binder.Bind<Test2>().ToSingle();

            Assert.Throws<ZenjectResolveException>(() => sub2.Resolver.Resolve<Test2>());
            Assert.That(sub2.Resolver.ValidateResolve<Test2>().Any());
        }

        [Test]
        public void TestParent3()
        {
            var rootContainer = new DiContainer();

            rootContainer.Binder.Bind<Test0>().ToSingle();
            rootContainer.Binder.Bind<Test2>().ToSingle();

            Assert.Throws<ZenjectResolveException>(() => rootContainer.Resolver.Resolve<Test2>());
            Assert.That(rootContainer.Resolver.ValidateResolve<Test2>().Any());
        }

        [Test]
        public void TestParentAny1()
        {
            var rootContainer = new DiContainer();
            var sub1 = rootContainer.CreateSubContainer();
            var sub2 = sub1.CreateSubContainer();

            rootContainer.Binder.Bind<Test0>().ToSingle();
            sub2.Binder.Bind<Test3>().ToSingle();

            Assert.IsNotNull(sub2.Resolver.Resolve<Test3>());
            Assert.That(sub2.Resolver.ValidateResolve<Test3>().IsEmpty());
        }
    }
}



