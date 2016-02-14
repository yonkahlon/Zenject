using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class TestToSinglePrefabResource : MonoInstallerTestFixture
    {
        [InstallerTest]
        public void TestMultipleBindingsOnSamePrefab()
        {
            Binder.Bind<FooMono1>().ToSinglePrefabResource("FooMono1");
            Binder.Bind<IInitializable>().ToSinglePrefabResource<FooMono1>("FooMono1");

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
        public void Test2()
        {
            Binder.Bind<FooMono1>().ToSinglePrefabResource("FooMono1");
            Binder.Bind<BarMono1>().ToSinglePrefabResource("BarMono1");

            Binder.BindAllInterfacesToSingle<Runner2>();
        }

        public class Runner2 : IInitializable
        {
            readonly BarMono1 _bar;
            readonly FooMono1 _foo;

            public Runner2(
                FooMono1 foo,
                BarMono1 bar)
            {
                _bar = bar;
                _foo = foo;
            }

            public void Initialize()
            {
                Assert.IsNotNull(_bar);
                Assert.IsNotNull(_foo);
                Assert.IsEqual(GameObject.FindObjectsOfType<FooMono1>().Length, 1);
                Assert.IsEqual(GameObject.FindObjectsOfType<BarMono1>().Length, 1);
            }
        }
    }
}
