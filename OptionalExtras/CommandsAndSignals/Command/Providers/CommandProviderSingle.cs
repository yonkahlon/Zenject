using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using ModestTree.Util;
using System.Linq;

namespace Zenject.Commands
{
    // Zero params
    public class CommandProviderSingle<TCommand, THandler>
        : CommandProviderSingleBase<TCommand, THandler, Action>
        where TCommand : Command
        where THandler : ICommandHandler
    {
        public CommandProviderSingle(DiContainer container, ProviderBase singletonProvider)
            : base(container, singletonProvider)
        {
        }

        protected override Action GetCommandAction(InjectContext context)
        {
            return () =>
            {
                GetSingleton(context).Execute();
            };
        }
    }

    // One param
    public class CommandProviderSingle<TCommand, THandler, TParam1>
        : CommandProviderSingleBase<TCommand, THandler, Action<TParam1>>
        where TCommand : Command<TParam1>
        where THandler : ICommandHandler<TParam1>
    {
        public CommandProviderSingle(DiContainer container, ProviderBase singletonProvider)
            : base(container, singletonProvider)
        {
        }

        protected override Action<TParam1> GetCommandAction(InjectContext context)
        {
            return (p1) =>
            {
                GetSingleton(context).Execute(p1);
            };
        }
    }

    // Two params
    public class CommandProviderSingle<TCommand, THandler, TParam1, TParam2>
        : CommandProviderSingleBase<TCommand, THandler, Action<TParam1, TParam2>>
        where TCommand : Command<TParam1, TParam2>
        where THandler : ICommandHandler<TParam1, TParam2>
    {
        public CommandProviderSingle(DiContainer container, ProviderBase singletonProvider)
            : base(container, singletonProvider)
        {
        }

        protected override Action<TParam1, TParam2> GetCommandAction(InjectContext context)
        {
            return (p1, p2) =>
            {
                GetSingleton(context).Execute(p1, p2);
            };
        }
    }

    // Three params
    public class CommandProviderSingle<TCommand, THandler, TParam1, TParam2, TParam3>
        : CommandProviderSingleBase<TCommand, THandler, Action<TParam1, TParam2, TParam3>>
        where TCommand : Command<TParam1, TParam2, TParam3>
        where THandler : ICommandHandler<TParam1, TParam2, TParam3>
    {
        public CommandProviderSingle(DiContainer container, ProviderBase singletonProvider)
            : base(container, singletonProvider)
        {
        }

        protected override Action<TParam1, TParam2, TParam3> GetCommandAction(InjectContext context)
        {
            return (p1, p2, p3) =>
            {
                GetSingleton(context).Execute(p1, p2, p3);
            };
        }
    }

    // Four params
    public class CommandProviderSingle<TCommand, THandler, TParam1, TParam2, TParam3, TParam4>
        : CommandProviderSingleBase<TCommand, THandler, Action<TParam1, TParam2, TParam3, TParam4>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4>
        where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4>
    {
        public CommandProviderSingle(DiContainer container, ProviderBase singletonProvider)
            : base(container, singletonProvider)
        {
        }

        protected override Action<TParam1, TParam2, TParam3, TParam4> GetCommandAction(InjectContext context)
        {
            return (p1, p2, p3, p4) =>
            {
                GetSingleton(context).Execute(p1, p2, p3, p4);
            };
        }
    }

    // Five params
    public class CommandProviderSingle<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>
        : CommandProviderSingleBase<TCommand, THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5>
        where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        public CommandProviderSingle(DiContainer container, ProviderBase singletonProvider)
            : base(container, singletonProvider)
        {
        }

        protected override ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5> GetCommandAction(InjectContext context)
        {
            return (p1, p2, p3, p4, p5) =>
            {
                GetSingleton(context).Execute(p1, p2, p3, p4, p5);
            };
        }
    }

    // Six params
    public class CommandProviderSingle<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        : CommandProviderSingleBase<TCommand, THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
    {
        public CommandProviderSingle(DiContainer container, ProviderBase singletonProvider)
            : base(container, singletonProvider)
        {
        }

        protected override ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> GetCommandAction(InjectContext context)
        {
            return (p1, p2, p3, p4, p5, p6) =>
            {
                GetSingleton(context).Execute(p1, p2, p3, p4, p5, p6);
            };
        }
    }
}

