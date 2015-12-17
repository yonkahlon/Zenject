using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using System.Linq;

namespace Zenject.Commands
{
    // Zero params
    public class CommandProviderHandlerSingle<TCommand, THandler>
        : CommandProviderSingle<TCommand, THandler, Action>
        where TCommand : Command
        where THandler : ICommandHandler
    {
        public CommandProviderHandlerSingle(DiContainer container, ProviderBase singletonProvider)
            : base(container, singletonProvider)
        {
        }

        protected override Action GetCommandAction(InjectContext context)
        {
            return () =>
            {
                GetSingleton(context).Execute();
            };
        }
    }

    // One param
    public class CommandProviderHandlerSingle<TCommand, THandler, TParam1>
        : CommandProviderSingle<TCommand, THandler, Action<TParam1>>
        where TCommand : Command<TParam1>
        where THandler : ICommandHandler<TParam1>
    {
        public CommandProviderHandlerSingle(DiContainer container, ProviderBase singletonProvider)
            : base(container, singletonProvider)
        {
        }

        protected override Action<TParam1> GetCommandAction(InjectContext context)
        {
            return (p1) =>
            {
                GetSingleton(context).Execute(p1);
            };
        }
    }

    // Two params
    public class CommandProviderHandlerSingle<TCommand, THandler, TParam1, TParam2>
        : CommandProviderSingle<TCommand, THandler, Action<TParam1, TParam2>>
        where TCommand : Command<TParam1, TParam2>
        where THandler : ICommandHandler<TParam1, TParam2>
    {
        public CommandProviderHandlerSingle(DiContainer container, ProviderBase singletonProvider)
            : base(container, singletonProvider)
        {
        }

        protected override Action<TParam1, TParam2> GetCommandAction(InjectContext context)
        {
            return (p1, p2) =>
            {
                GetSingleton(context).Execute(p1, p2);
            };
        }
    }
}

