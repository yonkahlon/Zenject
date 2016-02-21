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

            AssertValidates();

            Assert.IsNotNull(ReferenceEquals(test4, Resolver.Resolve<IFactory<Test4>>().Create()));
        }

        [Test]
        public void TestToMethod()
        {
            var test3 = new Test3();

            Binder.BindIFactory<ITest>().ToMethod(c => test3);

            AssertValidates();

            Assert.That(ReferenceEquals(test3, Resolver.Resolve<IFactory<ITest>>().Create()));
        }

        [Test]
        public void TestToFactory()
        {
            Binder.BindIFactory<Test4>().ToFactory();

            AssertValidates();

            Assert.IsNotNull(Resolver.Resolve<IFactory<Test4>>().Create());
        }

        [Test]
        public void TestToDerivedFactory()
        {
            Binder.BindIFactory<ITest>().ToFactory<Test2>();

            AssertValidates();

            Assert.IsNotNull(Resolver.Resolve<IFactory<ITest>>().Create() is Test2);
        }

        [Test]
        public void TestToDerivedFactoryUntyped()
        {
            Binder.BindIFactory<ITest>().ToFactory(typeof(Test2));

            AssertValidates();

            Assert.IsNotNull(Resolver.Resolve<IFactory<ITest>>().Create() is Test2);
        }

        [Test]
        public void TestToIFactory()
        {
            Binder.BindIFactory<Test3>().ToFactory();
            Binder.BindIFactory<ITest>().ToIFactory<Test3>();

            AssertValidates();

            Assert.IsNotNull(Resolver.Resolve<IFactory<ITest>>().Create() is Test3);
        }

        [Test]
        public void TestToCustomFactory1()
        {
            Binder.BindIFactory<Test2>().ToCustomFactory<Test2.Factory>();

            AssertValidates();

            Assert.IsNotNull(Resolver.Resolve<IFactory<Test2>>().Create() is Test2);
        }

        [Test]
        public void TestToCustomFactory1Untyped()
        {
            Binder.BindIFactory<Test2>().ToCustomFactory(typeof(Test2.Factory));

            AssertValidates();

            Assert.IsNotNull(Resolver.Resolve<IFactory<Test2>>().Create() is Test2);
        }

        [Test]
        public void TestToCustomFactory()
        {
            Binder.Bind<Test2.Factory>().ToSingle();

            Binder.BindIFactory<ITest>().ToCustomFactory<Test2, Test2.Factory>();

            AssertValidates();

            Assert.IsNotNull(Resolver.Resolve<IFactory<ITest>>().Create() is Test2);
        }

        [Test]
        public void TestToMethodOneParam()
        {
            Binder.BindIFactory<string, ITest>().ToMethod((c, param) => new Test5(param));

            AssertValidates();

            Assert.IsNotNull(Resolver.Resolve<IFactory<string, ITest>>().Create("sdf"));
        }

        [Test]
        public void TestToFactoryOneParam()
        {
            Binder.BindIFactory<string, Test5>().ToFactory();

            AssertValidates();

            Assert.IsNotNull(Resolver.Resolve<IFactory<string, Test5>>().Create("sdf"));
        }

        [Test]
        public void TestToDerivedFactoryOneParam()
        {
            Binder.BindIFactory<string, ITest>().ToFactory<Test5>();

            AssertValidates();

            Assert.IsNotNull(Resolver.Resolve<IFactory<string, ITest>>().Create("sdf") is Test5);
        }

        [Test]
        public void TestToIFactoryOneParam()
        {
            Binder.BindIFactory<string, Test5>().ToFactory();
            Binder.BindIFactory<string, ITest>().ToIFactory<Test5>();

            AssertValidates();

            Assert.IsNotNull(Resolver.Resolve<IFactory<string, ITest>>().Create("sdfds") is Test5);
        }

        [Test]
        public void TestToCustomFactoryOneParam()
        {
            Binder.Bind<Test5.Factory>().ToSingle();

            Binder.BindIFactory<string, ITest>().ToCustomFactory<Test5, Test5.Factory>();

            AssertValidates();

            Assert.IsNotNull(Resolver.Resolve<IFactory<string, ITest>>().Create("sdfsd") is Test5);
        }

        [Test]
        public void TestToFacadeFactoryMethod1()
        {
            bool wasCalled = false;

            Binder.BindIFactory<FooFacade>().ToFacadeFactoryMethod<FooFacade.Factory>((c) =>
                {
                    wasCalled = true;

                    c.Bind<FooFacade>().ToSingle();
                });

            AssertValidates();

            Assert.That(!wasCalled);
            Assert.IsNotNull(Resolver.Resolve<IFactory<FooFacade>>().Create() is FooFacade);
            Assert.That(wasCalled);
        }

        [Test]
        public void TestToFacadeFactoryMethod2()
        {
            bool wasCalled = false;

            Binder.BindIFactory<FooFacade>().ToFacadeFactoryMethod<FooFacade, FooFacade.Factory>((c) =>
                {
                    wasCalled = true;
                    c.Bind<FooFacade>().ToSingle();
                });

            AssertValidates();

            Assert.That(!wasCalled);
            Assert.IsNotNull(Resolver.Resolve<IFactory<FooFacade>>().Create());
            Assert.That(wasCalled);
        }

        [Test]
        public void TestToFacadeFactoryInstaller1()
        {
            FooInstaller.WasCalled = false;

            Binder.BindIFactory<FooFacade>().ToFacadeFactoryInstaller<FooFacade.Factory, FooInstaller>();

            AssertValidates();

            Assert.That(!FooInstaller.WasCalled);
            Assert.IsNotNull(Resolver.Resolve<IFactory<FooFacade>>().Create() is FooFacade);
            Assert.That(FooInstaller.WasCalled);
        }

        [Test]
        public void TestToFacadeFactoryInstaller2()
        {
            FooInstaller.WasCalled = false;

            Binder.BindIFactory<FooFacade>().ToFacadeFactoryInstaller<FooFacade, FooFacade.Factory, FooInstaller>();

            AssertValidates();

            Assert.That(!FooInstaller.WasCalled);
            Assert.IsNotNull(Resolver.Resolve<IFactory<FooFacade>>().Create());
            Assert.That(FooInstaller.WasCalled);
        }

        public class FooFacade
        {
            public class Factory : FacadeFactory<FooFacade>
            {
            }
        }

        public class FooInstaller : Installer
        {
            public static bool WasCalled;

            public override void InstallBindings()
            {
                Binder.Bind<FooFacade>().ToSingle();
                WasCalled = true;
            }
        }
    }
}
