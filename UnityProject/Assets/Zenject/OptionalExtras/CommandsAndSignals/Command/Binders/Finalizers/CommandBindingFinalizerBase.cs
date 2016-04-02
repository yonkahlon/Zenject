using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;
using Zenject.Commands;

namespace Zenject
{
    // Zero Parameters

    public abstract class CommandBindingFinalizerBase<TCommand, THandler, TAction>
        : ProviderBindingFinalizer
        where TCommand : ICommand
    {
        readonly Func<IProvider> _handlerProviderFactory;

        public CommandBindingFinalizerBase(
            Func<IProvider> handlerProviderFactory)
        {
            _handlerProviderFactory = handlerProviderFactory;
        }

        public override void FinalizeBinding()
        {
            Assert.That(Binding.ContractTypes.IsLength(1));
            Assert.IsEqual(Binding.ContractTypes.Single(), typeof(TCommand));

            // Note that the singleton here applies to the handler, not the command class
            // The command itself is always cached
            RegisterProvider<TCommand>(
                new CachedProvider(
                    new TransientProvider(
                        typeof(TCommand), Container,
                        InjectUtil.CreateArgListExplicit(GetCommandAction()), null)));
        }

        // The returned delegate is executed every time the command is executed
        TAction GetCommandAction()
        {
            var handlerProvider = GetHandlerProvider();
            var handlerInjectContext = new InjectContext(Container, typeof(THandler));

            return GetCommandAction(handlerProvider, handlerInjectContext);
        }

        IProvider GetHandlerProvider()
        {
            switch (Binding.CreationType)
            {
                case CreationTypes.Singleton:
                {
                    return Container.SingletonProviderCreator.CreateProviderStandard(
                        new StandardSingletonDeclaration(
                            typeof(THandler),
                            Binding.ConcreteIdentifier,
                            SingletonTypes.To,
                            null),
                        (type) => _handlerProviderFactory());
                }
                case CreationTypes.Transient:
                {
                    return _handlerProviderFactory();
                }
                case CreationTypes.Cached:
                {
                    return new CachedProvider(
                        _handlerProviderFactory());
                }
            }

            throw Assert.CreateException();
        }

        protected abstract TAction GetCommandAction(
            IProvider handlerProvider, InjectContext handlerContext);
    }
}

