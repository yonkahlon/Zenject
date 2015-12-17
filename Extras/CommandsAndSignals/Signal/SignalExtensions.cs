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
        public static BindingConditionSetter BindSignalTrigger<TTrigger, TSignal>(this IBinder binder, string identifier)
            where TSignal : Signal
            where TTrigger : Signal.TriggerBase
        {
            var container = (DiContainer)binder;
            container.Bind<TSignal>().ToSingle();
            container.Bind<Signal>().ToSingle<TSignal>().WhenInjectedInto<TTrigger>();
            return container.Bind<TTrigger>().ToSingle();
        }

        public static BindingConditionSetter BindSignalTrigger<TTrigger, TSignal>(this IBinder container)
            where TSignal : Signal
            where TTrigger : Signal.TriggerBase
        {
            return BindSignalTrigger<TTrigger, TSignal>((DiContainer)container, null);
        }

        public static BindingConditionSetter BindSignalTrigger<TTrigger, TSignal, TParam1>(this IBinder binder, string identifier)
            where TSignal : Signal<TParam1>
            where TTrigger : Signal<TParam1>.TriggerBase
        {
            var container = (DiContainer)binder;
            container.Bind<TSignal>().ToSingle();
            container.Bind<Signal<TParam1>>().ToSingle<TSignal>().WhenInjectedInto<TTrigger>();
            return container.Bind<TTrigger>().ToSingle();
        }

        public static BindingConditionSetter BindSignalTrigger<TTrigger, TSignal, TParam1>(this IBinder container)
            where TSignal : Signal<TParam1>
            where TTrigger : Signal<TParam1>.TriggerBase
        {
            return BindSignalTrigger<TTrigger, TSignal, TParam1>((DiContainer)container, null);
        }
    }
}
