using System;
using UnityEngine;
using ModestTree;

namespace Zenject
{
    public class BinderUntyped : Binder
    {
        readonly protected SingletonProviderMap _singletonMap;
        readonly protected PrefabSingletonProviderMap _prefabSingletonMap;

        public BinderUntyped(
            DiContainer container, Type contractType,
            string identifier, SingletonProviderMap singletonMap, PrefabSingletonProviderMap prefabSingletonMap)
            : base(container, contractType, identifier)
        {
            _singletonMap = singletonMap;
            _prefabSingletonMap = prefabSingletonMap;
        }

        public BindingConditionSetter ToTransient(Type concreteType)
        {
            if (_contractType.DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Should not use ToTransient for Monobehaviours (when binding type '{0}'), you probably want either ToLookup or ToTransientFromPrefab"
                    .Fmt(_contractType.Name()));
            }

            return ToProvider(new TransientProvider(_container, concreteType));
        }

        public BindingConditionSetter ToTransient()
        {
            if (_contractType.DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Should not use ToTransient for Monobehaviours (when binding type '{0}'), you probably want either ToLookup or ToTransientFromPrefab"
                    .Fmt(_contractType.Name()));
            }

            return ToProvider(new TransientProvider(_container, _contractType));
        }

        public BindingConditionSetter ToSingle()
        {
            return ToSingle((string)null);
        }

        public BindingConditionSetter ToSingle(string identifier)
        {
            if (_contractType.DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Should not use ToSingle for Monobehaviours (when binding type '{0}'), you probably want either ToLookup or ToSingleFromPrefab or ToSingleGameObject"
                    .Fmt(_contractType.Name()));
            }

            return ToProvider(_singletonMap.CreateProviderFromType(identifier, _contractType));
        }

        public BindingConditionSetter ToSingle(Type concreteType)
        {
            return ToSingle(concreteType, null);
        }

        public BindingConditionSetter ToSingle(Type concreteType, string identifier)
        {
            if (!concreteType.DerivesFromOrEqual(_contractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), _contractType.Name()));
            }

            return ToProvider(_singletonMap.CreateProviderFromType(identifier, concreteType));
        }

        public BindingConditionSetter To(object instance)
        {
            var concreteType = instance.GetType();

            if (!concreteType.DerivesFromOrEqual(_contractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), _contractType.Name()));
            }

            if (UnityUtil.IsNull(instance) && !_container.AllowNullBindings)
            {
                string message;

                if (_contractType == concreteType)
                {
                    message = "Received null instance during Bind command with type '{0}'".Fmt(_contractType.Name());
                }
                else
                {
                    message =
                        "Received null instance during Bind command when binding type '{0}' to '{1}'".Fmt(_contractType.Name(), concreteType.Name());
                }

                throw new ZenjectBindException(message);
            }

            return ToProvider(new InstanceProvider(concreteType, instance));
        }

        public BindingConditionSetter ToSingleInstance<TConcrete>(TConcrete instance)
        {
            return ToSingleInstance<TConcrete>(instance, null);
        }

        public BindingConditionSetter ToSingleInstance<TConcrete>(TConcrete instance, string identifier)
        {
            var concreteType = typeof(TConcrete);

            if (!concreteType.DerivesFromOrEqual(_contractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), _contractType.Name()));
            }

            if (UnityUtil.IsNull(instance) && !_container.AllowNullBindings)
            {
                string message;

                if (_contractType == concreteType)
                {
                    message = "Received null singleton instance during Bind command with type '{0}'".Fmt(_contractType.Name());
                }
                else
                {
                    message =
                        "Received null singleton instance during Bind command when binding type '{0}' to '{1}'".Fmt(_contractType.Name(), concreteType.Name());
                }

                throw new ZenjectBindException(message);
            }

            return ToProvider(_singletonMap.CreateProviderFromInstance(identifier, instance));
        }

        public BindingConditionSetter ToSingle<TConcrete>()
        {
            return ToSingle<TConcrete>(null);
        }

        public BindingConditionSetter ToSingle<TConcrete>(string identifier)
        {
            var concreteType = typeof(TConcrete);

            if (!concreteType.DerivesFromOrEqual(_contractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), _contractType.Name()));
            }

            if (concreteType.DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Should not use ToSingle for Monobehaviours (when binding type '{0}' to '{1}'), you probably want either ToLookup or ToSingleFromPrefab or ToSingleGameObject"
                    .Fmt(_contractType.Name(), concreteType.Name()));
            }

            return ToProvider(_singletonMap.CreateProviderFromType(identifier, typeof(TConcrete)));
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToSingleFromPrefab<TConcrete>(GameObject prefab)
            where TConcrete : Component
        {
            return ToSingleFromPrefab<TConcrete>(null, prefab);
        }

        public BindingConditionSetter ToSingleFromPrefab<TConcrete>(string identifier, GameObject prefab)
            where TConcrete : Component
        {
            var concreteType = typeof(TConcrete);

            if (!concreteType.DerivesFromOrEqual(_contractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), _contractType.Name()));
            }

            // We have to cast to object otherwise we get SecurityExceptions when this function is run outside of unity
            if (UnityUtil.IsNull(prefab) && !_container.AllowNullBindings)
            {
                throw new ZenjectBindException("Received null prefab while binding type '{0}'".Fmt(concreteType.Name()));
            }

            return ToProvider(_prefabSingletonMap.CreateProvider(identifier, typeof(TConcrete), prefab));
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToTransientFromPrefab<TConcrete>(GameObject prefab) where TConcrete : Component
        {
            var concreteType = typeof(TConcrete);

            if (!concreteType.DerivesFromOrEqual(_contractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), _contractType.Name()));
            }

            // We have to cast to object otherwise we get SecurityExceptions when this function is run outside of unity
            if (UnityUtil.IsNull(prefab) && !_container.AllowNullBindings)
            {
                throw new ZenjectBindException("Received null prefab while binding type '{0}'".Fmt(concreteType.Name()));
            }

            return ToProvider(new GameObjectTransientProviderFromPrefab<TConcrete>(_container, prefab));
        }

        public BindingConditionSetter ToSingleGameObject()
        {
            return ToSingleGameObject(_contractType.Name());
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject(string name)
        {
            if (!_contractType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException("Expected MonoBehaviour derived type when binding type '{0}'".Fmt(_contractType.Name()));
            }

            return ToProvider(new GameObjectSingletonProvider(_contractType, _container, name));
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject<TConcrete>(string name) where TConcrete : MonoBehaviour
        {
            var concreteType = typeof(TConcrete);

            if (!concreteType.DerivesFromOrEqual(_contractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), _contractType.Name()));
            }

            return ToProvider(new GameObjectSingletonProvider(typeof(TConcrete), _container, name));
        }

        public BindingConditionSetter ToSingleMethod<TConcrete>(Func<DiContainer, TConcrete> method)
        {
            return ToSingleMethod<TConcrete>(method, null);
        }

        public BindingConditionSetter ToSingleMethod<TConcrete>(Func<DiContainer, TConcrete> method, string identifier)
        {
            return ToProvider(_singletonMap.CreateProviderFromMethod(identifier, method));
        }
    }
}
