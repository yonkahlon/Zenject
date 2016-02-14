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
    public class TestSubContainers : TestWithContainer
    {
        class Test0
        {
        }

        [Test]
        public void TestIsRemoved()
        {
            var subContainer = Container.CreateSubContainer();
            var test1 = new Test0();

            subContainer.Binder.Bind<Test0>().ToInstance(test1);

            Assert.That(subContainer.Resolver.ValidateResolve<Test0>().IsEmpty());
            Assert.That(ReferenceEquals(test1, subContainer.Resolver.Resolve<Test0>()));

            Assert.That(Resolver.ValidateResolve<Test0>().Any());

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test0>(); });
        }

        class Test1
        {
            [Inject]
            public Test0 Test = null;
        }

        [Test]
        public void TestCase2()
        {
            Test0 test0;
            Test1 test1;

            var subContainer = Container.CreateSubContainer();
            var test0Local = new Test0();

            subContainer.Binder.Bind<Test0>().ToInstance(test0Local);
            subContainer.Binder.Bind<Test1>().ToSingle();

            Assert.That(subContainer.Resolver.ValidateResolve<Test0>().IsEmpty());
            test0 = subContainer.Resolver.Resolve<Test0>();
            Assert.IsEqual(test0Local, test0);

            Assert.That(subContainer.Resolver.ValidateResolve<Test1>().IsEmpty());
            test1 = subContainer.Resolver.Resolve<Test1>();

            Assert.That(Resolver.ValidateResolve<Test0>().Any());

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test0>(); });

            Assert.That(Resolver.ValidateResolve<Test1>().Any());

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test1>(); });

            Binder.Bind<Test0>().ToSingle();
            Binder.Bind<Test1>().ToSingle();

            Assert.That(Resolver.ValidateResolve<Test0>().IsEmpty());
            Assert.That(Resolver.Resolve<Test0>() != test0);

            Assert.That(Resolver.ValidateResolve<Test1>().IsEmpty());
            Assert.That(Resolver.Resolve<Test1>() != test1);
        }

        interface IFoo
        {
        }

        interface IFoo2
        {
        }

        class Foo : IFoo, IFoo2
        {
        }

        [Test]
        public void TestMultipleSingletonDifferentScope()
        {
            IFoo foo1;

            var subContainer1 = Container.CreateSubContainer();
            subContainer1.Binder.Bind<IFoo>().ToSingle<Foo>();
            foo1 = subContainer1.Resolver.Resolve<IFoo>();

            Assert.That(!Resolver.ValidateResolve<IFoo>().IsEmpty());

            var subContainer2 = Container.CreateSubContainer();
            subContainer2.Binder.Bind<IFoo>().ToSingle<Foo>();
            var foo2 = subContainer2.Resolver.Resolve<IFoo>();

            Assert.That(foo2 != foo1);
        }
    }
}

