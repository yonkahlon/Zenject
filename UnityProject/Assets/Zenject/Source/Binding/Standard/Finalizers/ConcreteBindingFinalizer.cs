using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;

namespace Zenject
{
    public class ConcreteBindingFinalizer : ConcreteBindingFinalizerBase
    {
        readonly Func<Type, IProvider> _providerFactory;
        readonly SingletonTypes _singletonType;
        readonly object _singletonSpecificId;

        public ConcreteBindingFinalizer(
            List<Type> concreteTypes, SingletonTypes singletonType, object singletonSpecificId,
            Func<Type, IProvider> providerFactory)
            : base(concreteTypes)
        {
            _providerFactory = providerFactory;
            _singletonType = singletonType;
            _singletonSpecificId = singletonSpecificId;
        }

        public ConcreteBindingFinalizer(
            Type concreteType, SingletonTypes singletonType, object singletonSpecificId,
            Func<Type, IProvider> providerFactory)
            : this( new List<Type>() { concreteType },
                singletonType,
                singletonSpecificId,
                providerFactory)
        {
        }

        public override void FinalizeBinding()
        {
            switch (Binding.CreationType)
            {
                case CreationTypes.Singleton:
                {
                    RegisterProvidersForAllContractsPerConcreteType(
                        (concreteType) => Container.SingletonProviderCreator.CreateProviderStandard(
                            new StandardSingletonDeclaration(
                                concreteType,
                                Binding.ConcreteIdentifier,
                                _singletonType,
                                _singletonSpecificId),
                            _providerFactory));
                    break;
                }
                case CreationTypes.Transient:
                {
                    RegisterProvidersForAllContractsPerConcreteType(_providerFactory);
                    break;
                }
                case CreationTypes.Cached:
                {
                    RegisterProvidersForAllContractsPerConcreteType(
                        (concreteType) =>
                            new CachedProvider(
                                _providerFactory(concreteType)));
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
