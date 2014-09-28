using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;
using System.Linq;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestCircularDependencies : TestWithContainer
    {
        class Test1
        {
            [Inject]
            public Test2 test = null;
        }

        class Test2
        {
            [Inject]
            public Test2 test = null;
        }

        [Test]
        [ExpectedException]
        public void Test()
        {
            Container.Bind<Test1>().ToSingle();
            Container.Bind<Test2>().ToSingle();

            TestAssert.Throws<ZenjectResolveException>(
                delegate { Container.Resolve<Test2>(); });

            TestAssert.That(Container.ValidateResolve<Test2>().Any());
        }
    }
}


