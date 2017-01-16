using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public class PooledFactoryInitialSizeBinder<TContract> : PooledFactoryExpandBinder<TContract>
    {
        public PooledFactoryInitialSizeBinder(
            BindInfo bindInfo, FactoryBindInfo factoryBindInfo, PooledFactoryBindInfo poolBindInfo)
            : base(bindInfo, factoryBindInfo, poolBindInfo)
        {
        }

        public PooledFactoryExpandBinder<TContract> WithInitialSize(int size)
        {
            PooledFactoryBindInfo.InitialSize = size;
            return this;
        }

        public FactoryToChoiceIdBinder<TContract> WithFixedSize(int size)
        {
            PooledFactoryBindInfo.InitialSize = size;
            PooledFactoryBindInfo.ExpandMethod = PoolExpandMethods.Fixed;
            return this;
        }
    }
}

