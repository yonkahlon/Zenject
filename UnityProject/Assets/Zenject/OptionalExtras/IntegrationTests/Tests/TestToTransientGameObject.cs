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
            Container.Bind<Foo>().ToTransientGameObject();
            Container.Bind<Foo>().ToTransientGameObject();
            Container.Bind<Bar>().ToTransientGameObject();

            Container.BindAllInterfaces<Runner>().ToSingle<Runner>();
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
