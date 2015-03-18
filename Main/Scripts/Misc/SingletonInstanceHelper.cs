using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public class SingletonInstanceHelper
    {
        readonly PrefabSingletonProviderMap _prefabSingletonProviderMap;
        readonly SingletonProviderMap _singletonProviderMap;

        public SingletonInstanceHelper(
            SingletonProviderMap singletonProviderMap,
            PrefabSingletonProviderMap prefabSingletonProviderMap)
        {
            _prefabSingletonProviderMap = prefabSingletonProviderMap;
            _singletonProviderMap = singletonProviderMap;
        }

        public IEnumerable<Type> GetSingletonInstances<T>(
            IEnumerable<Type> ignoreTypes)
        {
            var unboundTypes = _singletonProviderMap.Creators
                .Select(x => x.GetInstanceType())
                .Where(x => x.DerivesFrom<T>())
                .Distinct()
                .Where(x => !ignoreTypes.Contains(x));

            var unboundPrefabTypes = _prefabSingletonProviderMap.Creators
                .SelectMany(x => x.GetAllComponentTypes())
                .Where(x => x.DerivesFrom<T>())
                .Distinct()
                .Where(x => !ignoreTypes.Contains(x));

            return unboundTypes.Concat(unboundPrefabTypes);
        }
    }
}
