using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

using Zenject.Internal;

namespace Zenject
{
    public class TypeBinderBase
    {
        readonly DiContainer _container;
        readonly StandardBindingDescriptor _binding;

        public TypeBinderBase(
            List<Type> contractTypes, string identifier, DiContainer container)
        {
            _container = container;

            _binding = new StandardBindingDescriptor();
            _binding.ContractTypes = contractTypes.ToList();
            _binding.Identifier = identifier;

            container.StartBinding(_binding);
        }

        protected StandardBindingDescriptor Binding
        {
            get
            {
                return _binding;
            }
        }

        protected List<Type> ContractTypes
        {
            get
            {
                return Binding.ContractTypes;
            }
        }

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        public ScopeBinder ToSelf()
        {
            BindingUtil.AssertTypesAreNotComponents(ContractTypes);
            BindingUtil.AssertTypesAreNotAbstract(ContractTypes);

            Binding.Finalizer = BindingFinalizerUtil
                .CreateSelfFinalizerStandard(
                    SingletonTypes.To, _container, Binding);

            return new ScopeBinder(Binding);
        }

        public ScopeBinder To(Type concreteType)
        {
            return To(new List<Type>() { concreteType });
        }

        // We don't just use params so that we ensure a min of 1
        public ScopeBinder To(List<Type> concreteTypes)
        {
            BindingUtil.AssertTypesAreNotComponents(concreteTypes);
            BindingUtil.AssertConcreteTypeListIsNotEmpty(concreteTypes);

            Binding.Finalizer = BindingFinalizerUtil
                .CreateConcreteFinalizerStandard(
                    concreteTypes,
                    SingletonTypes.To, _container, Binding);

            return new ScopeBinder(Binding);
        }

        public ScopeBinder ToResolve(Type concreteType)
        {
            return ToResolve(concreteType, null);
        }

        public ScopeBinder ToResolve(Type concreteType, string identifier)
        {
            return ToResolve(new List<Type>() { concreteType }, identifier);
        }

        public ScopeBinder ToResolve(List<Type> concreteTypes)
        {
            BindingUtil.AssertConcreteTypeListIsNotEmpty(concreteTypes);

            return ToResolve(concreteTypes, null);
        }

        public ScopeBinder ToResolve(List<Type> concreteTypes, string identifier)
        {
            BindingUtil.AssertConcreteTypeListIsNotEmpty(concreteTypes);
            BindingUtil.AssertIsDerivedFromTypes(concreteTypes, ContractTypes);

            Binding.Finalizer = new ConcreteBindingFinalizer(
                concreteTypes,
                SingletonTypes.ToResolve,
                identifier,
                (type) => new ResolveProvider(type, _container, identifier, false));

            return new ScopeBinder(Binding);
        }

        public ScopeBinder ToInstance(Type concreteType, object instance)
        {
            BindingUtil.AssertIsDerivedFromTypes(concreteType, ContractTypes);
            BindingUtil.AssertInstanceDerivesFromOrEqual(instance, concreteType);

            Binding.Finalizer = new ConcreteBindingFinalizer(
                concreteType, SingletonTypes.ToInstance,
                instance,
                (type) => new InstanceProvider(concreteType, instance));

            return new ScopeBinder(Binding);
        }

        // ToSubContainerSelf (method)

        public ScopeBinder ToSubContainerSelf(Action<DiContainer> installerFunc)
        {
            return ToSubContainerSelf(installerFunc, null);
        }

        public ScopeBinder ToSubContainerSelf(Action<DiContainer> installerFunc, string identifier)
        {
            Binding.Finalizer = new SubContainerMethodSelfBindingFinalizer(installerFunc, identifier);
            return new ScopeBinder(Binding);
        }

        // ToSubContainerSelf (installer)

        public ScopeBinder ToSubContainerSelf<TInstaller>()
            where TInstaller : Installer
        {
            return ToSubContainerSelf<TInstaller>(null);
        }

        public ScopeBinder ToSubContainerSelf<TInstaller>(string identifier)
            where TInstaller : Installer
        {
            return ToSubContainerSelf(typeof(TInstaller), identifier);
        }

