using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using System.Linq;

namespace Zenject.Commands
{
    public class CommandProviderTransient<TCommand, THandler>
        : CommandProviderTransientBase<TCommand, THandler, Action>
        where TCommand : Command
        where THandler : ICommandHandler
    {
        protected override Action GetCommandAction(InjectContext context)
        {
            return () =>
            {
                CreateHandler(context).Execute();
            };
        }
    }
}


