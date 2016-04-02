#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class PrefabInstantiatorResource : PrefabInstantiatorBase
    {
        readonly string _resourcePath;

        public PrefabInstantiatorResource(
            DiContainer container,
            string gameObjectName,
            List<TypeValuePair> extraArguments,
            string resourcePath)
            : base(container, gameObjectName, extraArguments)
        {
            _resourcePath = resourcePath;
        }

        public override GameObject GetPrefab()
        {
            var prefab = (GameObject)Resources.Load(_resourcePath);

            Assert.IsNotNull(prefab,
                "Expected to find prefab at resource path '{0}'", _resourcePath);

            return prefab;
        }

        public override IEnumerable<ZenjectException> Validate(List<Type> args)
        {
            var prefab = (GameObject)Resources.Load(_resourcePath);

            if (prefab == null)
            {
                yield return new ZenjectException(
                    "Expected to find prefab at resource path '{0}'", _resourcePath);
                yield break;
            }

            foreach (var error in base.Validate(args))
            {
                yield return error;
            }
        }
    }
}

#endif
