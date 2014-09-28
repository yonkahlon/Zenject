using System;
using System.Collections.Generic;

namespace ModestTree.Zenject
{
    public interface IValidatable
    {
        IEnumerable<ZenjectResolveException> Validate();
    }

    public abstract class ValidatableFactory<T> : IValidatable
    {
        [Inject]
        protected readonly DiContainer _container;

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<T>(GetProvidedTypes());
        }

        protected virtual Type[] GetProvidedTypes()
        {
            // Optional
            return new Type[0];
        }
    }

    // Zero parameters
    public class TypedFactory<T> : ValidatableFactory<T>
    {
        public T Create()
        {
            return _container.Instantiate<T>();
        }
    }

    // One parameter
    public class TypedFactory<TParam1, TValue>
        : ValidatableFactory<TValue>
    {
        public virtual TValue Create(TParam1 param)
        {
            return _container.Instantiate<TValue>(param);
        }

        protected override Type[] GetProvidedTypes()
        {
            return new[] { typeof(TParam1) };
        }
    }

    // Two parameters
    public class TypedFactory<TParam1, TParam2, TValue> : ValidatableFactory<TValue>
    {
        public virtual TValue Create(TParam1 param1, TParam2 param2)
        {
            return _container.Instantiate<TValue>(param1, param2);
        }

        protected override Type[] GetProvidedTypes()
        {
            return new[] { typeof(TParam1), typeof(TParam2) };
        }
    }

    // Three parameters
    public class TypedFactory<TParam1, TParam2, TParam3, TValue>
        : ValidatableFactory<TValue>
    {
        public virtual TValue Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return _container.Instantiate<TValue>(param1, param2, param3);
        }

        protected override Type[] GetProvidedTypes()
        {
            return new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3) };
        }
    }

    // Four parameters
    public class TypedFactory<TParam1, TParam2, TParam3, TParam4, TValue>
        : ValidatableFactory<TValue>
    {
        public virtual TValue Create(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return _container.Instantiate<TValue>(param1, param2, param3, param4);
        }

        protected override Type[] GetProvidedTypes()
        {
            return new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4) };
        }
    }
}
