
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
        public void TestToSingleMethod1()
        {
            Binder.Bind<Foo>().ToSingle();

            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToSingleMethod((container) => new Foo()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToSingleInstance(new Foo()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToSingleFactory<FooFactory>());
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToInstance(new Foo()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToTransient());
        }

        [Test]
        public void TestToSingleMethod()
        {
            Binder.Bind<Foo>().ToSingleMethod((container) => new Foo());

            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToSingle());
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToSingleInstance(new Foo()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToSingleFactory<FooFactory>());
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToInstance(new Foo()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToTransient());
        }

        [Test]
        public void TestToSingleInstance()
        {
            Binder.Bind<Foo>().ToSingleInstance(new Foo());

            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToSingle());
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToSingleMethod((container) => new Foo()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToSingleFactory<FooFactory>());
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToInstance(new Foo()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToTransient());
        }

        [Test]
        public void TestToSingleFactory()
        {
            Binder.Bind<Foo>().ToSingleFactory<FooFactory>();

            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToSingle());
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToSingleMethod((container) => new Foo()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToSingleInstance(new Foo()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToInstance(new Foo()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<Foo>().ToTransient());
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


