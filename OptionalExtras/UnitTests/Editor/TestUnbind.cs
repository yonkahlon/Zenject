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
            Container.Bind<IFoo>().ToSingle<Foo>();

            Assert.That(Container.ValidateResolve<IFoo>().IsEmpty());

            Container.Unbind<IFoo>();

            Assert.That(!Container.ValidateResolve<IFoo>().IsEmpty());
        }
    }
}
