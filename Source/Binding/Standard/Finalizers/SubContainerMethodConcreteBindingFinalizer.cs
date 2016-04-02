using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public class SubContainerMethodConcreteBindingFinalizer : ConcreteBindingFinalizerBase
    {
        readonly Action<DiContainer> _installMethod;
        readonly string _identifier;

        public SubContainerMethodConcreteBindingFinalizer(
            List<Type> concreteTypes, Action<DiContainer> installMethod, string identifier)
            : base(concreteTypes)
        {
            _installMethod = installMethod;
            _identifier = identifier;
        }

        ISubContainerCreator CreateContainerCreator()
        {
            return new SubContainerCreatorCached(
                new SubContainerCreatorByMethod(Container, _installMethod));
        }

        public override void FinalizeBinding()
        {
            switch (Binding.CreationType)
            {
                case CreationTypes.Singleton:
                {
                    RegisterProvidersForAllContractsPerConcreteType(
                        (concreteType) =>
                            Container.SingletonProviderCreator.CreateProviderForSubContainerMethod(
                                concreteType, Binding.ConcreteIdentifier, _installMethod, _identifier));
                    break;
                }
                case CreationTypes.Transient:
                {
                    // Note: each contract/concrete pair gets its own container here
                    RegisterProvidersPerContractAndConcreteType(
                        (contractType, concreteType) => new SubContainerDependencyProvider(
                            concreteType, _identifier, CreateContainerCreator()));
                    break;
                }
                case CreationTypes.Cached:
                {
                    var creator = CreateContainerCreator();

                    // Note: each contract/concrete pair gets its own container
                    RegisterProvidersForAllContractsPerConcreteType(
                        (concreteType) => new SubContainerDependencyProvider(
                            concreteType, _identifier, creator));
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
