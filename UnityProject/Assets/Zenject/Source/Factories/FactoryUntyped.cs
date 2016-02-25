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
        DiContainer _container = null;

        // So it can be created without using instantiator
        public DiContainer Container
        {
            get
            {
                return _container;
            }
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
    [System.Diagnostics.DebuggerStepThrough]
    public class FactoryUntyped<TContract> : IFactoryUntyped<TContract>
    {
        [Inject]
        DiContainer _container = null;

        [InjectOptional]
        Type _concreteType = null;

        // So it can be created without using container
        public DiContainer Container
        {
            get
            {
                return _container;
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
            return (TContract)_container.Instantiate(_concreteType, constructorArgs);
        }

        public IEnumerable<ZenjectResolveException> Validate(params Type[] extras)
        {
            return _container.ValidateObjectGraph(_concreteType, extras);
        }
    }
}
