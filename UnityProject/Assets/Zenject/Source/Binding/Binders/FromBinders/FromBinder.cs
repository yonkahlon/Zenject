using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;

#if !NOT_UNITY3D
using UnityEngine;
#endif

using Zenject.Internal;

namespace Zenject
{
    public abstract class FromBinder : ScopeArgConditionCopyNonLazyBinder
    {
        public FromBinder(
            BindInfo bindInfo,
            BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo)
        {
            FinalizerWrapper = finalizerWrapper;
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

        protected IEnumerable<Type> AllParentTypes
        {
            get
            {
                return BindInfo.ContractTypes.Concat(BindInfo.ToTypes);
            }
        }

        protected IEnumerable<Type> ConcreteTypes
        {
            get
            {
                if (BindInfo.ToChoice == ToChoices.Self)
                {
                    return BindInfo.ContractTypes;
                }

                Assert.IsNotEmpty(BindInfo.ToTypes);
                return BindInfo.ToTypes;
            }
        }

        // This is the default if nothing else is called
        public ScopeArgConditionCopyNonLazyBinder FromNew()
        {
            BindingUtil.AssertTypesAreNotComponents(ConcreteTypes);
            BindingUtil.AssertTypesAreNotAbstract(ConcreteTypes);

            return this;
        }

        public ScopeConditionCopyNonLazyBinder FromResolve()
        {
            return FromResolve(null);
        }

        public ScopeConditionCopyNonLazyBinder FromResolve(object subIdentifier)
        {
            SubFinalizer = new ScopableBindingFinalizer(
                BindInfo,
                SingletonTypes.ToResolve, subIdentifier,
                (container, type) => new ResolveProvider(type, container, subIdentifier, false));

            return new ScopeConditionCopyNonLazyBinder(BindInfo);
        }

        public SubContainerBinder FromSubContainerResolve()
        {
            return FromSubContainerResolve(null);
        }

        public SubContainerBinder FromSubContainerResolve(object subIdentifier)
        {
            return new SubContainerBinder(
                BindInfo, FinalizerWrapper, subIdentifier);
        }

        public ScopeArgConditionCopyNonLazyBinder FromFactory(Type factoryType)
        {
            Assert.That(factoryType.DerivesFrom<IFactory>());

            SubFinalizer = new ScopableBindingFinalizer(
                BindInfo,
                SingletonTypes.ToFactory, factoryType,
                (container, type) => new UntypedFactoryProvider(
                    factoryType, container, BindInfo.Arguments));

            return new ScopeArgConditionCopyNonLazyBinder(BindInfo);
        }

#if !NOT_UNITY3D

        public ScopeArgConditionCopyNonLazyBinder FromComponent(GameObject gameObject)
        {
            BindingUtil.AssertIsValidGameObject(gameObject);
            BindingUtil.AssertIsComponent(ConcreteTypes);
            BindingUtil.AssertTypesAreNotAbstract(ConcreteTypes);

            SubFinalizer = new ScopableBindingFinalizer(
                BindInfo, SingletonTypes.ToComponentGameObject, gameObject,
                (container, type) => new AddToExistingGameObjectComponentProvider(
                    gameObject, container, type, BindInfo.ConcreteIdentifier, BindInfo.Arguments));

            return new ScopeArgConditionCopyNonLazyBinder(BindInfo);
        }

        public ArgConditionCopyNonLazyBinder FromSiblingComponent()
        {
            BindingUtil.AssertIsComponent(ConcreteTypes);
            BindingUtil.AssertTypesAreNotAbstract(ConcreteTypes);

            SubFinalizer = new SingleProviderBindingFinalizer(
                BindInfo, (container, type) => new AddToCurrentGameObjectComponentProvider(
                    container, type, BindInfo.ConcreteIdentifier, BindInfo.Arguments));

            return new ArgConditionCopyNonLazyBinder(BindInfo);
        }

        public NameTransformScopeArgConditionCopyNonLazyBinder FromGameObject()
        {
            return FromGameObject(new GameObjectCreationParameters());
        }

        public NameTransformScopeArgConditionCopyNonLazyBinder FromGameObject(
            GameObjectCreationParameters gameObjectInfo)
        {
            BindingUtil.AssertIsAbstractOrComponentOrGameObject(BindInfo.ContractTypes);
            BindingUtil.AssertIsComponentOrGameObject(ConcreteTypes);

            if (ConcreteTypes.All(x => x == typeof(GameObject)))
            {
                SubFinalizer = new ScopableBindingFinalizer(
                    BindInfo, SingletonTypes.ToGameObject, gameObjectInfo,
                    (container, type) =>
                    {
                        Assert.That(BindInfo.Arguments.IsEmpty(), "Cannot inject arguments into empty game object");
                        return new EmptyGameObjectProvider(
                            container, gameObjectInfo);
                    });
            }
            else
            {
                BindingUtil.AssertIsComponent(ConcreteTypes);
                BindingUtil.AssertTypesAreNotAbstract(ConcreteTypes);

                SubFinalizer = new ScopableBindingFinalizer(
                    BindInfo, SingletonTypes.ToGameObject, gameObjectInfo,
                    (container, type) => new AddToNewGameObjectComponentProvider(
                        container,
                        type,
                        BindInfo.ConcreteIdentifier,
                        BindInfo.Arguments,
                        gameObjectInfo));
            }

            return new NameTransformScopeArgConditionCopyNonLazyBinder(BindInfo, gameObjectInfo);
        }

        public NameTransformScopeArgConditionCopyNonLazyBinder FromPrefab(UnityEngine.Object prefab)
        {
            return FromPrefab(
                prefab, new GameObjectCreationParameters());
        }

        public NameTransformScopeArgConditionCopyNonLazyBinder FromPrefab(
            UnityEngine.Object prefab, GameObjectCreationParameters gameObjectInfo)
        {
            BindingUtil.AssertIsValidPrefab(prefab);
            BindingUtil.AssertIsAbstractOrComponentOrGameObject(AllParentTypes);

            SubFinalizer = new PrefabBindingFinalizer(
                BindInfo, gameObjectInfo, prefab);

            return new NameTransformScopeArgConditionCopyNonLazyBinder(BindInfo, gameObjectInfo);
        }

        public NameTransformScopeArgConditionCopyNonLazyBinder FromPrefabResource(string resourcePath)
        {
            return FromPrefabResource(resourcePath, new GameObjectCreationParameters());
        }

        public NameTransformScopeArgConditionCopyNonLazyBinder FromPrefabResource(
            string resourcePath, GameObjectCreationParameters gameObjectInfo)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);
            BindingUtil.AssertIsAbstractOrComponentOrGameObject(AllParentTypes);

