#if !ZEN_NOT_UNITY3D

using System;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class PrefabResourceSelfBindingFinalizer : ProviderBindingFinalizer
    {
        readonly string _resourcePath;
        readonly string _gameObjectName;

        public PrefabResourceSelfBindingFinalizer(string resourcePath, string gameObjectName)
        {
            _resourcePath = resourcePath;
            _gameObjectName = gameObjectName;
        }

        public override void FinalizeBinding()
        {
            switch (Binding.CreationType)
            {
                case CreationTypes.Singleton:
                {
                    RegisterProviderPerContract(
                        (contractType) => Container.SingletonProviderCreator.CreateProviderForPrefabResource(
                            _resourcePath,
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
                                new PrefabInstantiatorResource(
                                    Container, _gameObjectName, Binding.Arguments, _resourcePath)));
                    break;
                }
                case CreationTypes.Cached:
                {
                    var prefabCreator = new PrefabInstantiatorCached(
                        new PrefabInstantiatorResource(
                            Container, _gameObjectName, Binding.Arguments, _resourcePath));

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

