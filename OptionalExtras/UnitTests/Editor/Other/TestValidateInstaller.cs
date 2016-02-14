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
            Binder.Bind<IFoo>().ToSingle<Foo>();
            Binder.Bind<Bar>().ToSingle();

            Assert.That(Resolver.ValidateResolve<IFoo>().IsEmpty());
        }

        [Test]
        public void TestBasicFailure()
        {
            Binder.Bind<IFoo>().ToSingle<Foo>();
            //Container.Bind<Bar>().ToSingle();

            Assert.That(!Resolver.ValidateResolve<IFoo>().IsEmpty());
        }

        [Test]
        public void TestList()
        {
            Binder.Bind<IFoo>().ToSingle<Foo>();
            Binder.Bind<IFoo>().ToSingle<Foo2>();

            Binder.Bind<Bar>().ToSingle();

            Binder.Bind<Qux>().ToSingle();

            Assert.That(Resolver.ValidateResolve<Qux>().IsEmpty());
        }

        [Test]
        public void TestValidateDynamicSuccess()
        {
            Binder.Bind<Foo>().ToSingle();

            Assert.That(Resolver.ValidateObjectGraph<Foo>(typeof(Bar)).IsEmpty());
        }

        [Test]
        public void TestValidateDynamicFailure()
        {
            Binder.Bind<Foo>().ToSingle();

            Assert.That(!Resolver.ValidateObjectGraph<Foo>().IsEmpty());
        }

        [Test]
        public void TestValidateDynamicFailure2()
        {
            Binder.Bind<Foo>().ToSingle();

            Assert.That(!Resolver.ValidateObjectGraph<Foo>(typeof(Bar), typeof(string)).IsEmpty());
        }

        [Test]
        public void TestValidateNestedContainerSuccess()
        {
            var nestedContainer = new DiContainer(Container);

            // Should fail without Bar<> bound
            Assert.That(!nestedContainer.Resolver.ValidateObjectGraph<Foo>().IsEmpty());

            Binder.Bind<Bar>().ToSingle();

            Assert.That(nestedContainer.Resolver.ValidateObjectGraph<Foo>().IsEmpty());
        }

        [Test]
        public void TestValidateNestedContainerList()
        {
            var nestedContainer = new DiContainer(Container);

            Binder.Bind<IFoo>().ToSingle<Foo>();
            Binder.Bind<IFoo>().ToSingle<Foo2>();

            Assert.That(!Resolver.ValidateResolve<List<IFoo>>().IsEmpty());
            Assert.That(!nestedContainer.Resolver.ValidateResolve<List<IFoo>>().IsEmpty());

            Binder.Bind<Bar>().ToSingle();

            Assert.That(Resolver.ValidateResolve<List<IFoo>>().IsEmpty());

            // Should not throw
            nestedContainer.Resolver.Resolve<List<IFoo>>();

            Assert.That(nestedContainer.Resolver.ValidateResolve<List<IFoo>>().IsEmpty());
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
