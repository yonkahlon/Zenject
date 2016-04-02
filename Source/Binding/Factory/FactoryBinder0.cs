using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;

namespace Zenject
{
    public class FactoryBinder<TContract, TFactory> : FactoryBinderBase<TContract, TFactory>
        where TFactory : Factory<TContract>
    {
        public FactoryBinder(
            string identifier, DiContainer container)
            : base(identifier, container)
        {
        }

        public ConditionBinder ToResolveSelf()
        {
            return ToResolveSelf(null);
        }

        public ConditionBinder ToResolveSelf(string identifier)
        {
            return ToProvider(new ResolveProvider(
                ContractType, Container, identifier, false));
        }

        public ConditionBinder ToResolve<TConcrete>()
            where TConcrete : TContract
        {
            return ToResolve(typeof(TConcrete), null);
        }

        public ConditionBinder ToResolve<TConcrete>(string identifier)
            where TConcrete : TContract
        {
            return ToResolve(typeof(TConcrete), identifier);
        }

        public ConditionBinder ToResolve(Type concreteType)
        {
            return ToResolve(concreteType, null);
        }

        public ConditionBinder ToResolve(Type concreteType, string identifier)
        {
            BindingUtil.AssertIsDerivedFromType(concreteType, ContractType);

            return ToProvider(new ResolveProvider(
                concreteType, Container, identifier, false));
        }

        public ConditionBinder ToInstance<TConcrete>(TConcrete instance)
            where TConcrete : TContract
        {
            return ToInstance(
                instance == null ? typeof(TConcrete) : instance.GetType(), instance);
        }

        public ConditionBinder ToInstance(Type concreteType, object instance)
        {
            BindingUtil.AssertIsDerivedFromType(concreteType, ContractType);
            BindingUtil.AssertInstanceDerivesFromOrEqual(instance, concreteType);

            return ToProvider(
                new InstanceProvider(concreteType, instance));
        }

        public ConditionBinder ToGetterSelf<TObj>(Func<TObj, TContract> method)
        {
            return ToGetterSelf<TObj>(null, method);
        }

        public ConditionBinder ToGetterSelf<TObj>(string identifier, Func<TObj, TContract> method)
        {
            return ToProvider(
                new GetterProvider<TObj, TContract>(identifier, method, Container));
        }

        public ConditionBinder ToGetter<TConcrete, TObj>(Func<TObj, TConcrete> method)
            where TConcrete : TContract
        {
            return ToGetter<TConcrete, TObj>(null, method);
        }

        public ConditionBinder ToGetter<TConcrete, TObj>(string identifier, Func<TObj, TConcrete> method)
            where TConcrete : TContract
        {
            return ToProvider(
                new GetterProvider<TObj, TConcrete>(identifier, method, Container));
        }

        public ConditionBinder ToMethod<TConcrete>(Func<DiContainer, TConcrete> method)
            where TConcrete : TContract
        {
            BindingUtil.AssertIsDerivedFromType(typeof(TConcrete), ContractType);

            return ToProvider(new MethodProviderWithContainer<TConcrete>(method));
        }

        public ConditionBinder ToMethodSelf(Func<DiContainer, TContract> method)
        {
            return ToProvider(new MethodProviderWithContainer<TContract>(method));
        }

        public ConditionBinder ToSubContainer(
            Type concreteType, Action<DiContainer> installerFunc)
        {
            return ToSubContainer(concreteType, installerFunc, null);
        }

        public ConditionBinder ToSubContainer(
            Type concreteType, Action<DiContainer> installerFunc, string identifier)
        {
            BindingUtil.AssertIsDerivedFromType(concreteType, ContractType);

            return ToProvider(
                new SubContainerDependencyProvider(
                    concreteType, identifier,
                    new SubContainerCreatorByMethod(
                        Container, installerFunc)));
        }

        public ConditionBinder ToSubContainer<TConcrete>(Action<DiContainer> installerFunc)
            where TConcrete : TContract
        {
            return ToSubContainer(typeof(TConcrete), installerFunc);
        }

        public ConditionBinder ToSubContainerSelf(Action<DiContainer> installerFunc)
        {
            return ToSubContainerSelf(installerFunc, null);
        }

        public ConditionBinder ToSubContainerSelf(Action<DiContainer> installerFunc, string identifier)
        {
            return ToProvider(
                new SubContainerDependencyProvider(
                    ContractType, identifier,
                    new SubContainerCreatorByMethod(
                        Container, installerFunc)));
        }

        public ConditionBinder ToFactorySelf<TSubFactory>()
            where TSubFactory : IFactory<TContract>
        {
            return ToProvider(
                new FactoryProvider<TContract, TSubFactory>(Container));
        }

        public ConditionBinder ToFactory<TConcrete, TSubFactory>()
            where TSubFactory : IFactory<TConcrete>
            where TConcrete : TContract
        {
            return ToProvider(
                new FactoryProvider<TConcrete, TSubFactory>(Container));
        }

#if !ZEN_NOT_UNITY3D
        public ConditionBinder ToResourceSelf(string resourcePath)
        {
            BindingUtil.AssertDerivesFromUnityObject(ContractType);

            return ToProvider(
                new ResourceProvider(resourcePath, ContractType));
        }

        public ConditionBinder ToResource<TConcrete>(string resourcePath)
            where TConcrete : TContract
        {
            return ToResource(typeof(TConcrete), resourcePath);
        }

        public ConditionBinder ToResource(Type concreteType, string resourcePath)
        {
            BindingUtil.AssertIsDerivedFromType(concreteType, ContractType);
            BindingUtil.AssertDerivesFromUnityObject(concreteType);

            return ToProvider(
                new ResourceProvider(resourcePath, concreteType));
        }
#endif
    }
}

