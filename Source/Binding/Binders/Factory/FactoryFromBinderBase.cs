using System;
using System.Collections.Generic;
using ModestTree;

#if !NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public class FactoryFromBinderBase<TContract> : ArgConditionCopyNonLazyBinder
    {
        public FactoryFromBinderBase(
            BindInfo bindInfo,
            Type factoryType,
            BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo)
        {
            // Note that it doesn't derive from Factory<TContract>
            // when used with To<>, so we can only check IDynamicFactory
            Assert.That(factoryType.DerivesFrom<IDynamicFactory>());

            FactoryType = factoryType;
            FinalizerWrapper = finalizerWrapper;

            // Default to just creating it using new
            finalizerWrapper.SubFinalizer = CreateFinalizer(
                (container) => new TransientProvider(ContractType, container, BindInfo.Arguments, null, BindInfo.ContextInfo));
        }

        protected Type FactoryType
        {
            get;
            private set;
        }

        protected Type ContractType
        {
            get
            {
                return typeof(TContract);
            }
        }

        protected BindFinalizerWrapper FinalizerWrapper
        {
            get;
            private set;
        }

        protected IBindingFinalizer SubFinalizer
        {
            set
            {
                FinalizerWrapper.SubFinalizer = value;
            }
        }

        public IEnumerable<Type> AllParentTypes
        {
            get
            {
                yield return ContractType;

                foreach (var type in BindInfo.ToTypes)
                {
                    yield return type;
                }
            }
        }

        protected IBindingFinalizer CreateFinalizer(Func<DiContainer, IProvider> providerFunc)
        {
            return new DynamicFactoryBindingFinalizer<TContract>(
                BindInfo, FactoryType, providerFunc);
        }

        // Note that this isn't necessary to call since it's the default
        public ConditionCopyNonLazyBinder FromNew()
        {
            BindingUtil.AssertIsNotComponent(ContractType);
            BindingUtil.AssertIsNotAbstract(ContractType);

            return this;
        }

        public ConditionCopyNonLazyBinder FromResolve()
        {
            return FromResolve(null);
        }

        public ConditionCopyNonLazyBinder FromResolve(object subIdentifier)
        {
            SubFinalizer = CreateFinalizer(
                (container) => new ResolveProvider(
                    ContractType, container, subIdentifier, false));

            return this;
        }

#if !NOT_UNITY3D

        public NameTransformConditionCopyNonLazyBinder FromGameObject()
        {
            var gameObjectInfo = new GameObjectCreationParameters();

            if (ContractType == typeof(GameObject))
            {
                SubFinalizer = CreateFinalizer(
                    (container) => new EmptyGameObjectProvider(
                        container, gameObjectInfo));
            }
            else
            {
                BindingUtil.AssertIsComponent(ContractType);
                BindingUtil.AssertIsNotAbstract(ContractType);

                SubFinalizer = CreateFinalizer(
                    (container) => new AddToNewGameObjectComponentProvider(
                        container, ContractType, null,
                        new List<TypeValuePair>(), gameObjectInfo));
            }

            return new NameTransformConditionCopyNonLazyBinder(BindInfo, gameObjectInfo);
        }

        public ConditionCopyNonLazyBinder FromComponent(GameObject gameObject)
        {
            BindingUtil.AssertIsValidGameObject(gameObject);
            BindingUtil.AssertIsComponent(ContractType);
            BindingUtil.AssertIsNotAbstract(ContractType);

            SubFinalizer = CreateFinalizer(
                (container) => new AddToExistingGameObjectComponentProvider(
                    gameObject, container, ContractType,
                    null, new List<TypeValuePair>()));

            return this;
        }

        public NameTransformConditionCopyNonLazyBinder FromPrefab(UnityEngine.Object prefab)
        {
            BindingUtil.AssertIsValidPrefab(prefab);

            var gameObjectInfo = new GameObjectCreationParameters();

            if (ContractType == typeof(GameObject))
            {
                SubFinalizer = CreateFinalizer(
                    (container) => new PrefabGameObjectProvider(
                        new PrefabInstantiator(
                            container, gameObjectInfo,
                            new List<TypeValuePair>(), new PrefabProvider(prefab))));
            }
            else
            {
                BindingUtil.AssertIsAbstractOrComponent(ContractType);

                SubFinalizer = CreateFinalizer(
                    (container) => new GetFromPrefabComponentProvider(
                        ContractType,
                        new PrefabInstantiator(
                            container, gameObjectInfo,
                            new List<TypeValuePair>(), new PrefabProvider(prefab))));
            }

            return new NameTransformConditionCopyNonLazyBinder(BindInfo, gameObjectInfo);
        }

        public NameTransformConditionCopyNonLazyBinder FromPrefabResource(string resourcePath)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);

            var gameObjectInfo = new GameObjectCreationParameters();

            if (ContractType == typeof(GameObject))
            {
                SubFinalizer = CreateFinalizer(
                    (container) => new PrefabGameObjectProvider(
                        new PrefabInstantiator(
                            container, gameObjectInfo,
                            new List<TypeValuePair>(), new PrefabProviderResource(resourcePath))));
            }
            else
            {
                BindingUtil.AssertIsAbstractOrComponent(ContractType);

                SubFinalizer = CreateFinalizer(
                    (container) => new GetFromPrefabComponentProvider(
                        ContractType,
                        new PrefabInstantiator(
                            container, gameObjectInfo,
                            new List<TypeValuePair>(), new PrefabProviderResource(resourcePath))));
            }

            return new NameTransformConditionCopyNonLazyBinder(BindInfo, gameObjectInfo);
        }
#endif
    }
}
