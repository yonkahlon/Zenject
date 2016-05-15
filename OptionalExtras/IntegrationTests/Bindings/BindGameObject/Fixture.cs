using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject.TestFramework;
using ModestTree;

namespace Zenject.Tests.BindGameObject
{
    public class Fixture : ZenjectIntegrationTestFixture
    {
        public GameObject CubePrefab;

        const string GameObjName = "TestObj";

        [Test]
        public void TestGameObjectSelfSingle1()
        {
            Container.Bind<GameObject>().FromGameObject().WithGameObjectName(GameObjName).AsSingle();

            Container.BindRootResolve<GameObject>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, GameObjName, 1);
        }

        [Test]
        public void TestGameObjectSelfSingle2()
        {
            Container.Bind<GameObject>().FromGameObject().WithGameObjectName(GameObjName).AsSingle();
            Container.Bind<GameObject>().FromGameObject().WithGameObjectName(GameObjName).AsSingle();
            Container.Bind<GameObject>().WithId("asdf").FromGameObject().WithGameObjectName(GameObjName).AsSingle();

            Container.BindRootResolve<GameObject>();
            Container.BindRootResolve<GameObject>("asdf");

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, GameObjName, 1);
        }

        [Test]
        [ExpectedException]
        [ExpectedValidationException]
        public void TestGameObjectSelfSingleConflict()
        {
            Container.Bind<GameObject>().FromGameObject().WithGameObjectName(GameObjName).AsSingle();
            Container.Bind<GameObject>().FromGameObject().WithGameObjectName("asdf").AsSingle();

            Container.BindRootResolve<GameObject>();
        }

        [Test]
        public void TestGameObjectSelfTransient()
        {
            Container.Bind<GameObject>().FromGameObject().WithGameObjectName(GameObjName).AsTransient();
            Container.Bind<GameObject>().FromGameObject().WithGameObjectName(GameObjName).AsTransient();
            Container.Bind<GameObject>().FromGameObject().WithGameObjectName("asdf").AsTransient();
            Container.BindRootResolve<GameObject>();

            FixtureUtil.AssertNumGameObjects(Container, 3);
            FixtureUtil.AssertNumGameObjectsWithName(Container, GameObjName, 2);
        }

        [Test]
        public void TestGameObjectConcreteSingle()
        {
            Container.Bind<UnityEngine.Object>().To<GameObject>().FromGameObject().WithGameObjectName(GameObjName).AsSingle();

            Container.BindRootResolve<UnityEngine.Object>();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, GameObjName, 1);
        }

        [Test]
        public void TestPrefabSelfSingle1()
        {
            Container.Bind<GameObject>().FromPrefab(CubePrefab)
                .WithGameObjectName(GameObjName).AsSingle();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, GameObjName, 1);

            FixtureUtil.AddInitMethod(Container, (GameObject gameObject) =>
                {
                    Assert.IsNotNull(gameObject.GetComponent<BoxCollider>());
                });
        }

        [Test]
        public void TestPrefabConcreteSingle1()
        {
            Container.Bind<UnityEngine.Object>().To<GameObject>()
                .FromPrefab(CubePrefab).WithGameObjectName(GameObjName).AsSingle();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, GameObjName, 1);

            FixtureUtil.AddInitMethod(Container, (UnityEngine.Object obj) =>
                {
                    Assert.IsNotNull(((GameObject)obj).GetComponent<BoxCollider>());
                });
        }

        [Test]
        public void TestPrefabResourceSelfSingle1()
        {
            Container.Bind<GameObject>().FromPrefabResource("BindGameObject/Cube").WithGameObjectName(GameObjName).AsSingle();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, GameObjName, 1);

            FixtureUtil.AddInitMethod(Container, (GameObject gameObject) =>
                {
                    Assert.IsNotNull(gameObject.GetComponent<BoxCollider>());
                });
        }

        [Test]
        public void TestPrefabResourceConcreteSingle1()
        {
            Container.Bind<UnityEngine.Object>().To<GameObject>()
                .FromPrefabResource("BindGameObject/Cube").WithGameObjectName(GameObjName).AsSingle();

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, GameObjName, 1);

            FixtureUtil.AddInitMethod(Container, (UnityEngine.Object obj) =>
                {
                    Assert.IsNotNull(((GameObject)obj).GetComponent<BoxCollider>());
                });
        }
    }
}
