using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.BindFeatures
{
    [TestFixture]
    public class TestWithArguments : TestWithContainer
    {
        class Foo
        {
            public Foo(int value)
            {
                Value = value;
            }

            public int Value
            {
                get;
                private set;
            }
        }

        [Test]
        public void Test1()
        {
            Container.Bind<Foo>().ToSelf().AsTransient().WithArguments(3);

            AssertValidates();

            Assert.IsEqual(Container.Resolve<Foo>().Value, 3);
        }
    }
}

