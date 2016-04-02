#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public class PrefabResourceConcreteBindingFinalizer : ConcreteBindingFinalizerBase
    {
        readonly string _resourcePath;
        readonly string _gameObjectName;

        public PrefabResourceConcreteBindingFinalizer(
            string resourcePath, List<Type> concreteTypes, string gameObjectName)
            : base(concreteTypes)
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
                    RegisterProvidersForAllContractsPerConcreteType(
                        (concreteType) => Container.SingletonProviderCreator.CreateProviderForPrefabResource(
                            _resourcePath,
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
                                new PrefabInstantiatorResource(
                                    Container, _gameObjectName, Binding.Arguments, _resourcePath)));
                    break;
                }
                case CreationTypes.Cached:
                {
                    var prefabCreator = new PrefabInstantiatorCached(
                        new PrefabInstantiatorResource(
                            Container, _gameObjectName, Binding.Arguments, _resourcePath));

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
