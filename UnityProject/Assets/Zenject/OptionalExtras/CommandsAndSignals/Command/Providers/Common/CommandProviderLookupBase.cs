
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using System.Linq;

namespace Zenject.Commands
{
    public abstract class CommandProviderResolveBase<TCommand, THandler, TAction>
        : CommandProviderBase<TCommand, TAction>
        where TCommand : ICommand
    {
        readonly string _identifier;
        readonly bool _isOptional;
        readonly DiContainer _container;
        readonly ContainerTypes _containerType;

        public CommandProviderResolveBase(
            DiContainer container, ContainerTypes containerType, string identifier, bool isOptional)
        {
            _container = container;
            _containerType = containerType;
            _identifier = identifier;
            _isOptional = isOptional;
        }

        protected THandler TryGetResolve(InjectContext ctx)
        {
            var container = _containerType == ContainerTypes.RuntimeContainer ? ctx.Container : _container;
            return container.Resolve<THandler>(GetNewInjectContext(container, ctx));
        }

        InjectContext GetNewInjectContext(DiContainer container, InjectContext ctx)
        {
            return new InjectContext(
                container, typeof(THandler), _identifier,
                _isOptional, ctx.ObjectType, ctx.ObjectInstance, ctx.MemberName,
                ctx, null, ctx.FallBackValue, ctx.SourceType);
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext ctx)
        {
            var container = _containerType == ContainerTypes.RuntimeContainer ? ctx.Container : _container;
            return base.ValidateBinding(ctx)
                .Concat(container.ValidateResolve(GetNewInjectContext(container, ctx)));
        }
    }
}

