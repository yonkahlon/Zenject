using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFactoryToFactory0 : TestWithContainer
    {
        static Foo StaticFoo = new Foo();

        [Test]
        public void TestSelf()
        {
            Container.BindFactory<Foo, Foo.Factory>().ToFactorySelf<CustomFooFactory>();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo.Factory>().Create(), StaticFoo);
        }

        [Test]
        public void TestConcrete()
        {
            Container.BindFactory<IFoo, IFooFactory>().ToFactory<Foo, CustomFooFactory>();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFooFactory>().Create(), StaticFoo);
        }

        [Test]
        public void TestFactoryValidation()
        {
            Container.BindFactory<IFoo, IFooFactory>().ToFactory<Foo, CustomFooFactoryWithValidate>();

            AssertValidationFails();
        }

        class CustomFooFactoryWithValidate : IFactory<Foo>, IValidatable
        {
            public Foo Create()
            {
                return StaticFoo;
            }

            public IEnumerable<ZenjectException> Validate()
            {
                yield return new ZenjectException("Test error");
            }
        }

        class CustomFooFactory : IFactory<Foo>
        {
            public Foo Create()
            {
                return StaticFoo;
            }
        }

        interface IFoo
        {
        }

        class IFooFactory : Factory<IFoo>
        {
        }

        class Foo : IFoo
        {
            public class Factory : Factory<Foo>
            {
            }
        }
    }
}


