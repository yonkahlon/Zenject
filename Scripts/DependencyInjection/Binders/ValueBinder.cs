using UnityEngine;

namespace ModestTree.Zenject
{
    public class ValueBinder<T> : Binder<T>
    {
        public ValueBinder(DiContainer container, SingletonProviderMap singletonMap)
            : base(container, singletonMap)
        {
        }

        public BindingConditionSetter As(T value)
        {
            return Bind(new SingletonInstanceProvider(value));
        }
    }
}

