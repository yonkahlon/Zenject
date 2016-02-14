
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

        public CommandProviderResolveBase(string identifier, bool isOptional)
        {
            _identifier = identifier;
            _isOptional = isOptional;
        }

        protected THandler TryGetResolve(InjectContext ctx)
        {
            return ctx.Resolver.Resolve<THandler>(GetNewInjectContext(ctx));
        }

        InjectContext GetNewInjectContext(InjectContext ctx)
        {
            return new InjectContext(
                ctx.Container, typeof(THandler), _identifier,
                _isOptional, ctx.ObjectType, ctx.ObjectInstance, ctx.MemberName,
                ctx, null, ctx.FallBackValue, ctx.SourceType);
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext ctx)
        {
            return base.ValidateBinding(ctx)
                .Concat(ctx.Resolver.ValidateResolve(GetNewInjectContext(ctx)));
        }
    }
}

