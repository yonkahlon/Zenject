﻿using ModestTree;
using UnityEngine;
using Zenject.TestFramework;
using Zenject;

namespace Zenject.Tests.TestPrefabFactory
{
    public class Fixture : ZenjectIntegrationTestFixture
    {
        public GameObject FooPrefab;

        [Test]
        public void Test1()
        {
            Container.Bind<FooFactory>().ToSelf().AsSingle();
            Container.Bind<IInitializable>().To<Runner>().AsSingle().WithArguments(FooPrefab);
        }

        public class FooFactory : PrefabFactory<Foo>
        {
        }

        public class Runner : IInitializable
        {
            readonly GameObject _prefab;
            readonly FooFactory _fooFactory;

            public Runner(
                FooFactory fooFactory,
                GameObject prefab)
            {
                _prefab = prefab;
                _fooFactory = fooFactory;
            }

            public void Initialize()
            {
                var foo = _fooFactory.Create(_prefab);

                Assert.That(foo.WasInitialized);
            }
        }
    }
}
