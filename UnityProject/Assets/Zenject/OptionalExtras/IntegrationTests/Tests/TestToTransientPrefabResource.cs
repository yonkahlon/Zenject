using System.Collections.Generic;
using Zenject;
using UnityEngine;

namespace ModestTree
{
    public class TestToTransientPrefabResource : MonoInstallerTestFixture
    {
        [InstallerTest]
        public void Test1()
        {
            Binder.Bind<FooMono1>().ToTransientPrefabResource("FooMono1");
            Binder.Bind<FooMono1>().ToTransientPrefabResource("FooMono1");
            Binder.Bind<FooMono1>().ToTransientPrefabResource("FooMono1");

            Binder.BindAllInterfacesToSingle<Runner1>();
        }

        public class Runner1 : IInitializable
        {
            public Runner1(List<FooMono1> foo)
            {
            }

            public void Initialize()
            {
                Assert.IsEqual(GameObject.FindObjectsOfType<FooMono1>().Length, 3);
            }
        }
    }
}
