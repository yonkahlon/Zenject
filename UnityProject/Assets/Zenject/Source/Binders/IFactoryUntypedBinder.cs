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
        readonly ContainerTypes _containerType;
        readonly DiContainer _container;
        readonly string _identifier;

        public IFactoryUntypedBinder(
            DiContainer binder, string identifier,
            ContainerTypes containerType)
        {
            _containerType = containerType;
            _container = binder;
            _identifier = identifier;
        }

        public BindingConditionSetter ToInstance(TContract instance)
        {
            return ToMethod((c, args) => instance);
        }

        public BindingConditionSetter ToMethod(
            Func<DiContainer, object[], TContract> method)
        {
            return _container.Bind<IFactoryUntyped<TContract>>()
                .ToMethod(c => new FactoryMethodUntyped<TContract>((_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container), method));
        }

        public BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactoryUntyped<TContract>
        {
            return _container.Bind<IFactoryUntyped<TContract>>(_identifier)
                .ToTransient<TFactory>(_containerType);
        }

        public BindingConditionSetter ToFactory()
        {
            return _container.Bind<IFactoryUntyped<TContract>>()
                .ToSingle<FactoryUntyped<TContract>>();
        }

        public BindingConditionSetter ToFactory<TConcrete>()
            where TConcrete : TContract
        {
            return _container.Bind<IFactoryUntyped<TContract>>()
                .ToSingle<FactoryUntyped<TContract, TConcrete>>();
        }
    }
}
