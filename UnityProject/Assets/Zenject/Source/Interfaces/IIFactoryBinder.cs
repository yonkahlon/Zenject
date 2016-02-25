using System;
using ModestTree;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    ////////////////////////////// Zero parameters //////////////////////////////

    public interface IIFactoryBinderBase<TContract>
    {
        BindingConditionSetter ToFactory();

        BindingConditionSetter ToFactory<TConcrete>()
            where TConcrete : TContract;

        BindingConditionSetter ToIFactory<TConcrete>()
            where TConcrete : TContract;

#if !ZEN_NOT_UNITY3D
        BindingConditionSetter ToPrefab(GameObject prefab);
#endif
    }

    public interface IIFactoryBinder<TContract> : IIFactoryBinderBase<TContract>
    {
        BindingConditionSetter ToInstance(TContract instance);

        BindingConditionSetter ToMethod(Func<DiContainer, TContract> method);

        BindingConditionSetter ToFactory(Type concreteType);

        BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TContract>;

        BindingConditionSetter ToCustomFactory(Type factoryType);

        BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TConcrete>
            where TConcrete : TContract;

        BindingConditionSetter ToCustomFactory(Type concreteType, Type factoryType);

        BindingConditionSetter ToFacadeFactoryMethod<TFactory>(Action<DiContainer> facadeInstaller)
            where TFactory : IFactory<TContract>, IFacadeFactory;

        BindingConditionSetter ToFacadeFactoryMethod<TConcrete, TFactory>(Action<DiContainer> facadeInstaller)
            where TFactory : IFactory<TConcrete>, IFacadeFactory
            where TConcrete : TContract;

        BindingConditionSetter ToFacadeFactoryInstaller<TFactory, TInstaller>()
            where TFactory : IFactory<TContract>, IFacadeFactory
            where TInstaller : Installer;

        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory, TInstaller>()
            where TFactory : IFactory<TConcrete>, IFacadeFactory
            where TConcrete : TContract
            where TInstaller : Installer;

        BindingConditionSetter ToFacadeFactoryInstaller<TFactory>(Type installerType)
            where TFactory : IFactory<TContract>, IFacadeFactory;

        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory>(Type installerType)
            where TFactory : IFactory<TConcrete>, IFacadeFactory
            where TConcrete : TContract;
    }

    ////////////////////////////// One parameters //////////////////////////////

    public interface IIFactoryBinder<TParam1, TContract> : IIFactoryBinderBase<TContract>
    {
        BindingConditionSetter ToMethod(
            Func<DiContainer, TParam1, TContract> method);

        BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TParam1, TContract>;

        BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TParam1, TConcrete>
            where TConcrete : TContract;

        BindingConditionSetter ToFacadeFactoryMethod<TFactory>(Action<DiContainer, TParam1> facadeInstaller)
            where TFactory : IFactory<TParam1, TContract>, IFacadeFactory;

        BindingConditionSetter ToFacadeFactoryMethod<TConcrete, TFactory>(Action<DiContainer, TParam1> facadeInstaller)
            where TFactory : IFactory<TParam1, TConcrete>, IFacadeFactory
            where TConcrete : TContract;

        BindingConditionSetter ToFacadeFactoryInstaller<TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TContract>, IFacadeFactory
            where TInstaller : Installer;

        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TConcrete>, IFacadeFactory
            where TConcrete : TContract
            where TInstaller : Installer;

        BindingConditionSetter ToFacadeFactoryInstaller<TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TContract>, IFacadeFactory;

        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TConcrete>, IFacadeFactory
            where TConcrete : TContract;
    }

    ////////////////////////////// Two parameters //////////////////////////////

    public interface IIFactoryBinder<TParam1, TParam2, TContract> : IIFactoryBinderBase<TContract>
    {
        BindingConditionSetter ToMethod(
            Func<DiContainer, TParam1, TParam2, TContract> method);

        BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TContract>;

        BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TConcrete>
            where TConcrete : TContract;

        BindingConditionSetter ToFacadeFactoryMethod<TFactory>(Action<DiContainer, TParam1, TParam2> facadeInstaller)
            where TFactory : IFactory<TParam1, TParam2, TContract>, IFacadeFactory;

        BindingConditionSetter ToFacadeFactoryMethod<TConcrete, TFactory>(Action<DiContainer, TParam1, TParam2> facadeInstaller)
            where TFactory : IFactory<TParam1, TParam2, TConcrete>, IFacadeFactory
            where TConcrete : TContract;

        BindingConditionSetter ToFacadeFactoryInstaller<TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TParam2, TContract>, IFacadeFactory
            where TInstaller : Installer;

        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TParam2, TConcrete>, IFacadeFactory
            where TConcrete : TContract
            where TInstaller : Installer;

        BindingConditionSetter ToFacadeFactoryInstaller<TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TParam2, TContract>, IFacadeFactory;

        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TParam2, TConcrete>, IFacadeFactory
            where TConcrete : TContract;
    }

    ////////////////////////////// Three parameters //////////////////////////////

    public interface IIFactoryBinder<TParam1, TParam2, TParam3, TContract> : IIFactoryBinderBase<TContract>
    {
        BindingConditionSetter ToMethod(
            Func<DiContainer, TParam1, TParam2, TParam3, TContract> method);

        BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TContract>;

        BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TConcrete>
            where TConcrete : TContract;

        BindingConditionSetter ToFacadeFactoryMethod<TFactory>(Action<DiContainer, TParam1, TParam2, TParam3> facadeInstaller)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TContract>, IFacadeFactory;

        BindingConditionSetter ToFacadeFactoryMethod<TConcrete, TFactory>(Action<DiContainer, TParam1, TParam2, TParam3> facadeInstaller)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TConcrete>, IFacadeFactory
            where TConcrete : TContract;

        BindingConditionSetter ToFacadeFactoryInstaller<TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TContract>, IFacadeFactory
            where TInstaller : Installer;

        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TConcrete>, IFacadeFactory
            where TConcrete : TContract
            where TInstaller : Installer;

        BindingConditionSetter ToFacadeFactoryInstaller<TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TContract>, IFacadeFactory;

        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TConcrete>, IFacadeFactory
            where TConcrete : TContract;
    }

    ////////////////////////////// Four parameters //////////////////////////////

    public interface IIFactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract>
        : IIFactoryBinderBase<TContract>
    {
        BindingConditionSetter ToMethod(
            ModestTree.Util.Func<DiContainer, TParam1, TParam2, TParam3, TParam4, TContract> method);

        BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TContract>;

        BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TConcrete>
            where TConcrete : TContract;

        BindingConditionSetter ToFacadeFactoryMethod<TFactory>(ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4> facadeInstaller)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TContract>, IFacadeFactory;

        BindingConditionSetter ToFacadeFactoryMethod<TConcrete, TFactory>(ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4> facadeInstaller)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TConcrete>, IFacadeFactory
            where TConcrete : TContract;

        BindingConditionSetter ToFacadeFactoryInstaller<TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TContract>, IFacadeFactory
            where TInstaller : Installer;

        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TConcrete>, IFacadeFactory
            where TConcrete : TContract
            where TInstaller : Installer;

        BindingConditionSetter ToFacadeFactoryInstaller<TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TContract>, IFacadeFactory;

        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TConcrete>, IFacadeFactory
            where TConcrete : TContract;
    }
}



