using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;
using System.Linq;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestRebind : TestWithContainer
    {
        class Test1
        {
        }

        class Test2 : Test1
        {
        }

        class Test3 : Test1
        {
        }

        [Test]
        public void Run()
        {
            _container.Bind<Test1>().ToSingle<Test2>();

            TestAssert.That(_container.ValidateResolve<Test1>().IsEmpty());
            TestAssert.That(_container.Resolve<Test1>() is Test2);

            _container.Rebind<Test1>().ToSingle<Test3>();

            TestAssert.That(_container.ValidateResolve<Test1>().IsEmpty());
            TestAssert.That(_container.Resolve<Test1>() is Test3);
        }
    }
}

