using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public class SelfBindingFinalizer : ProviderBindingFinalizer
    {
        readonly SingletonTypes _singletonType;
        readonly Func<Type, IProvider> _providerFactory;
        readonly object _singletonSpecificId;

        public SelfBindingFinalizer(
            SingletonTypes singletonType, object singletonSpecificId,
            Func<Type, IProvider> providerFactory)
        {
            _singletonType = singletonType;
            _providerFactory = providerFactory;
            _singletonSpecificId = singletonSpecificId;
        }

        public override void FinalizeBinding()
        {
            switch (Binding.CreationType)
            {
                case CreationTypes.Singleton:
                {
                    RegisterProviderPerContract(
                        (contractType) => Container.SingletonProviderCreator.CreateProviderStandard(
                            new StandardSingletonDeclaration(
                                contractType,
                                Binding.ConcreteIdentifier,
                                _singletonType,
                                _singletonSpecificId),
                            _providerFactory));
                    break;
                }
                case CreationTypes.Transient:
                {
                    RegisterProviderPerContract(_providerFactory);
                    break;
                }
                case CreationTypes.Cached:
                {
                    RegisterProviderPerContract(
                        (contractType) =>
                            new CachedProvider(
                                _providerFactory(contractType)));
                    break;
                }
                default:
                {
                    throw Assert.CreateException();
                }
            }
        }
    }
}
