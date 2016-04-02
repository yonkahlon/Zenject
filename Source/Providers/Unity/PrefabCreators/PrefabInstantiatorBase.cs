#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public abstract class PrefabInstantiatorBase : IPrefabInstantiator
    {
        readonly DiContainer _container;
        readonly string _gameObjectName;
        readonly List<TypeValuePair> _extraArguments;
        readonly List<Type> _extraArgumentTypes;

        public PrefabInstantiatorBase(
            DiContainer container,
            string gameObjectName,
            List<TypeValuePair> extraArguments)
        {
            _extraArguments = extraArguments;
            _container = container;
            _gameObjectName = gameObjectName;

            // Cache this since _extraArguments changes
            _extraArgumentTypes = extraArguments.Select(x => x.Type).ToList();
        }

        public List<TypeValuePair> ExtraArguments
        {
            get
            {
                return _extraArguments;
            }
        }

        public string GameObjectName
        {
            get
            {
                return _gameObjectName;
            }
        }

        public abstract GameObject GetPrefab();

        public virtual IEnumerable<ZenjectException> Validate(List<Type> args)
        {
            return PrefabValidator.ValidatePrefab(
                _container, GetPrefab(),
                _extraArgumentTypes.Concat(args).ToList(), true);
        }

        public IEnumerator<GameObject> Instantiate(List<TypeValuePair> args)
        {
            var gameObject = _container.CreateAndParentPrefab(GetPrefab(), null);
            Assert.IsNotNull(gameObject);

            if (_gameObjectName != null)
            {
                gameObject.name = _gameObjectName;
            }

            // Return it before inject so we can do circular dependencies
            yield return gameObject;

            _container.InjectGameObjectExplicit(
                gameObject, true, _container.IncludeInactiveDefault,
                _extraArguments.Concat(args).ToList());
        }
    }
}

#endif
