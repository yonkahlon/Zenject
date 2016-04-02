using System;
using System.ComponentModel;
using ModestTree;

namespace Zenject
{
    public class SubContainerInstallerSelfBindingFinalizer : ProviderBindingFinalizer
    {
        readonly string _identifier;
        readonly Type _installerType;

        public SubContainerInstallerSelfBindingFinalizer(
            Type installerType,
            string identifier)
        {
            _identifier = identifier;
            _installerType = installerType;
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
                    RegisterProviderPerContract(
                        (contractType) => Container.SingletonProviderCreator.CreateProviderForSubContainerInstaller(
                            contractType, Binding.ConcreteIdentifier, _installerType, _identifier));
                    break;
                }
                case CreationTypes.Transient:
                {
                    RegisterProviderPerContract(
                        (contractType) => new SubContainerDependencyProvider(
                            contractType, _identifier, CreateContainerCreator()));
                    break;
                }
                case CreationTypes.Cached:
                {
                    var containerCreator = CreateContainerCreator();

                    RegisterProviderPerContract(
                        (contractType) =>
                            new SubContainerDependencyProvider(
                                contractType, _identifier, containerCreator));
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

