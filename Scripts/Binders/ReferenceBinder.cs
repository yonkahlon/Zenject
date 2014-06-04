using System;
using UnityEngine;
using Fasterflect;

namespace ModestTree.Zenject
{
    public class ReferenceBinder<TContract> : BinderGeneric<TContract> where TContract : class
    {
        readonly protected SingletonProviderMap _singletonMap;

        public ReferenceBinder(DiContainer container, SingletonProviderMap singletonMap)
            : base(container)
        {
            _singletonMap = singletonMap;
        }

        public BindingConditionSetter ToTransient()
        {
            return ToProvider(new TransientProvider(_container, typeof(TContract)));
        }

        public BindingConditionSetter ToTransient<TConcrete>() where TConcrete : TContract
        {
            return ToProvider(new TransientProvider(_container, typeof(TConcrete)));
        }

        public BindingConditionSetter ToSingle()
        {
            //Assert.That(!_contractType.IsSubclassOf(typeof(MonoBehaviour)),
            //    "Should not use ToSingle for Monobehaviours (when binding type " + _contractType.Name() + "), you probably want ToSingleFromPrefab or ToSingleGameObject");

            return ToProvider(_singletonMap.CreateProvider<TContract>());
        }

        public BindingConditionSetter ToSingle<TConcrete>() where TConcrete : TContract
        {
            //Assert.That(!typeof(TConcrete).IsSubclassOf(typeof(MonoBehaviour)),
            //    "Should not use ToSingle for Monobehaviours (when binding type " + _contractType.Name() + "), you probably want ToSingleFromPrefab or ToSingleGameObject");

            return ToProvider(_singletonMap.CreateProvider<TConcrete>());
        }

        public BindingConditionSetter ToSingle(Type concreteType)
        {
            if (!concreteType.DerivesFromOrEqual(_contractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'",
                    concreteType.Name(), _contractType.Name());
            }

            return ToProvider(_singletonMap.CreateProvider(concreteType));
        }

        public BindingConditionSetter To<TConcrete>(TConcrete instance) where TConcrete : TContract
        {
            if (UnityUtil.IsNull(instance) && !_container.AllowNullBindings)
            {
                string message;

                if (_contractType == typeof(TConcrete))
                {
                    message = string.Format("Received null instance during Bind command with type '{0}'", _contractType.Name());
                }
                else
                {
                    message = string.Format(
                        "Received null instance during Bind command when binding type '{0}' to '{1}'", _contractType.Name(), typeof(TConcrete).Name());
                }

                throw new ZenjectBindException(message);
            }

            return ToProvider(new InstanceProvider(typeof(TConcrete), instance));
        }

        public BindingConditionSetter ToSingle<TConcrete>(TConcrete instance) where TConcrete : TContract
        {
            if (UnityUtil.IsNull(instance) && !_container.AllowNullBindings)
            {
                string message;

                if (_contractType == typeof(TConcrete))
                {
                    message = string.Format("Received null singleton instance during Bind command with type '{0}'", _contractType.Name());
                }
                else
                {
                    message = string.Format(
                        "Received null singleton instance during Bind command when binding type '{0}' to '{1}'", _contractType.Name(), typeof(TConcrete).Name());
                }

                throw new ZenjectBindException(message);
            }

            return ToProvider(_singletonMap.CreateProvider(instance));
        }

        // we can't have this method because of the necessary where() below, so in this case they have to specify TContract twice
        //public BindingConditionSetter ToSingle(GameObject prefab)

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToSingleFromPrefab<TConcrete>(GameObject prefab) where TConcrete : Component, TContract
        {
            // We have to cast to object otherwise we get SecurityExceptions when this function is run outside of unity
            if (UnityUtil.IsNull(prefab) && !_container.AllowNullBindings)
            {
                throw new ZenjectBindException("Received null prefab while binding type '{0}'", typeof(TConcrete).Name());
            }

            return ToProvider(new GameObjectSingletonProviderFromPrefab<TConcrete>(_container, prefab));
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToTransientFromPrefab<TConcrete>(GameObject prefab) where TConcrete : Component, TContract
        {
            // We have to cast to object otherwise we get SecurityExceptions when this function is run outside of unity
            if (UnityUtil.IsNull(prefab) && !_container.AllowNullBindings)
            {
                throw new ZenjectBindException("Received null prefab while binding type '{0}'", typeof(TConcrete).Name());
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
                throw new ZenjectBindException("Expected MonoBehaviour derived type when binding type '{0}'", _contractType.Name());
            }

            return ToProvider(new GameObjectSingletonProvider<TContract>(_container, name));
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject<TConcrete>(string name) where TConcrete : MonoBehaviour, TContract
        {
            if (!_contractType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException("Expected MonoBehaviour derived type when binding type '{0}'", _contractType.Name());
            }

            return ToProvider(new GameObjectSingletonProvider<TConcrete>(_container, name));
        }
    }
}
