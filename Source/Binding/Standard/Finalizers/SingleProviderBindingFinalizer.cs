using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public class SingleProviderBindingFinalizer : ProviderBindingFinalizer
    {
        readonly IProvider _provider;

        public SingleProviderBindingFinalizer(IProvider provider)
        {
            _provider = provider;
        }

        public override void FinalizeBinding()
        {
            Assert.IsEqual(Binding.CreationType, CreationTypes.Transient);

            RegisterProviderForAllContracts(_provider);
        }
    }
}

