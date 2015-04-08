#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;
using UnityEngine;

namespace Zenject
{
    public class PrefabSingletonLazyCreator
    {
        int _referenceCount;
        GameObject _rootObj;
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

            Assert.IsNotNull(id.Prefab);
        }

        public GameObject Prefab
        {
            get
            {
                return _id.Prefab;
            }
        }

        public GameObject RootObject
        {
            get
            {
                return _rootObj;
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

        public IEnumerable<Type> GetAllComponentTypes()
        {
            return _id.Prefab.GetComponentsInChildren<Component>(true).Where(x => x != null).Select(x => x.GetType());
        }

        public bool ContainsComponent(Type type)
        {
            return !_id.Prefab.GetComponentsInChildren(type, true).IsEmpty();
        }

        public object GetComponent(Type componentType, InjectContext context)
        {
            if (_rootObj == null)
            {
                _rootObj = GameObjectInstantiator.InstantiateWithContext(_id.Prefab, context);

                if (_rootObj == null)
                {
                    throw new ZenjectResolveException(
                        "Unable to instantiate prefab in PrefabSingletonLazyCreator");
                }
            }

            var component = _rootObj.GetComponentInChildren(componentType);

            if (component == null)
            {
                throw new ZenjectResolveException(
                    "Could not find component with type '{0}' in given singleton prefab".Fmt(componentType));
            }

            return component;
        }
    }
}

#endif
