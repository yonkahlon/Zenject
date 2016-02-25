using System;
using NUnit.Framework;
using Assert=ModestTree.Assert;
using ModestTree;
using System.Linq;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestConditionsComplex : TestWithContainer
    {
        class Foo
        {
        }

        class Bar
        {
            public Foo Foo;

            public Bar(Foo foo)
            {
                Foo = foo;
            }
        }

        [Test]
        public void TestCorrespondingIdentifiers()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();

            Container.Bind<Bar>("Bar1").ToTransient();
            Container.Bind<Bar>("Bar2").ToTransient();

            Container.BindInstance(foo1).When(c => c.ParentContexts.Where(x => x.MemberType == typeof(Bar) && x.Identifier == "Bar1").Any());
            Container.BindInstance(foo2).When(c => c.ParentContexts.Where(x => x.MemberType == typeof(Bar) && x.Identifier == "Bar2").Any());

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Bar>("Bar1").Foo, foo1);
            Assert.IsEqual(Container.Resolve<Bar>("Bar2").Foo, foo2);
        }
    }
}
