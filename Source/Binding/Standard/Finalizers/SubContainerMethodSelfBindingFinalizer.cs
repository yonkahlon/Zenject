using System;
using System.ComponentModel;
using ModestTree;

namespace Zenject
{
    public class SubContainerMethodSelfBindingFinalizer : ProviderBindingFinalizer
    {
        readonly Action<DiContainer> _installMethod;
        readonly string _identifier;

        public SubContainerMethodSelfBindingFinalizer(
            Action<DiContainer> installMethod, string identifier)
        {
            _installMethod = installMethod;
            _identifier = identifier;
        }

        public override void FinalizeBinding()
        {
            switch (Binding.CreationType)
            {
                case CreationTypes.Singleton:
                {
                    RegisterProviderPerContract(
                        (contractType) => Container.SingletonProviderCreator.CreateProviderForSubContainerMethod(
                            contractType, Binding.ConcreteIdentifier, _installMethod, _identifier));
                    break;
                }
                case CreationTypes.Transient:
                {
                    RegisterProviderPerContract(
                        (contractType) => new SubContainerDependencyProvider(
                            contractType, _identifier,
                            new SubContainerCreatorByMethod(
                                Container, _installMethod)));
                    break;
                }
                case CreationTypes.Cached:
                {
                    var containerCreator = new SubContainerCreatorCached(
                        new SubContainerCreatorByMethod(Container, _installMethod));

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
