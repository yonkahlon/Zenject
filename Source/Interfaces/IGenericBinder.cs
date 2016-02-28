using System;
using ModestTree;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public interface IGenericBinder<TContract> : ITypeBinder
    {
        //  See description in ITypeBinder
        BindingConditionSetter ToSingle<TConcrete>()
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingle<TConcrete>(string concreteIdentifier)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToInstance<TConcrete>(TConcrete instance)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToTransient<TConcrete>()
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToTransient<TConcrete>(ContainerTypes containerType)
            where TConcrete : TContract;

#if !ZEN_NOT_UNITY3D
        //  See description in ITypeBinder
        BindingConditionSetter ToSinglePrefab<TConcrete>(GameObject prefab)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSinglePrefab<TConcrete>(
            string concreteIdentifier, GameObject prefab)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToTransientPrefab<TConcrete>(GameObject prefab)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToTransientPrefab<TConcrete>(GameObject prefab, ContainerTypes containerType)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSinglePrefabResource<TConcrete>(string resourcePath)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSinglePrefabResource<TConcrete>(string concreteIdentifier, string resourcePath)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToTransientPrefabResource<TConcrete>(string resourcePath)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToTransientPrefabResource<TConcrete>(string resourcePath, ContainerTypes containerType)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleGameObject<TConcrete>()
            where TConcrete : Component, TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleGameObject<TConcrete>(string concreteIdentifier)
            where TConcrete : Component, TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToTransientGameObject<TConcrete>()
            where TConcrete : Component, TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToTransientGameObject<TConcrete>(ContainerTypes containerType)
            where TConcrete : Component, TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleMonoBehaviour<TConcrete>(GameObject gameObject)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleMonoBehaviour<TConcrete>(string concreteIdentifier, GameObject gameObject)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToResource<TConcrete>(string resourcePath)
            where TConcrete : TContract;
#endif

        //  See description in ITypeBinder
        BindingConditionSetter ToMethod(Func<InjectContext, TContract> method);

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleMethod<TConcrete>(
            string concreteIdentifier, Func<InjectContext, TConcrete> method)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleMethod<TConcrete>(Func<InjectContext, TConcrete> method)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToGetter<TObj>(Func<TObj, TContract> method);

        //  See description in ITypeBinder
        BindingConditionSetter ToGetter<TObj>(string identifier, Func<TObj, TContract> method);

        //  See description in ITypeBinder
        BindingConditionSetter ToGetter<TObj>(string identifier, Func<TObj, TContract> method, ContainerTypes containerType);

        //  See description in ITypeBinder
        BindingConditionSetter ToResolve<TConcrete>()
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToResolve<TConcrete>(string identifier)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToResolve<TConcrete>(string identifier, ContainerTypes containerType)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleInstance<TConcrete>(TConcrete instance)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleInstance<TConcrete>(
            string concreteIdentifier, TConcrete instance)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleFactory<TFactory>()
            where TFactory : IFactory<TContract>;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleFactory<TFactory>(string concreteIdentifier)
            where TFactory : IFactory<TContract>;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleFactory<TFactory, TConcrete>()
            where TFactory : IFactory<TConcrete>
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleFactory<TFactory, TConcrete>(string concreteIdentifier)
            where TFactory : IFactory<TConcrete>
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleFacadeMethod<TConcrete>(Action<DiContainer> installerFunc)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleFacadeMethod<TConcrete>(
            string concreteIdentifier, Action<DiContainer> installerFunc)
            where TConcrete : TContract;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleFacadeInstaller<TConcrete, TInstaller>()
            where TConcrete : TContract
            where TInstaller : Installer;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleFacadeInstaller<TConcrete, TInstaller>(
            string concreteIdentifier)
            where TConcrete : TContract
            where TInstaller : Installer;
    }
}


