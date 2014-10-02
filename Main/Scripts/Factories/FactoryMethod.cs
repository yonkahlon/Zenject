using System;
using System.Collections.Generic;

namespace ModestTree.Zenject
{
    // Zero parameters
    public sealed class FactoryMethod<TValue>
        : ValidatableFactory<TValue>
    {
        [Inject]
        readonly Func<DiContainer, TValue> _method;

        public override TValue Create()
        {
            return _method(_container);
        }
    }

    // One parameters
    public sealed class FactoryMethod<TParam1, TValue>
        : ValidatableFactory<TParam1, TValue>
    {
        [Inject]
        readonly Func<DiContainer, TParam1, TValue> _method;

        public override TValue Create(TParam1 param)
        {
            return _method(_container, param);
        }
    }

    // Two parameters
    public sealed class FactoryMethod<TParam1, TParam2, TValue>
        : ValidatableFactory<TParam1, TParam2, TValue>
    {
        [Inject]
        readonly Func<DiContainer, TParam1, TParam2, TValue> _method;

        public override TValue Create(TParam1 param1, TParam2 param2)
        {
            return _method(_container, param1, param2);
        }
    }

    // Three parameters
    public sealed class FactoryMethod<TParam1, TParam2, TParam3, TValue>
        : ValidatableFactory<TParam1, TParam2, TParam3, TValue>
    {
        [Inject]
        readonly Func<DiContainer, TParam1, TParam2, TParam3, TValue> _method;

        public override TValue Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return _method(_container, param1, param2, param3);
        }
    }
}
