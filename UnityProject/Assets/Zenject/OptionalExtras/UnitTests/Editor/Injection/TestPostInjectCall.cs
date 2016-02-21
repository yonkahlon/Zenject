using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestPostInjectCall : TestWithContainer
    {
        class Test0
        {
        }

        class Test1
        {
        }

        class Test2
        {
        }

        class Test3
        {
            public bool HasInitialized;
            public bool HasInitialized2;

            [Inject]
            public Test1 test1 = null;

            [Inject]
            public Test0 test0 = null;

            Test2 _test2;

            public Test3(Test2 test2)
            {
                _test2 = test2;
            }

            [PostInject]
            public void Init()
            {
                Assert.That(!HasInitialized);
                Assert.IsNotNull(test1);
                Assert.IsNotNull(test0);
                Assert.IsNotNull(_test2);
                HasInitialized = true;
            }

            [PostInject]
            void TestPrivatePostInject()
            {
                HasInitialized2 = true;
            }
        }

        [Test]
        public void Test()
        {
            Binder.Bind<Test0>().ToSingle();
            Binder.Bind<Test1>().ToSingle();
            Binder.Bind<Test2>().ToSingle();
            Binder.Bind<Test3>().ToSingle();

            AssertValidates();

            Assert.That(Resolver.ValidateResolve<Test3>().IsEmpty());
            var test3 = Resolver.Resolve<Test3>();
            Assert.That(test3.HasInitialized);
            Assert.That(test3.HasInitialized2);
        }

        public class SimpleBase
        {
            public bool WasCalled = false;

            [PostInject]
            void Init()
            {
                WasCalled = true;
            }
        }

        public class SimpleDerived : SimpleBase
        {
        }

        [Test]
        public void TestPrivateBaseClassPostInject()
        {
            Binder.Bind<SimpleBase>().ToSingle<SimpleDerived>();

            AssertValidates();

            var simple = Resolver.Resolve<SimpleBase>();

            Assert.That(simple.WasCalled);
        }

        [Test]
        public void TestInheritance()
        {
            Binder.Bind<IFoo>().ToSingle<FooDerived>();

            AssertValidates();

            Assert.That(Resolver.ValidateResolve<IFoo>().IsEmpty());
            var foo = Resolver.Resolve<IFoo>();

            Assert.That(((FooDerived)foo).WasDerivedCalled);
            Assert.That(((FooBase)foo).WasBaseCalled);
            Assert.That(((FooDerived)foo).WasDerivedCalled2);
            Assert.That(((FooBase)foo).WasBaseCalled2);
        }

        [Test]
        public void TestInheritanceOrder()
        {
            Binder.Bind<IFoo>().ToSingle<FooDerived2>();

            // base post inject methods should be called first
            _initOrder = 0;
            FooBase.BaseCallOrder = 0;
            FooDerived.DerivedCallOrder = 0;
            FooDerived2.Derived2CallOrder = 0;

            AssertValidates();

            Resolver.Resolve<IFoo>();

            //Log.Info("FooBase.BaseCallOrder = {0}".Fmt(FooBase.BaseCallOrder));
            //Log.Info("FooDerived.DerivedCallOrder = {0}".Fmt(FooDerived.DerivedCallOrder));

            Assert.IsEqual(FooBase.BaseCallOrder, 0);
            Assert.IsEqual(FooDerived.DerivedCallOrder, 1);
            Assert.IsEqual(FooDerived2.Derived2CallOrder, 2);
        }

        static int _initOrder;

        interface IFoo
        {
        }

        class FooBase : IFoo
        {
            public bool WasBaseCalled;
            public bool WasBaseCalled2;
            public static int BaseCallOrder;

            [PostInject]
            void TestBase()
            {
                Assert.That(!WasBaseCalled);
                WasBaseCalled = true;
                BaseCallOrder = _initOrder++;
            }

            [PostInject]
            public virtual void TestVirtual1()
            {
                Assert.That(!WasBaseCalled2);
                WasBaseCalled2 = true;
            }
        }

        class FooDerived : FooBase
        {
            public bool WasDerivedCalled;
            public bool WasDerivedCalled2;
            public static int DerivedCallOrder;

            [PostInject]
            void TestDerived()
            {
                Assert.That(!WasDerivedCalled);
                WasDerivedCalled = true;
                DerivedCallOrder = _initOrder++;
            }

            public override void TestVirtual1()
            {
                base.TestVirtual1();
                Assert.That(!WasDerivedCalled2);
                WasDerivedCalled2 = true;
            }
        }

        class FooDerived2 : FooDerived
        {
            public bool WasDerived2Called;
            public static int Derived2CallOrder;

            [PostInject]
            public void TestVirtual2()
            {
                Assert.That(!WasDerived2Called);
                WasDerived2Called = true;
                Derived2CallOrder = _initOrder++;
            }
        }
    }
}

