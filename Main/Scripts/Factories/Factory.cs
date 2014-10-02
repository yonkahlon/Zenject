using System;
using System.Collections.Generic;

namespace ModestTree.Zenject
{
    // Zero parameters
    public class Factory<T> : ValidatableFactory<T>
    {
        public override T Create()
        {
            return _container.Instantiate<T>();
        }
    }

    // One parameter
    public class Factory<TParam1, TValue> : ValidatableFactory<TParam1, TValue>
    {
        public override TValue Create(TParam1 param)
        {
            return _container.InstantiateExplicit<TValue>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param),
                });
        }
    }

    // Two parameters
    public class Factory<TParam1, TParam2, TValue>
        : ValidatableFactory<TParam1, TParam2, TValue>
    {
        public override TValue Create(TParam1 param1, TParam2 param2)
        {
            return _container.InstantiateExplicit<TValue>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                });
        }
    }

    // Three parameters
    public class Factory<TParam1, TParam2, TParam3, TValue>
        : ValidatableFactory<TParam1, TParam2, TParam3, TValue>
    {
        public override TValue Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return _container.InstantiateExplicit<TValue>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                    InstantiateUtil.CreateTypePair(param3),
                });
        }
    }

    // Four parameters
    public class Factory<TParam1, TParam2, TParam3, TParam4, TValue>
        : ValidatableFactory<TParam1, TParam2, TParam3, TParam4, TValue>
    {
        public override TValue Create(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return _container.InstantiateExplicit<TValue>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                    InstantiateUtil.CreateTypePair(param3),
                    InstantiateUtil.CreateTypePair(param4),
                });
        }
    }
}
