using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public class SubContainerInstallerConcreteBindingFinalizer : ConcreteBindingFinalizerBase
    {
        readonly Type _installerType;
        readonly string _identifier;

        public SubContainerInstallerConcreteBindingFinalizer(
            List<Type> concreteTypes, Type installerType, string identifier)
            : base(concreteTypes)
        {
            _installerType = installerType;
            _identifier = identifier;
        }

        ISubContainerCreator CreateContainerCreator()
        {
            return new SubContainerCreatorCached(
                new SubContainerCreatorByInstaller(Container, _installerType));
        }

        public override void FinalizeBinding()
        {
            switch (Binding.CreationType)
            {
                case CreationTypes.Singleton:
                {
                    RegisterProvidersForAllContractsPerConcreteType(
                        (concreteType) =>
                            Container.SingletonProviderCreator.CreateProviderForSubContainerInstaller(
                                concreteType, Binding.ConcreteIdentifier, _installerType, _identifier));
                    break;
                }
                case CreationTypes.Transient:
                {
                    RegisterProvidersForAllContractsPerConcreteType(
                        (concreteType) =>
                            new SubContainerDependencyProvider(
                                concreteType, _identifier, CreateContainerCreator()));
                    break;
                }
                case CreationTypes.Cached:
                {
                    var containerCreator = CreateContainerCreator();

                    RegisterProvidersForAllContractsPerConcreteType(
                        (concreteType) =>
                            new SubContainerDependencyProvider(
                                concreteType, _identifier, containerCreator));
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
