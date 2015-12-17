using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using System.Linq;

namespace Zenject.Commands
{
    public abstract class CommandProviderMethodTransientBase<TCommand, THandler, TAction>
        : CommandProviderTransient<TCommand, THandler, TAction>
        where TCommand : ICommand
    {
        readonly Func<THandler, TAction> _methodGetter;

        public CommandProviderMethodTransientBase(
            DiContainer container, Func<THandler, TAction> methodGetter)
            : base(container)
        {
            _methodGetter = methodGetter;
        }

        protected TAction GetHandlerMethod(InjectContext context)
        {
            return _methodGetter(CreateHandler(context));
        }
    }

    public class CommandProviderMethodTransient<TCommand, THandler>
        : CommandProviderMethodTransientBase<TCommand, THandler, Action>
        where TCommand : Command
    {
        public CommandProviderMethodTransient(
            DiContainer container, Func<THandler, Action> methodGetter)
            : base(container, methodGetter)
        {
        }

        protected override Action GetCommandAction(InjectContext context)
        {
            return () =>
            {
                GetHandlerMethod(context)();
            };
        }
    }

    public class CommandProviderMethodTransient<TCommand, THandler, TParam1>
        : CommandProviderMethodTransientBase<TCommand, THandler, Action<TParam1>>
        where TCommand : Command<TParam1>
    {
        public CommandProviderMethodTransient(
            DiContainer container, Func<THandler, Action<TParam1>> methodGetter)
            : base(container, methodGetter)
        {
        }

        protected override Action<TParam1> GetCommandAction(InjectContext context)
        {
            return (p1) =>
            {
                GetHandlerMethod(context)(p1);
            };
        }
    }

    public class CommandProviderMethodTransient<TCommand, THandler, TParam1, TParam2>
        : CommandProviderMethodTransientBase<TCommand, THandler, Action<TParam1, TParam2>>
        where TCommand : Command<TParam1, TParam2>
    {
        public CommandProviderMethodTransient(
            DiContainer container, Func<THandler, Action<TParam1, TParam2>> methodGetter)
            : base(container, methodGetter)
        {
        }

        protected override Action<TParam1, TParam2> GetCommandAction(InjectContext context)
        {
            return (p1, p2) =>
            {
                GetHandlerMethod(context)(p1, p2);
            };
        }
    }
}

