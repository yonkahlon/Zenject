using System;
using NUnit.Framework;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestFacadeFactory2 : TestWithContainer
    {
        [Test]
        public void TestParam1Method()
        {
            Container.BindFacadeFactoryMethod<string, FooFacadeOne, FooFacadeOne.Factory>(FacadeInstallerOne);

            AssertValidates();

            var facadeFactory = Container.Resolve<FooFacadeOne.Factory>();
            var facade = facadeFactory.Create("bob");

            Assert.IsEqual(facade.Value, "bob");
        }

        [Test]
        public void TestParam1Installer()
        {
            Container.BindFacadeFactoryInstaller<FooFacadeOne, FooFacadeOne.Factory, FooInstallerOne>();

            AssertValidates();

            var facadeFactory = Container.Resolve<FooFacadeOne.Factory>();
            var facade = facadeFactory.Create("bob");

            Assert.IsEqual(facade.Value, "bob");
        }

        void FacadeInstallerOne(DiContainer container, string param1)
        {
            container.Bind<FooFacadeOne>().ToSingle();
            container.BindInstance(param1);
        }

        public class FooFacadeOne
        {
            public FooFacadeOne(string value)
            {
                Value = value;
            }

            public string Value
            {
                get;
                set;
            }

            public class Factory : FacadeFactory<string, FooFacadeOne>
            {
            }
        }

        class FooInstallerOne : Installer
        {
            string _param1;

            public FooInstallerOne(string param1)
            {
                _param1 = param1;
            }

            public override void InstallBindings()
            {
                Container.Bind<FooFacadeOne>().ToSingle();
                Container.BindInstance(_param1);
            }
        }

        [Test]
        public void TestParam6Method()
        {
            Container.BindFacadeFactoryMethod<string, int, float, int, string, int, FooFacadeSix, FooFacadeSix.Factory>(FacadeInstallerSix);

            AssertValidates();

            var facadeFactory = Container.Resolve<FooFacadeSix.Factory>();
            var facade = facadeFactory.Create("bob", 0, 2.1f, 4, "", 8);

            Assert.IsEqual(facade.Value, "bob");
        }

        [Test]
        public void TestParam6Installer()
        {
            Container.BindFacadeFactoryInstaller<FooFacadeSix, FooFacadeSix.Factory, FooInstallerSix>();

            AssertValidates();

            var facadeFactory = Container.Resolve<FooFacadeSix.Factory>();
            var facade = facadeFactory.Create("bob", 0, 2.1f, 4, "", 8);

            Assert.IsEqual(facade.Value, "bob");
        }

        public class FooFacadeSix
        {
            public FooFacadeSix(string value)
            {
                Value = value;
            }

            public string Value
            {
                get;
                set;
            }

            public class Factory : FacadeFactory<string, int, float, int, string, int, FooFacadeSix>
            {
            }
        }

        void FacadeInstallerSix(
            DiContainer container, string param1, int param2, float param3,
            int param4, string param5, int param6)
        {
            container.Bind<FooFacadeSix>().ToSingle();
            container.BindInstance(param1);
        }

        class FooInstallerSix : Installer
        {
            string _param1;

            public FooInstallerSix(
                string param1, int param2, float param3,
                int param4, string param5, int param6)
            {
                _param1 = param1;
            }

            public override void InstallBindings()
            {
                Container.Bind<FooFacadeSix>().ToSingle();
                Container.BindInstance(_param1);
            }
        }
    }
}

