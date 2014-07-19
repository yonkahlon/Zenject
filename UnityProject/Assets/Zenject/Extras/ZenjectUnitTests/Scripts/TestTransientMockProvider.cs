using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;
using System.Linq;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestTransientMockProvider : TestWithContainer
    {
        public interface IFoo
        {
            int GetBar();
        }

        [Test]
        public void TestCase1()
        {
            // Commented out because this requires that zenject be installed with mocking support which isn't always the case

            //_container.FallbackProvider = new TransientMockProvider(_container);

            //var foo = _container.Resolve<IFoo>();

            //TestAssert.AreEqual(foo.GetBar(), 0);
        }
    }
}
