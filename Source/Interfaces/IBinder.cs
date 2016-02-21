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

        // _____ BindIFactory _____
        //
        BindingConditionSetter BindInstance<TContract>(string identifier, TContract obj);
        BindingConditionSetter BindInstance<TContract>(TContract obj);

        // _____ BindAllInterfacesToSingle _____
        //
        void BindAllInterfacesToSingle<TConcrete>();
        void BindAllInterfacesToSingle(Type concreteType);
        void BindAllInterfacesToSingle(Type concreteType, string concreteIdentifier);

        // _____ BindAllInterfacesToSingleFacadeMethod _____
        //
        void BindAllInterfacesToSingleFacadeMethod<TConcrete>(Action<IBinder> installerMethod);
        void BindAllInterfacesToSingleFacadeMethod(Type concreteType, Action<IBinder> installerMethod);
        void BindAllInterfacesToSingleFacadeMethod(Type concreteType, string concreteIdentifier, Action<IBinder> installerMethod);

        // _____ BindAllInterfacesToInstance _____
        //
        void BindAllInterfacesToInstance(object value);
        void BindAllInterfacesToInstance(Type concreteType, object value);

        // _____ HasBinding _____
        //
        bool HasBinding(InjectContext context);
        bool HasBinding<TContract>();
        bool HasBinding<TContract>(string identifier);

        // _____ Install _____
        //
        void Install(IEnumerable<IInstaller> installers);
        void Install(IInstaller installer);

        void Install<T>(params object[] extraArgs)
            where T : IInstaller;

        void Install(Type installerType, params object[] extraArgs);

        bool HasInstalled(Type installerType);

        bool HasInstalled<T>()
            where T : IInstaller;

        // _____ Rebind _____
        //
        IGenericBinder<TContract> Rebind<TContract>();

        // _____ BindIFactory _____
        //
        IIFactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract> BindIFactory<TParam1, TParam2, TParam3, TParam4, TContract>(string identifier);
        IIFactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract> BindIFactory<TParam1, TParam2, TParam3, TParam4, TContract>();

        IIFactoryBinder<TParam1, TParam2, TParam3, TContract> BindIFactory<TParam1, TParam2, TParam3, TContract>(string identifier);
        IIFactoryBinder<TParam1, TParam2, TParam3, TContract> BindIFactory<TParam1, TParam2, TParam3, TContract>();

        IIFactoryBinder<TParam1, TParam2, TContract> BindIFactory<TParam1, TParam2, TContract>(string identifier);
        IIFactoryBinder<TParam1, TParam2, TContract> BindIFactory<TParam1, TParam2, TContract>();

        IIFactoryBinder<TParam1, TContract> BindIFactory<TParam1, TContract>(string identifier);
        IIFactoryBinder<TParam1, TContract> BindIFactory<TParam1, TContract>();

        IIFactoryBinder<TContract> BindIFactory<TContract>(string identifier);
        IIFactoryBinder<TContract> BindIFactory<TContract>();

        // _____ BindIFactoryUntyped _____
        //
        IFactoryUntypedBinder<TContract> BindIFactoryUntyped<TContract>(string identifier);
        IFactoryUntypedBinder<TContract> BindIFactoryUntyped<TContract>();

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
            Action<IBinder> facadeInstaller)
            where TFacadeFactory : FacadeFactory<TFacade>;

        BindingConditionSetter BindFacadeFactoryMethod<TParam1, TFacade, TFacadeFactory>(
            Action<IBinder, TParam1> facadeInstaller)
            where TFacadeFactory : FacadeFactory<TParam1, TFacade>;

        BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TFacade, TFacadeFactory>(
            Action<IBinder, TParam1, TParam2> facadeInstaller)
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TFacade> ;

        BindingConditionSetter BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TFacade, TFacadeFactory>(
            Action<IBinder, TParam1, TParam2, TParam3> facadeInstaller)
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TParam3, TFacade>;

        // _____ BindFacadeFactoryInstaller _____
        //
        // NOTE: If you inherit from the Facade you need to install FacadeCommonInstaller
        // in your custom install method
        BindingConditionSetter BindFacadeFactoryInstaller<TFacade, TFacadeFactory, TInstaller>()
            where TFacadeFactory : FacadeFactory<TFacade>
            where TInstaller : Installer;

        BindingConditionSetter BindFacadeFactoryInstaller<TParam1, TFacade, TFacadeFactory, TInstaller>()
            where TFacadeFactory : FacadeFactory<TParam1, TFacade>
            where TInstaller : Installer;

        BindingConditionSetter BindFacadeFactoryInstaller<TParam1, TParam2, TFacade, TFacadeFactory, TInstaller>()
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TFacade>
            where TInstaller : Installer;

        BindingConditionSetter BindFacadeFactoryInstaller<TParam1, TParam2, TParam3, TFacade, TFacadeFactory, TInstaller>()
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TParam3, TFacade>
            where TInstaller : Installer;

        // Non-generic versions
        BindingConditionSetter BindFacadeFactoryInstaller<TFacade, TFacadeFactory>(Type installerType)
            where TFacadeFactory : FacadeFactory<TFacade>;

        BindingConditionSetter BindFacadeFactoryInstaller<TParam1, TFacade, TFacadeFactory>(Type installerType)
            where TFacadeFactory : FacadeFactory<TParam1, TFacade>;

        BindingConditionSetter BindFacadeFactoryInstaller<TParam1, TParam2, TFacade, TFacadeFactory>(Type installerType)
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TFacade> ;

        BindingConditionSetter BindFacadeFactoryInstaller<TParam1, TParam2, TParam3, TFacade, TFacadeFactory>(Type installerType)
            where TFacadeFactory : FacadeFactory<TParam1, TParam2, TParam3, TFacade>;


#if !ZEN_NOT_UNITY3D

        // _____ BindMonoFacadeFactory _____
        //
        BindingConditionSetter BindMonoFacadeFactory<T>(
            GameObject prefab)
            where T : MonoFacadeFactory;

        BindingConditionSetter BindMonoFacadeFactory<T>(
            GameObject prefab, string groupName)
            where T : MonoFacadeFactory;

        // _____ BindGameObjectFactory _____
        //
        BindingConditionSetter BindGameObjectFactory<T>(
            GameObject prefab)
            where T : GameObjectFactory;

        BindingConditionSetter BindGameObjectFactory<T>(
            GameObject prefab, string groupName)
            where T : GameObjectFactory;
#endif

        IInstantiator Instantiator
        {
            get;
        }

        IResolver Resolver
        {
            get;
        }

        // Returns true if the DiContainer waas made for validation purposes only
        // This is run at edit time.  This is one of the reasons you should never instantiate
        // any objects during the bind phase.
        bool IsValidating
        {
            get;
        }
    }
}

