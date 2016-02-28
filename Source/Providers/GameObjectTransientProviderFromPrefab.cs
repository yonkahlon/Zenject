#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class GameObjectTransientProviderFromPrefab : ProviderBase
    {
        readonly ContainerTypes _containerType;
        readonly Type _concreteType;
        readonly GameObject _prefab;
        readonly DiContainer _container;

        public GameObjectTransientProviderFromPrefab(
            Type concreteType, GameObject prefab, DiContainer container,
            ContainerTypes containerType)
        {
            _containerType = containerType;
            // Don't do this because it might be an interface
            //Assert.That(typeof(T).DerivesFrom<Component>());

            _container = container;
            _concreteType = concreteType;
            _prefab = prefab;

            var singletonMark = container.SingletonRegistry.TryGetSingletonType(concreteType);

            if (singletonMark.HasValue)
            {
                throw new ZenjectBindException(
                    "Attempted to use 'ToTransientPrefab' with the same type ('{0}') that is already marked with '{1}'".Fmt(concreteType.Name(), singletonMark.Value));
            }
        }

        public override Type GetInstanceType()
        {
            return _concreteType;
        }

        public override object GetInstance(InjectContext context)
        {
            Assert.That(_concreteType.DerivesFromOrEqual(context.MemberType));

            var container = _containerType == ContainerTypes.RuntimeContainer ? context.Container : _container;
            var rootGameObject = container.InstantiatePrefab(_prefab);

            var component = rootGameObject.GetComponentInChildren(_concreteType);

            if (component == null)
            {
                throw new ZenjectResolveException(
                    "Could not find component with type '{0}' in given transient prefab".Fmt(_concreteType));
            }

            return component;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            var container = _containerType == ContainerTypes.RuntimeContainer ? context.Container : _container;

            return ZenValidator.ValidatePrefab(
                container, _prefab, _concreteType, context);
        }
    }
}

#endif
