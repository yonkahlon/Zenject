using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class GameObjectFactory<TContract> : IFactory<TContract> where TContract : Component
    {
        DiContainer _container;
        GameObject _template;
        GameObjectInstantiator _instantiator;

        public GameObjectFactory(DiContainer container, GameObject template)
        {
            _template = template;
            _container = container;
            _instantiator = _container.Resolve<GameObjectInstantiator>();
        }

        public TContract Create(params object[] constructorArgs)
        {
            var gameObj = _instantiator.Instantiate(_template, constructorArgs);

            var component = gameObj.GetComponentInChildren<TContract>();

            if (component == null)
            {
                throw new ZenjectResolveException(
                    "Could not find component '" + typeof(TContract).GetPrettyName() + "' when creating game object from prefab");
            }

            return component;
        }
    }
}
