using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class ValueBinder<TContract> : Binder<TContract> where TContract : struct
    {
        public ValueBinder(DiContainer container, SingletonProviderMap singletonMap)
            : base(container, singletonMap)
        {
        }

        public BindingConditionSetter As(TContract value)
        {
            return Bind(new SingletonInstanceProvider(value));
        }

        public override BindingConditionSetter Bind(ProviderBase provider)
        {
            var conditionSetter = base.Bind(provider);

            // Also bind to nullable primitives
            // this is useful so that we can have optional primitive dependencies
            _container.RegisterProvider<Nullable<TContract>>(provider);

            return conditionSetter;
        }
    }
}
