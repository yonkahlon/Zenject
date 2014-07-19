using System;

namespace ModestTree.Zenject
{
    public class BinderUntyped : Binder
    {
        readonly protected SingletonProviderMap _singletonMap;

        public BinderUntyped(
            DiContainer container, Type contractType, SingletonProviderMap singletonMap)
            : base(container, contractType)
        {
            _singletonMap = singletonMap;
        }

        public BindingConditionSetter ToTransient()
        {
            return ToProvider(new TransientProvider(_container, _contractType));
        }

        public BindingConditionSetter ToSingle()
        {
            return ToProvider(_singletonMap.CreateProvider(_contractType));
        }
    }
}
