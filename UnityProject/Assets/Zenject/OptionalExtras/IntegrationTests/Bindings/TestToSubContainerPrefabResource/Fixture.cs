using System.Collections.Generic;
using ModestTree.UnityUnitTester;
using UnityEngine;
using Zenject;

namespace ModestTree.Tests.Zenject.ToSubContainerPrefabResource
{
    public class Fixture : MonoTestFixture
    {
        const string PathPrefix = "ToSubContainerPrefabResource/";
        const string FooResourcePath = PathPrefix + "FooSubContainer";

        [Test]
        [ExpectedException]
        [ExpectedValidationException]
        public void TestTransientError()
        {
            // Validation should detect that it doesn't exist
            Container.Bind<Foo>().FromSubContainerResolve().ByPrefabResource(PathPrefix + "asdfasdfas").AsTransient();

            Container.BindRootResolve<Foo>();
        }

        [Test]
        public void TestSelfSingle()
        {
            Container.Bind<Foo>().FromSubContainerResolve().ByPrefabResource(FooResourcePath).AsSingle();

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Foo>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
        }

        [Test]
        public void TestSelfTransient()
        {
            Container.Bind<Foo>().FromSubContainerResolve().ByPrefabResource(FooResourcePath).AsTransient();

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Foo>();

            FixtureUtil.AssertNumGameObjects(Container, 3);
            FixtureUtil.AssertComponentCount<Foo>(Container, 3);
        }

        [Test]
        public void TestSelfCached()
        {
            Container.Bind<Foo>().FromSubContainerResolve().ByPrefabResource(FooResourcePath).AsCached();

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Foo>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
        }

        [Test]
        public void TestSelfSingleMultipleContracts()
        {
            Container.Bind<Foo>().FromSubContainerResolve().ByPrefabResource(FooResourcePath).AsSingle();
            Container.Bind<Bar>().FromSubContainerResolve().ByPrefabResource(FooResourcePath).AsSingle();

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Bar>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertComponentCount<Bar>(Container, 1);
        }

        [Test]
        public void TestSelfCachedMultipleContracts()
        {
            Container.Bind(typeof(Foo), typeof(Bar)).FromSubContainerResolve().ByPrefabResource(FooResourcePath).AsCached();

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Bar>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertComponentCount<Bar>(Container, 1);
        }

        [Test]
        public void TestSelfTransientMultipleContracts()
        {
            Container.Bind(typeof(Foo), typeof(Bar)).FromSubContainerResolve().ByPrefabResource(FooResourcePath).AsTransient();

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Bar>();

            FixtureUtil.AssertNumGameObjects(Container, 2);
            FixtureUtil.AssertComponentCount<Foo>(Container, 2);
            FixtureUtil.AssertComponentCount<Bar>(Container, 2);
        }

        [Test]
        public void TestConcreteSingle()
        {
            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve().ByPrefabResource(FooResourcePath).AsSingle();

            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
        }

        [Test]
        public void TestConcreteTransient()
        {
            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve().ByPrefabResource(FooResourcePath).AsTransient();

            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();

            FixtureUtil.AssertNumGameObjects(Container, 3);
            FixtureUtil.AssertComponentCount<Foo>(Container, 3);
        }

        [Test]
        public void TestConcreteCached()
        {
            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve().ByPrefabResource(FooResourcePath).AsCached();

            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<IFoo>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
        }

        [Test]
        public void TestConcreteSingleMultipleContracts()
        {
            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve().ByPrefabResource(FooResourcePath).AsSingle();
            Container.Bind<Bar>().FromSubContainerResolve().ByPrefabResource(FooResourcePath).AsSingle();

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
            Container.Bind(typeof(Foo), typeof(IFoo)).To<Foo>().FromSubContainerResolve().ByPrefabResource(FooResourcePath).AsCached();

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
            Container.Bind<Gorp>().FromSubContainerResolve().ByPrefabResource(FooResourcePath).AsSingle();

            Container.BindRootResolve<Gorp>();
        }

        [Test]
        public void TestSelfIdentifiers()
        {
            Container.Bind<Gorp>().FromSubContainerResolve("gorp").ByPrefabResource(FooResourcePath).AsSingle();

            Container.BindRootResolve<Gorp>();
            Container.BindRootResolve<Gorp>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
        }
    }
}
