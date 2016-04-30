using ModestTree.UnityUnitTester;
using UnityEngine;
using Zenject;

namespace ModestTree.Tests.Zenject.TestBindFactory
{
    public class Fixture : MonoTestFixture
    {
        public GameObject FooPrefab;
        public GameObject FooSubContainerPrefab;

        [Test]
        public void TestToGameObjectSelf()
        {
            Container.BindFactory<Foo, Foo.Factory>().FromGameObject();

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);

            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertNumGameObjects(Container, 1);
        }

        [Test]
        [ExpectedException]
        [ExpectedValidationException]
        public void TestToGameObjectSelfFail()
        {
            Container.BindFactory<Foo2, Foo2.Factory>().FromGameObject();

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);
        }

        [Test]
        public void TestToGameObjectConcrete()
        {
            Container.BindFactory<IFoo, IFooFactory>().To<Foo>().FromGameObject();

            FixtureUtil.CallFactoryCreateMethod<IFoo, IFooFactory>(Container);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertNumGameObjects(Container, 1);
        }

        [Test]
        public void TestToMonoBehaviourSelf()
        {
            var gameObject = Container.CreateEmptyGameObject("foo");

            Container.BindFactory<Foo, Foo.Factory>().FromComponent(gameObject);

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertNumGameObjects(Container, 1);
        }

        [Test]
        [ExpectedException]
        [ExpectedValidationException]
        public void TestToMonoBehaviourSelfFail()
        {
            Container.BindFactory<Foo2, Foo2.Factory>().FromComponent((GameObject)null);
        }

        [Test]
        public void TestToMonoBehaviourConcrete()
        {
            var gameObject = Container.CreateEmptyGameObject("foo");

            Container.BindFactory<IFoo, IFooFactory>().To<Foo>().FromComponent(gameObject);

            FixtureUtil.CallFactoryCreateMethod<IFoo, IFooFactory>(Container);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertNumGameObjects(Container, 1);
        }

        [Test]
        public void TestToPrefabSelf()
        {
            Container.BindFactory<Foo, Foo.Factory>().FromPrefab(FooPrefab).WithGameObjectName("asdf");

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);

            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, "asdf", 1);
        }

        [Test]
        [ExpectedException]
        [ExpectedValidationException]
        public void TestToPrefabSelfFail()
        {
            // Foo3 is not on the prefab
            Container.BindFactory<Foo3, Foo3.Factory>().FromPrefab(FooPrefab);

            FixtureUtil.CallFactoryCreateMethod<Foo3, Foo3.Factory>(Container);
        }

        [Test]
        public void TestToPrefabConcrete()
        {
            Container.BindFactory<IFoo, IFooFactory>().To<Foo>().FromPrefab(FooPrefab).WithGameObjectName("asdf");

            FixtureUtil.CallFactoryCreateMethod<IFoo, IFooFactory>(Container);

            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, "asdf", 1);
        }

        [Test]
        public void TestToResourceSelf()
        {
            Container.BindFactory<Texture, Factory<Texture>>()
                .FromResource("TestBindFactory/TestTexture");
            Container.BindRootResolve<Factory<Texture>>();

            FixtureUtil.CallFactoryCreateMethod<Texture, Factory<Texture>>(Container);
        }

        [Test]
        public void TestToResource()
        {
            Container.BindFactory<UnityEngine.Object, Factory<UnityEngine.Object>>()
                .To<Texture>().FromResource("TestBindFactory/TestTexture");
            Container.BindRootResolve<Factory<UnityEngine.Object>>();
        }

        [Test]
        public void TestToPrefabResourceSelf()
        {
            Container.BindFactory<Foo, Foo.Factory>().FromPrefabResource("TestBindFactory/Foo").WithGameObjectName("asdf");

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);

            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, "asdf", 1);
        }

        [Test]
        public void TestToPrefabResourceConcrete()
        {
            Container.BindFactory<Foo, Foo.Factory>().To<Foo>().FromPrefabResource("TestBindFactory/Foo").WithGameObjectName("asdf");

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);

            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertNumGameObjects(Container, 1);
            FixtureUtil.AssertNumGameObjectsWithName(Container, "asdf", 1);
        }

        [Test]
        public void TestToSubContainerPrefabSelf()
        {
            Container.BindFactory<Foo, Foo.Factory>().FromSubContainerResolve().ByPrefab(FooSubContainerPrefab);

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);

            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertNumGameObjects(Container, 1);
        }

        [Test]
        public void TestToSubContainerPrefabConcrete()
        {
            Container.BindFactory<IFoo, IFooFactory>()
                .To<Foo>().FromSubContainerResolve().ByPrefab(FooSubContainerPrefab);

            FixtureUtil.CallFactoryCreateMethod<IFoo, IFooFactory>(Container);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertNumGameObjects(Container, 1);
        }

        [Test]
        public void TestToSubContainerPrefabResourceSelf()
        {
            Container.BindFactory<Foo, Foo.Factory>()
                .FromSubContainerResolve().ByPrefabResource("TestBindFactory/FooSubContainer");

            FixtureUtil.CallFactoryCreateMethod<Foo, Foo.Factory>(Container);

            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertNumGameObjects(Container, 1);
        }

        [Test]
        public void TestToSubContainerPrefabResourceConcrete()
        {
            Container.BindFactory<IFoo, IFooFactory>()
                .To<Foo>().FromSubContainerResolve().ByPrefabResource("TestBindFactory/FooSubContainer");

            FixtureUtil.CallFactoryCreateMethod<IFoo, IFooFactory>(Container);
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertNumGameObjects(Container, 1);
        }

        public class Foo3 : MonoBehaviour
        {
            public class Factory : Factory<Foo3>
            {
            }
        }

        public class Foo2 : MonoBehaviour
        {
            [Inject]
            int _value;

            public class Factory : Factory<Foo2>
            {
            }
        }
    }
}
