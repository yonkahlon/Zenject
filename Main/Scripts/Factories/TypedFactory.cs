using System;
using System.Collections.Generic;

namespace ModestTree.Zenject
{
    public interface IValidatable
    {
        IEnumerable<ZenjectResolveException> Validate();
    }

    // Zero parameters
    public interface ITypedFactory<TBase> : IValidatable
    {
        TBase Create();
    }

    public class TypedFactory<T> : ITypedFactory<T>
    {
        [Inject]
        protected readonly DiContainer _container;

        public T Create()
        {
            return _container.Instantiate<T>();
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<T>();
        }
    }

    // One parameter
    public class TypedFactory<TParam1, TValue>
        : IValidatable
    {
        [Inject]
        protected readonly DiContainer _container;

        public virtual TValue Create(TParam1 param)
        {
            return _container.Instantiate<TValue>(param);
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(typeof(TParam1));
        }
    }

    // Two parameters
    public class TypedFactory<TParam1, TParam2, TValue> : IValidatable
    {
        [Inject]
        protected readonly DiContainer _container;

        public virtual TValue Create(TParam1 param1, TParam2 param2)
        {
            return _container.Instantiate<TValue>(param1, param2);
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(
                typeof(TParam1), typeof(TParam2));
        }
    }

    // Three parameters
    public class TypedFactory<TParam1, TParam2, TParam3, TValue>
        : IValidatable
    {
        [Inject]
        protected readonly DiContainer _container;

        public virtual TValue Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return _container.Instantiate<TValue>(param1, param2, param3);
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(
                typeof(TParam1), typeof(TParam2), typeof(TParam3));
        }
    }

    // Four parameters
    public class TypedFactory<TParam1, TParam2, TParam3, TParam4, TValue>
        : IValidatable
    {
        [Inject]
        protected readonly DiContainer _container;

        public virtual TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return _container.Instantiate<TValue>(param1, param2, param3, param4);
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(
                typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4));
        }
    }
}
