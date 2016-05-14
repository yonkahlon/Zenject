using System.Collections.Generic;
using UnityEngine;
using Zenject.TestFramework;
using Zenject;

namespace Zenject.Tests.ToSubContainerPrefab
{
    public class Fixture : ZenjectIntegrationTestFixture
    {
        [SerializeField]
        GameObject _fooPrefab;

        [Test]
        public void TestSelfSingle()
        {
            Container.Bind<Foo>().FromSubContainerResolve()
                .ByPrefab(_fooPrefab).AsSingle();

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Foo>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
        }

        [Test]
        public void TestSelfTransient()
        {
            Container.Bind<Foo>().FromSubContainerResolve().ByPrefab(_fooPrefab).AsTransient();

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Foo>();

            FixtureUtil.AssertNumGameObjects(Container, 3);
            FixtureUtil.AssertComponentCount<Foo>(Container, 3);
        }

        [Test]
        public void TestSelfCached()
        {
            Container.Bind<Foo>().FromSubContainerResolve().ByPrefab(_fooPrefab).AsCached();

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Foo>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
        }

        [Test]
        public void TestSelfSingleMultipleContracts()
        {
            Container.Bind<Foo>().FromSubContainerResolve().ByPrefab(_fooPrefab).AsSingle();
            Container.Bind<Bar>().FromSubContainerResolve().ByPrefab(_fooPrefab).AsSingle();

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Bar>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertComponentCount<Bar>(Container, 1);
        }

        [Test]
        public void TestSelfCachedMultipleContracts()
        {
            Container.Bind(typeof(Foo), typeof(Bar)).FromSubContainerResolve().ByPrefab(_fooPrefab).AsCached();

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Bar>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertComponentCount<Bar>(Container, 1);
        }

        [Test]
        public void TestSelfTransientMultipleContracts()
        {
            Container.Bind(typeof(Foo), typeof(Bar)).FromSubContainerResolve().ByPrefab(_fooPrefab).AsTransient();

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Bar>();

            FixtureUtil.AssertNumGameObjects(Container, 2);
            FixtureUtil.AssertComponentCount<Foo>(Container, 2);
            FixtureUtil.AssertComponentCount<Bar>(Container, 2);
        }

        [Test]
        public void TestConcreteSingle()
        {
            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve().ByPrefab(_fooPrefab).AsSingle();

            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
        }

        [Test]
        public void TestConcreteTransient()
        {
            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve().ByPrefab(_fooPrefab).AsTransient();

            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();

            FixtureUtil.AssertNumGameObjects(Container, 3);
            FixtureUtil.AssertComponentCount<Foo>(Container, 3);
        }

        [Test]
        public void TestConcreteCached()
        {
            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve().ByPrefab(_fooPrefab).AsCached();

            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
        }

        [Test]
        public void TestConcreteSingleMultipleContracts()
        {
            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve().ByPrefab(_fooPrefab).AsSingle();
            Container.Bind<Bar>().FromSubContainerResolve().ByPrefab(_fooPrefab).AsSingle();

            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<Bar>();
            Container.BindRootResolve<Bar>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertComponentCount<Bar>(Container, 1);
        }

        [Test]
        public void TestConcreteCachedMultipleContracts()
        {
            Container.Bind(typeof(Foo), typeof(IFoo)).To<Foo>().FromSubContainerResolve().ByPrefab(_fooPrefab).AsCached();

            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Foo>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
        }

        [Test]
        [ExpectedException]
        [ExpectedValidationException]
        public void TestSelfIdentifiersFails()
        {
            Container.Bind<Gorp>().FromSubContainerResolve().ByPrefab(_fooPrefab).AsSingle();

            Container.BindRootResolve<Gorp>();
        }

        [Test]
        public void TestSelfIdentifiers()
        {
            Container.Bind<Gorp>().FromSubContainerResolve("gorp").ByPrefab(_fooPrefab).AsSingle();

            Container.BindRootResolve<Gorp>();
            Container.BindRootResolve<Gorp>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
        }
    }
}
