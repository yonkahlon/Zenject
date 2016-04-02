using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public abstract class FactoryBinderBase<TContract, TFactory>
        where TFactory : DynamicFactory<TContract>
    {
        readonly DiContainer _container;
        readonly StandardBindingDescriptor _binding;

        public FactoryBinderBase(
            string identifier, DiContainer container)
        {
            _container = container;

            _binding = new StandardBindingDescriptor();
            _binding.ContractTypes = new List<Type>()
            {
                typeof(TFactory),
            };
            _binding.Identifier = identifier;

            container.StartBinding(_binding);
        }

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        protected Type ContractType
        {
            get
            {
                return typeof(TContract);
            }
        }

        public ConditionBinder ToProvider(IProvider provider)
        {
            _binding.Finalizer = BindingFinalizerUtil.
                CreateCachedSingleProviderFinalizerStandard<TFactory>(
                    Container,
                    InjectUtil.CreateArgListExplicit(
                        provider,
                        // We could forward the InjectContext used for the factory itself
                        // but this doesn't really make much sense
                        new InjectContext(Container, typeof(TContract))));

            return new ConditionBinder(_binding);
        }

        public ConditionBinder ToSelf()
        {
            BindingUtil.AssertIsNotComponent(ContractType);
            BindingUtil.AssertIsNotAbstract(ContractType);

            return ToProvider(
                new TransientProvider(ContractType, Container));
        }

        public ConditionBinder To<TConcrete>()
            where TConcrete : TContract
        {
            return To(typeof(TConcrete));
        }

        public ConditionBinder To(Type concreteType)
        {
            BindingUtil.AssertIsNotComponent(concreteType);

            return ToProvider(
                new TransientProvider(concreteType, Container));
        }

        public ConditionBinder ToSubContainerSelf(Type installerType)
        {
            return ToSubContainerSelf(installerType, null);
        }

        public ConditionBinder ToSubContainerSelf(Type installerType, string identifier)
        {
            BindingUtil.AssertIsInstallerType(installerType);

            return ToProvider(
                new SubContainerDependencyProvider(
                    ContractType, identifier,
                    new SubContainerCreatorByInstaller(
                        Container, installerType)));
        }

        public ConditionBinder ToSubContainerSelf<TInstaller>()
            where TInstaller : Installer
        {
            return ToSubContainerSelf(typeof(TInstaller));
        }

        public ConditionBinder ToSubContainer<TConcrete, TInstaller>()
            where TInstaller : Installer
            where TConcrete : TContract
        {
            return ToSubContainer(typeof(TConcrete), typeof(TInstaller));
        }

        public ConditionBinder ToSubContainer<TInstaller>(Type concreteType)
            where TInstaller : Installer
        {
            return ToSubContainer(concreteType, typeof(TInstaller));
        }

        public ConditionBinder ToSubContainer(Type concreteType, Type installerType)
        {
            return ToSubContainer(concreteType, installerType, null);
        }

        public ConditionBinder ToSubContainer(Type concreteType, Type installerType, string identifier)
        {
            BindingUtil.AssertIsDerivedFromType(concreteType, ContractType);
            BindingUtil.AssertIsInstallerType(installerType);

            return ToProvider(
                new SubContainerDependencyProvider(
                    concreteType, identifier,
                    new SubContainerCreatorByInstaller(
                        Container, installerType)));
        }

#if !ZEN_NOT_UNITY3D

        public ConditionBinder ToGameObjectSelf()
        {
            return ToGameObjectSelf(null);
        }

        public ConditionBinder ToGameObjectSelf(string gameObjectName)
        {
            if (ContractType == typeof(GameObject))
            {
                return ToProvider(
                    new EmptyGameObjectProvider(Container, gameObjectName));
            }

            BindingUtil.AssertIsComponent(ContractType);
            BindingUtil.AssertIsNotAbstract(ContractType);

            return ToProvider(
                new AddToNewGameObjectComponentProvider(
                    Container, ContractType, null,
                    new List<TypeValuePair>(), gameObjectName));
        }

        // Creates a new game object and adds the given type as a new component on it
        public ConditionBinder ToGameObject<TConcrete>()
            where TConcrete : TContract
        {
            return ToGameObject(typeof(TConcrete));
        }

        // Creates a new game object and adds the given type as a new component on it
        public ConditionBinder ToGameObject<TConcrete>(string gameObjectName)
            where TConcrete : TContract
        {
            return ToGameObject(typeof(TConcrete), gameObjectName);
        }

        public ConditionBinder ToGameObject(Type concreteType)
        {
            return ToGameObject(concreteType, null);
        }

        public ConditionBinder ToGameObject(Type concreteType, string gameObjectName)
        {
            BindingUtil.AssertIsDerivedFromType(concreteType, ContractType);

            if (concreteType == typeof(GameObject))
            {
                return ToProvider(
                    new EmptyGameObjectProvider(Container, gameObjectName));
            }

            BindingUtil.AssertIsComponent(concreteType);
            BindingUtil.AssertIsNotAbstract(concreteType);

            return ToProvider(
                new AddToNewGameObjectComponentProvider(
                    Container, concreteType, null,
                    new List<TypeValuePair>(), gameObjectName));
        }

        public ConditionBinder ToComponentSelf(GameObject gameObject)
        {
            BindingUtil.AssertIsValidGameObject(gameObject, Container.IsValidating);
            BindingUtil.AssertIsComponent(ContractType);
            BindingUtil.AssertIsNotAbstract(ContractType);

            return ToProvider(
                new AddToExistingGameObjectComponentProvider(
                    gameObject, Container, ContractType,
                    null, new List<TypeValuePair>()));
        }

        public ConditionBinder ToComponentSelf(Func<InjectContext, GameObject> gameObjectGetter)
        {
            BindingUtil.AssertIsComponent(ContractType);
            BindingUtil.AssertIsNotAbstract(ContractType);

            return ToProvider(
                new AddToCustomGameObjectComponentProvider(
                    gameObjectGetter, Container, ContractType,
                    null, new List<TypeValuePair>()));
        }

        public ConditionBinder ToComponent<TConcrete>(GameObject gameObject)
            where TConcrete : TContract
        {
            return ToComponent(typeof(TConcrete), gameObject);
        }

        public ConditionBinder ToComponent<TConcrete>(Func<InjectContext, GameObject> gameObjectGetter)
            where TConcrete : TContract
        {
            return ToComponent(typeof(TConcrete), gameObjectGetter);
        }

        public ConditionBinder ToComponent(Type concreteType, GameObject gameObject)
        {
            BindingUtil.AssertIsValidGameObject(gameObject, Container.IsValidating);
            BindingUtil.AssertIsDerivedFromType(concreteType, ContractType);
            BindingUtil.AssertIsComponent(concreteType);
            BindingUtil.AssertIsNotAbstract(concreteType);

            return ToProvider(
                new AddToExistingGameObjectComponentProvider(
                    gameObject, Container, concreteType,
                    null, new List<TypeValuePair>()));
        }

        public ConditionBinder ToComponent(
            Type concreteType, Func<InjectContext, GameObject> gameObjectGetter)
        {
            BindingUtil.AssertIsDerivedFromType(concreteType, ContractType);
            BindingUtil.AssertIsComponent(concreteType);
            BindingUtil.AssertIsNotAbstract(concreteType);

            return ToProvider(
                new AddToCustomGameObjectComponentProvider(
                    gameObjectGetter, Container, concreteType,
                    null, new List<TypeValuePair>()));
        }

        public ConditionBinder ToPrefabSelf(GameObject prefab)
        {
            return ToPrefabSelf(prefab, null);
        }

        public ConditionBinder ToPrefabSelf(GameObject prefab, string gameObjectName)
        {
            var prefabInstantiator = new PrefabInstantiator(
                Container, gameObjectName,
                new List<TypeValuePair>(), prefab);

            if (ContractType == typeof(GameObject))
            {
                return ToProvider(
                    new PrefabGameObjectProvider(prefabInstantiator));
            }

            BindingUtil.AssertIsAbstractOrComponent(ContractType);
            BindingUtil.AssertIsValidPrefab(prefab);

            return ToProvider(
                new GetFromPrefabComponentProvider(
                    ContractType, prefabInstantiator));
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public ConditionBinder ToPrefab<TConcrete>(GameObject prefab)
            where TConcrete : TContract
        {
            return ToPrefab(typeof(TConcrete), prefab);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public ConditionBinder ToPrefab<TConcrete>(GameObject prefab, string gameObjectName)
            where TConcrete : TContract
        {
            return ToPrefab(typeof(TConcrete), prefab, gameObjectName);
        }

        public ConditionBinder ToPrefab(
            Type concreteType, GameObject prefab)
        {
            return ToPrefab(concreteType, prefab, null);
        }

        public ConditionBinder ToPrefab(
            Type concreteType, GameObject prefab, string gameObjectName)
        {
            BindingUtil.AssertIsDerivedFromType(concreteType, ContractType);
            BindingUtil.AssertIsValidPrefab(prefab);

            var prefabInstantiator = new PrefabInstantiator(
                Container, gameObjectName,
                new List<TypeValuePair>(), prefab);

            if (concreteType == typeof(GameObject))
            {
                return ToProvider(
                    new PrefabGameObjectProvider(prefabInstantiator));
            }

            BindingUtil.AssertIsAbstractOrComponent(ContractType);

            return ToProvider(
                new GetFromPrefabComponentProvider(
                    concreteType, prefabInstantiator));
        }

        public ConditionBinder ToPrefabResourceSelf(string resourcePath)
        {
            return ToPrefabResourceSelf(resourcePath, null);
        }

        public ConditionBinder ToPrefabResourceSelf(string resourcePath, string gameObjectName)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);

            var prefabInstantiator = new PrefabInstantiatorResource(
                Container, gameObjectName,
                new List<TypeValuePair>(), resourcePath);

            if (ContractType == typeof(GameObject))
            {
                return ToProvider(
                    new PrefabGameObjectProvider(prefabInstantiator));
            }

            BindingUtil.AssertIsAbstractOrComponent(ContractType);

            return ToProvider(
                new GetFromPrefabComponentProvider(
                    ContractType, prefabInstantiator));
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public ConditionBinder ToPrefabResource<TConcrete>(string resourcePath)
            where TConcrete : TContract
        {
            return ToPrefabResource(typeof(TConcrete), resourcePath);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public ConditionBinder ToPrefabResource<TConcrete>(string resourcePath, string gameObjectName)
            where TConcrete : TContract
        {
            return ToPrefabResource(typeof(TConcrete), resourcePath, gameObjectName);
        }

        public ConditionBinder ToPrefabResource(Type concreteType, string resourcePath)
        {
            return ToPrefabResource(concreteType, resourcePath, null);
        }

        public ConditionBinder ToPrefabResource(
            Type concreteType, string resourcePath, string gameObjectName)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);

            var prefabInstantiator = new PrefabInstantiatorResource(
                Container, gameObjectName,
                new List<TypeValuePair>(), resourcePath);

            if (concreteType == typeof(GameObject))
            {
                return ToProvider(
                    new PrefabGameObjectProvider(prefabInstantiator));
            }

            BindingUtil.AssertIsAbstractOrComponent(concreteType);

            return ToProvider(
                new GetFromPrefabComponentProvider(
                    concreteType, prefabInstantiator));
        }
#endif
    }
}
