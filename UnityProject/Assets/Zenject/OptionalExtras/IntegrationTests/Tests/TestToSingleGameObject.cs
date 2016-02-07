using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class TestToSingleGameObject : MonoInstallerTestFixture
    {
        [InstallerTest]
        public void TestBindingsOnSameGameObject()
        {
            Container.Bind<Foo>().ToSingleGameObject();
            Container.Bind<IInitializable>().ToSingleGameObject<Foo>();

            Container.BindAllInterfacesToSingle<Runner>();
        }

        public class Runner : IInitializable
        {
            readonly Foo _foo;

            public Runner(Foo foo)
            {
                _foo = foo;
            }

            public void Initialize()
            {
                Assert.IsNotNull(_foo);
                Assert.IsEqual(GameObject.FindObjectsOfType<Foo>().Length, 1,
                    "Found multiple instances of type Foo in scene");
            }
        }

        public class Foo : MonoBehaviour, IInitializable
        {
            public void Initialize()
            {
            }
        }
    }
}
