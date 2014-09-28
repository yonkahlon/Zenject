using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModestTree.Zenject
{
    public abstract class TypedGameObjectFactory : IValidatable
    {
        [Inject]
        protected readonly DiContainer _container;

        [Inject]
        protected readonly GameObject _prefab;

        [Inject]
        protected readonly GameObjectInstantiator _instantiator;

        public abstract IEnumerable<ZenjectResolveException> Validate();
    }

    public class TypedGameObjectFactory<TValue> : TypedGameObjectFactory
         where TValue : Component
    {
        public virtual TValue Create()
        {
            return _instantiator.Instantiate<TValue>(_prefab);
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>();
        }
    }

    // One parameter
    public class TypedGameObjectFactory<TParam1, TValue> : TypedGameObjectFactory
        where TValue : Component
    {
        public virtual TValue Create(TParam1 param)
        {
            return _instantiator.Instantiate<TValue>(_prefab, param);
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(typeof(TParam1));
        }
    }

    // Two parameters
    public class TypedGameObjectFactory<TParam1, TParam2, TValue> : TypedGameObjectFactory
        where TValue : Component
    {
        public virtual TValue Create(TParam1 param1, TParam2 param2)
        {
            return _instantiator.Instantiate<TValue>(_prefab, param1, param2);
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(typeof(TParam1), typeof(TParam2));
        }
    }

    // Three parameters
    public class TypedGameObjectFactory<TParam1, TParam2, TParam3, TValue> : TypedGameObjectFactory
        where TValue : Component
    {
        public virtual TValue Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return _instantiator.Instantiate<TValue>(_prefab, param1, param2, param3);
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(typeof(TParam1), typeof(TParam2), typeof(TParam3));
        }
    }

    // Four parameters
    public class TypedGameObjectFactory<TParam1, TParam2, TParam3, TParam4, TValue> : TypedGameObjectFactory
        where TValue : Component
    {
        public virtual TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return _instantiator.Instantiate<TValue>(_prefab, param1, param2, param3, param4);
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4));
        }
    }
}
