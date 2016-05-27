using System;

namespace Zenject.Commands
{
    public class CommandProviderHandlerTransient<TCommand, THandler>
        : CommandProviderTransient<TCommand, THandler, Action>
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


