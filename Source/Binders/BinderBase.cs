using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
#if !ZEN_NOT_UNITY3D
#endif

namespace Zenject
{
    public abstract class BinderBase
    {
        readonly List<Type> _contractTypes;
        readonly DiContainer _container;
        readonly string _bindIdentifier;

        public BinderBase(
            DiContainer container, List<Type> contractTypes, string bindIdentifier)
        {
            _container = container;
            _contractTypes = contractTypes;
            _bindIdentifier = bindIdentifier;
        }

        protected List<Type> ContractTypes
        {
            get
            {
                return _contractTypes;
            }
        }

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        void RegisterProviderInternal(
            Type contractType, ProviderBase provider)
        {
            _container.RegisterProvider(
                provider, new BindingId(contractType, _bindIdentifier));

            if (contractType.IsValueType)
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(contractType);

                // Also bind to nullable primitives
                // this is useful so that we can have optional primitive dependencies
                _container.RegisterProvider(
                    provider, new BindingId(nullableType, _bindIdentifier));
            }
        }

        protected BindingConditionSetter RegisterSingleProvider(ProviderBase provider)
        {
            foreach (var contractType in ContractTypes)
            {
                RegisterProviderInternal(contractType, provider);
            }

            return new BindingConditionSetter(provider);
        }

        protected BindingConditionSetter RegisterProvidersPerContract(IEnumerable<ProviderBase> providers)
        {
            var providersList = providers.ToList();

            foreach (var pair in ContractTypes.Zipper(providersList))
            {
                RegisterProviderInternal(pair.First, pair.Second);
            }

            return new BindingConditionSetter(providersList);
        }
    }
}
