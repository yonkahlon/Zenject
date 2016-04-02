using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;

namespace Zenject
{
    public class FactoryBinder<TParam1, TContract, TFactory> : FactoryBinderBase<TContract, TFactory>
        where TFactory : Factory<TParam1, TContract>
    {
        public FactoryBinder(
            string identifier, DiContainer container)
            : base(identifier, container)
        {
        }

        public ConditionBinder ToMethod<TConcrete>(Func<DiContainer, TParam1, TConcrete> method)
            where TConcrete : TContract
        {
            BindingUtil.AssertIsDerivedFromType(typeof(TConcrete), ContractType);

            return ToProvider(
                new MethodProviderWithContainer<TParam1, TConcrete>(method));
        }

        public ConditionBinder ToSubContainer(
            Type concreteType, Action<DiContainer, TParam1> installerFunc)
        {
            return ToSubContainer(concreteType, installerFunc, null);
        }

        public ConditionBinder ToSubContainer(
            Type concreteType, Action<DiContainer, TParam1> installerFunc, string identifier)
        {
            BindingUtil.AssertIsDerivedFromType(concreteType, ContractType);

            return ToProvider(
                new SubContainerDependencyProvider(
                    concreteType, identifier,
                    new SubContainerCreatorByMethod<TParam1>(
                        Container, installerFunc)));
        }

        public ConditionBinder ToSubContainer<TConcrete>(Action<DiContainer, TParam1> installerFunc)
            where TConcrete : TContract
        {
            return ToSubContainer(typeof(TConcrete), installerFunc);
        }

        public ConditionBinder ToSubContainerSelf(Action<DiContainer, TParam1> installerFunc)
        {
            return ToSubContainerSelf(installerFunc, null);
        }

        public ConditionBinder ToSubContainerSelf(
            Action<DiContainer, TParam1> installerFunc, string identifier)
        {
            return ToProvider(
                new SubContainerDependencyProvider(
                    ContractType, identifier,
                    new SubContainerCreatorByMethod<TParam1>(
                        Container, installerFunc)));
        }

        public ConditionBinder ToFactorySelf<TSubFactory>()
            where TSubFactory : IFactory<TParam1, TContract>
        {
            return ToProvider(
                new FactoryProvider<TParam1, TContract, TSubFactory>(Container));
        }

        public ConditionBinder ToFactory<TConcrete, TSubFactory>()
            where TSubFactory : IFactory<TParam1, TConcrete>
            where TConcrete : TContract
        {
            return ToProvider(
                new FactoryProvider<TParam1, TConcrete, TSubFactory>(Container));
        }
    }
}


