using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestMultipleBindingsAtOnce : TestWithContainer
    {
        public class Bar
        {
        }

        public interface IFoo
        {
        }

        public interface IQux
        {
        }

        public class Foo : IQux, IFoo
        {
        }

        [Test]
        public void Test1()
        {
            var types = new List<Type>()
            {
                typeof(Bar),
                typeof(Foo),
            };

            Container.Bind(types).ToSingle();

            AssertValidates();

            Container.Resolve<Bar>();
            Container.Resolve<Foo>();
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestInterfaces()
        {
            Container.Bind<IFoo>().ToSingle();
        }

        [Test]
        public void TestAllInterfaces()
        {
            Container.BindAllInterfaces<Foo>().ToSingle<Foo>();

            AssertValidates();

            Assert.IsNull(Container.TryResolve<Foo>());
            Assert.IsNotNull(Container.Resolve<IFoo>());
            Assert.IsNotNull(Container.Resolve<IQux>());
        }

        [Test]
        public void TestAllInterfacesAndSelf()
        {
            Container.BindAllInterfacesAndSelf<Foo>().ToSingle<Foo>();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsNotNull(Container.Resolve<IFoo>());
            Assert.IsNotNull(Container.Resolve<IQux>());
        }
    }
}



