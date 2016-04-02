#if !ZEN_NOT_UNITY3D

using System;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class PrefabSelfBindingFinalizer : ProviderBindingFinalizer
    {
        readonly GameObject _prefab;
        readonly string _gameObjectName;

        public PrefabSelfBindingFinalizer(GameObject prefab, string gameObjectName)
        {
            _prefab = prefab;
            _gameObjectName = gameObjectName;
        }

        public override void FinalizeBinding()
        {
            switch (Binding.CreationType)
            {
                case CreationTypes.Singleton:
                {
                    RegisterProviderPerContract(
                        (contractType) => Container.SingletonProviderCreator.CreateProviderForPrefab(
                            _prefab,
                            contractType,
                            _gameObjectName,
                            Binding.Arguments,
                            Binding.ConcreteIdentifier));
                    break;
                }
                case CreationTypes.Transient:
                {
                    RegisterProviderPerContract(
                        (contractType) =>
                            new GetFromPrefabComponentProvider(contractType,
                                new PrefabInstantiator(
                                    Container, _gameObjectName, Binding.Arguments, _prefab)));
                    break;
                }
                case CreationTypes.Cached:
                {
                    var prefabCreator = new PrefabInstantiatorCached(
                        new PrefabInstantiator(
                            Container, _gameObjectName, Binding.Arguments, _prefab));

                    RegisterProviderPerContract(
                        (contractType) =>
                            new CachedProvider(
                                new GetFromPrefabComponentProvider(contractType, prefabCreator)));
                    break;
                }
                default:
                {
                    throw Assert.CreateException();
                }
            }
        }
    }
}

#endif
