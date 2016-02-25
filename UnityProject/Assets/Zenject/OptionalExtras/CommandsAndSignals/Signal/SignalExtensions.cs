using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;

namespace Zenject.Commands
{
    public static class SignalExtensions
    {
        public static BindingConditionSetter BindSignal<TSignal>(this DiContainer container)
            where TSignal : ISignal
        {
            return container.BindSignal<TSignal>(null);
        }

        public static BindingConditionSetter BindSignal<TSignal>(this DiContainer container, string identifier)
            where TSignal : ISignal
        {
            return container.Bind<TSignal>(identifier).ToSingle(identifier);
        }

        public static BindingConditionSetter BindTrigger<TTrigger>(this DiContainer container)
            where TTrigger : ITrigger
        {
            return container.BindTrigger<TTrigger>(null);
        }

        public static BindingConditionSetter BindTrigger<TTrigger>(this DiContainer container, string identifier)
            where TTrigger : ITrigger
        {
            Type concreteSignalType = typeof(TTrigger).DeclaringType;

            Assert.IsNotNull(concreteSignalType);
            Assert.That(concreteSignalType.DerivesFrom<ISignal>());

            container.Bind(concreteSignalType.BaseType)
                .ToSingle(concreteSignalType, identifier)
                .When(ctx => ctx.ObjectType != null && ctx.ObjectType.DerivesFromOrEqual<TTrigger>() && ctx.ConcreteIdentifier == identifier);

            return container.Bind<TTrigger>(identifier).ToSingle(identifier);
        }
    }
}
