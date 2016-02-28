using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public interface IBinder
    {
        // _____ Bind<> _____
        //
        IGenericBinder<TContract> Bind<TContract>(string identifier);
        IGenericBinder<TContract> Bind<TContract>();

        // _____ Bind _____
        //
        // Non-generic version of Bind<> for cases where you only have the runtime type
        IUntypedBinder Bind(Type contractType, string identifier);
        IUntypedBinder Bind(Type contractType);

        IUntypedBinder Bind(List<Type> contractTypes);
        IUntypedBinder Bind(List<Type> contractTypes, string identifier);

        // _____ BindAllInterfaces _____
        //
        IUntypedBinder BindAllInterfaces<T>();
        IUntypedBinder BindAllInterfaces(Type type);

        // _____ BindAllInterfaces _____
        //
        IUntypedBinder BindAllInterfacesAndSelf<T>();
        IUntypedBinder BindAllInterfacesAndSelf(Type type);

        // _____ BindInstance _____
        //
        BindingConditionSetter BindInstance<TContract>(string identifier, TContract obj);
        BindingConditionSetter BindInstance<TContract>(TContract obj);

        // _____ HasBinding _____
        //
        bool HasBinding(InjectContext context);
        bool HasBinding<TContract>();
        bool HasBinding<TContract>(string identifier);

        // _____ Install _____
        //
        void Install(IEnumerable<IInstaller> installers);
        void Install(IInstaller installer, params object[] extraArgs);

        void Install<T>(params object[] extraArgs)
            where T : IInstaller;

        void Install(Type installerType, params object[] extraArgs);

        void InstallExplicit(IInstaller installer, List<TypeValuePair> extraArgs);
        void InstallExplicit(Type installerType, List<TypeValuePair> extraArgs);

        bool HasInstalled(Type installerType);

        bool HasInstalled<T>()
            where T : IInstaller;

        // _____ Rebind _____
        //
        IGenericBinder<TContract> Rebind<TContract>();

        // _____ BindIFactory _____
        //
        IIFactoryBinder<TContract> BindIFactory<TContract>();
        IIFactoryBinder<TContract> BindIFactory<TContract>(string identifier);
        IIFactoryBinder<TContract> BindIFactory<TContract>(string identifier, ContainerTypes containerType);

        IIFactoryBinder<TParam1, TContract> BindIFactory<TParam1, TContract>();
        IIFactoryBinder<TParam1, TContract> BindIFactory<TParam1, TContract>(string identifier);
        IIFactoryBinder<TParam1, TContract> BindIFactory<TParam1, TContract>(string identifier, ContainerTypes containerType);

        IIFactoryBinder<TParam1, TParam2, TContract> BindIFactory<TParam1, TParam2, TContract>();
        IIFactoryBinder<TParam1, TParam2, TContract> BindIFactory<TParam1, TParam2, TContract>(string identifier);
        IIFactoryBinder<TParam1, TParam2, TContract> BindIFactory<TParam1, TParam2, TContract>(string identifier, ContainerTypes containerType);

        IIFactoryBinder<TParam1, TParam2, TParam3, TContract> BindIFactory<TParam1, TParam2, TParam3, TContract>();
        IIFactoryBinder<TParam1, TParam2, TParam3, TContract> BindIFactory<TParam1, TParam2, TParam3, TContract>(string identifier);
        IIFactoryBinder<TParam1, TParam2, TParam3, TContract> BindIFactory<TParam1, TParam2, TParam3, TContract>(string identifier, ContainerTypes containerType);

        IIFactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract> BindIFactory<TParam1, TParam2, TParam3, TParam4, TContract>();
        IIFactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract> BindIFactory<TParam1, TParam2, TParam3, TParam4, TContract>(string identifier);
        IIFactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract> BindIFactory<TParam1, TParam2, TParam3, TParam4, TContract>(string identifier, ContainerTypes containerType);

        // _____ BindIFactoryUntyped _____
        //
        IFactoryUntypedBinder<TContract> BindIFactoryUntyped<TContract>();
        IFactoryUntypedBinder<TContract> BindIFactoryUntyped<TContract>(string identifier);
        IFactoryUntypedBinder<TContract> BindIFactoryUntyped<TContract>(string identifier, ContainerTypes containerType);

        // _____ Unbind _____
        //
        bool Unbind<TContract>(string identifier);
        bool Unbind<TContract>();
        void UnbindAll();

        // _____ BindFacadeFactoryMethod _____
        //
        // NOTE: If you inherit from the Facade you need to install FacadeCommonInstaller
        // in your custom install method
        BindingConditionSetter BindFacadeFactoryMethod<TFacade, TFacadeFactory>(
            Action<DiContainer> facadeInstaller)
            where TFacadeFactory : FacadeFactory<TFacade>;

        BindingConditionSetter BindFacadeFactoryMethod<TFacade, TFacadeFactory>(
            Action<DiContainer> facadeInstaller, ContainerTypes containerType)
            where TFacadeFactory : FacadeFactory<TFacade>;

        BindingConditionSetter BindFacadeFactoryMethod<TParam1, TFacade, TFacadeFactory>(
            Action<DiContainer, TParam1> facadeInstaller)
            where TFacadeFactory : FacadeFactory<TParam1, TFacade>;

        BindingConditionSetter BindFacadeFactoryMethod<TParam1, TFacade, TFacadeFactory>(
            Action<DiContainer, TParam1> facadeInstaller, ContainerTypes containerType)
            where TFacadeFactory : FacadeFactory<TParam1, TFacade>;

        BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TFacade, TFacadeFactory>(
            Action<DiContainer, TParam1, TParam2> facadeInstaller)
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TFacade> ;

        BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TFacade, TFacadeFactory>(
            Action<DiContainer, TParam1, TParam2> facadeInstaller, ContainerTypes containerType)
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TFacade> ;

        BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TFacade, TFacadeFactory>(
            Action<DiContainer, TParam1, TParam2, TParam3> facadeInstaller)
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TParam3, TFacade>;

        BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TFacade, TFacadeFactory>(
            Action<DiContainer, TParam1, TParam2, TParam3> facadeInstaller, ContainerTypes containerType)
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TParam3, TFacade>;

        BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TParam4, TFacade, TFacadeFactory>(
            ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4> facadeInstaller)
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TParam3, TParam4, TFacade>;

        BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TParam4, TFacade, TFacadeFactory>(
            ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4> facadeInstaller, ContainerTypes containerType)
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TParam3, TParam4, TFacade>;

        BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TParam4, TParam5, TFacade, TFacadeFactory>(
            ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5> facadeInstaller)
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TFacade>;

        BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TParam4, TParam5, TFacade, TFacadeFactory>(
            ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5> facadeInstaller, ContainerTypes containerType)
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TFacade>;

        BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TFacade, TFacadeFactory>(
            ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> facadeInstaller)
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TFacade>;

        BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TFacade, TFacadeFactory>(
            ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> facadeInstaller, ContainerTypes containerType)
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TFacade>;

        // _____ BindFacadeFactoryInstaller _____
        //
        // NOTE: If you inherit from the Facade you need to install FacadeCommonInstaller
        // in your custom install method
        BindingConditionSetter BindFacadeFactoryInstaller<TFacade, TFacadeFactory, TInstaller>()
            where TFacadeFactory : FacadeFactoryBase<TFacade>
            where TInstaller : Installer;

        // Non-generic versions
        BindingConditionSetter BindFacadeFactoryInstaller<TFacade, TFacadeFactory>(Type installerType)
            where TFacadeFactory : FacadeFactoryBase<TFacade>;

        // Non-generic versions
        BindingConditionSetter BindFacadeFactoryInstaller<TFacade, TFacadeFactory>(Type installerType, ContainerTypes containerType)
            where TFacadeFactory : FacadeFactoryBase<TFacade>;

#if !ZEN_NOT_UNITY3D

        // _____ BindMonoFacadeFactory _____
        //
        BindingConditionSetter BindMonoFacadeFactory<TFactory>(
            GameObject prefab)
            where TFactory : IMonoFacadeFactoryZeroParams;

        BindingConditionSetter BindMonoFacadeFactory<TFactory>(
            GameObject prefab, string groupName)
            where TFactory : IMonoFacadeFactoryZeroParams;

        BindingConditionSetter BindMonoFacadeFactory<TFactory>(
            GameObject prefab, string groupName, ContainerTypes containerType)
            where TFactory : IMonoFacadeFactoryZeroParams;

        BindingConditionSetter BindMonoFacadeFactory<TInstaller, TFactory>(
            GameObject prefab)
            where TInstaller : MonoInstaller
            where TFactory : IMonoFacadeFactoryMultipleParams;

        BindingConditionSetter BindMonoFacadeFactory<TInstaller, TFactory>(
            GameObject prefab, string groupName)
            where TInstaller : MonoInstaller
            where TFactory : IMonoFacadeFactoryMultipleParams;

        BindingConditionSetter BindMonoFacadeFactory<TInstaller, TFactory>(
            GameObject prefab, string groupName, ContainerTypes containerType)
            where TInstaller : MonoInstaller
            where TFactory : IMonoFacadeFactoryMultipleParams;

        // _____ BindMonoBehaviourFactory _____
        //
        BindingConditionSetter BindMonoBehaviourFactory<TFactory>(
            GameObject prefab)
            where TFactory : IMonoBehaviourFactory;

        BindingConditionSetter BindMonoBehaviourFactory<TFactory>(
            GameObject prefab, string groupName)
            where TFactory : IMonoBehaviourFactory;

        BindingConditionSetter BindMonoBehaviourFactory<TFactory>(
            string identifier, GameObject prefab, string groupName)
            where TFactory : IMonoBehaviourFactory;

        BindingConditionSetter BindMonoBehaviourFactory<TFactory>(
            GameObject prefab, string groupName, ContainerTypes createContainer)
            where TFactory : IMonoBehaviourFactory;

        BindingConditionSetter BindMonoBehaviourFactory<TFactory>(
            string identifier, GameObject prefab, string groupName, ContainerTypes createContainer)
            where TFactory : IMonoBehaviourFactory;

        BindingConditionSetter BindMonoBehaviourFactory(
            Type factoryType, GameObject prefab);

        BindingConditionSetter BindMonoBehaviourFactory(
            Type factoryType, GameObject prefab, string groupName);

        BindingConditionSetter BindMonoBehaviourFactory(
            Type factoryType, string identifier, GameObject prefab, string groupName);

        BindingConditionSetter BindMonoBehaviourFactory(
            Type factoryType, GameObject prefab, string groupName, ContainerTypes createContainer);

        BindingConditionSetter BindMonoBehaviourFactory(
            Type factoryType, string identifier, GameObject prefab,
            string groupName, ContainerTypes createContainer);
#endif

        // Returns true if the DiContainer waas made for validation purposes only
        // This is run at edit time.  This is one of the reasons you should never instantiate
        // any objects during the bind phase.
        bool IsValidating
        {
            get;
            set;
        }
    }
}

