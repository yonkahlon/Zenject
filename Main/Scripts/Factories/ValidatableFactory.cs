using System;
using System.Collections.Generic;

namespace ModestTree.Zenject
{
    public abstract class ValidatableFactoryBase<TValue> : IValidatable
    {
        [Inject]
        protected readonly DiContainer _container;

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(GetProvidedTypes());
        }

        protected abstract Type[] GetProvidedTypes();
    }

    // Zero parameters
    public abstract class ValidatableFactory<TValue>
        : ValidatableFactoryBase<TValue>, IFactory<TValue>
    {
        protected override Type[] GetProvidedTypes()
        {
            return new Type[0];
        }

        public abstract TValue Create();
    }

    // One parameter
    public abstract class ValidatableFactory<TParam1, TValue>
        : ValidatableFactoryBase<TValue>, IFactory<TParam1, TValue>
    {
        protected override Type[] GetProvidedTypes()
        {
            return new[] { typeof(TParam1) };
        }

        public abstract TValue Create(TParam1 param);
    }

    // Two parameters
    public abstract class ValidatableFactory<TParam1, TParam2, TValue> :
        ValidatableFactoryBase<TValue>, IFactory<TParam1, TParam2, TValue>
    {
        protected override Type[] GetProvidedTypes()
        {
            return new[] { typeof(TParam1), typeof(TParam2) };
        }

        public abstract TValue Create(TParam1 param1, TParam2 param2);
    }

    // Three parameters
    public abstract class ValidatableFactory<TParam1, TParam2, TParam3, TValue>
        : ValidatableFactoryBase<TValue>, IFactory<TParam1, TParam2, TParam3, TValue>
    {
        protected override Type[] GetProvidedTypes()
        {
            return new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3) };
        }

        public abstract TValue Create(TParam1 param1, TParam2 param2, TParam3 param3);
    }

    // Four parameters
    public abstract class ValidatableFactory<TParam1, TParam2, TParam3, TParam4, TValue>
        : ValidatableFactoryBase<TValue>, IFactory<TParam1, TParam2, TParam3, TParam4, TValue>
    {
        protected override Type[] GetProvidedTypes()
        {
            return new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4) };
        }

        public abstract TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4);
    }
}