        public ScopeBinder ToSubContainerSelf(Type installerType)
        {
            return ToSubContainerSelf(installerType, null);
        }

        public ScopeBinder ToSubContainerSelf(Type installerType, string identifier)
        {
            BindingUtil.AssertIsInstallerType(installerType);

            Binding.Finalizer = new SubContainerInstallerSelfBindingFinalizer(installerType, identifier);

            return new ScopeBinder(Binding);
        }

        // ToSubContainer (method)

        public ScopeBinder ToSubContainer(
            Type concreteType, Action<DiContainer> installerFunc)
        {
            return ToSubContainer(concreteType, installerFunc, null);
        }

        public ScopeBinder ToSubContainer(
            Type concreteType, Action<DiContainer> installerFunc, string identifier)
        {
            return ToSubContainer(new List<Type>() { concreteType }, installerFunc, identifier);
        }

        public ScopeBinder ToSubContainer(
            List<Type> concreteTypes, Action<DiContainer> installerFunc)
        {
            return ToSubContainer(concreteTypes, installerFunc, null);
        }

        public ScopeBinder ToSubContainer(
            List<Type> concreteTypes, Action<DiContainer> installerFunc, string identifier)
        {
            BindingUtil.AssertConcreteTypeListIsNotEmpty(concreteTypes);
            BindingUtil.AssertIsDerivedFromTypes(concreteTypes, ContractTypes);

            Binding.Finalizer = new SubContainerMethodConcreteBindingFinalizer(
                concreteTypes, installerFunc, identifier);

            return new ScopeBinder(Binding);
        }

        // ToSubContainer (installer)

        public ScopeBinder ToSubContainer<TInstaller>(Type concreteType)
            where TInstaller : Installer
        {
            return ToSubContainer<TInstaller>(concreteType, null);
        }

        public ScopeBinder ToSubContainer<TInstaller>(Type concreteType, string identifier)
            where TInstaller : Installer
        {
            return ToSubContainer<TInstaller>(new List<Type>() { concreteType }, identifier);
        }

        public ScopeBinder ToSubContainer<TInstaller>(List<Type> concreteTypes)
            where TInstaller : Installer
        {
            return ToSubContainer<TInstaller>(concreteTypes, null);
        }

        public ScopeBinder ToSubContainer<TInstaller>(List<Type> concreteTypes, string identifier)
            where TInstaller : Installer
        {
            BindingUtil.AssertConcreteTypeListIsNotEmpty(concreteTypes);

            return ToSubContainer(concreteTypes, typeof(TInstaller), identifier);
        }

        public ScopeBinder ToSubContainer(Type concreteType, Type installerType)
        {
            return ToSubContainer(concreteType, installerType, null);
        }

        public ScopeBinder ToSubContainer(Type concreteType, Type installerType, string identifier)
        {
            return ToSubContainer(new List<Type>() { concreteType }, installerType, identifier);
        }

        public ScopeBinder ToSubContainer(List<Type> concreteTypes, Type installerType)
        {
            return ToSubContainer(concreteTypes, installerType, null);
        }

        public ScopeBinder ToSubContainer(List<Type> concreteTypes, Type installerType, string identifier)
        {
            BindingUtil.AssertConcreteTypeListIsNotEmpty(concreteTypes);
            BindingUtil.AssertIsDerivedFromTypes(concreteTypes, ContractTypes);
            BindingUtil.AssertIsInstallerType(installerType);

            Binding.Finalizer = new SubContainerInstallerConcreteBindingFinalizer(
                concreteTypes, installerType, identifier);

            return new ScopeBinder(Binding);
        }
#if !ZEN_NOT_UNITY3D

        public ScopeBinder ToGameObjectSelf()
        {
            return ToGameObjectSelf(null);
        }

