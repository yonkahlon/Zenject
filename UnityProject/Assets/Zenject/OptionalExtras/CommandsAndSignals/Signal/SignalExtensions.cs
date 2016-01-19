using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;

namespace Zenject.Commands
{
    public class Trigger<TSignal> : Signal.TriggerBase
        where TSignal : Signal
    {
    }

    public class Trigger<TSignal, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> : Signal<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>.TriggerBase
        where TSignal : Signal<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
    {
    }

    public static class SignalExtensions
    {
        // Zero Parameters
        public static BindingConditionSetter BindSignal<TSignal>(this IBinder binder)
            where TSignal : Signal
        {
            return ((DiContainer)binder).Bind<TSignal>().ToSingle();
        }

        public static BindingConditionSetter BindTrigger<TSignal>(this IBinder binder)
            where TSignal : Signal
        {
            var container = (DiContainer)binder;
            container.Bind<Signal>().ToSingle<TSignal>().WhenInjectedInto<Trigger<TSignal>>();
            return container.Bind<Trigger<TSignal>>().ToSingle();
        }

        // Six Parameters
        public static BindingConditionSetter BindSignal<TSignal, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(this IBinder binder)
            where TSignal : Signal<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            var container = (DiContainer)binder;
            return container.Bind<TSignal>().ToSingle();
        }

        public static BindingConditionSetter BindTrigger<TSignal, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(this IBinder binder)
            where TSignal : Signal<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            var container = (DiContainer)binder;
            container.Bind<Signal<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>().ToSingle<TSignal>().WhenInjectedInto<Trigger<TSignal, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>();
            return container.Bind<Trigger<TSignal, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>().ToSingle();
        }
    }
}
