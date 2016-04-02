using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;

namespace Zenject
{
    public abstract class ConcreteBindingFinalizerBase : ProviderBindingFinalizer
    {
        readonly List<Type> _concreteTypes;

        public ConcreteBindingFinalizerBase(List<Type> concreteTypes)
        {
            _concreteTypes = concreteTypes;
        }

        protected void RegisterProvidersPerContractAndConcreteType(
            Func<Type, Type, IProvider> providerFunc)
        {
            Assert.That(!Binding.ContractTypes.IsEmpty());
            Assert.That(!_concreteTypes.IsEmpty());

            foreach (var contractType in Binding.ContractTypes)
            {
                foreach (var concreteType in _concreteTypes)
                {
                    Assert.DerivesFromOrEqual(concreteType, contractType);

                    RegisterProvider(
                        contractType, providerFunc(contractType, concreteType));
                }
            }
        }

        // Note that if multiple contract types are provided per concrete type,
        // it will re-use the same provider for each contract type
        // (each concrete type will have its own provider though)
        protected void RegisterProvidersForAllContractsPerConcreteType(
            Func<Type, IProvider> providerFunc)
        {
            Assert.That(!Binding.ContractTypes.IsEmpty());
            Assert.That(!_concreteTypes.IsEmpty());

            var providerMap = _concreteTypes.ToDictionary(x => x, x => providerFunc(x));

            foreach (var contractType in Binding.ContractTypes)
            {
                foreach (var concreteType in _concreteTypes)
                {
                    Assert.DerivesFromOrEqual(concreteType, contractType);

                    RegisterProvider(contractType, providerMap[concreteType]);
                }
            }
        }
    }
}
