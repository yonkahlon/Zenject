using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using System.Linq;

namespace Zenject.Commands
{
    public abstract class CommandProviderSingleBase<TCommand, THandler, TAction>
        : CommandProviderBase<TCommand, TAction>
        where TCommand : ICommand
    {
        readonly DiContainer _container;
        readonly ProviderBase _singletonProvider;

        public CommandProviderSingleBase(
            DiContainer container,
            ProviderBase singletonProvider)
        {
            _container = container;
            _singletonProvider = singletonProvider;
        }

        public override void Dispose()
        {
            _singletonProvider.Dispose();
        }

        protected THandler GetSingleton(InjectContext c)
        {
            return (THandler)_singletonProvider.GetInstance(GetNewInjectContext(c));
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return base.ValidateBinding(context)
                .Concat(_singletonProvider.ValidateBinding(GetNewInjectContext(context)));
        }

        InjectContext GetNewInjectContext(InjectContext c)
        {
            return new InjectContext(
                _container, typeof(THandler), null, false, c.ObjectType,
                c.ObjectInstance, c.MemberName, c.ParentContext, c.ConcreteIdentifier,
                null, c.SourceType);
        }
    }
}

