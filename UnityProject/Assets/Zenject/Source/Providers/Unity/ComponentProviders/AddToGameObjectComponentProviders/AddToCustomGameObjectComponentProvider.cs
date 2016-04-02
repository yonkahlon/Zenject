#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Zenject
{
    public class AddToCustomGameObjectComponentProvider : AddToGameObjectComponentProviderBase
    {
        readonly Func<InjectContext, GameObject> _gameObjectGetter;

        public AddToCustomGameObjectComponentProvider(
            Func<InjectContext, GameObject> gameObjectGetter, DiContainer container, Type componentType,
            string concreteIdentifier, List<TypeValuePair> extraArguments)
            : base(container, componentType, concreteIdentifier, extraArguments)
        {
            _gameObjectGetter = gameObjectGetter;
        }

        protected override GameObject GetGameObject(InjectContext context)
        {
            var gameObject = _gameObjectGetter(context);
            Assert.IsNotNull(gameObject,
                "Could not find game object when attempting to create Component");
            return gameObject;
        }
    }
}

#endif
