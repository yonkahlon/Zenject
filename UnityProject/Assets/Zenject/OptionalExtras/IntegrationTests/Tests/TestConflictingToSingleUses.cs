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
            Container.Bind<FooMono1>().ToSinglePrefab(FooMono1Prefab);

            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleGameObject());
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSinglePrefab(FooMono1OtherPrefab));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSinglePrefabResource("FooMono1"));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSinglePrefabResource("FooMono1Other"));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToTransientPrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToTransientPrefab(FooMono1OtherPrefab));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleMonoBehaviour(new GameObject()));
        }

        [InstallerTest]
        public void TestToSingleGameObject()
        {
            Container.Bind<FooMono1>().ToSingleGameObject();

            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSinglePrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSinglePrefabResource("FooMono1"));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSinglePrefabResource("FooMono1Other"));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToTransientPrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleMonoBehaviour(new GameObject()));
        }

        [InstallerTest]
        public void TestToSinglePrefabResource()
        {
            Container.Bind<FooMono1>().ToSinglePrefabResource("FooMono1");

            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleGameObject());
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSinglePrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSinglePrefabResource("FooMono1Other"));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToTransientPrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleMonoBehaviour(new GameObject()));
        }

        [InstallerTest]
        public void TestToSingleMethod()
        {
            Container.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>());

            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleGameObject());
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSinglePrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSinglePrefabResource("FooMono1"));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToTransientPrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleMonoBehaviour(new GameObject()));
        }

        [InstallerTest]
        public void TestToSingleInstance()
        {
            Container.Bind<FooMono1>().ToSingleInstance(new GameObject().AddComponent<FooMono1>());

            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleGameObject());
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSinglePrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSinglePrefabResource("FooMono1"));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToTransientPrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleMonoBehaviour(new GameObject()));
        }

        [InstallerTest]
        public void TestToSingleMonoBehaviour()
        {
            Container.Bind<FooMono1>().ToSingleMonoBehaviour(new GameObject());

            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleGameObject());
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSinglePrefab(FooMono1Prefab));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSinglePrefabResource("FooMono1"));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleMethod((ctx) => new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToSingleInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToInstance(new GameObject().AddComponent<FooMono1>()));
            Assert.Throws<ZenjectBindException>(() => Container.Bind<FooMono1>().ToTransientPrefab(FooMono1Prefab));
        }
    }
}
