using System.Linq;
using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class TestToSinglePrefab : MonoInstallerTestFixture
    {
        public GameObject FooMono1Prefab;
        public GameObject FooMono1AndBarMono1Prefab;

        [InstallerTest]
        public void TestSamePrefabMultipleTypes()
        {
            Binder.Bind<FooMono1>().ToSinglePrefab(FooMono1Prefab);
            Binder.Bind<IInitializable>().ToSinglePrefab<FooMono1>(FooMono1Prefab);

            Binder.BindAllInterfacesToSingle<Runner1>();
        }

        public class Runner1 : IInitializable
        {
            readonly FooMono1 _foo;

            public Runner1(FooMono1 foo)
            {
                _foo = foo;
            }

            public void Initialize()
            {
                Assert.IsNotNull(_foo);
                Assert.IsEqual(GameObject.FindObjectsOfType<FooMono1>().Length, 1);
            }
        }

        [InstallerTest]
        public void TestToSinglePrefabSamePrefabMultipleTypes()
        {
            Binder.Bind<FooMono1>().ToSinglePrefab(FooMono1AndBarMono1Prefab);
            Binder.Bind<IInitializable>().ToSinglePrefab<FooMono1>(FooMono1AndBarMono1Prefab);

            Binder.Bind<BarMono1>().ToSinglePrefab(FooMono1AndBarMono1Prefab);
            Binder.Bind<IInitializable>().ToSinglePrefab<BarMono1>(FooMono1AndBarMono1Prefab);

            Binder.BindAllInterfacesToSingle<Runner2>();
        }

        public class Runner2 : IInitializable
        {
            public Runner2(FooMono1 foo, BarMono1 bar)
            {
            }

            public void Initialize()
            {
                Assert.IsEqual(GameObject.FindObjectsOfType<FooMono1>().Length, 1);
                Assert.IsEqual(GameObject.FindObjectsOfType<BarMono1>().Length, 1);

                Assert.IsEqual(
                    GameObject.FindObjectsOfType<BarMono1>().Single().gameObject,
                    GameObject.FindObjectsOfType<FooMono1>().Single().gameObject);
            }
        }

        [InstallerTest]
        public void TestToSinglePrefabSameTypeDifferentPrefab()
        {
            Binder.Bind<FooMono1>().ToSinglePrefab(FooMono1AndBarMono1Prefab);
            Assert.Throws(() => Binder.Bind<IInitializable>().ToSinglePrefab<FooMono1>(FooMono1Prefab));
        }
    }
}
