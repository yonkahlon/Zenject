using System;
using System.Collections.Generic;
using ModestTree;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif


namespace Zenject
{
    public class TypeBinderGeneric<TContract> : TypeBinderBase
    {
        public TypeBinderGeneric(
            string identifier, DiContainer container)
            : base(new List<Type>() { typeof(TContract) }, identifier, container)
        {
        }

        public ScopeBinder To<TConcrete>()
            where TConcrete : TContract
        {
            return To(typeof(TConcrete));
        }

        public ScopeBinder ToResolve<TConcrete>()
            where TConcrete : TContract
        {
            return ToResolve(typeof(TConcrete), null);
        }

        public ScopeBinder ToResolve<TConcrete>(string identifier)
            where TConcrete : TContract
        {
            return ToResolve(typeof(TConcrete), identifier);
        }

        public ScopeBinder ToMethodSelf(Func<InjectContext, TContract> method)
        {
            return ToMethodBase<TContract>(method);
        }

        public ScopeBinder ToMethod<TConcrete>(Func<InjectContext, TConcrete> method)
            where TConcrete : TContract
        {
            return ToMethodBase<TConcrete>(method);
        }

        public ScopeBinder ToGetterSelf<TObj>(Func<TObj, TContract> method)
        {
            return ToGetterBase<TObj, TContract>(null, method);
        }

        public ScopeBinder ToGetterSelf<TObj>(string identifier, Func<TObj, TContract> method)
        {
            return ToGetterBase<TObj, TContract>(identifier, method);
        }

        public ScopeBinder ToGetter<TConcrete, TObj>(Func<TObj, TConcrete> method)
            where TConcrete : TContract
        {
            return ToGetterBase<TObj, TConcrete>(null, method);
        }

        public ScopeBinder ToGetter<TConcrete, TObj>(string identifier, Func<TObj, TConcrete> method)
            where TConcrete : TContract
        {
            return ToGetterBase<TObj, TConcrete>(identifier, method);
        }

        public ScopeBinder ToInstance<TConcrete>(TConcrete instance)
            where TConcrete : TContract
        {
            return ToInstance(
                instance == null ? typeof(TConcrete) : instance.GetType(), instance);
        }

        public ScopeBinder ToFactorySelf<TFactory>()
            where TFactory : IFactory<TContract>
        {
            return ToFactoryBase<TContract, TFactory>();
        }

        public ScopeBinder ToFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TConcrete>
            where TConcrete : TContract
        {
            return ToFactoryBase<TConcrete, TFactory>();
        }

#if !ZEN_NOT_UNITY3D

        public ScopeBinder ToComponent<TConcrete>(GameObject gameObject)
            where TConcrete : TContract
        {
            return ToComponent(typeof(TConcrete), gameObject);
        }

        public ScopeBinder ToComponent<TConcrete>(Func<InjectContext, GameObject> gameObjectGetter)
            where TConcrete : TContract
        {
            return ToComponent(typeof(TConcrete), gameObjectGetter);
        }

        public ScopeBinder ToResource<TConcrete>(string resourcePath)
            where TConcrete : TContract
        {
            return ToResource(typeof(TConcrete), resourcePath);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public ScopeBinder ToPrefabResource<TConcrete>(string resourcePath)
            where TConcrete : TContract
        {
            return ToPrefabResource(typeof(TConcrete), resourcePath);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public ScopeBinder ToPrefabResource<TConcrete>(string resourcePath, string gameObjectName)
            where TConcrete : TContract
        {
            return ToPrefabResource(typeof(TConcrete), resourcePath, gameObjectName);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public ScopeBinder ToPrefab<TConcrete>(GameObject prefab)
            where TConcrete : TContract
        {
            return ToPrefab(typeof(TConcrete), prefab);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public ScopeBinder ToPrefab<TConcrete>(GameObject prefab, string gameObjectName)
            where TConcrete : TContract
        {
            return ToPrefab(typeof(TConcrete), prefab, gameObjectName);
        }

        // Creates a new game object and adds the given type as a new component on it
        public ScopeBinder ToGameObject<TConcrete>()
            where TConcrete : TContract
        {
            return ToGameObject(typeof(TConcrete));
        }

        // Creates a new game object and adds the given type as a new component on it
        public ScopeBinder ToGameObject<TConcrete>(string gameObjectName)
            where TConcrete : TContract
        {
            return ToGameObject(typeof(TConcrete), gameObjectName);
        }

#endif
        // ToSubContainer (method)

        public ScopeBinder ToSubContainer<TConcrete>(Action<DiContainer> installerFunc)
            where TConcrete : TContract
        {
            return ToSubContainer<TConcrete>(installerFunc, null);
        }

        public ScopeBinder ToSubContainer<TConcrete>(
            Action<DiContainer> installerFunc, string identifier)
            where TConcrete : TContract
        {
            return ToSubContainer(typeof(TConcrete), installerFunc, identifier);
        }

        // ToSubContainer (installer)

        public ScopeBinder ToSubContainer<TConcrete, TInstaller>()
            where TInstaller : Installer
            where TConcrete : TContract
        {
            return ToSubContainer<TConcrete, TInstaller>(null);
        }

        public ScopeBinder ToSubContainer<TConcrete, TInstaller>(string identifier)
            where TInstaller : Installer
            where TConcrete : TContract
        {
            return ToSubContainer(
                typeof(TConcrete), typeof(TInstaller), identifier);
        }
    }
}
