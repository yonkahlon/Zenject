using System;
using System.Collections.Generic;

namespace Zenject
{
    // Zero parameters
    public sealed class FactoryMethod<TValue> : IValidatableFactory, IFactory<TValue>
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        readonly Func<DiContainer, TValue> _method = null;

        public Type ConstructedType
        {
            get { return typeof(TValue); }
        }

        public Type[] ProvidedTypes
        {
            get { return new Type[0]; }
        }

        public TValue Create()
        {
            return _method(_container);
        }
    }

    // One parameters
    public sealed class FactoryMethod<TParam1, TValue> : IValidatableFactory, IFactory<TParam1, TValue>
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        readonly Func<DiContainer, TParam1, TValue> _method = null;

        public Type ConstructedType
        {
            get { return typeof(TValue); }
        }

        public Type[] ProvidedTypes
        {
            get { return new Type[] { typeof(TParam1) }; }
        }

        public TValue Create(TParam1 param)
        {
            return _method(_container, param);
        }
    }

    // Two parameters
    public sealed class FactoryMethod<TParam1, TParam2, TValue> : IValidatableFactory, IFactory<TParam1, TParam2, TValue>
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        readonly Func<DiContainer, TParam1, TParam2, TValue> _method = null;

        public Type ConstructedType
        {
            get { return typeof(TValue); }
        }

        public Type[] ProvidedTypes
        {
            get { return new Type[] { typeof(TParam1), typeof(TParam2) }; }
        }

        public TValue Create(TParam1 param1, TParam2 param2)
        {
            return _method(_container, param1, param2);
        }
    }

    // Three parameters
    public sealed class FactoryMethod<TParam1, TParam2, TParam3, TValue> : IValidatableFactory, IFactory<TParam1, TParam2, TParam3, TValue>
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        readonly Func<DiContainer, TParam1, TParam2, TParam3, TValue> _method = null;

        public Type ConstructedType
        {
            get { return typeof(TValue); }
        }

        public Type[] ProvidedTypes
        {
            get { return new Type[] { typeof(TParam1), typeof(TParam2), typeof(TParam3) }; }
        }

        public TValue Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return _method(_container, param1, param2, param3);
        }
    }
}
