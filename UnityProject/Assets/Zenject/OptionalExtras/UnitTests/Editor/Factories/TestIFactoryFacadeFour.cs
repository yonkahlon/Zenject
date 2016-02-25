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
    public class TestIFactoryFacadeFour : TestWithContainer
    {
        [Test]
        public void TestToFacadeFactoryMethod1()
        {
            Container.BindIFactory<string, int, double, float, FooFacade>().ToFacadeFactoryMethod<FooFacade.Factory>((c, p1, p2, p3, p4) =>
                {
                    c.BindInstance(p1);
                    c.Bind<FooFacade>().ToSingle();
                });

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFactory<string, int, double, float, FooFacade>>().Create("bob", 0, 2.0, 4.0f).Value, "bob");
        }

        [Test]
        public void TestToFacadeFactoryMethod2()
        {
            Container.BindIFactory<string, int, double, float, IFoo>().ToFacadeFactoryMethod<FooFacade, FooFacade.Factory>((c, p1, p2, p3, p4) =>
                {
                    c.BindInstance(p1);
                    c.Bind<FooFacade>().ToSingle();
                });

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFactory<string, int, double, float, IFoo>>().Create("bob", 0, 2.0, 4.0f).Value, "bob");
        }

        [Test]
        public void TestToFacadeFactoryInstallerParam1()
        {
            Container.BindIFactory<string, int, double, float, FooFacade>().ToFacadeFactoryInstaller<FooFacade.Factory, FooInstaller>();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFactory<string, int, double, float, FooFacade>>().Create("bob", 0, 2.0, 4.0f).Value, "bob");
        }

        [Test]
        public void TestToFacadeFactoryInstallerParam2()
        {
            Container.BindIFactory<string, int, double, float, IFoo>().ToFacadeFactoryInstaller<FooFacade, FooFacade.Factory, FooInstaller>();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFactory<string, int, double, float, IFoo>>().Create("bob", 0, 2.0, 4.0f).Value, "bob");
        }

        public interface IFoo
        {
            string Value
            {
                get;
            }
        }

        public class FooFacade : IFoo
        {
            readonly string _value;

            public FooFacade(string value)
            {
                _value = value;
            }

            public string Value
            {
                get
                {
                    return _value;
                }
            }

            public class Factory : FacadeFactory<string, int, double, float, FooFacade>
            {
            }
        }

        public class FooInstaller : Installer
        {
            string _param1;

            public FooInstaller(string param1, int p2, double p3, float p4)
            {
                _param1 = param1;
            }

            public override void InstallBindings()
            {
                Container.Bind<FooFacade>().ToSingle();
                Container.BindInstance(_param1);
            }
        }
    }
}



