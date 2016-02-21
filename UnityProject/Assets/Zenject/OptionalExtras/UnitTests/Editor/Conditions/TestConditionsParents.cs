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
    public class TestConditionsParents : TestWithContainer
    {
        class Test0
        {
        }

        interface ITest1
        {
        }

        class Test1 : ITest1
        {
            public Test0 test0;

            public Test1(Test0 test0)
            {
                this.test0 = test0;
            }
        }

        class Test2 : ITest1
        {
            public Test0 test0;

            public Test2(Test0 test0)
            {
                this.test0 = test0;
            }
        }

        class Test3 : ITest1
        {
            public Test1 test1;

            public Test3(Test1 test1)
            {
                this.test1 = test1;
            }
        }

        class Test4 : ITest1
        {
            public Test1 test1;

            public Test4(Test1 test1)
            {
                this.test1 = test1;
            }
        }

        [Test]
        public void TestCase1()
        {
            Binder.Bind<Test1>().ToSingle();
            Binder.Bind<Test0>().ToSingle().When(c => c.AllObjectTypes.Contains(typeof(Test2)));

            AssertValidationFails();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test1>(); });

            Assert.That(Resolver.ValidateResolve<Test0>().Any());
        }

        [Test]
        public void TestCase2()
        {
            Binder.Bind<Test1>().ToSingle();
            Binder.Bind<Test0>().ToSingle().When(c => c.AllObjectTypes.Contains(typeof(Test1)));

            AssertValidates();

            var test1 = Resolver.Resolve<Test1>();
            Assert.That(Resolver.ValidateResolve<Test1>().IsEmpty());
            Assert.That(test1 != null);
        }

        // Test using parents to look deeper up the heirarchy..
        [Test]
        public void TestCase3()
        {
            var t0a = new Test0();
            var t0b = new Test0();

            Binder.Bind<Test3>().ToSingle();
            Binder.Bind<Test4>().ToSingle();
            Binder.Bind<Test1>().ToTransient();

            Binder.Bind<Test0>().ToInstance(t0a).When(c => c.AllObjectTypes.Contains(typeof(Test3)));
            Binder.Bind<Test0>().ToInstance(t0b).When(c => c.AllObjectTypes.Contains(typeof(Test4)));

            Assert.That(Resolver.ValidateResolve<Test3>().IsEmpty());
            var test3 = Resolver.Resolve<Test3>();

            Assert.That(Resolver.ValidateResolve<Test4>().IsEmpty());
            var test4 = Resolver.Resolve<Test4>();

            Assert.That(ReferenceEquals(test3.test1.test0, t0a));
            Assert.That(ReferenceEquals(test4.test1.test0, t0b));
        }

        [Test]
        public void TestCase4()
        {
            Binder.Bind<ITest1>().ToSingle<Test2>();
            Binder.Bind<Test0>().ToSingle().When(c => c.AllObjectTypes.Contains(typeof(ITest1)));

            AssertValidationFails();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<ITest1>(); });

            Assert.That(Resolver.ValidateResolve<Test1>().Any());
        }

        [Test]
        public void TestCase5()
        {
            Binder.Bind<ITest1>().ToSingle<Test2>();
            Binder.Bind<Test0>().ToSingle().When(c => c.AllObjectTypes.Contains(typeof(Test2)));

            AssertValidates();

            Assert.That(Resolver.ValidateResolve<ITest1>().IsEmpty());
            var test1 = Resolver.Resolve<ITest1>();
            Assert.That(test1 != null);
        }

        [Test]
        public void TestCase6()
        {
            Binder.Bind<ITest1>().ToSingle<Test2>();
            Binder.Bind<Test0>().ToSingle().When(c => c.AllObjectTypes.Where(x => typeof(ITest1).IsAssignableFrom(x)).Any());

            AssertValidates();

            Assert.That(Resolver.ValidateResolve<ITest1>().IsEmpty());
            var test1 = Resolver.Resolve<ITest1>();
            Assert.That(test1 != null);
        }
    }
}

