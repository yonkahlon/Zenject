#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class PrefabConcreteBindingFinalizer : ConcreteBindingFinalizerBase
    {
        readonly GameObject _prefab;
        readonly string _gameObjectName;

        public PrefabConcreteBindingFinalizer(
            GameObject prefab, List<Type> concreteTypes, string gameObjectName)
            : base(concreteTypes)
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
                    RegisterProvidersForAllContractsPerConcreteType(
                        (concreteType) => Container.SingletonProviderCreator.CreateProviderForPrefab(
                            _prefab,
                            concreteType,
                            _gameObjectName,
                            Binding.Arguments,
                            Binding.ConcreteIdentifier));
                    break;
                }
                case CreationTypes.Transient:
                {
                    RegisterProvidersForAllContractsPerConcreteType(
                        (concreteType) =>
                            new GetFromPrefabComponentProvider(
                                concreteType,
                                new PrefabInstantiator(
                                    Container, _gameObjectName, Binding.Arguments, _prefab)));
                    break;
                }
                case CreationTypes.Cached:
                {
                    var prefabCreator = new PrefabInstantiatorCached(
                        new PrefabInstantiator(
                            Container, _gameObjectName, Binding.Arguments, _prefab));

                    RegisterProvidersForAllContractsPerConcreteType(
                        (concreteType) => new CachedProvider(
                            new GetFromPrefabComponentProvider(concreteType, prefabCreator)));
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
