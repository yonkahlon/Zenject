using System;
using System.Collections.Generic;
using ModestTree;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public class UntypedBinder : TypeBinder, IUntypedBinder
    {
        public UntypedBinder(
            DiContainer container, List<Type> contractTypes, string identifier)
            : base(container, contractTypes, identifier)
        {
        }

        public BindingConditionSetter ToResolve<TConcrete>()
        {
            return ToResolveBase<TConcrete>(null, ContainerTypes.RuntimeContainer);
        }

        public BindingConditionSetter ToResolve<TConcrete>(string identifier)
        {
            return ToResolveBase<TConcrete>(identifier, ContainerTypes.RuntimeContainer);
        }

        public BindingConditionSetter ToResolve<TConcrete>(string identifier, ContainerTypes containerType)
        {
            return ToResolveBase<TConcrete>(identifier, containerType);
        }

        public BindingConditionSetter ToMethod(Type concreteType, Func<InjectContext, object> method)
        {
            AssertIsDerivedFromContracts(concreteType);

            return RegisterSingleProvider(new MethodProviderUntyped(concreteType, method));
        }

        public BindingConditionSetter ToMethod<TContract>(Func<InjectContext, TContract> method)
        {
            return ToMethodBase<TContract>(method);
        }

        public BindingConditionSetter ToGetter<TObj, TContract>(Func<TObj, TContract> method)
        {
            return ToGetterBase<TObj, TContract>(null, method, ContainerTypes.RuntimeContainer);
        }

        public BindingConditionSetter ToGetter<TObj, TContract>(string identifier, Func<TObj, TContract> method)
        {
            return ToGetterBase<TObj, TContract>(identifier, method, ContainerTypes.RuntimeContainer);
        }

        public BindingConditionSetter ToGetter<TObj, TContract>(string identifier, Func<TObj, TContract> method, ContainerTypes containerType)
        {
            return ToGetterBase<TObj, TContract>(identifier, method, containerType);
        }

        public BindingConditionSetter ToTransient<TConcrete>()
        {
            return ToTransient(typeof(TConcrete));
        }

        public BindingConditionSetter ToSingle<TConcrete>(string concreteIdentifier)
        {
            return ToSingle(typeof(TConcrete), concreteIdentifier);
        }

        public BindingConditionSetter ToInstance<TConcrete>(TConcrete instance)
        {
            return ToInstance(instance == null ? typeof(TConcrete) : instance.GetType(), instance);
        }

        public BindingConditionSetter ToSingleInstance<TConcrete>(TConcrete instance)
        {
            return ToSingleInstance(instance == null ? typeof(TConcrete) : instance.GetType(), null, instance);
        }

        public BindingConditionSetter ToSingleInstance<TConcrete>(string concreteIdentifier, TConcrete instance)
        {
            return ToSingleInstance(instance == null ? typeof(TConcrete) : instance.GetType(), concreteIdentifier, instance);
        }

        public BindingConditionSetter ToSingleMethod<TConcrete>(string concreteIdentifier, Func<InjectContext, TConcrete> method)
        {
            return ToSingleMethodBase<TConcrete>(concreteIdentifier, method);
        }

        public BindingConditionSetter ToSingleMethod<TConcrete>(Func<InjectContext, TConcrete> method)
        {
            return ToSingleMethodBase<TConcrete>(null, method);
        }

        public BindingConditionSetter ToSingleFacadeMethod<TConcrete>(string concreteIdentifier, Action<DiContainer> installerFunc)
        {
            return ToSingleFacadeMethod(typeof(TConcrete), concreteIdentifier, installerFunc);
        }

        public BindingConditionSetter ToSingleFacadeMethod<TConcrete>(Action<DiContainer> installerFunc)
        {
            return ToSingleFacadeMethod(typeof(TConcrete), null, installerFunc);
        }

        public BindingConditionSetter ToSingle<TConcrete>()
        {
            return ToSingle(typeof(TConcrete), null);
        }

#if !ZEN_NOT_UNITY3D

        public BindingConditionSetter ToSingleMonoBehaviour<TConcrete>(GameObject gameObject)
        {
            return ToSingleMonoBehaviour<TConcrete>(null, gameObject);
        }

        public BindingConditionSetter ToSingleMonoBehaviour<TConcrete>(string concreteIdentifier, GameObject gameObject)
        {
            return ToSingleMonoBehaviour(concreteIdentifier, typeof(TConcrete), gameObject);
        }

        public BindingConditionSetter ToResource<TConcrete>(string resourcePath)
        {
            return ToResource(typeof(TConcrete), resourcePath);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToTransientPrefabResource<TConcrete>(string resourcePath)
        {
            return ToTransientPrefabResource(typeof(TConcrete), resourcePath);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToTransientPrefab<TConcrete>(GameObject prefab)
        {
            return ToTransientPrefab(typeof(TConcrete), prefab);
        }

        public BindingConditionSetter ToSingleGameObject<TConcrete>()
            where TConcrete : Component
        {
            return ToSingleGameObject(typeof(TConcrete));
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject<TConcrete>(string name)
            where TConcrete : Component
        {
            return ToSingleGameObject(typeof(TConcrete), name);
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToTransientGameObject<TConcrete>()
            where TConcrete : Component
        {
            return ToTransientGameObject(typeof(TConcrete));
        }

        public BindingConditionSetter ToSinglePrefab<TConcrete>(GameObject prefab)
        {
            return ToSinglePrefab(typeof(TConcrete), null, prefab);
        }

        public BindingConditionSetter ToSinglePrefab<TConcrete>(string identifier, GameObject prefab)
        {
            return ToSinglePrefab(typeof(TConcrete), identifier, prefab);
        }

        public BindingConditionSetter ToSinglePrefabResource<TConcrete>(string resourcePath)
        {
            return ToSinglePrefabResource(typeof(TConcrete), null, resourcePath);
        }

        public BindingConditionSetter ToSinglePrefabResource<TConcrete>(string identifier, string resourcePath)
        {
            return ToSinglePrefabResource(typeof(TConcrete), identifier, resourcePath);
        }
#endif
    }
}
