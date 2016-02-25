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
        readonly Type _concreteType;
        readonly GameObject _prefab;

        public GameObjectTransientProviderFromPrefab(
            Type concreteType, GameObject prefab, DiContainer container)
        {
            // Don't do this because it might be an interface
            //Assert.That(typeof(T).DerivesFrom<Component>());

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

            var rootGameObject = context.Container.InstantiatePrefab(_prefab);

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
            return ZenValidator.ValidatePrefab(
                context.Container, _prefab, _concreteType, context);
        }
    }
}

#endif
