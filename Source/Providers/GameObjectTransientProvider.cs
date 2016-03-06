#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class GameObjectTransientProvider : ProviderBase
    {
        readonly ContainerTypes _containerType;
        readonly Type _componentType;
        readonly DiContainer _container;

        public GameObjectTransientProvider(
            DiContainer container,
            Type componentType,
            ContainerTypes containerType)
        {
            _containerType = containerType;
            Assert.That(componentType.DerivesFrom<Component>());
            _componentType = componentType;
            _container = container;
        }

        public override Type GetInstanceType()
        {
            return _componentType;
        }

        public override object GetInstance(InjectContext context)
        {
            Assert.That(_componentType.DerivesFromOrEqual(context.MemberType));

            var container = _containerType == ContainerTypes.RuntimeContainer ? context.Container : _container;

            if (container.IsValidating)
            {
                // This can happen when injecting into installers
                return null;
            }

            return container.InstantiateComponentOnNewGameObjectExplicit(
                _componentType, _componentType.Name(), new List<TypeValuePair>(), context);
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            var container = _containerType == ContainerTypes.RuntimeContainer ? context.Container : _container;

            return container.ValidateObjectGraph(_componentType, context);
        }
    }
}

#endif

