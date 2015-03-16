using System;
using UnityEngine;
using ModestTree;

namespace Zenject
{
    public class PrefabSingletonLazyCreator
    {
        int _referenceCount;
        GameObject _prefab;
        PrefabSingletonProviderMap _owner;
        DiContainer _container;
        GameObjectInstantiator _gameObjectInstantiator;
        PrefabSingletonId _id;

        public PrefabSingletonLazyCreator(
            DiContainer container, PrefabSingletonProviderMap owner,
            PrefabSingletonId id)
        {
            _container = container;
            _owner = owner;
            _id = id;
        }

        public GameObject Prefab
        {
            get
            {
                return _prefab;
            }
        }

        public GameObjectInstantiator GameObjectInstantiator
        {
            get
            {
                return _gameObjectInstantiator ?? (_gameObjectInstantiator = _container.Resolve<GameObjectInstantiator>());
            }
        }

        public void IncRefCount()
        {
            _referenceCount += 1;
        }

        public void DecRefCount()
        {
            _referenceCount -= 1;

            if (_referenceCount <= 0)
            {
                _owner.RemoveCreator(_id);
            }
        }

        public bool ContainsComponent(Type type)
        {
            return _prefab.GetComponentInChildren(type) != null;
        }

        public T GetComponent<T>()
        {
            return (T)GetComponent(typeof(T));
        }

        public object GetComponent(Type componentType)
        {
            if (_prefab == null)
            {
                _prefab = GameObjectInstantiator.Instantiate(_id.Prefab);

                if (_prefab == null)
                {
                    throw new ZenjectResolveException(
                        "Unable to instantiate prefab in PrefabSingletonLazyCreator");
                }
            }

            var component = _prefab.GetComponentInChildren(componentType);

            if (component == null)
            {
                throw new ZenjectResolveException(
                    "Could not find component with type '{0}' in given singleton prefab".Fmt(componentType));
            }

            return component;
        }
    }
}

