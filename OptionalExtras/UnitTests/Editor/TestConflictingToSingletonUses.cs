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
    public class TestConflictingToSingletonUses : TestWithContainer
    {
        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestToSingleMethod1()
        {
            var foo = new Foo();

            Container.Bind(typeof(Foo)).ToSingleMethod((container) => foo);
            Container.Bind(typeof(IFoo)).ToSingle<Foo>();
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestToSingleMethod3()
        {
            Container.Bind<Foo>().ToSingle();
            Container.Bind(typeof(IFoo)).ToSingleMethod((container) => new Foo());
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestToSingleMethod4()
        {
            // Cannot bind different singleton providers
            Container.Bind<Foo>().ToSingleMethod((container) => new Foo());
            Container.Bind<Foo>().ToSingle();
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestToSingleMethod5()
        {
            Container.Bind<Foo>().ToSingleMethod((container) => new Foo());
            Container.Bind<Foo>().ToSingleMethod((container) => new Foo());
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestToSingleFactory2()
        {
            // Cannot bind different singleton providers
            Container.Bind<Foo>().ToSingleFactory<FooFactory>();
            Container.Bind<Foo>().ToSingle();
        }

        interface IFoo
        {
            int ReturnValue();
        }

        class Foo : IFoo, ITickable, IInitializable
        {
            public int ReturnValue()
            {
                return 5;
            }

            public void Initialize()
            {
            }

            public void Tick()
            {
            }
        }

        class FooFactory : IFactory<Foo>
        {
            public static bool WasCalled;

            public Foo Create()
            {
                WasCalled = true;
                return new Foo();
            }
        }
    }
}

