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
        public static BindingConditionSetter BindSignal<TSignal>(this IBinder binder)
            where TSignal : ISignal
        {
            var container = (DiContainer)binder;
            return container.Bind<TSignal>().ToSingle();
        }

        public static BindingConditionSetter BindTrigger<TTrigger>(this IBinder binder)
            where TTrigger : ITrigger
        {
            var container = (DiContainer)binder;
            Type concreteSignalType = typeof(TTrigger).DeclaringType;
            Assert.IsNotNull(concreteSignalType);
            Assert.That(concreteSignalType.DerivesFrom<ISignal>());
            container.Bind(concreteSignalType.BaseType).ToSingle(concreteSignalType).WhenInjectedInto<TTrigger>();
            return container.Bind<TTrigger>().ToSingle();
        }
    }
}
