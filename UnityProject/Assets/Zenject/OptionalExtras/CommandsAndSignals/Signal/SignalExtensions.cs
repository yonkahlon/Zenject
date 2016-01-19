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
            where TSignal : Signal<TSignal>
        {
            return ((DiContainer)binder).Bind<TSignal>().ToSingle();
        }

        public static BindingConditionSetter BindTrigger<TSignal>(this IBinder binder)
            where TSignal : Signal<TSignal>
        {
            var container = (DiContainer)binder;
            container.Bind<Signal<TSignal>>().ToSingle<TSignal>().WhenInjectedInto<Signal<TSignal>.Trigger>();
            return container.Bind<Signal<TSignal>.Trigger>().ToSingle();
        }

        // Six Parameters
        public static BindingConditionSetter BindSignal<TSignal, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(this IBinder binder)
            where TSignal : Signal<TSignal, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            var container = (DiContainer)binder;
            return container.Bind<TSignal>().ToSingle();
        }

        public static BindingConditionSetter BindTrigger<TSignal, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(this IBinder binder)
            where TSignal : Signal<TSignal, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            var container = (DiContainer)binder;
            container.Bind<Signal<TSignal, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>().ToSingle<TSignal>()
                .WhenInjectedInto<Signal<TSignal, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>.Trigger>();
            return container.Bind<Signal<TSignal, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>.Trigger>().ToSingle();
        }
    }
}