            SubFinalizer = new PrefabResourceBindingFinalizer(
                BindInfo, gameObjectInfo, resourcePath);

            return new NameTransformScopeArgConditionCopyNonLazyBinder(BindInfo, gameObjectInfo);
        }

        public ScopeConditionCopyNonLazyBinder FromResource(string resourcePath)
        {
            BindingUtil.AssertDerivesFromUnityObject(ConcreteTypes);

            SubFinalizer = new ScopableBindingFinalizer(
                BindInfo,
                SingletonTypes.ToResource,
                resourcePath.ToLower(),
                (_, type) => new ResourceProvider(resourcePath, type));

            return new ScopeConditionCopyNonLazyBinder(BindInfo);
        }

#endif

        public ScopeArgConditionCopyNonLazyBinder FromMethodUntyped(Func<InjectContext, object> method)
        {
            SubFinalizer = new ScopableBindingFinalizer(
                BindInfo,
                SingletonTypes.ToMethod, new SingletonImplIds.ToMethod(method),
                (container, type) => new MethodProviderUntyped(method, container));

            return this;
        }

        protected ScopeArgConditionCopyNonLazyBinder FromMethodBase<TConcrete>(Func<InjectContext, TConcrete> method)
        {
            BindingUtil.AssertIsDerivedFromTypes(typeof(TConcrete), AllParentTypes);

            SubFinalizer = new ScopableBindingFinalizer(
                BindInfo,
                SingletonTypes.ToMethod, new SingletonImplIds.ToMethod(method),
                (container, type) => new MethodProvider<TConcrete>(method, container));

            return this;
        }

        protected ScopeArgConditionCopyNonLazyBinder FromFactoryBase<TConcrete, TFactory>()
            where TFactory : IFactory<TConcrete>
        {
            BindingUtil.AssertIsDerivedFromTypes(typeof(TConcrete), AllParentTypes);

            SubFinalizer = new ScopableBindingFinalizer(
                BindInfo,
                SingletonTypes.ToFactory, typeof(TFactory),
                (container, type) => new FactoryProvider<TConcrete, TFactory>(container, BindInfo.Arguments));

            return new ScopeArgConditionCopyNonLazyBinder(BindInfo);
        }

        protected ScopeConditionCopyNonLazyBinder FromResolveGetterBase<TObj, TResult>(
            object identifier, Func<TObj, TResult> method)
        {
            BindingUtil.AssertIsDerivedFromTypes(typeof(TResult), AllParentTypes);

            SubFinalizer = new ScopableBindingFinalizer(
                BindInfo,
                SingletonTypes.ToGetter,
                new SingletonImplIds.ToGetter(identifier, method),
                (container, type) => new GetterProvider<TObj, TResult>(identifier, method, container));

            return new ScopeConditionCopyNonLazyBinder(BindInfo);
        }

        protected ScopeConditionCopyNonLazyBinder FromInstanceBase(object instance, bool allowNull)
        {
            if (!allowNull)
            {
                Assert.That(!ZenUtilInternal.IsNull(instance),
                    "Found null instance for type '{0}' in FromInstance method",
                    ConcreteTypes.First().Name());
            }

            BindingUtil.AssertInstanceDerivesFromOrEqual(instance, AllParentTypes);

            SubFinalizer = new ScopableBindingFinalizer(
                BindInfo, SingletonTypes.ToInstance, instance,
                (container, type) => new InstanceProvider(container, type, instance));

            return new ScopeConditionCopyNonLazyBinder(BindInfo);
        }
    }
}
