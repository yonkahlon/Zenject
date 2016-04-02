#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class PrefabInstantiator : PrefabInstantiatorBase
    {
        readonly GameObject _prefab;

        public PrefabInstantiator(
            DiContainer container,
            string gameObjectName,
            List<TypeValuePair> extraArguments,
            GameObject prefab)
            : base(container, gameObjectName, extraArguments)
        {
            Assert.IsNotNull(prefab);
            _prefab = prefab;
        }

        public override GameObject GetPrefab()
        {
            return _prefab;
        }
    }
}

#endif
