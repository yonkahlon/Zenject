using System;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestMultipleInstallers : TestWithContainer
    {
        class Test0 : Installer
        {
            public static int Count;
            public override void InstallBindings()
            {
                Count++;
            }
        }

        class Test1 : Installer
        {
            public static int Count;
            public override void InstallBindings()
            {
                Container.Install<Test0>();
                Count++;
            }
        }

        class Test2 : Installer
        {
            public static int Count;
            public override void InstallBindings()
            {
                Container.Install<Test0>();
                Count++;
            }
        }

        public override void Setup()
        {
            base.Setup();

            // Reset counters since static state is not being reset by 'Unity Test Tools'.
            Test0.Count = 0;
            Test1.Count = 0;
            Test2.Count = 0;
        }

        [Test]
        public void Test()
        {
            Container.Install<Test1>();
            Container.Install<Test2>();

            Assert.That(Test1.Count == 1);
            Assert.That(Test2.Count == 1);
            Assert.That(Test0.Count == 1);
        }
    }
}
