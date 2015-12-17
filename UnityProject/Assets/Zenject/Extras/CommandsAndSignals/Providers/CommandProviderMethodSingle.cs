using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using System.Linq;

namespace Zenject.Commands
{
    // Zero params
    public class CommandProviderMethodSingle<TCommand, THandler>
        : CommandProviderSingle<TCommand, THandler, Action>
        where TCommand : Command
    {
        readonly Func<THandler, Action> _methodGetter;

        public CommandProviderMethodSingle(
            DiContainer container, Func<THandler, Action> methodGetter, ProviderBase singletonProvider)
            : base(container, singletonProvider)
        {
            _methodGetter = methodGetter;
        }

        protected override Action GetCommandAction(InjectContext context)
        {
            return () =>
            {
                var singleton = GetSingleton(context);
                Assert.IsNotNull(singleton);
                _methodGetter(singleton)();
            };
        }
    }

    // One param
    public class CommandProviderMethodSingle<TCommand, THandler, TParam1>
        : CommandProviderSingle<TCommand, THandler, Action<TParam1>>
        where TCommand : Command<TParam1>
    {
        readonly Func<THandler, Action<TParam1>> _methodGetter;

        public CommandProviderMethodSingle(
            DiContainer container,
            Func<THandler, Action<TParam1>> methodGetter, ProviderBase singletonProvider)
            : base(container, singletonProvider)
        {
            _methodGetter = methodGetter;
        }

        protected override Action<TParam1> GetCommandAction(InjectContext context)
        {
            return (p1) =>
            {
                var singleton = GetSingleton(context);
                Assert.IsNotNull(singleton);
                _methodGetter(singleton)(p1);
            };
        }
    }

    // Two params
    public class CommandProviderMethodSingle<TCommand, THandler, TParam1, TParam2>
        : CommandProviderSingle<TCommand, THandler, Action<TParam1, TParam2>>
        where TCommand : Command<TParam1, TParam2>
    {
        readonly Func<THandler, Action<TParam1, TParam2>> _methodGetter;

        public CommandProviderMethodSingle(
            DiContainer container,
            Func<THandler, Action<TParam1, TParam2>> methodGetter,
            ProviderBase singletonProvider)
            : base(container, singletonProvider)
        {
            _methodGetter = methodGetter;
        }

        protected override Action<TParam1, TParam2> GetCommandAction(InjectContext context)
        {
            return (p1, p2) =>
            {
                var singleton = GetSingleton(context);
                Assert.IsNotNull(singleton);
                _methodGetter(singleton)(p1, p2);
            };
        }
    }
}
