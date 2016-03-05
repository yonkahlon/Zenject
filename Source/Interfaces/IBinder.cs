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
        // Map the given type to a way of obtaining it
        // See ITypeBinder.cs for the full list of methods to call on the return value
        // Note that this can include open generic types as well such as List<>
        IGenericBinder<TContract> Bind<TContract>(string identifier);
        IGenericBinder<TContract> Bind<TContract>();

        // _____ Bind _____
        // Non-generic version of Bind<> for cases where you only have the runtime type
        // Note that this can include open generic types as well such as List<>
        IUntypedBinder Bind(Type contractType, string identifier);
        IUntypedBinder Bind(Type contractType);

        IUntypedBinder Bind(params Type[] contractTypes);
        IUntypedBinder Bind(string identifier, params Type[] contractTypes);

        // _____ BindAllInterfaces _____
        // Bind all the interfaces for the given type to the same thing.
        //
        // Example:
        //
        //    public class Foo : ITickable, IInitializable
        //    {
        //    }
        //
        //    Container.BindAllInterfacesToSingle<Foo>();
        //
        //  This line above is equivalent to the following:
        //
        //    Container.Bind<ITickable>().ToSingle<Foo>();
        //    Container.Bind<IInitializable>().ToSingle<Foo>();
        //
        // Note here that we do not bind Foo to itself.  For that, use BindAllInterfacesAndSelf
        IUntypedBinder BindAllInterfaces<T>();
        IUntypedBinder BindAllInterfaces(Type type);

        // _____ BindAllInterfacesAndSelf _____
        // Same as BindAllInterfaces except also binds to self
        IUntypedBinder BindAllInterfacesAndSelf<T>();
        IUntypedBinder BindAllInterfacesAndSelf(Type type);

        // _____ BindInstance _____
        //  This is simply a shortcut to using the ToInstance method.
        //
        //  Example:
        //      Container.BindInstance(new Foo());
        //
        //  This line above is equivalent to the following:
        //
        //      Container.Bind<Foo>().ToInstance(new Foo());
        //
        BindingConditionSetter BindInstance<TContract>(string identifier, TContract obj);
        BindingConditionSetter BindInstance<TContract>(TContract obj);

        // _____ HasBinding _____
        // Returns true if the given type is bound to something in the container
        bool HasBinding(InjectContext context);
        bool HasBinding<TContract>();
        bool HasBinding<TContract>(string identifier);

        // _____ Install _____
        // This will cause the container to Inject all dependencies on the given installers,
        // and then call InstallBindings() on them to add more bindings to the container.
        void Install(IEnumerable<IInstaller> installers);
        void Install(IInstaller installer, params object[] extraArgs);

        // _____ Install _____
        // This will create a new instance of the given type, inject all dependencies into it,
        // and then call InstallBindings() on them to add more bindings to the container.
        void Install<T>(params object[] extraArgs)
            where T : IInstaller;

        void Install(Type installerType, params object[] extraArgs);

        // This is only necessary if you have to pass in null values as parameters to the installer
        void InstallExplicit(IInstaller installer, List<TypeValuePair> extraArgs);
        void InstallExplicit(Type installerType, List<TypeValuePair> extraArgs);

        // _____ Install _____
        // Returns true if the given installer type has already been installed on this container
        bool HasInstalled(Type installerType);

        bool HasInstalled<T>()
            where T : IInstaller;

        // _____ Rebind _____
        // This method can be used to override any existing bindings that were added previously.
        // It will first clear all previous bindings and then add the new binding.
        // This method is especially useful for tests, where you often want to use almost all the
        // same bindings used in production, except override a few specific ones.
        IGenericBinder<TContract> Rebind<TContract>();

        // _____ BindIFactory _____
        // This can be used to add a binding for dependencies that derive from IFactory<TContract>
        // See the "Abstract Factories" section in the zenject documentation for more information
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
        // This method can be used in cases where you'd like to use BindIFactory but you do not know the type until runtime
        // Note that one drawback with using IFactoryUntyped<TContract> is that the parameters are not checked at compile time
        IFactoryUntypedBinder<TContract> BindIFactoryUntyped<TContract>();
        IFactoryUntypedBinder<TContract> BindIFactoryUntyped<TContract>(string identifier);
        IFactoryUntypedBinder<TContract> BindIFactoryUntyped<TContract>(string identifier, ContainerTypes containerType);

        // _____ Unbind _____
        // Remove whatever bindings have been added for the given type
        bool Unbind<TContract>(string identifier);
        bool Unbind<TContract>();
        void UnbindAll();

        // _____ BindFacadeFactoryMethod _____
        //  Add a factory to a Facade on the container and use the given method to initialize the
        //  sub container for the facade.
        //
        //  See documentation for more information on facades.
        //
        //  NOTE: It is assumed that TFacade is bound to something within the given method
        //
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
        //  Add a factory to a Facade on the container and use the given installer to initialize the
        //  sub container for the facade.
        //
        //  See documentation for more information on facades.
        //
        //  NOTE: It is assumed that TFacade is bound to something within the given instasller
        //
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
        // Bind a type that inherits from MonoFacadeFactory<> with a given prefab.
        //
        // It is assumed here that the prefab contains a FacadeCompositionRoot component and that
        // the Facade property of the FacadeCompositionRoot is set to an instance of whatever
        // class the factory is creating and which also exists on the given prefab.
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

        // _____ BindMonoFacadeFactory _____
        // Bind a type that inherits from MonoBehaviourFactory<> with a given prefab.
        //
        // It is assumed here that the prefab contains whatever MonoBehaviour derived class
        // you passed to the MonoBehaviourFactory
        //
        // See documentation for details
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

        // _____ BindGameObjectFactory _____
        // Bind a class that inherits from GameObjectFactory<> and have it use the given prefab
        //
        // Note: If you have a MonoBehaviour on your prefab that you want the factory to return,
        // you should use MonoBehaviourFactory instead of GameObjectFactory
        BindingConditionSetter BindGameObjectFactory<TFactory>(
            GameObject prefab)
            where TFactory : GameObjectFactory;

        BindingConditionSetter BindGameObjectFactory<TFactory>(
            GameObject prefab, string groupName)
            where TFactory : GameObjectFactory;

        BindingConditionSetter BindGameObjectFactory<TFactory>(
            string identifier, GameObject prefab, string groupName)
            where TFactory : GameObjectFactory;

        BindingConditionSetter BindGameObjectFactory<TFactory>(
            GameObject prefab, string groupName, ContainerTypes createContainer)
            where TFactory : GameObjectFactory;

        BindingConditionSetter BindGameObjectFactory<TFactory>(
            string identifier, GameObject prefab, string groupName, ContainerTypes createContainer)
            where TFactory : GameObjectFactory;

        BindingConditionSetter BindGameObjectFactory(
            Type factoryType, GameObject prefab);

        BindingConditionSetter BindGameObjectFactory(
            Type factoryType, GameObject prefab, string groupName);

        BindingConditionSetter BindGameObjectFactory(
            Type factoryType, string identifier, GameObject prefab, string groupName);

        BindingConditionSetter BindGameObjectFactory(
            Type factoryType, GameObject prefab, string groupName, ContainerTypes createContainer);

        BindingConditionSetter BindGameObjectFactory(
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

