using System;
using System.Collections.Generic;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public class SingletonProviderCreator
    {
        readonly StandardSingletonProviderCreator _standardProviderCreator;
        readonly SubContainerSingletonProviderCreatorByMethod _subContainerMethodProviderCreator;
        readonly SubContainerSingletonProviderCreatorByInstaller _subContainerInstallerProviderCreator;

#if !ZEN_NOT_UNITY3D
        readonly PrefabSingletonProviderCreator _prefabProviderCreator;
        readonly PrefabResourceSingletonProviderCreator _prefabResourceProviderCreator;
#endif
        public SingletonProviderCreator(
            DiContainer container, SingletonMarkRegistry markRegistry)
        {
            _standardProviderCreator = new StandardSingletonProviderCreator(container, markRegistry);

            _subContainerMethodProviderCreator = new SubContainerSingletonProviderCreatorByMethod(container, markRegistry);
            _subContainerInstallerProviderCreator = new SubContainerSingletonProviderCreatorByInstaller(container, markRegistry);

#if !ZEN_NOT_UNITY3D
            _prefabProviderCreator = new PrefabSingletonProviderCreator(container, markRegistry);
            _prefabResourceProviderCreator = new PrefabResourceSingletonProviderCreator(container, markRegistry);
#endif
        }

        public IProvider CreateProviderStandard(
            StandardSingletonDeclaration dec, Func<Type, IProvider> providerCreator)
        {
            return _standardProviderCreator.GetOrCreateProvider(dec, providerCreator);
        }

        public IProvider CreateProviderForSubContainerMethod(
            Type resultType, string concreteIdentifier,
            Action<DiContainer> installMethod, string identifier)
        {
            return _subContainerMethodProviderCreator.CreateProvider(
                resultType, concreteIdentifier, installMethod, identifier);
        }

        public IProvider CreateProviderForSubContainerInstaller(
            Type resultType, string concreteIdentifier,
            Type installerType, string identifier)
        {
            return _subContainerInstallerProviderCreator.CreateProvider(
                resultType, concreteIdentifier, installerType, identifier);
        }

#if !ZEN_NOT_UNITY3D
        public IProvider CreateProviderForPrefab(
            GameObject prefab, Type resultType, string gameObjectName,
            List<TypeValuePair> extraArguments, string concreteIdentifier)
        {
            return _prefabProviderCreator.CreateProvider(
                prefab, resultType, gameObjectName,
                extraArguments, concreteIdentifier);
        }

        public IProvider CreateProviderForPrefabResource(
            string resourcePath, Type resultType, string gameObjectName,
            List<TypeValuePair> extraArguments, string concreteIdentifier)
        {
            return _prefabResourceProviderCreator.CreateProvider(
                resourcePath, resultType, gameObjectName,
                extraArguments, concreteIdentifier);
        }
#endif
    }
}