        public ScopeBinder ToGameObjectSelf(string gameObjectName)
        {
            if (ContractTypes.All(x => x == typeof(GameObject)))
            {
                Binding.Finalizer = new SelfBindingFinalizer(
                    SingletonTypes.ToGameObject, gameObjectName,
                    (type) => new EmptyGameObjectProvider(_container, gameObjectName));

                return new ScopeBinder(Binding);
            }

            BindingUtil.AssertIsComponent(ContractTypes);
            BindingUtil.AssertTypesAreNotAbstract(ContractTypes);

            Binding.Finalizer = new SelfBindingFinalizer(
                SingletonTypes.ToGameObject, gameObjectName,
                (type) => new AddToNewGameObjectComponentProvider(
                    _container, type, Binding.ConcreteIdentifier, Binding.Arguments, gameObjectName));

            return new ScopeBinder(Binding);
        }

        public ScopeBinder ToGameObject(Type concreteType)
        {
            return ToGameObject(concreteType, null);
        }

        public ScopeBinder ToGameObject(Type concreteType, string gameObjectName)
        {
            return ToGameObject(new List<Type>() { concreteType }, gameObjectName);
        }

        // Creates a new game object and adds the given type as a new component on it
        public ScopeBinder ToGameObject(List<Type> concreteTypes, string gameObjectName)
        {
            BindingUtil.AssertConcreteTypeListIsNotEmpty(concreteTypes);
            BindingUtil.AssertIsDerivedFromTypes(concreteTypes, ContractTypes);

            if (concreteTypes.All(x => x == typeof(GameObject)))
                // This is super rare but might as well support
            {
                Binding.Finalizer = new ConcreteBindingFinalizer(
                    concreteTypes, SingletonTypes.ToGameObject, gameObjectName,
                    (type) => new EmptyGameObjectProvider(_container, gameObjectName));

                return new ScopeBinder(Binding);
            }

            BindingUtil.AssertIsComponent(concreteTypes);
            BindingUtil.AssertIsNotAbstract(concreteTypes);

            Binding.Finalizer = new ConcreteBindingFinalizer(
                concreteTypes, SingletonTypes.ToGameObject, gameObjectName,
                (type) => new AddToNewGameObjectComponentProvider(
                    _container, type, Binding.ConcreteIdentifier, Binding.Arguments, gameObjectName));

            return new ScopeBinder(Binding);
        }

        public ScopeBinder ToComponentSelf(GameObject gameObject)
        {
            BindingUtil.AssertIsValidGameObject(gameObject, Container.IsValidating);
            BindingUtil.AssertIsComponent(ContractTypes);
            BindingUtil.AssertTypesAreNotAbstract(ContractTypes);

            Binding.Finalizer = new SelfBindingFinalizer(
                SingletonTypes.ToComponentGameObject,
                gameObject,
                (type) => new AddToExistingGameObjectComponentProvider(
                    gameObject, _container, type, Binding.ConcreteIdentifier, Binding.Arguments));

            return new ScopeBinder(Binding);
        }

        public ScopeBinder ToComponentSelf(Func<InjectContext, GameObject> gameObjectGetter)
        {
            BindingUtil.AssertIsComponent(ContractTypes);
            BindingUtil.AssertTypesAreNotAbstract(ContractTypes);

            Binding.Finalizer = new SelfBindingFinalizer(
                SingletonTypes.ToComponent,
                new SingletonImplIds.ToMethod(gameObjectGetter),
                (type) => new AddToCustomGameObjectComponentProvider(
                    gameObjectGetter, _container, type, Binding.ConcreteIdentifier, Binding.Arguments));

            return new ScopeBinder(Binding);
        }

        public ScopeBinder ToComponent(Type concreteType, GameObject gameObject)
        {
            return ToComponent(
                new List<Type>() { concreteType }, gameObject);
        }

        public ScopeBinder ToComponent(List<Type> concreteTypes, GameObject gameObject)
        {
            BindingUtil.AssertIsValidGameObject(gameObject, Container.IsValidating);
            BindingUtil.AssertConcreteTypeListIsNotEmpty(concreteTypes);
            BindingUtil.AssertIsDerivedFromTypes(concreteTypes, ContractTypes);
            BindingUtil.AssertIsComponent(concreteTypes);
            BindingUtil.AssertIsNotAbstract(concreteTypes);

            Binding.Finalizer = new ConcreteBindingFinalizer(
                concreteTypes, SingletonTypes.ToComponentGameObject,
                gameObject,
                (type) => new AddToExistingGameObjectComponentProvider(
                    gameObject, _container, type, Binding.ConcreteIdentifier, Binding.Arguments));

            return new ScopeBinder(Binding);
        }

