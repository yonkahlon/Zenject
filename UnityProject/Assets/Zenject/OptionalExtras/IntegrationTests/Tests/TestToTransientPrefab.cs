using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class TestToTransientPrefab : MonoInstallerTestFixture
    {
        public GameObject FooMono1Prefab;

        [InstallerTest]
        public void TestSamePrefabMultipleTypes()
        {
            Container.Bind<FooMono1>().ToTransientPrefab(FooMono1Prefab);
            Container.Bind<FooMono1>().ToTransientPrefab(FooMono1Prefab);

            Container.BindAllInterfacesToSingle<Runner1>();
        }

        public class Runner1 : IInitializable
        {
            public Runner1(List<FooMono1> foos)
            {
            }

            public void Initialize()
            {
                Assert.IsEqual(GameObject.FindObjectsOfType<FooMono1>().Length, 2);
            }
        }
    }
}
