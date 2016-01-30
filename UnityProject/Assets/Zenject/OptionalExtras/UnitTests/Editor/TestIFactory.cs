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
    public class TestIFactory : TestWithContainer
    {
        public interface ITest
        {
        }

        class Test2 : ITest
        {
            public class Factory : Factory<Test2>
            {
            }
        }

        class Test3 : ITest
        {
        }

        class Test4
        {
        }

        class Test5 : ITest
        {
            public Test5(string param)
            {
            }

            public class Factory : Factory<string, Test5>
            {
            }
        }

        [Test]
        public void TestToInstance()
        {
            var test4 = new Test4();
            Container.BindIFactory<Test4>().ToInstance(test4);

            Assert.IsNotNull(ReferenceEquals(test4, Container.Resolve<IFactory<Test4>>().Create()));
        }

        [Test]
        public void TestToMethod()
        {
            var test3 = new Test3();

            Container.BindIFactory<ITest>().ToMethod(c => test3);

            Assert.That(ReferenceEquals(test3, Container.Resolve<IFactory<ITest>>().Create()));
        }

        [Test]
        public void TestToFactory()
        {
            Container.BindIFactory<Test4>().ToFactory();

            Assert.IsNotNull(Container.Resolve<IFactory<Test4>>().Create());
        }

        [Test]
        public void TestToDerivedFactory()
        {
            Container.BindIFactory<ITest>().ToFactory<Test2>();

            Assert.IsNotNull(Container.Resolve<IFactory<ITest>>().Create() is Test2);
        }

        [Test]
        public void TestToIFactory()
        {
            Container.BindIFactory<Test3>().ToFactory();
            Container.BindIFactory<ITest>().ToIFactory<Test3>();

            Assert.IsNotNull(Container.Resolve<IFactory<ITest>>().Create() is Test3);
        }

        [Test]
        public void TestToCustomFactory1()
        {
            Container.Bind<Test2.Factory>().ToSingle();
            Container.BindIFactory<Test2>().ToCustomFactory<Test2.Factory>();

            Assert.IsNotNull(Container.Resolve<IFactory<Test2>>().Create() is Test2);
        }

        [Test]
        public void TestToCustomFactory()
        {
            Container.Bind<Test2.Factory>().ToSingle();

            Container.BindIFactory<ITest>().ToCustomFactory<Test2, Test2.Factory>();

            Assert.IsNotNull(Container.Resolve<IFactory<ITest>>().Create() is Test2);
        }

        [Test]
        public void TestToMethodOneParam()
        {
            Container.BindIFactory<string, ITest>().ToMethod((c, param) => new Test5(param));
            Assert.IsNotNull(Container.Resolve<IFactory<string, ITest>>().Create("sdf"));
        }

        [Test]
        public void TestToFactoryOneParam()
        {
            Container.BindIFactory<string, Test5>().ToFactory();

            Assert.IsNotNull(Container.Resolve<IFactory<string, Test5>>().Create("sdf"));
        }

        [Test]
        public void TestToDerivedFactoryOneParam()
        {
            Container.BindIFactory<string, ITest>().ToFactory<Test5>();

            Assert.IsNotNull(Container.Resolve<IFactory<string, ITest>>().Create("sdf") is Test5);
        }

        [Test]
        public void TestToIFactoryOneParam()
        {
            Container.BindIFactory<string, Test5>().ToFactory();
            Container.BindIFactory<string, ITest>().ToIFactory<Test5>();

            Assert.IsNotNull(Container.Resolve<IFactory<string, ITest>>().Create("sdfds") is Test5);
        }

        [Test]
        public void TestToCustomFactoryOneParam()
        {
            Container.Bind<Test5.Factory>().ToSingle();

            Container.BindIFactory<string, ITest>().ToCustomFactory<Test5, Test5.Factory>();

            Assert.IsNotNull(Container.Resolve<IFactory<string, ITest>>().Create("sdfsd") is Test5);
        }

        [Test]
        public void TestToFacadeFactory1()
        {
            bool wasCalled = false;

            Container.BindIFactory<FooFacade>().ToFacadeFactory<FooFacade.Factory>((c) =>
                {
                    wasCalled = true;
                });

            Assert.That(!wasCalled);
            Assert.IsNotNull(Container.Resolve<IFactory<FooFacade>>().Create() is FooFacade);
            Assert.That(wasCalled);
        }

        [Test]
        public void TestToFacadeFactory2()
        {
            bool wasCalled = false;

            Container.BindIFactory<Facade>().ToFacadeFactory<FooFacade, FooFacade.Factory>((c) =>
                {
                    wasCalled = true;
                });

            Assert.That(!wasCalled);
            Assert.IsNotNull(Container.Resolve<IFactory<Facade>>().Create() is FooFacade);
            Assert.That(wasCalled);
        }

        public class FooFacade : Facade
        {
            public class Factory : FacadeFactory<FooFacade>
            {
            }
        }
    }
}
