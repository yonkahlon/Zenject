using System.Collections.Generic;
using Zenject;
using UnityEngine;

namespace ModestTree
{
    public class TestToSingleMonoBehaviour : MonoInstallerTestFixture
    {
        GameObject _gameObject;

        [InstallerTest]
        public void Test1()
        {
            _gameObject = new GameObject();
            _gameObject.transform.SetParent(this.transform, false);

            Binder.BindInstance(this);
            Binder.Bind<FooMono1>().ToSingleMonoBehaviour<FooMono1>(_gameObject);
            Binder.Bind<IInitializable>().ToSingle<Runner1>();
        }

        public class Runner1 : IInitializable
        {
            public Runner1(FooMono1 foo, TestToSingleMonoBehaviour owner)
            {
                Assert.IsEqual(owner._gameObject, foo.gameObject);
            }

            public void Initialize()
            {
                Assert.IsEqual(GameObject.FindObjectsOfType<FooMono1>().Length, 1);
            }
        }

        [InstallerTest]
        public void TestMultipleBindings()
        {
            _gameObject = new GameObject();
            _gameObject.transform.SetParent(this.transform, false);

            Binder.BindInstance(this);
            Binder.Bind<FooMono1>().ToSingleMonoBehaviour(_gameObject);
            Binder.Bind<IInitializable>().ToSingleMonoBehaviour<FooMono1>(_gameObject);
            Binder.Bind<IInitializable>().ToSingle<Runner1>();
        }

        [InstallerTest]
        public void TestDuplicatesError()
        {
            _gameObject = new GameObject();
            _gameObject.transform.SetParent(this.transform, false);

            var gameObject2 = new GameObject();
            gameObject2.transform.SetParent(this.transform, false);

            Binder.BindInstance(this);
            Binder.Bind<FooMono1>().ToSingleMonoBehaviour(_gameObject);
            Assert.Throws<ZenjectBindException>(() => Binder.Bind<IInitializable>().ToSingleMonoBehaviour<FooMono1>(gameObject2));
        }
    }
}
