using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using Zenject.TestFramework;
using Zenject;

namespace Zenject.Tests.ToPrefabResource
{
    public class Fixture : ZenjectIntegrationTestFixture
    {
        const string PathPrefix = "TestToPrefabResource/";

        [Test]
        [ExpectedException]
        [ExpectedValidationException]
        public void TestTransientError()
        {
            // Validation should detect that it doesn't exist
            Container.Bind<Foo>().FromPrefabResource(PathPrefix + "asdfasdfas").AsTransient();

            Container.BindRootResolve<Foo>();
        }

        [Test]
        public void TestTransient()
        {
            Container.Bind<Foo>().FromPrefabResource(PathPrefix + "Foo").AsTransient();
            Container.Bind<Foo>().FromPrefabResource(PathPrefix + "Foo").AsTransient();

            Container.BindRootResolve<Foo>();

            FixtureUtil.AssertComponentCount<Foo>(Container, 2);
        }

        [Test]
        public void TestSingle()
        {
            Container.Bind<IFoo>().To<Foo>().FromPrefabResource(PathPrefix + "Foo").AsSingle();
            Container.Bind<Foo>().FromPrefabResource(PathPrefix + "Foo").AsSingle();

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<IFoo>();

            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
        }

        [Test]
        public void TestSingle2()
        {
            // For ToPrefab, the 'AsSingle' applies to the prefab and not the type, so this is valid
            Container.Bind<IFoo>().To<Foo>().FromPrefabResource(PathPrefix + "Foo").AsSingle();
            Container.Bind<Foo>().FromPrefabResource(PathPrefix + "Foo2").AsSingle();
            Container.Bind<Foo>().FromMethod(ctx => ctx.Container.CreateEmptyGameObject("Foo").AddComponent<Foo>());

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<IFoo>();

            FixtureUtil.AssertComponentCount<Foo>(Container, 3);
            FixtureUtil.AssertNumGameObjects(Container, 3);
        }

        [Test]
        public void TestSingleIdentifiers()
        {
            Container.Bind<Foo>().FromPrefabResource(PathPrefix + "Foo").WithGameObjectName("Foo").AsSingle();
            Container.Bind<Bar>().FromPrefabResource(PathPrefix + "Foo").WithGameObjectName("Foo").AsSingle();

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Bar>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertComponentCount<Bar>(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, "Foo", 1);
        }

        [Test]
        public void TestCached1()
        {
            Container.Bind(typeof(Foo), typeof(Bar)).FromPrefabResource(PathPrefix + "Foo").WithGameObjectName("Foo").AsCached();

            Container.BindRootResolve<Foo>();
            Container.BindRootResolve<Bar>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertComponentCount<Bar>(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, "Foo", 1);
        }

        [Test]
        [ExpectedException]
        [ExpectedValidationException]
        public void TestWithArgumentsFail()
        {
            // They have required arguments
            Container.Bind(typeof(Gorp), typeof(Qux)).FromPrefabResource(PathPrefix + "GorpAndQux").AsCached();

            Container.BindRootResolve<Gorp>();
            Container.BindRootResolve<Qux>();
        }

        [Test]
        public void TestWithArguments()
        {
            Container.Bind(typeof(Gorp), typeof(Qux))
                .FromPrefabResource(PathPrefix + "GorpAndQux").WithGameObjectName("GorpAndQux").AsCached()
                .WithArguments(5, "test1");

            Container.BindRootResolve<Gorp>();
            Container.BindRootResolve<Qux>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<Gorp>(Container, 1);
            FixtureUtil.AssertComponentCount<Qux>(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, "GorpAndQux", 1);
        }

        [Test]
        public void TestWithAbstractSearch()
        {
            // There are three components that implement INorf on this prefab
            // and so this should result in a list of 3 INorf's
            Container.Bind<INorf>().FromPrefabResource(PathPrefix + "Norf");

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertComponentCount<INorf>(Container, 3);
            FixtureUtil.AssertResolveCount<INorf>(Container, 3);
        }

        [Test]
        public void TestAbstractBindingConcreteSearch()
        {
            // Should ignore the Norf2 component on it
            Container.Bind<INorf>().To<Norf>().FromPrefabResource(PathPrefix + "Norf");

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertResolveCount<INorf>(Container, 2);
        }

        [Test]
        public void TestCircularDependencies()
        {
            // Jim and Bob both depend on each other
            Container.Bind(typeof(Jim), typeof(Bob)).FromPrefabResource(PathPrefix + "JimAndBob").AsCached();

            Container.BindRootResolve<Jim>();
            Container.BindRootResolve<Bob>();

            Container.BindAllInterfaces<JimAndBobRunner>().To<JimAndBobRunner>().AsSingle();
        }

        public class JimAndBobRunner : IInitializable
        {
            readonly Bob _bob;
            readonly Jim _jim;

            public JimAndBobRunner(Jim jim, Bob bob)
            {
                _bob = bob;
                _jim = jim;
            }

            public void Initialize()
            {
                Assert.IsNotNull(_jim.Bob);
                Assert.IsNotNull(_bob.Jim);

                Log.Info("Jim and bob successfully got the other reference");
            }
        }
    }
}
