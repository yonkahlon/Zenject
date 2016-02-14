using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class TestConflictingToSingleUses : MonoInstallerTestFixture
    {
        public GameObject FooMono1Prefab;
        public GameObject FooMono1OtherPrefab;

        [InstallerTest]
        public void TestToSinglePrefab()
        {
            Binder.Bind<FooMono1>().ToSinglePrefab(FooMono1Prefab);

            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleGameObject());
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSinglePrefab(FooMono1OtherPrefab));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSinglePrefabResource("FooMono1"));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSinglePrefabResource("FooMono1Other"));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToTransientPrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToTransientPrefab(FooMono1OtherPrefab));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleMonoBehaviour(new GameObject()));
        }

        [InstallerTest]
        public void TestToSingleGameObject()
        {
            Binder.Bind<FooMono1>().ToSingleGameObject();

            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSinglePrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSinglePrefabResource("FooMono1"));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSinglePrefabResource("FooMono1Other"));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToTransientPrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleMonoBehaviour(new GameObject()));
        }

        [InstallerTest]
        public void TestToSinglePrefabResource()
        {
            Binder.Bind<FooMono1>().ToSinglePrefabResource("FooMono1");

            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleGameObject());
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSinglePrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSinglePrefabResource("FooMono1Other"));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToTransientPrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleMonoBehaviour(new GameObject()));
        }

        [InstallerTest]
        public void TestToSingleMethod()
        {
            Binder.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>());

            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleGameObject());
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSinglePrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSinglePrefabResource("FooMono1"));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToTransientPrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleMonoBehaviour(new GameObject()));
        }

        [InstallerTest]
        public void TestToSingleInstance()
        {
            Binder.Bind<FooMono1>().ToSingleInstance(new GameObject().AddComponent<FooMono1>());

            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleGameObject());
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSinglePrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSinglePrefabResource("FooMono1"));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToTransientPrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleMonoBehaviour(new GameObject()));
        }

        [InstallerTest]
        public void TestToSingleMonoBehaviour()
        {
            Binder.Bind<FooMono1>().ToSingleMonoBehaviour(new GameObject());

            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleGameObject());
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSinglePrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSinglePrefabResource("FooMono1"));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToSingleInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<FooMono1>().ToTransientPrefab(FooMono1Prefab));
        }
    }
}
