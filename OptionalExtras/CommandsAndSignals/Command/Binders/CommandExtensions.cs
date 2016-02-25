using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;

namespace Zenject.Commands
{
    public static class CommandExtensions
    {
        // Zero parameters
        public static CommandBinder<TCommand> BindCommand<TCommand>(this DiContainer container, string identifier)
            where TCommand : Command
        {
            return new CommandBinder<TCommand>(container, identifier, container.SingletonProviderCreator);
        }

        public static CommandBinder<TCommand> BindCommand<TCommand>(this DiContainer container)
            where TCommand : Command
        {
            return BindCommand<TCommand>((DiContainer)container, null);
        }

        // One parameter
        public static CommandBinder<TCommand, TParam1> BindCommand<TCommand, TParam1>(this DiContainer container, string identifier)
            where TCommand : Command<TParam1>
        {
            return new CommandBinder<TCommand, TParam1>(container, identifier, container.SingletonProviderCreator);
        }

        public static CommandBinder<TCommand, TParam1> BindCommand<TCommand, TParam1>(this DiContainer container)
            where TCommand : Command<TParam1>
        {
            return BindCommand<TCommand, TParam1>((DiContainer)container, null);
        }

        // Two parameters
        public static CommandBinder<TCommand, TParam1, TParam2> BindCommand<TCommand, TParam1, TParam2>(this DiContainer container, string identifier)
            where TCommand : Command<TParam1, TParam2>
        {
            return new CommandBinder<TCommand, TParam1, TParam2>(container, identifier, container.SingletonProviderCreator);
        }

        public static CommandBinder<TCommand, TParam1, TParam2> BindCommand<TCommand, TParam1, TParam2>(this DiContainer container)
            where TCommand : Command<TParam1, TParam2>
        {
            return BindCommand<TCommand, TParam1, TParam2>((DiContainer)container, null);
        }

        // Three parameters
        public static CommandBinder<TCommand, TParam1, TParam2, TParam3> BindCommand<TCommand, TParam1, TParam2, TParam3>(this DiContainer container, string identifier)
            where TCommand : Command<TParam1, TParam2, TParam3>
        {
            return new CommandBinder<TCommand, TParam1, TParam2, TParam3>(container, identifier, container.SingletonProviderCreator);
        }

        public static CommandBinder<TCommand, TParam1, TParam2, TParam3> BindCommand<TCommand, TParam1, TParam2, TParam3>(this DiContainer container)
            where TCommand : Command<TParam1, TParam2, TParam3>
        {
            return BindCommand<TCommand, TParam1, TParam2, TParam3>((DiContainer)container, null);
        }

        // Four parameters
        public static CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4> BindCommand<TCommand, TParam1, TParam2, TParam3, TParam4>(this DiContainer container, string identifier)
            where TCommand : Command<TParam1, TParam2, TParam3, TParam4>
        {
            return new CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4>(container, identifier, container.SingletonProviderCreator);
        }

        public static CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4> BindCommand<TCommand, TParam1, TParam2, TParam3, TParam4>(this DiContainer container)
            where TCommand : Command<TParam1, TParam2, TParam3, TParam4>
        {
            return BindCommand<TCommand, TParam1, TParam2, TParam3, TParam4>((DiContainer)container, null);
        }

        // Five parameters
        public static CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5> BindCommand<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5>(this DiContainer container, string identifier)
            where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            return new CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5>(container, identifier, container.SingletonProviderCreator);
        }

        public static CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5> BindCommand<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5>(this DiContainer container)
            where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            return BindCommand<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5>((DiContainer)container, null);
        }

        // Six parameters
        public static CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> BindCommand<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(this DiContainer container, string identifier)
            where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            return new CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(container, identifier, container.SingletonProviderCreator);
        }

        public static CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> BindCommand<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(this DiContainer container)
            where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            return BindCommand<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>((DiContainer)container, null);
        }
    }
}

