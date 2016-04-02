using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFactoryToGetter0 : TestWithContainer
    {
        [Test]
        public void TestSelf()
        {
            Container.Bind<Foo>().ToSelf().AsSingle();
            Container.BindFactory<Bar, Bar.Factory>().ToGetterSelf<Foo>(x => x.Bar);

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Bar.Factory>().Create());
            Assert.IsEqual(Container.Resolve<Bar.Factory>().Create(), Container.Resolve<Foo>().Bar);
        }

        class Bar
        {
            public class Factory : Factory<Bar>
            {
            }
        }

        class Foo
        {
            public Foo()
            {
                Bar = new Bar();
            }

            public Bar Bar
            {
                get;
                private set;
            }
        }
    }
}

