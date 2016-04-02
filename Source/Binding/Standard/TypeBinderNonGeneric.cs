using System;
using System.Collections.Generic;
using ModestTree;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif


namespace Zenject
{
    public class TypeBinderNonGeneric : TypeBinderBase
    {
        public TypeBinderNonGeneric(
             List<Type> contractTypes, string identifier, DiContainer container)
            : base(contractTypes, identifier, container)
        {
        }

        public ScopeBinder To<TConcrete>()
        {
            return To(typeof(TConcrete));
        }

        public ScopeBinder ToResolve<TConcrete>()
        {
            return ToResolve(typeof(TConcrete), null);
        }

        public ScopeBinder ToResolve<TConcrete>(string identifier)
        {
            return ToResolve(typeof(TConcrete), identifier);
        }

        public ScopeBinder ToMethod<TConcrete>(Func<InjectContext, TConcrete> method)
        {
            return ToMethodBase<TConcrete>(method);
        }

        public ScopeBinder ToFactory<TFactory, TConcrete>()
            where TFactory : IFactory<TConcrete>
        {
            return ToFactoryBase<TConcrete, TFactory>();
        }

        public ScopeBinder ToGetter<TObj, TConcrete>(Func<TObj, TConcrete> method)
        {
            return ToGetter<TObj, TConcrete>(null, method);
        }

        public ScopeBinder ToGetter<TObj, TConcrete>(
            string identifier, Func<TObj, TConcrete> method)
        {
            return ToGetterBase<TObj, TConcrete>(identifier, method);
        }

        public ScopeBinder ToInstance<TConcrete>(TConcrete instance)
        {
            return ToInstance(instance == null ? typeof(TConcrete) : instance.GetType(), instance);
        }

        public ScopeBinder ToSubContainer<TConcrete>(Action<DiContainer> installerFunc)
        {
            return ToSubContainer(typeof(TConcrete), installerFunc);
        }

        public ScopeBinder ToSubContainer<TConcrete, TInstaller>()
            where TInstaller : Installer
        {
            return ToSubContainer(typeof(TConcrete), typeof(TInstaller));
        }

#if !ZEN_NOT_UNITY3D

        public ScopeBinder ToComponent<TConcrete>(GameObject gameObject)
        {
            return ToComponent(typeof(TConcrete), gameObject);
        }

        public ScopeBinder ToComponent<TConcrete>(Func<InjectContext, GameObject> gameObjectGetter)
        {
            return ToComponent(typeof(TConcrete), gameObjectGetter);
        }

        public ScopeBinder ToResource<TConcrete>(string resourcePath)
        {
            return ToResource(typeof(TConcrete), resourcePath);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public ScopeBinder ToPrefabResource<TConcrete>(string resourcePath)
        {
            return ToPrefabResource(typeof(TConcrete), resourcePath);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public ScopeBinder ToPrefabResource<TConcrete>(string resourcePath, string gameObjectName)
        {
            return ToPrefabResource(typeof(TConcrete), resourcePath, gameObjectName);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public ScopeBinder ToPrefab<TConcrete>(GameObject prefab)
        {
            return ToPrefab(typeof(TConcrete), prefab);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public ScopeBinder ToPrefab<TConcrete>(GameObject prefab, string gameObjectName)
        {
            return ToPrefab(typeof(TConcrete), prefab, gameObjectName);
        }

        public ScopeBinder ToGameObject<TConcrete>()
            where TConcrete : Component
        {
            return ToGameObject(typeof(TConcrete));
        }

        // Creates a new game object and adds the given type as a new component on it
        public ScopeBinder ToGameObject<TConcrete>(string gameObjectName)
        {
            return ToGameObject(typeof(TConcrete), gameObjectName);
        }
#endif
    }
}

