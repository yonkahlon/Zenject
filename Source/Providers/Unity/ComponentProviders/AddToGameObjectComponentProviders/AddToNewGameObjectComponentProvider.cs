#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Zenject
{
    public class AddToNewGameObjectComponentProvider : AddToGameObjectComponentProviderBase
    {
        readonly string _gameObjectName;

        public AddToNewGameObjectComponentProvider(
            DiContainer container, Type componentType,
            string concreteIdentifier, List<TypeValuePair> extraArguments, string gameObjectName)
            : base(container, componentType, concreteIdentifier, extraArguments)
        {
            _gameObjectName = gameObjectName;
        }

        protected override GameObject GetGameObject(InjectContext context)
        {
            return Container.CreateEmptyGameObject(
                _gameObjectName ?? ConcreteIdentifier ?? ComponentType.Name());
        }
    }
}

#endif
