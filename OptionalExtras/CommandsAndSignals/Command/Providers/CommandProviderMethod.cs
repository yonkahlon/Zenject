using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using System.Linq;

namespace Zenject.Commands
{
    public class CommandProviderMethod<TCommand, TAction> : ProviderBase
        where TCommand : ICommand
    {
        readonly TAction _method;

        public CommandProviderMethod(TAction method)
        {
            _method = method;
        }

        public override Type GetInstanceType()
        {
            return typeof(TCommand);
        }

        public override object GetInstance(InjectContext context)
        {
            var obj = context.Instantiator.InstantiateExplicit<TCommand>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(_method),
                });
            Assert.That(obj != null);
            return obj;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return context.Resolver.ValidateObjectGraph<TCommand>(context, typeof(TAction));
        }
    }
}

