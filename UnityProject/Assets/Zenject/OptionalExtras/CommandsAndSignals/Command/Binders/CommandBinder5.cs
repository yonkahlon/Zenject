using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using ModestTree.Util;
using System.Linq;

namespace Zenject.Commands
{
    // Five parameters

    public class CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5> : CommandBinderBase<TCommand, Action<TParam1, TParam2, TParam3, TParam4, TParam5>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        public CommandBinder(string identifier, DiContainer container)
            : base(identifier, container)
        {
        }

        public ScopeBinder To<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            Binding.Finalizer = new CommandBindingFinalizer<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>(
                methodGetter,
                () => new TransientProvider(
                    typeof(THandler), Container, Binding.Arguments, Binding.ConcreteIdentifier));

            return new ScopeBinder(Binding);
        }

        public ScopeBinder ToResolve<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return ToResolve<THandler>(null, methodGetter);
        }

        public ScopeBinder ToResolve<THandler>(
            string identifier, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return ToResolveInternal<THandler>(identifier, methodGetter, false);
        }

        public ScopeBinder ToOptionalResolve<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return ToOptionalResolve<THandler>(null, methodGetter);
        }

        public ScopeBinder ToOptionalResolve<THandler>(
            string identifier, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return ToResolveInternal<THandler>(identifier, methodGetter, true);
        }

        public ConditionBinder ToNothing()
        {
            return ToMethod((p1, p2, p3, p4, p5) => {});
        }

        // AsSingle / AsCached / etc. don't make sense in this case so just return ConditionBinder
        public ConditionBinder ToMethod(Action<TParam1, TParam2, TParam3, TParam4, TParam5> action)
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
            string identifier, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter, bool optional)
        {
            Binding.Finalizer = new CommandBindingFinalizer<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>(
                methodGetter,
                () => new ResolveProvider(typeof(THandler), Container, identifier, optional));

            return new ScopeBinder(Binding);
        }
    }
}

