using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class Binder<TContract>
    {
        readonly protected DiContainer _container;
        readonly protected SingletonProviderMap _singletonMap;

        public Binder(DiContainer container, SingletonProviderMap singletonMap)
        {
            _container = container;
            _singletonMap = singletonMap;
        }

        public BindingConditionSetter AsLookup<TConcrete>() where TConcrete : TContract
        {
            return AsMethod(c => c.Resolve<TConcrete>());
        }

        public BindingConditionSetter AsFactory<TConcrete>() where TConcrete : IFactory<TContract>
        {
            return AsMethod(c => c.Resolve<TConcrete>().Create());
        }

        public virtual BindingConditionSetter Bind(ProviderBase provider)
        {
            _container.RegisterProvider<TContract>(provider);
            return new BindingConditionSetter(provider);
        }

        public BindingConditionSetter AsMethod(MethodProvider<TContract>.Method method)
        {
            return Bind(new MethodProvider<TContract>(method, _container));
        }
    }
}
