using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class TestToTransientGameObject : MonoInstallerTestFixture
    {
        [InstallerTest]
        public void Test1()
        {
            Binder.Bind<Foo>().ToTransientGameObject();
            Binder.Bind<Foo>().ToTransientGameObject();
            Binder.Bind<Bar>().ToTransientGameObject();

            Binder.BindAllInterfacesToSingle<Runner>();
        }

        public class Runner : IInitializable
        {
            public Runner(
                List<Foo> foo, Bar bar)
            {
            }

            public void Initialize()
            {
                Assert.IsEqual(GameObject.FindObjectsOfType<Foo>().Length, 2);
                Assert.IsEqual(GameObject.FindObjectsOfType<Bar>().Length, 1);
            }
        }

        public class Bar : MonoBehaviour
        {
        }

        public class Foo : MonoBehaviour
        {
        }
    }
}
