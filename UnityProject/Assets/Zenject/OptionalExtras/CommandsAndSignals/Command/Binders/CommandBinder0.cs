using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using ModestTree.Util;
using System.Linq;

namespace Zenject.Commands
{
    // Zero parameters
    public class CommandBinder<TCommand> : CommandBinderBase<TCommand, Action>
        where TCommand : Command
    {
        public CommandBinder(string identifier, DiContainer container)
            : base(identifier, container)
        {
        }

        public ScopeBinder To<THandler>(Func<THandler, Action> methodGetter)
        {
            Binding.Finalizer = new CommandBindingFinalizer<TCommand, THandler>(
                methodGetter,
                () => new TransientProvider(
                    typeof(THandler), Container, Binding.Arguments, Binding.ConcreteIdentifier));

            return new ScopeBinder(Binding);
        }

        public ScopeBinder ToResolve<THandler>(Func<THandler, Action> methodGetter)
        {
            return ToResolve<THandler>(null, methodGetter);
        }

        public ScopeBinder ToResolve<THandler>(
            string identifier, Func<THandler, Action> methodGetter)
        {
            return ToResolveInternal<THandler>(identifier, methodGetter, false);
        }

        public ScopeBinder ToOptionalResolve<THandler>(Func<THandler, Action> methodGetter)
        {
            return ToOptionalResolve<THandler>(null, methodGetter);
        }

        public ScopeBinder ToOptionalResolve<THandler>(
            string identifier, Func<THandler, Action> methodGetter)
        {
            return ToResolveInternal<THandler>(identifier, methodGetter, true);
        }

        public ConditionBinder ToNothing()
        {
            return ToMethod(() => {});
        }

        // AsSingle / AsCached / etc. don't make sense in this case so just return ConditionBinder
        public ConditionBinder ToMethod(Action action)
        {
            // Create the command class once and re-use it everywhere
            Binding.Finalizer = new SingleProviderBindingFinalizer(
                new CachedProvider(
                    new TransientProvider(
                        typeof(TCommand), Container,
                        InjectUtil.CreateArgListExplicit(action), null)));

            return new ConditionBinder(Binding);
        }

        ScopeBinder ToResolveInternal<THandler>(
            string identifier, Func<THandler, Action> methodGetter, bool optional)
        {
            Binding.Finalizer = new CommandBindingFinalizer<TCommand, THandler>(
                methodGetter,
                () => new ResolveProvider(typeof(THandler), Container, identifier, optional));

            return new ScopeBinder(Binding);
        }
    }
}
