using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using Zenject;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestValidateInstaller : TestWithContainer
    {
        [Test]
        public void TestBasicSuccess()
        {
            Container.Bind<IFoo>().ToSingle<Foo>();
            Container.Bind<Bar>().ToSingle();

            Assert.That(Container.ValidateResolve<IFoo>().IsEmpty());
        }

        [Test]
        public void TestBasicFailure()
        {
            Container.Bind<IFoo>().ToSingle<Foo>();
            //Container.Bind<Bar>().ToSingle();

            Assert.That(!Container.ValidateResolve<IFoo>().IsEmpty());
        }

        [Test]
        public void TestList()
        {
            Container.Bind<IFoo>().ToSingle<Foo>();
            Container.Bind<IFoo>().ToSingle<Foo2>();

            Container.Bind<Bar>().ToSingle();

            Container.Bind<Qux>().ToSingle();

            Assert.That(Container.ValidateResolve<Qux>().IsEmpty());
        }

        [Test]
        public void TestValidateDynamicSuccess()
        {
            Container.Bind<Foo>().ToSingle();

            Assert.That(Container.ValidateObjectGraph<Foo>(typeof(Bar)).IsEmpty());
        }

        [Test]
        public void TestValidateDynamicFailure()
        {
            Container.Bind<Foo>().ToSingle();

            Assert.That(!Container.ValidateObjectGraph<Foo>().IsEmpty());
        }

        [Test]
        public void TestValidateDynamicFailure2()
        {
            Container.Bind<Foo>().ToSingle();

            Assert.That(!Container.ValidateObjectGraph<Foo>(typeof(Bar), typeof(string)).IsEmpty());
        }

        [Test]
        public void TestValidateNestedContainerSuccess()
        {
            var nestedContainer = new DiContainer(Container);

            // Should fail without Bar<> bound
            Assert.That(!nestedContainer.ValidateObjectGraph<Foo>().IsEmpty());

            Container.Bind<Bar>().ToSingle();

            Assert.That(nestedContainer.ValidateObjectGraph<Foo>().IsEmpty());
        }

        [Test]
        public void TestValidateNestedContainerList()
        {
            var nestedContainer = new DiContainer(Container);

            Container.Bind<IFoo>().ToSingle<Foo>();
            Container.Bind<IFoo>().ToSingle<Foo2>();

            Assert.That(!Container.ValidateResolve<List<IFoo>>().IsEmpty());
            Assert.That(!nestedContainer.ValidateResolve<List<IFoo>>().IsEmpty());

            Container.Bind<Bar>().ToSingle();

            AssertValidates();

            Assert.That(Container.ValidateResolve<List<IFoo>>().IsEmpty());

            // Should not throw
            nestedContainer.Resolve<List<IFoo>>();

            Assert.That(nestedContainer.ValidateResolve<List<IFoo>>().IsEmpty());
        }

        interface IFoo
        {
        }

        class Foo : IFoo
        {
            public Foo(Bar bar)
            {
            }
        }

        class Foo2 : IFoo
        {
            public Foo2(Bar bar)
            {
            }
        }

        class Bar
        {
        }

        class Qux
        {
            public Qux(List<IFoo> foos)
            {
            }
        }
    }
}
