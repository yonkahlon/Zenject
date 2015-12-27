#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class GameObjectSingletonProvider : ProviderBase
    {
        readonly string _name;
        readonly Type _componentType;

        object _instance;

        public GameObjectSingletonProvider(
            Type componentType, string name)
        {
            Assert.That(componentType.DerivesFrom<Component>());
            _componentType = componentType;
            _name = name;
        }

        public override Type GetInstanceType()
        {
            return _componentType;
        }

        public override object GetInstance(InjectContext context)
        {
            Assert.That(_componentType.DerivesFromOrEqual(context.MemberType));

            if (_instance == null)
            {
                // This is valid sometimes
                //Assert.That(!context.Container.IsValidating,
                    //"Tried to instantiate a MonoBehaviour with type '{0}' during validation. Object graph: {1}", _componentType, DiContainer.GetCurrentObjectGraph());

                // We don't use the generic version here to avoid duplicate generic arguments to binder
                _instance = context.Container.InstantiateComponentOnNewGameObjectExplicit(
                    _componentType, _name, new List<TypeValuePair>(), context);
                Assert.IsNotNull(_instance);
            }

            return _instance;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return context.Container.ValidateObjectGraph(_componentType, context);
        }
    }
}

#endif
