using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using ModestTree.Util;
using System.Linq;

namespace Zenject.Commands
{
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinderBase<TCommand, TAction> : BinderBase
        where TCommand : ICommand
        where TAction : class
    {
        readonly SingletonProviderCreator _singletonProviderFactory;

        public CommandBinderBase(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, new List<Type>() { typeof(TCommand) }, identifier)
        {
            _singletonProviderFactory = singletonProviderFactory;
        }

        public BindingConditionSetter ToNothing()
        {
            return ToMethod(null);
        }

        public BindingConditionSetter ToMethod(TAction action)
        {
            return RegisterSingleProvider(new CommandProviderMethod<TCommand, TAction>(action));
        }

        protected ProviderBase CreateSingletonProvider<THandler>(string concreteIdentifier)
        {
            return _singletonProviderFactory.CreateProviderFromType(concreteIdentifier, typeof(THandler));
        }
    }

    // Zero parameters
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinder<TCommand> : CommandBinderBase<TCommand, Action>
        where TCommand : Command
    {
        public CommandBinder(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, identifier, singletonProviderFactory)
        {
        }

        public BindingConditionSetter ToTransient<THandler>(
            Func<THandler, Action> methodGetter)
        {
            return RegisterSingleProvider(
                new CommandProviderTransientMethod<TCommand, THandler>(
                    methodGetter));
        }

        public BindingConditionSetter ToTransient<THandler>()
            where THandler : ICommandHandler
        {
            return RegisterSingleProvider(
                new CommandProviderTransient<TCommand, THandler>());
        }

        public BindingConditionSetter ToSingle<THandler>()
            where THandler : ICommandHandler
        {
            return ToSingle<THandler>((string)null);
        }

        public BindingConditionSetter ToSingle<THandler>(string concreteIdentifier)
            where THandler : ICommandHandler
        {
            return RegisterSingleProvider(
                new CommandProviderSingle<TCommand, THandler>(
                    CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter ToSingle<THandler>(
            string concreteIdentifier, Func<THandler, Action> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderSingleMethod<TCommand, THandler>(
                methodGetter, CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter ToSingle<THandler>(Func<THandler, Action> methodGetter)
        {
            return ToSingle<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToResolve<THandler>()
            where THandler : ICommandHandler
        {
            return ToResolve<THandler>((string)null);
        }

        public BindingConditionSetter ToResolve<THandler>(string identifier)
            where THandler : ICommandHandler
        {
            return RegisterSingleProvider(
                new CommandProviderResolve<TCommand, THandler>(identifier, false));
        }

        public BindingConditionSetter ToResolve<THandler>(Func<THandler, Action> methodGetter)
        {
            return ToResolve<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToResolve<THandler>(
            string identifier, Func<THandler, Action> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderResolveMethod<TCommand, THandler>(methodGetter, identifier, false));
        }

        public BindingConditionSetter ToOptionalResolve<THandler>()
            where THandler : ICommandHandler
        {
            return ToOptionalResolve<THandler>((string)null);
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(string identifier)
            where THandler : ICommandHandler
        {
            return RegisterSingleProvider(
                new CommandProviderResolve<TCommand, THandler>(identifier, true));
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(Func<THandler, Action> methodGetter)
        {
            return ToOptionalResolve<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(
            string identifier, Func<THandler, Action> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderResolveMethod<TCommand, THandler>(methodGetter, identifier, true));
        }
    }

    // One parameter
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinder<TCommand, TParam1> : CommandBinderBase<TCommand, Action<TParam1>>
        where TCommand : Command<TParam1>
    {
        public CommandBinder(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, identifier, singletonProviderFactory)
        {
        }

        public BindingConditionSetter ToTransient<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1>> methodGetter)
        {
            return RegisterSingleProvider(
                new CommandProviderTransientMethod<TCommand, THandler, TParam1>(
                    methodGetter));
        }

        public BindingConditionSetter ToTransient<THandler>(Func<THandler, Action<TParam1>> methodGetter)
        {
            return ToTransient<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToSingle<THandler>()
            where THandler : ICommandHandler<TParam1>
        {
            return ToSingle<THandler>((string)null);
        }

        public BindingConditionSetter ToSingle<THandler>(string concreteIdentifier)
            where THandler : ICommandHandler<TParam1>
        {
            return RegisterSingleProvider(
                new CommandProviderSingle<TCommand, THandler, TParam1>(
                    CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter ToSingle<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderSingleMethod<TCommand, THandler, TParam1>(
                methodGetter, CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter ToSingle<THandler>(Func<THandler, Action<TParam1>> methodGetter)
        {
            return ToSingle<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToResolve<THandler>()
            where THandler : ICommandHandler<TParam1>
        {
            return ToResolve<THandler>((string)null);
        }

        public BindingConditionSetter ToResolve<THandler>(string identifier)
            where THandler : ICommandHandler<TParam1>
        {
            return RegisterSingleProvider(
                new CommandProviderResolve<TCommand, THandler, TParam1>(identifier, false));
        }

        public BindingConditionSetter ToResolve<THandler>(Func<THandler, Action<TParam1>> methodGetter)
        {
            return ToResolve<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToResolve<THandler>(
            string identifier, Func<THandler, Action<TParam1>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderResolveMethod<TCommand, THandler, TParam1>(methodGetter, identifier, false));
        }

        public BindingConditionSetter ToOptionalResolve<THandler>()
            where THandler : ICommandHandler<TParam1>
        {
            return ToOptionalResolve<THandler>((string)null);
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(string identifier)
            where THandler : ICommandHandler<TParam1>
        {
            return RegisterSingleProvider(
                new CommandProviderResolve<TCommand, THandler, TParam1>(identifier, true));
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(Func<THandler, Action<TParam1>> methodGetter)
        {
            return ToOptionalResolve<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(
            string identifier, Func<THandler, Action<TParam1>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderResolveMethod<TCommand, THandler, TParam1>(methodGetter, identifier, true));
        }
    }

    // Two parameters
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinder<TCommand, TParam1, TParam2> : CommandBinderBase<TCommand, Action<TParam1, TParam2>>
        where TCommand : Command<TParam1, TParam2>
    {
        public CommandBinder(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, identifier, singletonProviderFactory)
        {
        }

        public BindingConditionSetter ToTransient<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1, TParam2>> methodGetter)
        {
            return RegisterSingleProvider(
                new CommandProviderTransientMethod<TCommand, THandler, TParam1, TParam2>(
                    methodGetter));
        }

        public BindingConditionSetter ToTransient<THandler>(Func<THandler, Action<TParam1, TParam2>> methodGetter)
        {
            return ToTransient<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToSingle<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2>
        {
            return ToSingle<THandler>((string)null);
        }

        public BindingConditionSetter ToSingle<THandler>(string concreteIdentifier)
            where THandler : ICommandHandler<TParam1, TParam2>
        {
            return RegisterSingleProvider(
                new CommandProviderSingle<TCommand, THandler, TParam1, TParam2>(
                    CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter ToSingle<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1, TParam2>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderSingleMethod<TCommand, THandler, TParam1, TParam2>(
                methodGetter, CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter ToSingle<THandler>(Func<THandler, Action<TParam1, TParam2>> methodGetter)
        {
            return ToSingle<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToResolve<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2>
        {
            return ToResolve<THandler>((string)null);
        }

        public BindingConditionSetter ToResolve<THandler>(string identifier)
            where THandler : ICommandHandler<TParam1, TParam2>
        {
            return RegisterSingleProvider(
                new CommandProviderResolve<TCommand, THandler, TParam1, TParam2>(identifier, false));
        }

        public BindingConditionSetter ToResolve<THandler>(Func<THandler, Action<TParam1, TParam2>> methodGetter)
        {
            return ToResolve<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToResolve<THandler>(
            string identifier, Func<THandler, Action<TParam1, TParam2>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderResolveMethod<TCommand, THandler, TParam1, TParam2>(methodGetter, identifier, false));
        }

        public BindingConditionSetter ToOptionalResolve<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2>
        {
            return ToOptionalResolve<THandler>((string)null);
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(string identifier)
            where THandler : ICommandHandler<TParam1, TParam2>
        {
            return RegisterSingleProvider(
                new CommandProviderResolve<TCommand, THandler, TParam1, TParam2>(identifier, true));
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(Func<THandler, Action<TParam1, TParam2>> methodGetter)
        {
            return ToOptionalResolve<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(
            string identifier, Func<THandler, Action<TParam1, TParam2>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderResolveMethod<TCommand, THandler, TParam1, TParam2>(methodGetter, identifier, true));
        }
    }

    // Three parameters
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinder<TCommand, TParam1, TParam2, TParam3> : CommandBinderBase<TCommand, Action<TParam1, TParam2, TParam3>>
        where TCommand : Command<TParam1, TParam2, TParam3>
    {
        public CommandBinder(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, identifier, singletonProviderFactory)
        {
        }

        public BindingConditionSetter ToTransient<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1, TParam2, TParam3>> methodGetter)
        {
            return RegisterSingleProvider(
                new CommandProviderTransientMethod<TCommand, THandler, TParam1, TParam2, TParam3>(
                    methodGetter));
        }

        public BindingConditionSetter ToTransient<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3>> methodGetter)
        {
            return ToTransient<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToSingle<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3>
        {
            return ToSingle<THandler>((string)null);
        }

        public BindingConditionSetter ToSingle<THandler>(string concreteIdentifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3>
        {
            return RegisterSingleProvider(
                new CommandProviderSingle<TCommand, THandler, TParam1, TParam2, TParam3>(
                    CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter ToSingle<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1, TParam2, TParam3>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderSingleMethod<TCommand, THandler, TParam1, TParam2, TParam3>(
                methodGetter, CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter ToSingle<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3>> methodGetter)
        {
            return ToSingle<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToResolve<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3>
        {
            return ToResolve<THandler>((string)null);
        }

        public BindingConditionSetter ToResolve<THandler>(string identifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3>
        {
            return RegisterSingleProvider(
                new CommandProviderResolve<TCommand, THandler, TParam1, TParam2, TParam3>(identifier, false));
        }

        public BindingConditionSetter ToResolve<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3>> methodGetter)
        {
            return ToResolve<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToResolve<THandler>(
            string identifier, Func<THandler, Action<TParam1, TParam2, TParam3>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderResolveMethod<TCommand, THandler, TParam1, TParam2, TParam3>(methodGetter, identifier, false));
        }

        public BindingConditionSetter ToOptionalResolve<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3>
        {
            return ToOptionalResolve<THandler>((string)null);
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(string identifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3>
        {
            return RegisterSingleProvider(
                new CommandProviderResolve<TCommand, THandler, TParam1, TParam2, TParam3>(identifier, true));
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3>> methodGetter)
        {
            return ToOptionalResolve<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(
            string identifier, Func<THandler, Action<TParam1, TParam2, TParam3>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderResolveMethod<TCommand, THandler, TParam1, TParam2, TParam3>(methodGetter, identifier, true));
        }
    }

    // Four parameters
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4> : CommandBinderBase<TCommand, Action<TParam1, TParam2, TParam3, TParam4>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4>
    {
        public CommandBinder(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, identifier, singletonProviderFactory)
        {
        }

        public BindingConditionSetter ToTransient<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return RegisterSingleProvider(
                new CommandProviderTransientMethod<TCommand, THandler, TParam1, TParam2, TParam3, TParam4>(
                    methodGetter));
        }

        public BindingConditionSetter ToTransient<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return ToTransient<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToSingle<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4>
        {
            return ToSingle<THandler>((string)null);
        }

        public BindingConditionSetter ToSingle<THandler>(string concreteIdentifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4>
        {
            return RegisterSingleProvider(
                new CommandProviderSingle<TCommand, THandler, TParam1, TParam2, TParam3, TParam4>(
                    CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter ToSingle<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderSingleMethod<TCommand, THandler, TParam1, TParam2, TParam3, TParam4>(
                methodGetter, CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter ToSingle<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return ToSingle<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToResolve<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4>
        {
            return ToResolve<THandler>((string)null);
        }

        public BindingConditionSetter ToResolve<THandler>(string identifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4>
        {
            return RegisterSingleProvider(
                new CommandProviderResolve<TCommand, THandler, TParam1, TParam2, TParam3, TParam4>(identifier, false));
        }

        public BindingConditionSetter ToResolve<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return ToResolve<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToResolve<THandler>(
            string identifier, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderResolveMethod<TCommand, THandler, TParam1, TParam2, TParam3, TParam4>(methodGetter, identifier, false));
        }

        public BindingConditionSetter ToOptionalResolve<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4>
        {
            return ToOptionalResolve<THandler>((string)null);
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(string identifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4>
        {
            return RegisterSingleProvider(
                new CommandProviderResolve<TCommand, THandler, TParam1, TParam2, TParam3, TParam4>(identifier, true));
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return ToOptionalResolve<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(
            string identifier, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderResolveMethod<TCommand, THandler, TParam1, TParam2, TParam3, TParam4>(methodGetter, identifier, true));
        }
    }

    // Five parameters
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5> : CommandBinderBase<TCommand, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        public CommandBinder(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, identifier, singletonProviderFactory)
        {
        }

        public BindingConditionSetter ToTransient<THandler>(
            string concreteIdentifier, Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return RegisterSingleProvider(
                new CommandProviderTransientMethod<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>(
                    methodGetter));
        }

        public BindingConditionSetter ToTransient<THandler>(Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return ToTransient<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToSingle<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            return ToSingle<THandler>((string)null);
        }

        public BindingConditionSetter ToSingle<THandler>(string concreteIdentifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            return RegisterSingleProvider(
                new CommandProviderSingle<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>(
                    CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter ToSingle<THandler>(
            string concreteIdentifier, Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderSingleMethod<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>(
                methodGetter, CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter ToSingle<THandler>(Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return ToSingle<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToResolve<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            return ToResolve<THandler>((string)null);
        }

        public BindingConditionSetter ToResolve<THandler>(string identifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            return RegisterSingleProvider(
                new CommandProviderResolve<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>(identifier, false));
        }

        public BindingConditionSetter ToResolve<THandler>(Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return ToResolve<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToResolve<THandler>(
            string identifier, Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderResolveMethod<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>(methodGetter, identifier, false));
        }

        public BindingConditionSetter ToOptionalResolve<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            return ToOptionalResolve<THandler>((string)null);
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(string identifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            return RegisterSingleProvider(
                new CommandProviderResolve<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>(identifier, true));
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return ToOptionalResolve<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(
            string identifier, Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderResolveMethod<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>(methodGetter, identifier, true));
        }
    }

    // Six parameters
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> : CommandBinderBase<TCommand, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
    {
        public CommandBinder(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, identifier, singletonProviderFactory)
        {
        }

        public BindingConditionSetter ToTransient<THandler>(
            string concreteIdentifier, Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>> methodGetter)
        {
            return RegisterSingleProvider(
                new CommandProviderTransientMethod<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(
                    methodGetter));
        }

        public BindingConditionSetter ToTransient<THandler>(Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>> methodGetter)
        {
            return ToTransient<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToSingle<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            return ToSingle<THandler>((string)null);
        }

        public BindingConditionSetter ToSingle<THandler>(string concreteIdentifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            return RegisterSingleProvider(
                new CommandProviderSingle<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(
                    CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter ToSingle<THandler>(
            string concreteIdentifier, Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>> methodGetter)
        {
            return RegisterSingleProvider(new CommandProviderSingleMethod<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(
                methodGetter, CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter ToSingle<THandler>(Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>> methodGetter)
        {
            return ToSingle<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToResolve<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            return ToResolve<THandler>((string)null);
        }

        public BindingConditionSetter ToResolve<THandler>(string identifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            return RegisterSingleProvider(
                new CommandProviderResolve<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(identifier, false));
        }

        public BindingConditionSetter ToResolve<THandler>(Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>> methodGetter)
        {
            return ToResolve<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToResolve<THandler>(
            string identifier, Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>> methodGetter)
        {
            return RegisterSingleProvider(
                new CommandProviderResolveMethod<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(methodGetter, identifier, false));
        }

        public BindingConditionSetter ToOptionalResolve<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            return ToOptionalResolve<THandler>((string)null);
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(string identifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            return RegisterSingleProvider(
                new CommandProviderResolve<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(identifier, true));
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>> methodGetter)
        {
            return ToOptionalResolve<THandler>(null, methodGetter);
        }

        public BindingConditionSetter ToOptionalResolve<THandler>(
            string identifier, Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>> methodGetter)
        {
            return RegisterSingleProvider(
                new CommandProviderResolveMethod<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(methodGetter, identifier, true));
        }
    }
}
