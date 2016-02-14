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
            Binder.BindIFactory<Test4>().ToInstance(test4);

            Assert.IsNotNull(ReferenceEquals(test4, Resolver.Resolve<IFactory<Test4>>().Create()));
        }

        [Test]
        public void TestToMethod()
        {
            var test3 = new Test3();

            Binder.BindIFactory<ITest>().ToMethod(c => test3);

            Assert.That(ReferenceEquals(test3, Resolver.Resolve<IFactory<ITest>>().Create()));
        }

        [Test]
        public void TestToFactory()
        {
            Binder.BindIFactory<Test4>().ToFactory();

            Assert.IsNotNull(Resolver.Resolve<IFactory<Test4>>().Create());
        }

        [Test]
        public void TestToDerivedFactory()
        {
            Binder.BindIFactory<ITest>().ToFactory<Test2>();

            Assert.IsNotNull(Resolver.Resolve<IFactory<ITest>>().Create() is Test2);
        }

        [Test]
        public void TestToDerivedFactoryUntyped()
        {
            Binder.BindIFactory<ITest>().ToFactory(typeof(Test2));

            Assert.IsNotNull(Resolver.Resolve<IFactory<ITest>>().Create() is Test2);
        }

        [Test]
        public void TestToIFactory()
        {
            Binder.BindIFactory<Test3>().ToFactory();
            Binder.BindIFactory<ITest>().ToIFactory<Test3>();

            Assert.IsNotNull(Resolver.Resolve<IFactory<ITest>>().Create() is Test3);
        }

        [Test]
        public void TestToCustomFactory1()
        {
            Binder.BindIFactory<Test2>().ToCustomFactory<Test2.Factory>();

            Assert.IsNotNull(Resolver.Resolve<IFactory<Test2>>().Create() is Test2);
        }

        [Test]
        public void TestToCustomFactory1Untyped()
        {
            Binder.BindIFactory<Test2>().ToCustomFactory(typeof(Test2.Factory));

            Assert.IsNotNull(Resolver.Resolve<IFactory<Test2>>().Create() is Test2);
        }

        [Test]
        public void TestToCustomFactory()
        {
            Binder.Bind<Test2.Factory>().ToSingle();

            Binder.BindIFactory<ITest>().ToCustomFactory<Test2, Test2.Factory>();

            Assert.IsNotNull(Resolver.Resolve<IFactory<ITest>>().Create() is Test2);
        }

        [Test]
        public void TestToMethodOneParam()
        {
            Binder.BindIFactory<string, ITest>().ToMethod((c, param) => new Test5(param));
            Assert.IsNotNull(Resolver.Resolve<IFactory<string, ITest>>().Create("sdf"));
        }

        [Test]
        public void TestToFactoryOneParam()
        {
            Binder.BindIFactory<string, Test5>().ToFactory();

            Assert.IsNotNull(Resolver.Resolve<IFactory<string, Test5>>().Create("sdf"));
        }

        [Test]
        public void TestToDerivedFactoryOneParam()
        {
            Binder.BindIFactory<string, ITest>().ToFactory<Test5>();

            Assert.IsNotNull(Resolver.Resolve<IFactory<string, ITest>>().Create("sdf") is Test5);
        }

        [Test]
        public void TestToIFactoryOneParam()
        {
            Binder.BindIFactory<string, Test5>().ToFactory();
            Binder.BindIFactory<string, ITest>().ToIFactory<Test5>();

            Assert.IsNotNull(Resolver.Resolve<IFactory<string, ITest>>().Create("sdfds") is Test5);
        }

        [Test]
        public void TestToCustomFactoryOneParam()
        {
            Binder.Bind<Test5.Factory>().ToSingle();

            Binder.BindIFactory<string, ITest>().ToCustomFactory<Test5, Test5.Factory>();

            Assert.IsNotNull(Resolver.Resolve<IFactory<string, ITest>>().Create("sdfsd") is Test5);
        }

        [Test]
        public void TestToFacadeFactory1()
        {
            bool wasCalled = false;

            Binder.BindIFactory<FooFacade>().ToFacadeFactory<FooFacade.Factory>((c) =>
                {
                    wasCalled = true;
                });

            Assert.That(!wasCalled);
            Assert.IsNotNull(Resolver.Resolve<IFactory<FooFacade>>().Create() is FooFacade);
            Assert.That(wasCalled);
        }

        [Test]
        public void TestToFacadeFactory2()
        {
            bool wasCalled = false;

            Binder.BindIFactory<Facade>().ToFacadeFactory<FooFacade, FooFacade.Factory>((c) =>
                {
                    wasCalled = true;
                });

            Assert.That(!wasCalled);
            Assert.IsNotNull(Resolver.Resolve<IFactory<Facade>>().Create() is FooFacade);
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
