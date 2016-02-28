using System;
using ModestTree;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public interface IUntypedBinder : ITypeBinder
    {
        //  See description in ITypeBinder
        BindingConditionSetter ToSingle<TConcrete>();
        BindingConditionSetter ToSingle<TConcrete>(string concreteIdentifier);

        //  See description in ITypeBinder
        BindingConditionSetter ToInstance<TConcrete>(TConcrete instance);

        //  See description in ITypeBinder
        BindingConditionSetter ToTransient<TConcrete>();

#if !ZEN_NOT_UNITY3D
        //  See description in ITypeBinder
        BindingConditionSetter ToSinglePrefab<TConcrete>(GameObject prefab);
        BindingConditionSetter ToSinglePrefab<TConcrete>(string identifier, GameObject prefab);

        //  See description in ITypeBinder
        BindingConditionSetter ToTransientPrefab<TConcrete>(GameObject prefab);

        //  See description in ITypeBinder
        BindingConditionSetter ToSinglePrefabResource<TConcrete>(string resourcePath);
        BindingConditionSetter ToSinglePrefabResource<TConcrete>(
            string identifier, string resourcePath);

        //  See description in ITypeBinder
        BindingConditionSetter ToTransientPrefabResource<TConcrete>(string resourcePath);

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleGameObject<TConcrete>()
            where TConcrete : Component;
        BindingConditionSetter ToSingleGameObject<TConcrete>(string concreteIdentifier)
            where TConcrete : Component;

        //  See description in ITypeBinder
        BindingConditionSetter ToTransientGameObject<TConcrete>()
            where TConcrete : Component;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleMonoBehaviour<TConcrete>(GameObject gameObject);
        BindingConditionSetter ToSingleMonoBehaviour<TConcrete>(string concreteIdentifier, GameObject gameObject);

        //  See description in ITypeBinder
        BindingConditionSetter ToResource<TConcrete>(string resourcePath);
#endif

        //  See description in ITypeBinder
        BindingConditionSetter ToMethod(Type returnType, Func<InjectContext, object> method);
        BindingConditionSetter ToMethod<TContract>(Func<InjectContext, TContract> method);

        //  See description in ITypeBinder
        BindingConditionSetter ToResolve<TConcrete>();
        BindingConditionSetter ToResolve<TConcrete>(string identifier);
        BindingConditionSetter ToResolve<TConcrete>(string identifier, ContainerTypes containerType);

        //  See description in ITypeBinder
        BindingConditionSetter ToGetter<TObj, TContract>(Func<TObj, TContract> method);
        BindingConditionSetter ToGetter<TObj, TContract>(
            string identifier, Func<TObj, TContract> method);
        BindingConditionSetter ToGetter<TObj, TContract>(
            string identifier, Func<TObj, TContract> method, ContainerTypes containerType);

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleInstance<TConcrete>(TConcrete instance);
        BindingConditionSetter ToSingleInstance<TConcrete>(
            string concreteIdentifier, TConcrete instance);

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleMethod<TConcrete>(
            string concreteIdentifier, Func<InjectContext, TConcrete> method);
        BindingConditionSetter ToSingleMethod<TConcrete>(Func<InjectContext, TConcrete> method);

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleFacadeMethod<TConcrete>(Action<DiContainer> installerFunc);
        BindingConditionSetter ToSingleFacadeMethod<TConcrete>(
            string concreteIdentifier, Action<DiContainer> installerFunc);

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleFacadeInstaller<TConcrete, TInstaller>()
            where TInstaller : Installer;

        //  See description in ITypeBinder
        BindingConditionSetter ToSingleFacadeInstaller<TConcrete, TInstaller>(
            string concreteIdentifier)
            where TInstaller : Installer;
    }
}



