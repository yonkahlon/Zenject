
using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings.Singletons
{
    [TestFixture]
    public class TestConflictingToSingletonUses : TestWithContainer
    {
        [Test]
        public void TestToSingleMethod1()
        {
            Container.Bind<Foo>().ToSelf().AsSingle();

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().ToMethod((container) => new Foo()).AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().ToInstance(new Foo()).AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().ToFactorySelf<FooFactory>().AsSingle();
                    Container.FlushBindings();
                });
        }

        [Test]
        public void TestToSingleMethod()
        {
            Container.Bind<Foo>().ToMethod((container) => new Foo()).AsSingle();

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().ToSelf().AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().ToInstance(new Foo()).AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().ToFactorySelf<FooFactory>().AsSingle();
                    Container.FlushBindings();
                });
        }

        [Test]
        public void TestToSingleInstance()
        {
            Container.Bind<Foo>().ToInstance(new Foo()).AsSingle();

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().ToSelf().AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().ToMethod((container) => new Foo()).AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().ToFactorySelf<FooFactory>().AsSingle();
                    Container.FlushBindings();
                });
        }

        [Test]
        public void TestToSingleFactory()
        {
            Container.Bind<Foo>().ToFactorySelf<FooFactory>().AsSingle();

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().ToSelf().AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().ToMethod((container) => new Foo()).AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().ToInstance(new Foo()).AsSingle();
                    Container.FlushBindings();
                });
        }

        class Bar
        {
            public Foo GetFoo()
            {
                return new Foo();
            }
        }

        class Foo
        {
        }

        class FooFactory : IFactory<Foo>
        {
            public Foo Create()
            {
                return new Foo();
            }
        }
    }
}


