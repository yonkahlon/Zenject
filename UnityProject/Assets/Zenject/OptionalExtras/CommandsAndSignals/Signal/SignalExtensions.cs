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
        // Zero Parameters
        public static BindingConditionSetter BindSignal<TSignal>(this IBinder binder)
            where TSignal : Signal
        {
            var container = (DiContainer)binder;
            return container.Bind<TSignal>().ToSingle();
        }

        public static BindingConditionSetter BindTrigger<TTrigger>(this IBinder binder)
            where TTrigger : Signal.TriggerBase
        {
            var container = (DiContainer)binder;
            Type concreteSignalType = typeof(TTrigger).DeclaringType;
            Assert.IsNotNull(concreteSignalType);
            Assert.That(concreteSignalType.DerivesFrom<Signal>());
            container.Bind(typeof(Signal)).ToSingle(concreteSignalType).WhenInjectedInto<TTrigger>();
            return container.Bind<TTrigger>().ToSingle();
        }

        // One Parameter
        public static BindingConditionSetter BindSignal<TSignal, TParam1>(this IBinder binder)
            where TSignal : Signal<TParam1>
        {
            var container = (DiContainer)binder;
            return container.Bind<TSignal>().ToSingle();
        }

        public static BindingConditionSetter BindTrigger<TTrigger, TParam1>(this IBinder binder)
            where TTrigger : Signal<TParam1>.TriggerBase
        {
            var container = (DiContainer)binder;
            Type concreteSignalType = typeof(TTrigger).DeclaringType;
            Assert.IsNotNull(concreteSignalType);
            Assert.That(concreteSignalType.DerivesFrom<Signal<TParam1>>());
            container.Bind(typeof(Signal<TParam1>)).ToSingle(concreteSignalType).WhenInjectedInto<TTrigger>();
            return container.Bind<TTrigger>().ToSingle();
        }

        // Two Parameters
        public static BindingConditionSetter BindSignal<TSignal, TParam1, TParam2>(this IBinder binder)
            where TSignal : Signal<TParam1, TParam2>
        {
            var container = (DiContainer)binder;
            return container.Bind<TSignal>().ToSingle();
        }

        public static BindingConditionSetter BindTrigger<TTrigger, TParam1, TParam2>(this IBinder binder)
            where TTrigger : Signal<TParam1, TParam2>.TriggerBase
        {
            var container = (DiContainer)binder;
            Type concreteSignalType = typeof(TTrigger).DeclaringType;
            Assert.IsNotNull(concreteSignalType);
            Assert.That(concreteSignalType.DerivesFrom<Signal<TParam1, TParam2>>());
            container.Bind(typeof(Signal<TParam1, TParam2>)).ToSingle(concreteSignalType).WhenInjectedInto<TTrigger>();
            return container.Bind<TTrigger>().ToSingle();
        }

        // Three Parameters
        public static BindingConditionSetter BindSignal<TSignal, TParam1, TParam2, TParam3>(this IBinder binder)
            where TSignal : Signal<TParam1, TParam2, TParam3>
        {
            var container = (DiContainer)binder;
            return container.Bind<TSignal>().ToSingle();
        }

        public static BindingConditionSetter BindTrigger<TTrigger, TParam1, TParam2, TParam3>(this IBinder binder)
            where TTrigger : Signal<TParam1, TParam2, TParam3>.TriggerBase
        {
            var container = (DiContainer)binder;
            Type concreteSignalType = typeof(TTrigger).DeclaringType;
            Assert.IsNotNull(concreteSignalType);
            Assert.That(concreteSignalType.DerivesFrom<Signal<TParam1, TParam2, TParam3>>());
            container.Bind(typeof(Signal<TParam1, TParam2, TParam3>)).ToSingle(concreteSignalType).WhenInjectedInto<TTrigger>();
            return container.Bind<TTrigger>().ToSingle();
        }

        // Four Parameters
        public static BindingConditionSetter BindSignal<TSignal, TParam1, TParam2, TParam3, TParam4>(this IBinder binder)
            where TSignal : Signal<TParam1, TParam2, TParam3, TParam4>
        {
            var container = (DiContainer)binder;
            return container.Bind<TSignal>().ToSingle();
        }

        public static BindingConditionSetter BindTrigger<TTrigger, TParam1, TParam2, TParam3, TParam4>(this IBinder binder)
            where TTrigger : Signal<TParam1, TParam2, TParam3, TParam4>.TriggerBase
        {
            var container = (DiContainer)binder;
            Type concreteSignalType = typeof(TTrigger).DeclaringType;
            Assert.IsNotNull(concreteSignalType);
            Assert.That(concreteSignalType.DerivesFrom<Signal<TParam1, TParam2, TParam3, TParam4>>());
            container.Bind(typeof(Signal<TParam1, TParam2, TParam3, TParam4>)).ToSingle(concreteSignalType).WhenInjectedInto<TTrigger>();
            return container.Bind<TTrigger>().ToSingle();
        }

        // Five Parameters
        public static BindingConditionSetter BindSignal<TSignal, TParam1, TParam2, TParam3, TParam4, TParam5>(this IBinder binder)
            where TSignal : Signal<TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            var container = (DiContainer)binder;
            return container.Bind<TSignal>().ToSingle();
        }

        public static BindingConditionSetter BindTrigger<TTrigger, TParam1, TParam2, TParam3, TParam4, TParam5>(this IBinder binder)
            where TTrigger : Signal<TParam1, TParam2, TParam3, TParam4, TParam5>.TriggerBase
        {
            var container = (DiContainer)binder;
            Type concreteSignalType = typeof(TTrigger).DeclaringType;
            Assert.IsNotNull(concreteSignalType);
            Assert.That(concreteSignalType.DerivesFrom<Signal<TParam1, TParam2, TParam3, TParam4, TParam5>>());
            container.Bind(typeof(Signal<TParam1, TParam2, TParam3, TParam4, TParam5>)).ToSingle(concreteSignalType).WhenInjectedInto<TTrigger>();
            return container.Bind<TTrigger>().ToSingle();
        }

        // Six Parameters
        public static BindingConditionSetter BindSignal<TSignal, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(this IBinder binder)
            where TSignal : Signal<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            var container = (DiContainer)binder;
            return container.Bind<TSignal>().ToSingle();
        }

        public static BindingConditionSetter BindTrigger<TTrigger, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(this IBinder binder)
            where TTrigger : Signal<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>.TriggerBase
        {
            var container = (DiContainer)binder;
            Type concreteSignalType = typeof(TTrigger).DeclaringType;
            Assert.IsNotNull(concreteSignalType);
            Assert.That(concreteSignalType.DerivesFrom<Signal<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>());
            container.Bind(typeof(Signal<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>)).ToSingle(concreteSignalType).WhenInjectedInto<TTrigger>();
            return container.Bind<TTrigger>().ToSingle();
        }
    }
}