        public ScopeBinder ToComponent(
            Type concreteType, Func<InjectContext, GameObject> gameObjectGetter)
        {
            BindingUtil.AssertIsDerivedFromTypes(concreteType, ContractTypes);
            BindingUtil.AssertIsComponent(concreteType);
            BindingUtil.AssertIsNotAbstract(concreteType);

            Binding.Finalizer = new ConcreteBindingFinalizer(
                concreteType, SingletonTypes.ToComponent,
                new SingletonImplIds.ToMethod(gameObjectGetter),
                (type) => new AddToCustomGameObjectComponentProvider(
                    gameObjectGetter, _container, type, Binding.ConcreteIdentifier, Binding.Arguments));

            return new ScopeBinder(Binding);
        }

        public ScopeBinder ToPrefabSelf(GameObject prefab)
        {
            return ToPrefabSelf(prefab, null);
        }

        public ScopeBinder ToPrefabSelf(GameObject prefab, string gameObjectName)
        {
            if (ContractTypes.All(x => x == typeof(GameObject)))
            {
                Binding.Finalizer = new SelfBindingFinalizer(
                    SingletonTypes.ToPrefab, gameObjectName,
                    (type) => new PrefabGameObjectProvider(
                        new PrefabInstantiator(
                            _container, gameObjectName, Binding.Arguments, prefab)));

                return new ScopeBinder(Binding);
            }

            BindingUtil.AssertIsAbstractOrComponent(ContractTypes);
            BindingUtil.AssertIsValidPrefab(prefab);

            Binding.Finalizer = new PrefabSelfBindingFinalizer(prefab, gameObjectName);

            return new ScopeBinder(Binding);
        }

        public ScopeBinder ToPrefab(
            Type concreteType, GameObject prefab)
        {
            return ToPrefab(concreteType, prefab, null);
        }

        public ScopeBinder ToPrefab(
            Type concreteType, GameObject prefab, string gameObjectName)
        {
            return ToPrefab(new List<Type>() { concreteType }, prefab, gameObjectName);
        }

        public ScopeBinder ToPrefab(
            List<Type> concreteTypes, GameObject prefab)
        {
            return ToPrefab(concreteTypes, prefab, null);
        }

        public ScopeBinder ToPrefab(
            List<Type> concreteTypes, GameObject prefab, string gameObjectName)
        {
            BindingUtil.AssertConcreteTypeListIsNotEmpty(concreteTypes);
            BindingUtil.AssertIsDerivedFromTypes(concreteTypes, ContractTypes);
            BindingUtil.AssertIsValidPrefab(prefab);

            if (concreteTypes.All(x => x == typeof(GameObject)))
            {
                Binding.Finalizer = new ConcreteBindingFinalizer(
                    concreteTypes, SingletonTypes.ToPrefab, gameObjectName,
                    (type) => new PrefabGameObjectProvider(
                        new PrefabInstantiator(
                            _container, gameObjectName, Binding.Arguments, prefab)));

                return new ScopeBinder(Binding);
            }

            BindingUtil.AssertIsAbstractOrComponent(ContractTypes);

            Binding.Finalizer = new PrefabConcreteBindingFinalizer(
                prefab, concreteTypes, gameObjectName);

            return new ScopeBinder(Binding);
        }

        public ScopeBinder ToPrefabResourceSelf(string resourcePath)
        {
            return ToPrefabResourceSelf(resourcePath, null);
        }

        public ScopeBinder ToPrefabResourceSelf(string resourcePath, string gameObjectName)
        {
            if (ContractTypes.All(x => x == typeof(GameObject)))
            {
                Binding.Finalizer = new SelfBindingFinalizer(
                    SingletonTypes.ToPrefab, gameObjectName,
                    (type) => new PrefabGameObjectProvider(
                        new PrefabInstantiatorResource(
                            _container, gameObjectName, Binding.Arguments, resourcePath)));

                return new ScopeBinder(Binding);
            }

            BindingUtil.AssertIsValidResourcePath(resourcePath);
            BindingUtil.AssertIsAbstractOrComponent(ContractTypes);

            Binding.Finalizer = new PrefabResourceSelfBindingFinalizer(
                resourcePath, gameObjectName);

            return new ScopeBinder(Binding);
        }

