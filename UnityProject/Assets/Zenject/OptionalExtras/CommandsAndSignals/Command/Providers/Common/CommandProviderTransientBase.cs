using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using System.Linq;

namespace Zenject.Commands
{
    public abstract class CommandProviderTransientBase<TCommand, THandler, TAction>
        : CommandProviderBase<TCommand, TAction>
        where TCommand : ICommand
    {
        readonly DiContainer _container;
        readonly ContainerTypes _containerType;

        public CommandProviderTransientBase(DiContainer container, ContainerTypes containerType)
        {
            _container = container;
            _containerType = containerType;
        }

        protected THandler CreateHandler(InjectContext c)
        {
            var container = (_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container);

            return container.InstantiateExplicit<THandler>(
                new List<TypeValuePair>(), GetInjectContext(c, container));
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            var container = (_containerType == ContainerTypes.RuntimeContainer ? context.Container : _container);

            return base.ValidateBinding(context)
                .Concat(container.ValidateObjectGraph<THandler>(GetInjectContext(context, container)));
        }

        InjectContext GetInjectContext(InjectContext c, DiContainer container)
        {
            return new InjectContext(
                container, typeof(THandler), null, false, c.ObjectType,
                c.ObjectInstance, c.MemberName, c.ParentContext, c.ConcreteIdentifier,
                null, c.SourceType);
        }
    }
}


