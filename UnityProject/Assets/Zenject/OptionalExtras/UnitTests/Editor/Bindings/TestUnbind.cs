using System;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestUnbind : TestWithContainer
    {
        interface IFoo
        {
        }

        class Foo : IFoo
        {
        }

        [Test]
        public void TestCase1()
        {
            Binder.Bind<IFoo>().ToSingle<Foo>();

            Assert.That(Resolver.ValidateResolve<IFoo>().IsEmpty());

            Binder.Unbind<IFoo>();

            Assert.That(!Resolver.ValidateResolve<IFoo>().IsEmpty());
        }
    }
}