        public ScopeBinder ToPrefabResource(Type concreteType, string resourcePath)
        {
            return ToPrefabResource(concreteType, resourcePath, null);
        }

        public ScopeBinder ToPrefabResource(
            Type concreteType, string resourcePath, string gameObjectName)
        {
            return ToPrefabResource(new List<Type>() { concreteType }, resourcePath, gameObjectName);
        }

        public ScopeBinder ToPrefabResource(
            List<Type> concreteTypes, string resourcePath, string gameObjectName)
        {
            if (concreteTypes.All(x => x == typeof(GameObject)))
            {
                Binding.Finalizer = new ConcreteBindingFinalizer(
                    concreteTypes, SingletonTypes.ToPrefab, gameObjectName,
                    (type) => new PrefabGameObjectProvider(
                        new PrefabInstantiatorResource(
                            _container, gameObjectName, Binding.Arguments, resourcePath)));

                return new ScopeBinder(Binding);
            }

            BindingUtil.AssertConcreteTypeListIsNotEmpty(concreteTypes);
            BindingUtil.AssertIsValidResourcePath(resourcePath);
            BindingUtil.AssertIsAbstractOrComponent(concreteTypes);

            Binding.Finalizer = new PrefabResourceConcreteBindingFinalizer(
                resourcePath, concreteTypes, gameObjectName);

            return new ScopeBinder(Binding);
        }

        public ScopeBinder ToResourceSelf(string resourcePath)
        {
            BindingUtil.AssertDerivesFromUnityObject(ContractTypes);

            Binding.Finalizer = new SelfBindingFinalizer(
                SingletonTypes.ToResource,
                resourcePath.ToLower(),
                (type) => new ResourceProvider(resourcePath, type));

            return new ScopeBinder(Binding);
        }

        public ScopeBinder ToResource(Type concreteType, string resourcePath)
        {
            BindingUtil.AssertIsDerivedFromTypes(concreteType, ContractTypes);
            BindingUtil.AssertDerivesFromUnityObject(concreteType);

            Binding.Finalizer = new ConcreteBindingFinalizer(
                concreteType, SingletonTypes.ToResource,
                resourcePath.ToLower(),
                (type) => new ResourceProvider(resourcePath, type));

            return new ScopeBinder(Binding);
        }
#endif

        protected ScopeBinder ToMethodBase<TConcrete>(Func<InjectContext, TConcrete> method)
        {
            BindingUtil.AssertIsDerivedFromTypes(typeof(TConcrete), ContractTypes);

            Binding.Finalizer = new ConcreteBindingFinalizer(
                typeof(TConcrete),
                SingletonTypes.ToMethod,
                new SingletonImplIds.ToMethod(method),
                (type) => new MethodProvider<TConcrete>(method));

            return new ScopeBinder(Binding);
        }

        protected ScopeBinder ToFactoryBase<TConcrete, TFactory>()
            where TFactory : IFactory<TConcrete>
        {
            BindingUtil.AssertIsDerivedFromTypes(typeof(TConcrete), ContractTypes);

            Binding.Finalizer = new ConcreteBindingFinalizer(
                typeof(TConcrete),
                SingletonTypes.ToFactory,
                typeof(TFactory),
                (type) => new FactoryProvider<TConcrete, TFactory>(Container));

            return new ScopeBinder(Binding);
        }

        protected ScopeBinder ToGetterBase<TObj, TResult>(
            string identifier, Func<TObj, TResult> method)
        {
            BindingUtil.AssertIsDerivedFromTypes(typeof(TResult), ContractTypes);

            Binding.Finalizer = new ConcreteBindingFinalizer(
                typeof(TResult),
                SingletonTypes.ToGetter,
                new SingletonImplIds.ToGetter(identifier, method),
                (type) => new GetterProvider<TObj, TResult>(identifier, method, _container));

            return new ScopeBinder(Binding);
        }
    }
}
