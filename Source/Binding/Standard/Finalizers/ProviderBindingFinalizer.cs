using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public abstract class ProviderBindingFinalizer : IBindingFinalizer
    {
        DiContainer _container;
        StandardBindingDescriptor _binding;

        protected DiContainer Container
        {
            get
            {
                Assert.IsNotNull(_container);
                return _container;
            }
        }

        protected StandardBindingDescriptor Binding
        {
            get
            {
                Assert.IsNotNull(_binding);
                return _binding;
            }
        }

        public void FinalizeBinding(DiContainer container, StandardBindingDescriptor binding)
        {
            if (binding.ContractTypes.IsEmpty())
            {
                // We could assert her instead but it is nice when used with things like
                // BindAllInterfaces() (and there aren't any interfaces) to allow
                // interfaces to be added later
                return;
            }

            // Allow calling multiple times for different containers
            //Assert.IsNull(_binding);
            //Assert.IsNull(_container);

            _container = container;
            _binding = binding;

            FinalizeBinding();
        }

        public abstract void FinalizeBinding();

        protected void RegisterProvider<TContract>(IProvider provider)
        {
            RegisterProvider(typeof(TContract), provider);
        }

        protected void RegisterProvider(
            Type contractType, IProvider provider)
        {
            Container.RegisterProvider(
                new BindingId(contractType, Binding.Identifier),
                Binding.Condition,
                provider);

            if (contractType.IsValueType)
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(contractType);

                // Also bind to nullable primitives
                // this is useful so that we can have optional primitive dependencies
                Container.RegisterProvider(
                    new BindingId(nullableType, Binding.Identifier),
                    Binding.Condition,
                    provider);
            }
        }

        protected void RegisterProviderPerContract(Func<Type, IProvider> providerFunc)
        {
            foreach (var contractType in Binding.ContractTypes)
            {
                RegisterProvider(contractType, providerFunc(contractType));
            }
        }

        protected void RegisterProviderForAllContracts(IProvider provider)
        {
            foreach (var contractType in Binding.ContractTypes)
            {
                RegisterProvider(contractType, provider);
            }
        }
    }
}


