using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestToGetter : TestWithContainer
    {
        class Bar
        {
        }

        class Foo
        {
            Bar _bar = new Bar();

            public Foo()
            {
            }

            public Bar GetBar()
            {
                return _bar;
            }
        }

        [Test]
        public void Test1()
        {
            Container.Bind<Foo>().ToSingle();
            Container.Bind<Bar>().ToGetter<Foo>(x => x.GetBar());

            AssertValidates();

            var foo = Container.Resolve<Foo>();
            var bar = Container.Resolve<Bar>();

            Assert.IsEqual(bar, foo.GetBar());
        }
    }
}

