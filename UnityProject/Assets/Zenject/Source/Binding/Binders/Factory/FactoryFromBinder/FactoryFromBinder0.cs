using System;
using System.Collections.Generic;

namespace Zenject
{
    public class FactoryFromBinder<TContract> : FactoryFromBinderBase<TContract>
    {
        public FactoryFromBinder(
            BindInfo bindInfo,
            Type factoryType,
            BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo, factoryType, finalizerWrapper)
        {
        }

        public ConditionCopyNonLazyBinder FromResolveGetter<TObj>(Func<TObj, TContract> method)
        {
            return FromResolveGetter<TObj>(null, method);
        }

        public ConditionCopyNonLazyBinder FromResolveGetter<TObj>(
            object subIdentifier, Func<TObj, TContract> method)
        {
            SubFinalizer = CreateFinalizer(
                (container) => new GetterProvider<TObj, TContract>(subIdentifier, method, container));

            return this;
        }

        public ConditionCopyNonLazyBinder FromMethod(Func<DiContainer, TContract> method)
        {
            SubFinalizer = CreateFinalizer(
                (container) => new MethodProviderWithContainer<TContract>(method));

            return this;
        }

        public ConditionCopyNonLazyBinder FromInstance(object instance)
        {
            BindingUtil.AssertInstanceDerivesFromOrEqual(instance, AllParentTypes);

            SubFinalizer = CreateFinalizer(
                (container) => new InstanceProvider(container, ContractType, instance));

            return this;
        }

        public ArgConditionCopyNonLazyBinder FromFactory<TSubFactory>()
            where TSubFactory : IFactory<TContract>
        {
            SubFinalizer = CreateFinalizer(
                (container) => new FactoryProvider<TContract, TSubFactory>(container, BindInfo.Arguments));

            return new ArgConditionCopyNonLazyBinder(BindInfo);
        }

        public FactorySubContainerBinder<TContract> FromSubContainerResolve()
        {
            return FromSubContainerResolve(null);
        }

        public FactorySubContainerBinder<TContract> FromSubContainerResolve(object subIdentifier)
        {
            return new FactorySubContainerBinder<TContract>(
                BindInfo, FactoryType, FinalizerWrapper, subIdentifier);
        }

#if !NOT_UNITY3D

        public ConditionCopyNonLazyBinder FromResource(string resourcePath)
        {
            BindingUtil.AssertDerivesFromUnityObject(ContractType);

            SubFinalizer = CreateFinalizer(
                (container) => new ResourceProvider(resourcePath, ContractType));

            return this;
        }
#endif
    }
}
