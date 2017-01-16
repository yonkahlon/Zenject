using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public class PooledFactoryExpandBinder<TContract> : FactoryToChoiceIdBinder<TContract>
    {
        public PooledFactoryExpandBinder(
            BindInfo bindInfo, FactoryBindInfo factoryBindInfo, PooledFactoryBindInfo poolBindInfo)
            : base(bindInfo, factoryBindInfo)
        {
            PooledFactoryBindInfo = poolBindInfo;

            ExpandByOneAtATime();
        }

        protected PooledFactoryBindInfo PooledFactoryBindInfo
        {
            get; private set;
        }

        public FactoryToChoiceIdBinder<TContract> ExpandByOneAtATime()
        {
            PooledFactoryBindInfo.ExpandMethod = PoolExpandMethods.OneAtATime;
            return this;
        }

        public FactoryToChoiceIdBinder<TContract> ExpandByDoubling()
        {
            PooledFactoryBindInfo.ExpandMethod = PoolExpandMethods.Double;
            return this;
        }
    }
}

