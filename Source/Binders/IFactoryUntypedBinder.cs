using System;
using ModestTree;
using ModestTree.Util;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public class IFactoryUntypedBinder<TContract>
    {
        readonly IBinder _binder;
        readonly string _identifier;

        public IFactoryUntypedBinder(IBinder binder, string identifier)
        {
            _binder = binder;
            _identifier = identifier;
        }

        public BindingConditionSetter ToInstance(TContract instance)
        {
            return ToMethod((c, args) => instance);
        }

        public BindingConditionSetter ToMethod(
            Func<IInstantiator, object[], TContract> method)
        {
            return _binder.Bind<IFactoryUntyped<TContract>>()
                .ToMethod(c => new FactoryMethodUntyped<TContract>(c.Container, method));
        }

        public BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactoryUntyped<TContract>
        {
            return _binder.Bind<IFactoryUntyped<TContract>>(_identifier)
                .ToTransient<TFactory>();
        }

        public BindingConditionSetter ToFactory()
        {
            return _binder.Bind<IFactoryUntyped<TContract>>()
                .ToSingle<FactoryUntyped<TContract>>();
        }

        public BindingConditionSetter ToFactory<TConcrete>()
            where TConcrete : TContract
        {
            return _binder.Bind<IFactoryUntyped<TContract>>()
                .ToSingle<FactoryUntyped<TContract, TConcrete>>();
        }
    }
}
