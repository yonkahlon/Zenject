using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class TestToSingleGameObject : MonoInstallerTestFixture
    {
        [InstallerTest]
        public void TestBindingsOnSameGameObject()
        {
            Binder.Bind<Foo>().ToSingleGameObject();
            Binder.Bind<IInitializable>().ToSingleGameObject<Foo>();

            Binder.BindAllInterfacesToSingle<Runner>();
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
