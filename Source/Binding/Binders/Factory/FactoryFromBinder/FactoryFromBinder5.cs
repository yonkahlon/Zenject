
using System;
using ModestTree;
using System.Linq;

namespace Zenject
{
    public class FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract> : FactoryFromBinderWithParams<TContract>
    {
        public FactoryFromBinder(BindInfo bindInfo, Type factoryType, BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo, factoryType, finalizerWrapper)
        {
        }

        public ConditionBinder FromMethod(ModestTree.Util.Func<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TContract> method)
        {
            SubFinalizer = CreateFinalizer(
                (container) => new MethodProviderWithContainer<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>(method));

            return this;
        }

        public ConditionBinder FromFactory<TSubFactory>()
            where TSubFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>
        {
            SubFinalizer = CreateFinalizer(
                (container) => new FactoryProvider<TParam1, TParam2, TParam3, TParam4, TParam5, TContract, TSubFactory>(container));

            return this;
        }

        public FactorySubContainerBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract> FromSubContainerResolve()
        {
            return FromSubContainerResolve(null);
        }

        public FactorySubContainerBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract> FromSubContainerResolve(string subIdentifier)
        {
            return new FactorySubContainerBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>(
                BindInfo, FactoryType, FinalizerWrapper, subIdentifier);
        }
    }
}



