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
    public class TestTestOptional : TestWithContainer
    {
        class Test1
        {
        }

        class Test2
        {
            [Inject]
            public Test1 val1 = null;
        }

        class Test3
        {
            [InjectOptional]
            public Test1 val1 = null;
        }

        class Test0
        {
            [InjectOptional]
            public int Val1 = 5;
        }

        [Test]
        public void TestFieldRequired()
        {
            Binder.Bind<Test2>().ToSingle();

            Assert.That(Resolver.ValidateResolve<Test2>().Any());

            AssertValidationFails();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test2>(); });
        }

        [Test]
        public void TestFieldOptional()
        {
            Binder.Bind<Test3>().ToSingle();

            AssertValidates();

            Assert.That(Resolver.ValidateResolve<Test3>().IsEmpty());
            var test = Resolver.Resolve<Test3>();
            Assert.That(test.val1 == null);
        }

        [Test]
        public void TestFieldOptional2()
        {
            Binder.Bind<Test3>().ToSingle();

            var test1 = new Test1();
            Binder.Bind<Test1>().ToInstance(test1);

            AssertValidates();

            Assert.That(Resolver.ValidateResolve<Test3>().IsEmpty());
            Assert.IsEqual(Resolver.Resolve<Test3>().val1, test1);
        }

        [Test]
        public void TestFieldOptional3()
        {
            Binder.Bind<Test0>().ToTransient();

            AssertValidates();

            // Should not redefine the hard coded value in this case
            Assert.IsEqual(Resolver.Resolve<Test0>().Val1, 5);

            Binder.Bind<int>().ToInstance(3);

            AssertValidates();

            Assert.IsEqual(Resolver.Resolve<Test0>().Val1, 3);
        }

        class Test4
        {
            public Test4(Test1 val1)
            {
            }
        }

        class Test5
        {
            public Test1 Val1;

            public Test5(
                [InjectOptional]
                Test1 val1)
            {
                Val1 = val1;
            }
        }

        [Test]
        public void TestParameterRequired()
        {
            Binder.Bind<Test4>().ToSingle();

            AssertValidationFails();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test4>(); });

            Assert.That(Resolver.ValidateResolve<Test2>().Any());
        }

        [Test]
        public void TestParameterOptional()
        {
            Binder.Bind<Test5>().ToSingle();

            AssertValidates();

            Assert.That(Resolver.ValidateResolve<Test5>().IsEmpty());
            var test = Resolver.Resolve<Test5>();
            Assert.That(test.Val1 == null);
        }

        class Test6
        {
            public Test6(Test2 test2)
            {
            }
        }

        [Test]
        public void TestChildDependencyOptional()
        {
            Binder.Bind<Test6>().ToSingle();
            Binder.Bind<Test2>().ToSingle();

            Assert.That(Resolver.ValidateResolve<Test6>().Any());

            AssertValidationFails();

            Assert.Throws<ZenjectResolveException>(
                delegate { Resolver.Resolve<Test6>(); });
        }

        class Test7
        {
            public int Val1;

            public Test7(
                [InjectOptional]
                int val1)
            {
                Val1 = val1;
            }
        }

        [Test]
        public void TestPrimitiveParamOptionalUsesDefault()
        {
            Binder.Bind<Test7>().ToSingle();

            Assert.That(Resolver.ValidateResolve<Test7>().IsEmpty());

            AssertValidates();

            Assert.IsEqual(Resolver.Resolve<Test7>().Val1, 0);
        }

        class Test8
        {
            public int Val1;

            public Test8(
                [InjectOptional]
                int val1 = 5)
            {
                Val1 = val1;
            }
        }

        [Test]
        public void TestPrimitiveParamOptionalUsesExplicitDefault()
        {
            Binder.Bind<Test8>().ToSingle();

            AssertValidates();

            Assert.That(Resolver.ValidateResolve<Test8>().IsEmpty());
            Assert.IsEqual(Resolver.Resolve<Test8>().Val1, 5);
        }

        class Test8_2
        {
            public int Val1;

            public Test8_2(int val1 = 5)
            {
                Val1 = val1;
            }
        }

        [Test]
        public void TestPrimitiveParamOptionalUsesExplicitDefault2()
        {
            Binder.Bind<Test8_2>().ToSingle();

            AssertValidates();

            Assert.That(Resolver.ValidateResolve<Test8_2>().IsEmpty());
            Assert.IsEqual(Resolver.Resolve<Test8_2>().Val1, 5);
        }

        class Test9
        {
            public int? Val1;

            public Test9(
                [InjectOptional]
                int? val1)
            {
                Val1 = val1;
            }
        }

        [Test]
        public void TestPrimitiveParamOptionalNullable()
        {
            Binder.Bind<Test9>().ToSingle();

            AssertValidates();

            Assert.That(Resolver.ValidateResolve<Test9>().IsEmpty());

            Assert.That(!Resolver.Resolve<Test9>().Val1.HasValue);
        }
    }
}



