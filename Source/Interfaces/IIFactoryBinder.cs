using System;
using ModestTree;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    ////////////////////////////// Zero parameters //////////////////////////////
    public interface IIFactoryBinder<TContract> : IIFactoryBinderBase<TContract>
    {
        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToInstance(TContract instance);

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToMethod(Func<DiContainer, TContract> method);

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFactory(Type concreteType);

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TContract>;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToCustomFactory(Type factoryType);

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TConcrete>
            where TConcrete : TContract;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToCustomFactory(Type concreteType, Type factoryType);

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryMethod<TFactory>(Action<DiContainer> facadeInstaller)
            where TFactory : IFactory<TContract>, IFacadeFactory;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryMethod<TConcrete, TFactory>(Action<DiContainer> facadeInstaller)
            where TFactory : IFactory<TConcrete>, IFacadeFactory
            where TConcrete : TContract;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TFactory, TInstaller>()
            where TFactory : IFactory<TContract>, IFacadeFactory
            where TInstaller : Installer;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory, TInstaller>()
            where TFactory : IFactory<TConcrete>, IFacadeFactory
            where TConcrete : TContract
            where TInstaller : Installer;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TFactory>(Type installerType)
            where TFactory : IFactory<TContract>, IFacadeFactory;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory>(Type installerType)
            where TFactory : IFactory<TConcrete>, IFacadeFactory
            where TConcrete : TContract;
    }

    ////////////////////////////// One parameters //////////////////////////////

    public interface IIFactoryBinder<TParam1, TContract> : IIFactoryBinderBase<TContract>
    {
        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToMethod(
            Func<DiContainer, TParam1, TContract> method);

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TParam1, TContract>;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TParam1, TConcrete>
            where TConcrete : TContract;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryMethod<TFactory>(Action<DiContainer, TParam1> facadeInstaller)
            where TFactory : IFactory<TParam1, TContract>, IFacadeFactory;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryMethod<TConcrete, TFactory>(Action<DiContainer, TParam1> facadeInstaller)
            where TFactory : IFactory<TParam1, TConcrete>, IFacadeFactory
            where TConcrete : TContract;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TContract>, IFacadeFactory
            where TInstaller : Installer;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TConcrete>, IFacadeFactory
            where TConcrete : TContract
            where TInstaller : Installer;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TContract>, IFacadeFactory;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TConcrete>, IFacadeFactory
            where TConcrete : TContract;
    }

    ////////////////////////////// Two parameters //////////////////////////////

    public interface IIFactoryBinder<TParam1, TParam2, TContract> : IIFactoryBinderBase<TContract>
    {
        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToMethod(
            Func<DiContainer, TParam1, TParam2, TContract> method);

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TContract>;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TConcrete>
            where TConcrete : TContract;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryMethod<TFactory>(Action<DiContainer, TParam1, TParam2> facadeInstaller)
            where TFactory : IFactory<TParam1, TParam2, TContract>, IFacadeFactory;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryMethod<TConcrete, TFactory>(Action<DiContainer, TParam1, TParam2> facadeInstaller)
            where TFactory : IFactory<TParam1, TParam2, TConcrete>, IFacadeFactory
            where TConcrete : TContract;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TParam2, TContract>, IFacadeFactory
            where TInstaller : Installer;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TParam2, TConcrete>, IFacadeFactory
            where TConcrete : TContract
            where TInstaller : Installer;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TParam2, TContract>, IFacadeFactory;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TParam2, TConcrete>, IFacadeFactory
            where TConcrete : TContract;
    }

    ////////////////////////////// Three parameters //////////////////////////////

    public interface IIFactoryBinder<TParam1, TParam2, TParam3, TContract> : IIFactoryBinderBase<TContract>
    {
        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToMethod(
            Func<DiContainer, TParam1, TParam2, TParam3, TContract> method);

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TContract>;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TConcrete>
            where TConcrete : TContract;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryMethod<TFactory>(Action<DiContainer, TParam1, TParam2, TParam3> facadeInstaller)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TContract>, IFacadeFactory;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryMethod<TConcrete, TFactory>(Action<DiContainer, TParam1, TParam2, TParam3> facadeInstaller)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TConcrete>, IFacadeFactory
            where TConcrete : TContract;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TContract>, IFacadeFactory
            where TInstaller : Installer;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TConcrete>, IFacadeFactory
            where TConcrete : TContract
            where TInstaller : Installer;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TContract>, IFacadeFactory;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TConcrete>, IFacadeFactory
            where TConcrete : TContract;
    }

    ////////////////////////////// Four parameters //////////////////////////////

    public interface IIFactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract>
        : IIFactoryBinderBase<TContract>
    {
        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToMethod(
            ModestTree.Util.Func<DiContainer, TParam1, TParam2, TParam3, TParam4, TContract> method);

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TContract>;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TConcrete>
            where TConcrete : TContract;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryMethod<TFactory>(ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4> facadeInstaller)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TContract>, IFacadeFactory;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryMethod<TConcrete, TFactory>(ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4> facadeInstaller)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TConcrete>, IFacadeFactory
            where TConcrete : TContract;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TContract>, IFacadeFactory
            where TInstaller : Installer;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TConcrete>, IFacadeFactory
            where TConcrete : TContract
            where TInstaller : Installer;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TContract>, IFacadeFactory;

        // See IIFactoryBinderBase.cs for description
        BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TConcrete>, IFacadeFactory
            where TConcrete : TContract;
    }
}



