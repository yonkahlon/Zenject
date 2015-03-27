using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;
using ModestTree;

namespace Zenject
{
    // Instantiate given concrete class
    public class FactoryUntyped<TContract, TConcrete> : IFactoryUntyped<TContract> where TConcrete : TContract
    {
        readonly DiContainer _container;

        public FactoryUntyped(DiContainer container)
        {
            _container = container;
        }

        public virtual TContract Create(params object[] constructorArgs)
        {
            return _container.Instantiate<TConcrete>(constructorArgs);
        }

        public IEnumerable<ZenjectResolveException> Validate(params Type[] extras)
        {
            return _container.ValidateObjectGraph<TConcrete>(extras);
        }
    }

    // Instantiate given contract class
    public class FactoryUntyped<TContract> : IFactoryUntyped<TContract>
    {
        readonly DiContainer _container;
        readonly Type _concreteType;

        [Inject]
        public FactoryUntyped(DiContainer container)
        {
            _container = container;
            _concreteType = typeof(TContract);
        }

        public FactoryUntyped(DiContainer container, Type concreteType)
        {
            if (!concreteType.DerivesFromOrEqual(typeof(TContract)))
            {
                throw new ZenjectResolveException(
                    "Expected type '{0}' to derive from '{1}'".Fmt(concreteType.Name(), typeof(TContract).Name()));
            }

            _container = container;
            _concreteType = concreteType;
        }

        public virtual TContract Create(params object[] constructorArgs)
        {
            return (TContract)_container.Instantiate(_concreteType, constructorArgs);
        }

        public IEnumerable<ZenjectResolveException> Validate(params Type[] extras)
        {
            return _container.ValidateObjectGraph(_concreteType, extras);
        }
    }
}
