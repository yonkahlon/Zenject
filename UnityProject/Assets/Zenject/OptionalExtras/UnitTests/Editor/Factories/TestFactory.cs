using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.AbstractFactory
{
    [TestFixture]
    public class TestFactory : TestWithContainer
    {
        [Test]
        public void TestToSelf()
        {
            Container.BindFactory<Foo, Foo.Factory>().ToSelf();

            AssertValidates();

            Assert.IsNotNull(Container.Resolve<Foo.Factory>().Create());
        }

        public interface IFoo
        {
        }

        public class Foo : IFoo
        {
            public class Factory : Factory<Foo>
            {
            }
        }
    }
}


