using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
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
            Binder.Bind<Test1>().ToSingle<Test2>();

            Assert.That(Resolver.ValidateResolve<Test1>().IsEmpty());
            Assert.That(Resolver.Resolve<Test1>() is Test2);

            Binder.Rebind<Test1>().ToSingle<Test3>();

            Assert.That(Resolver.ValidateResolve<Test1>().IsEmpty());
            Assert.That(Resolver.Resolve<Test1>() is Test3);
        }
    }
}

