using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;
using ModestTree;

namespace Zenject
{
    // Instantiate given concrete class
    [System.Diagnostics.DebuggerStepThrough]
    public class FactoryUntyped<TContract, TConcrete> : IFactoryUntyped<TContract> where TConcrete : TContract
    {
        [Inject]
        IInstantiator _instantiator = null;

        // So it can be created without using instantiator
        public IInstantiator Instantiator
        {
            get
            {
                return _instantiator;
            }
        }

        public virtual TContract Create(params object[] constructorArgs)
        {
            return _instantiator.Instantiate<TConcrete>(constructorArgs);
        }

        public IEnumerable<ZenjectResolveException> Validate(params Type[] extras)
        {
            return _instantiator.Resolver.ValidateObjectGraph<TConcrete>(extras);
        }
    }

    // Instantiate given contract class
    [System.Diagnostics.DebuggerStepThrough]
    public class FactoryUntyped<TContract> : IFactoryUntyped<TContract>
    {
        [Inject]
        IInstantiator _instantiator = null;

        [InjectOptional]
        Type _concreteType = null;

        // So it can be created without using container
        public IInstantiator Instantiator
        {
            get
            {
                return _instantiator;
            }
        }

        public Type ConcreteType
        {
            get
            {
                return _concreteType;
            }
        }

        [PostInject]
        void Initialize()
        {
            if (_concreteType == null)
            {
                _concreteType = typeof(TContract);
            }

            if (!_concreteType.DerivesFromOrEqual(typeof(TContract)))
            {
                throw new ZenjectResolveException(
                    "Expected type '{0}' to derive from '{1}'".Fmt(_concreteType.Name(), typeof(TContract).Name()));
            }
        }

        public virtual TContract Create(params object[] constructorArgs)
        {
            return (TContract)_instantiator.Instantiate(_concreteType, constructorArgs);
        }

        public IEnumerable<ZenjectResolveException> Validate(params Type[] extras)
        {
            return _instantiator.Resolver.ValidateObjectGraph(_concreteType, extras);
        }
    }
}
