using System;
using System.Collections.Generic;
using ModestTree;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public class StandardSingletonProviderCreator
    {
        readonly SingletonMarkRegistry _markRegistry;
        readonly Dictionary<SingletonId, ProviderInfo> _providerMap = new Dictionary<SingletonId, ProviderInfo>();
        readonly DiContainer _container;

        public StandardSingletonProviderCreator(
            DiContainer container,
            SingletonMarkRegistry markRegistry)
        {
            _markRegistry = markRegistry;
            _container = container;
        }

        public IProvider GetOrCreateProvider(
            StandardSingletonDeclaration dec, Func<Type, IProvider> providerCreator)
        {
            // These ones are actually fine when used with Bind<GameObject>() (see TypeBinderBase.ToPrefabSelf)
            //Assert.IsNotEqual(dec.Type, SingletonTypes.ToPrefab);
            //Assert.IsNotEqual(dec.Type, SingletonTypes.ToPrefabResource);

            Assert.IsNotEqual(dec.Type, SingletonTypes.ToSubContainerInstaller);
            Assert.IsNotEqual(dec.Type, SingletonTypes.ToSubContainerMethod);

            _markRegistry.MarkSingleton(dec.Id, dec.Type);

            ProviderInfo providerInfo;

            if (_providerMap.TryGetValue(dec.Id, out providerInfo))
            {
                if (providerInfo.Type != dec.Type)
                {
                    throw Assert.CreateException(
                        "Cannot use both '{0}' and '{1}' for the same dec.Type/ConcreteIdentifier!", providerInfo.Type, dec.Type);
                }

                if (!object.Equals(providerInfo.SingletonSpecificId, dec.SpecificId))
                {
                    throw Assert.CreateException(
                        "Invalid use of binding '{0}'.  Found ambiguous set of creation properties.", dec.Type);
                }
            }
            else
            {
                providerInfo = new ProviderInfo(
                    dec.Type,
                    new CachedProvider(
                        providerCreator(dec.Id.ConcreteType)), dec.SpecificId);

                _providerMap.Add(dec.Id, providerInfo);
            }

            return providerInfo.Provider;
        }

        public class ProviderInfo
        {
            public ProviderInfo(
                SingletonTypes type, CachedProvider provider, object singletonSpecificId)
            {
                Type = type;
                Provider = provider;
                SingletonSpecificId = singletonSpecificId;
            }

            public object SingletonSpecificId
            {
                get;
                private set;
            }

            public SingletonTypes Type
            {
                get;
                private set;
            }

            public CachedProvider Provider
            {
                get;
                private set;
            }
        }
    }
}
