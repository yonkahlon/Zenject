#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class GameObjectTransientProviderFromPrefabResource : ProviderBase
    {
        readonly ContainerTypes _containerType;
        readonly string _resourcePath;
        readonly Type _concreteType;
        readonly DiContainer _container;

        public GameObjectTransientProviderFromPrefabResource(
            Type concreteType, string resourcePath,
            DiContainer container,
            ContainerTypes containerType)
        {
            _containerType = containerType;
            // Don't do this because it might be an interface
            //Assert.That(_concreteType.DerivesFrom<Component>());

            _container = container;
            _concreteType = concreteType;
            _resourcePath = resourcePath;
        }

        public override Type GetInstanceType()
        {
            return _concreteType;
        }

        public override object GetInstance(InjectContext context)
        {
            Assert.That(_concreteType.DerivesFromOrEqual(context.MemberType));

            var container = _containerType == ContainerTypes.RuntimeContainer ? context.Container : _container;
            var rootGameObject = container.InstantiatePrefabResource(_resourcePath);

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
            var prefab = (GameObject)Resources.Load(_resourcePath);

            if (prefab == null)
            {
                yield return new ZenjectResolveException(
                    "Could not find prefab at given resource path '{0}'. \nObject graph:\n{1}"
                    .Fmt(_resourcePath, context.GetObjectGraphString()));
                yield break;
            }

            var container = _containerType == ContainerTypes.RuntimeContainer ? context.Container : _container;

            foreach (var error in ZenValidator.ValidatePrefab(
                container, prefab, _concreteType, context))
            {
                yield return error;
            }
        }
    }
}

#endif

