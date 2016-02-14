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
    public class TestConditionsBasic : TestWithContainer
    {
        public interface IFoo
        {
        }

        class Foo1 : IFoo
        {
        }

        class Foo2 : IFoo
        {
        }

        class Bar1
        {
            public IFoo Foo;

            public Bar1(IFoo foo)
            {
                Foo = foo;
            }
        }

        class Bar2
        {
            public IFoo Foo;

            public Bar2(IFoo foo)
            {
                Foo = foo;
            }
        }

        [Test]
        public void Test1()
        {
            Binder.Bind<Bar1>().ToSingle();
            Binder.Bind<Bar2>().ToSingle();
            Binder.Bind<IFoo>().ToSingle<Foo1>();
            Binder.Bind<IFoo>().ToSingle<Foo2>().WhenInjectedInto<Bar2>();

            Assert.IsNotEqual(
                Resolver.Resolve<Bar1>().Foo, Container.Resolver.Resolve<Bar2>().Foo);
        }
    }
}



