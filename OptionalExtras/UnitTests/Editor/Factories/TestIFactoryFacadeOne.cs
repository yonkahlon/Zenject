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
    public class TestIFactoryFacadeOne : TestWithContainer
    {
        [Test]
        public void TestToFacadeFactoryMethod1()
        {
            Container.BindIFactory<string, FooFacade>().ToFacadeFactoryMethod<FooFacade.Factory>((c, value) =>
                {
                    c.BindInstance(value);
                    c.Bind<FooFacade>().ToSingle();
                });

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFactory<string, FooFacade>>().Create("bob").Value, "bob");
        }

        [Test]
        public void TestToFacadeFactoryMethod2()
        {
            Container.BindIFactory<string, IFoo>().ToFacadeFactoryMethod<FooFacade, FooFacade.Factory>((c, value) =>
                {
                    c.BindInstance(value);
                    c.Bind<FooFacade>().ToSingle();
                });

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFactory<string, IFoo>>().Create("bob").Value, "bob");
        }

        [Test]
        public void TestToFacadeFactoryInstallerParam1()
        {
            Container.BindIFactory<string, FooFacade>().ToFacadeFactoryInstaller<FooFacade.Factory, FooInstaller>();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFactory<string, FooFacade>>().Create("bob").Value, "bob");
        }

        [Test]
        public void TestToFacadeFactoryInstallerParam2()
        {
            Container.BindIFactory<string, IFoo>().ToFacadeFactoryInstaller<FooFacade, FooFacade.Factory, FooInstaller>();

            AssertValidates();

            Assert.IsEqual(Container.Resolve<IFactory<string, IFoo>>().Create("bob").Value, "bob");
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

            public class Factory : FacadeFactory<string, FooFacade>
            {
            }
        }

        public class FooInstaller : Installer
        {
            string _param1;

            public FooInstaller(string param1)
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


