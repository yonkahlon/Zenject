using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using ModestTree.Util;
using System.Linq;

namespace Zenject.Commands
{
    // Zero params
    public class CommandProviderResolveMethod<TCommand, THandler>
        : CommandProviderResolveBase<TCommand, THandler, Action>
        where TCommand : Command
    {
        readonly Func<THandler, Action> _methodGetter;

        public CommandProviderResolveMethod(
            DiContainer container, ContainerTypes containerType, Func<THandler, Action> methodGetter, string identifier, bool isOptional)
            : base(container, containerType, identifier, isOptional)
        {
            _methodGetter = methodGetter;
        }

        protected override Action GetCommandAction(InjectContext context)
        {
            return () =>
            {
                var resolve = TryGetResolve(context);

                if (resolve != null)
                {
                    _methodGetter(resolve)();
                }
            };
        }
    }

    // One param
    public class CommandProviderResolveMethod<TCommand, THandler, TParam1>
        : CommandProviderResolveBase<TCommand, THandler, Action<TParam1>>
        where TCommand : Command<TParam1>
    {
        readonly Func<THandler, Action<TParam1>> _methodGetter;

        public CommandProviderResolveMethod(
            DiContainer container, ContainerTypes containerType, Func<THandler, Action<TParam1>> methodGetter, string identifier, bool isOptional)
            : base(container, containerType, identifier, isOptional)
        {
            _methodGetter = methodGetter;
        }

        protected override Action<TParam1> GetCommandAction(InjectContext context)
        {
            return (p1) =>
            {
                var resolve = TryGetResolve(context);

                if (resolve != null)
                {
                    _methodGetter(resolve)(p1);
                }
            };
        }
    }

    // Two params
    public class CommandProviderResolveMethod<TCommand, THandler, TParam1, TParam2>
        : CommandProviderResolveBase<TCommand, THandler, Action<TParam1, TParam2>>
        where TCommand : Command<TParam1, TParam2>
    {
        readonly Func<THandler, Action<TParam1, TParam2>> _methodGetter;

        public CommandProviderResolveMethod(
            DiContainer container, ContainerTypes containerType, Func<THandler, Action<TParam1, TParam2>> methodGetter, string identifier, bool isOptional)
            : base(container, containerType, identifier, isOptional)
        {
            _methodGetter = methodGetter;
        }

        protected override Action<TParam1, TParam2> GetCommandAction(InjectContext context)
        {
            return (p1, p2) =>
            {
                var resolve = TryGetResolve(context);

                if (resolve != null)
                {
                    _methodGetter(resolve)(p1, p2);
                }
            };
        }
    }

    // Three params
    public class CommandProviderResolveMethod<TCommand, THandler, TParam1, TParam2, TParam3>
        : CommandProviderResolveBase<TCommand, THandler, Action<TParam1, TParam2, TParam3>>
        where TCommand : Command<TParam1, TParam2, TParam3>
    {
        readonly Func<THandler, Action<TParam1, TParam2, TParam3>> _methodGetter;

        public CommandProviderResolveMethod(
            DiContainer container, ContainerTypes containerType, Func<THandler, Action<TParam1, TParam2, TParam3>> methodGetter,
            string identifier, bool isOptional)
            : base(container, containerType, identifier, isOptional)
        {
            _methodGetter = methodGetter;
        }

        protected override Action<TParam1, TParam2, TParam3> GetCommandAction(InjectContext context)
        {
            return (p1, p2, p3) =>
            {
                var resolve = TryGetResolve(context);

                if (resolve != null)
                {
                    _methodGetter(resolve)(p1, p2, p3);
                }
            };
        }
    }

    // Four params
    public class CommandProviderResolveMethod<TCommand, THandler, TParam1, TParam2, TParam3, TParam4>
        : CommandProviderResolveBase<TCommand, THandler, Action<TParam1, TParam2, TParam3, TParam4>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4>
    {
        readonly Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> _methodGetter;

        public CommandProviderResolveMethod(
            DiContainer container, ContainerTypes containerType, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter,
            string identifier, bool isOptional)
            : base(container, containerType, identifier, isOptional)
        {
            _methodGetter = methodGetter;
        }

        protected override Action<TParam1, TParam2, TParam3, TParam4> GetCommandAction(InjectContext context)
        {
            return (p1, p2, p3, p4) =>
            {
                var resolve = TryGetResolve(context);

                if (resolve != null)
                {
                    _methodGetter(resolve)(p1, p2, p3, p4);
                }
            };
        }
    }

    // Five params
    public class CommandProviderResolveMethod<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>
        : CommandProviderResolveBase<TCommand, THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        readonly Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>> _methodGetter;

        public CommandProviderResolveMethod(
            DiContainer container, ContainerTypes containerType, Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter,
            string identifier, bool isOptional)
            : base(container, containerType, identifier, isOptional)
        {
            _methodGetter = methodGetter;
        }

        protected override ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5> GetCommandAction(InjectContext context)
        {
            return (p1, p2, p3, p4, p5) =>
            {
                var resolve = TryGetResolve(context);

                if (resolve != null)
                {
                    _methodGetter(resolve)(p1, p2, p3, p4, p5);
                }
            };
        }
    }

    // Six params
    public class CommandProviderResolveMethod<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        : CommandProviderResolveBase<TCommand, THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
    {
        readonly Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>> _methodGetter;

        public CommandProviderResolveMethod(
            DiContainer container, ContainerTypes containerType, Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>> methodGetter,
            string identifier, bool isOptional)
            : base(container, containerType, identifier, isOptional)
        {
            _methodGetter = methodGetter;
        }

        protected override ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> GetCommandAction(InjectContext context)
        {
            return (p1, p2, p3, p4, p5, p6) =>
            {
                var resolve = TryGetResolve(context);

                if (resolve != null)
                {
                    _methodGetter(resolve)(p1, p2, p3, p4, p5, p6);
                }
            };
        }
    }
}

