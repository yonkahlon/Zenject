using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    // Zero parameters
    public class PooledFactory<TValue> : DynamicPooledFactory<TValue>, IPooledFactory<TValue>
        // This generic constraint might be undesirable if using an abstract pooled factory
        // where the base class does not implement IPoolable, but this seems less important
        // than catching the compiler error for the other much more common cases
        where TValue : IPoolable
    {
        [Inject]
        void Construct()
        {
            Assert.That(ConcreteType.DerivesFromOrEqual<IPoolable>(),
                "Expected pooled type '{0}' to derive from IPoolable", typeof(TValue));
        }

        // If you were hoping to override this method, use BindPooledFactory<>.ToFactory instead
        public TValue Spawn()
        {
            var item = GetInternal();
            ((IPoolable)item).OnSpawned();
            return item;
        }
    }

    // One parameter
    public class PooledFactory<TParam1, TValue>
        : DynamicPooledFactory<TValue>, IPooledFactory<TParam1, TValue>
        // This generic constraint might be undesirable if using an abstract pooled factory
        // where the base class does not implement IPoolable, but this seems less important
        // than catching the compiler error for the other much more common cases
        where TValue : IPoolable<TParam1>
    {
        [Inject]
        void Construct()
        {
            Assert.That(ConcreteType.DerivesFromOrEqual<IPoolable<TParam1>>(),
                "Expected pooled type '{0}' to derive from {1}", typeof(TValue), typeof(IPoolable<TParam1>));
        }

        // If you were hoping to override this method, use BindPooledFactory<>.ToPooledFactory instead
        public TValue Spawn(TParam1 param)
        {
            var item = GetInternal();
            ((IPoolable<TParam1>)item).OnSpawned(param);
            return item;
        }
    }

    // Two parameters
    public class PooledFactory<TParam1, TParam2, TValue>
        : DynamicPooledFactory<TValue>, IPooledFactory<TParam1, TParam2, TValue>
        // This generic constraint might be undesirable if using an abstract pooled factory
        // where the base class does not implement IPoolable, but this seems less important
        // than catching the compiler error for the other much more common cases
        where TValue : IPoolable<TParam1, TParam2>
    {
        [Inject]
        void Construct()
        {
            Assert.That(ConcreteType.DerivesFromOrEqual<IPoolable<TParam1, TParam2>>(),
            "Expected pooled type '{0}' to derive from {1}", typeof(TValue), typeof(IPoolable<TParam1, TParam2>));
        }

        // If you were hoping to override this method, use BindPooledFactory<>.ToPooledFactory instead
        public TValue Spawn(TParam1 param1, TParam2 param2)
        {
            var item = GetInternal();
            ((IPoolable<TParam1, TParam2>)item).OnSpawned(param1, param2);
            return item;
        }
    }

    // Three parameters
    public class PooledFactory<TParam1, TParam2, TParam3, TValue>
        : DynamicPooledFactory<TValue>, IPooledFactory<TParam1, TParam2, TParam3, TValue>
        // This generic constraint might be undesirable if using an abstract pooled factory
        // where the base class does not implement IPoolable, but this seems less important
        // than catching the compiler error for the other much more common cases
        where TValue : IPoolable<TParam1, TParam2, TParam3>
    {
        [Inject]
        void Construct()
        {
            Assert.That(ConcreteType.DerivesFromOrEqual<IPoolable<TParam1, TParam2, TParam3>>(),
                "Expected pooled type '{0}' to derive from {1}", typeof(TValue), typeof(IPoolable<TParam1, TParam2, TParam3>));
        }

        // If you were hoping to override this method, use BindPooledFactory<>.ToPooledFactory instead
        public TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            var item = GetInternal();
            ((IPoolable<TParam1, TParam2, TParam3>)item).OnSpawned(param1, param2, param3);
            return item;
        }
    }

    // Four parameters
    public class PooledFactory<TParam1, TParam2, TParam3, TParam4, TValue>
        : DynamicPooledFactory<TValue>, IPooledFactory<TParam1, TParam2, TParam3, TParam4, TValue>
        // This generic constraint might be undesirable if using an abstract pooled factory
        // where the base class does not implement IPoolable, but this seems less important
        // than catching the compiler error for the other much more common cases
        where TValue : IPoolable<TParam1, TParam2, TParam3, TParam4>
    {
        [Inject]
        void Construct()
        {
            Assert.That(ConcreteType.DerivesFromOrEqual<IPoolable<TParam1, TParam2, TParam3, TParam4>>(),
                "Expected pooled type '{0}' to derive from {1}", typeof(TValue), typeof(IPoolable<TParam1, TParam2, TParam3, TParam4>));
        }

        // If you were hoping to override this method, use BindPooledFactory<>.ToPooledFactory instead
        public TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            var item = GetInternal();
            ((IPoolable<TParam1, TParam2, TParam3, TParam4>)item).OnSpawned(param1, param2, param3, param4);
            return item;
        }
    }

    // Five parameters
    public class PooledFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TValue>
        : DynamicPooledFactory<TValue>, IPooledFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TValue>
        // This generic constraint might be undesirable if using an abstract pooled factory
        // where the base class does not implement IPoolable, but this seems less important
        // than catching the compiler error for the other much more common cases
        where TValue : IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        [Inject]
        void Construct()
        {
            Assert.That(ConcreteType.DerivesFromOrEqual<IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5>>(),
                "Expected pooled type '{0}' to derive from {1}", typeof(TValue), typeof(IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5>));
        }

        // If you were hoping to override this method, use BindPooledFactory<>.ToPooledFactory instead
        public TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            var item = GetInternal();
            ((IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5>)item).OnSpawned(param1, param2, param3, param4, param5);
            return item;
        }
    }
}
