using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using ModestTree.Util;
using System.Linq;

namespace Zenject.Commands
{
    // Zero params
    public class CommandProviderResolve<TCommand, THandler>
        : CommandProviderResolveBase<TCommand, THandler, Action>
        where TCommand : Command
        where THandler : ICommandHandler
    {
        public CommandProviderResolve(string identifier, bool isOptional)
            : base(identifier, isOptional)
        {
        }

        protected override Action GetCommandAction(InjectContext context)
        {
            return () =>
            {
                var resolve = TryGetResolve(context);

                if (resolve != null)
                {
                    resolve.Execute();
                }
            };
        }
    }

    // One param
    public class CommandProviderResolve<TCommand, THandler, TParam1>
        : CommandProviderResolveBase<TCommand, THandler, Action<TParam1>>
        where TCommand : Command<TParam1>
        where THandler : ICommandHandler<TParam1>
    {
        public CommandProviderResolve(string identifier, bool isOptional)
            : base(identifier, isOptional)
        {
        }

        protected override Action<TParam1> GetCommandAction(InjectContext context)
        {
            return (p1) =>
            {
                var resolve = TryGetResolve(context);

                if (resolve != null)
                {
                    resolve.Execute(p1);
                }
            };
        }
    }

    // Two params
    public class CommandProviderResolve<TCommand, THandler, TParam1, TParam2>
        : CommandProviderResolveBase<TCommand, THandler, Action<TParam1, TParam2>>
        where TCommand : Command<TParam1, TParam2>
        where THandler : ICommandHandler<TParam1, TParam2>
    {
        public CommandProviderResolve(string identifier, bool isOptional)
            : base(identifier, isOptional)
        {
        }

        protected override Action<TParam1, TParam2> GetCommandAction(InjectContext context)
        {
            return (p1, p2) =>
            {
                var resolve = TryGetResolve(context);

                if (resolve != null)
                {
                    resolve.Execute(p1, p2);
                }
            };
        }
    }

    // Three params
    public class CommandProviderResolve<TCommand, THandler, TParam1, TParam2, TParam3>
        : CommandProviderResolveBase<TCommand, THandler, Action<TParam1, TParam2, TParam3>>
        where TCommand : Command<TParam1, TParam2, TParam3>
        where THandler : ICommandHandler<TParam1, TParam2, TParam3>
    {
        public CommandProviderResolve(string identifier, bool isOptional)
            : base(identifier, isOptional)
        {
        }

        protected override Action<TParam1, TParam2, TParam3> GetCommandAction(InjectContext context)
        {
            return (p1, p2, p3) =>
            {
                var resolve = TryGetResolve(context);

                if (resolve != null)
                {
                    resolve.Execute(p1, p2, p3);
                }
            };
        }
    }

    // Four params
    public class CommandProviderResolve<TCommand, THandler, TParam1, TParam2, TParam3, TParam4>
        : CommandProviderResolveBase<TCommand, THandler, Action<TParam1, TParam2, TParam3, TParam4>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4>
        where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4>
    {
        public CommandProviderResolve(string identifier, bool isOptional)
            : base(identifier, isOptional)
        {
        }

        protected override Action<TParam1, TParam2, TParam3, TParam4> GetCommandAction(InjectContext context)
        {
            return (p1, p2, p3, p4) =>
            {
                var resolve = TryGetResolve(context);

                if (resolve != null)
                {
                    resolve.Execute(p1, p2, p3, p4);
                }
            };
        }
    }

    // Five params
    public class CommandProviderResolve<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>
        : CommandProviderResolveBase<TCommand, THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5>
        where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        public CommandProviderResolve(string identifier, bool isOptional)
            : base(identifier, isOptional)
        {
        }

        protected override ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5> GetCommandAction(InjectContext context)
        {
            return (p1, p2, p3, p4, p5) =>
            {
                var resolve = TryGetResolve(context);

                if (resolve != null)
                {
                    resolve.Execute(p1, p2, p3, p4, p5);
                }
            };
        }
    }

    // Six params
    public class CommandProviderResolve<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        : CommandProviderResolveBase<TCommand, THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
    {
        public CommandProviderResolve(string identifier, bool isOptional)
            : base(identifier, isOptional)
        {
        }

        protected override ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> GetCommandAction(InjectContext context)
        {
            return (p1, p2, p3, p4, p5, p6) =>
            {
                var resolve = TryGetResolve(context);

                if (resolve != null)
                {
                    resolve.Execute(p1, p2, p3, p4, p5, p6);
                }
            };
        }
    }
}


