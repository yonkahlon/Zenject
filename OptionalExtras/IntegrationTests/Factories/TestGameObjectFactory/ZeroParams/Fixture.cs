using UnityEngine;
using Zenject;
using Zenject.TestFramework;

namespace Zenject.Tests.TestGameObjectFactory.ZeroParams
{
    public class Fixture : ZenjectIntegrationTestFixture
    {
        public GameObject CubePrefab;

        const string GameObjName = "TestObj";

        [Test]
        public void TestGameObjectSelf1()
        {
            Container.BindFactory<GameObject, CubeFactory>().FromGameObject().WithGameObjectName(GameObjName);

            FixtureUtil.CallFactoryCreateMethod<GameObject, CubeFactory>(Container);

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, GameObjName, 1);
        }

        [Test]
        public void TestGameObjectConcreteSingle()
        {
            Container.BindFactory<UnityEngine.Object, ObjectFactory>().To<GameObject>().FromGameObject().WithGameObjectName(GameObjName);

            FixtureUtil.CallFactoryCreateMethod<UnityEngine.Object, ObjectFactory>(Container);

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, GameObjName, 1);
        }

        [Test]
        public void TestPrefabSelfSingle1()
        {
            Container.BindFactory<GameObject, CubeFactory>().FromPrefab(CubePrefab).WithGameObjectName(GameObjName);

            FixtureUtil.CallFactoryCreateMethod<GameObject, CubeFactory>(Container);

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, GameObjName, 1);
        }

        [Test]
        public void TestPrefabConcreteSingle1()
        {
            Container.BindFactory<UnityEngine.Object, ObjectFactory>().To<GameObject>().FromPrefab(CubePrefab).WithGameObjectName(GameObjName);

            FixtureUtil.CallFactoryCreateMethod<UnityEngine.Object, ObjectFactory>(Container);

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, GameObjName, 1);
        }

        [Test]
        public void TestPrefabResourceSelfSingle1()
        {
            Container.BindFactory<GameObject, CubeFactory>()
                .FromPrefabResource("BindGameObject/Cube").WithGameObjectName(GameObjName);

            FixtureUtil.CallFactoryCreateMethod<GameObject, CubeFactory>(Container);

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, GameObjName, 1);
        }

        [Test]
        public void TestPrefabResourceConcreteSingle1()
        {
            Container.BindFactory<UnityEngine.Object, ObjectFactory>()
                .To<GameObject>().FromPrefabResource("BindGameObject/Cube").WithGameObjectName(GameObjName);

            FixtureUtil.CallFactoryCreateMethod<UnityEngine.Object, ObjectFactory>(Container);

            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, GameObjName, 1);
        }

        public class ObjectFactory : Factory<UnityEngine.Object>
        {
        }

        public class CubeFactory : Factory<GameObject>
        {
        }
    }
}

