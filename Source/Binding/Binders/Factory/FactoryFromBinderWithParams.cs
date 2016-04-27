using System;
using ModestTree;
using System.Linq;

namespace Zenject
{
    public class FactoryFromBinderWithParams<TContract> : FactoryFromBinderBase<TContract>
    {
        public FactoryFromBinderWithParams(
            BindInfo bindInfo, BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo, finalizerWrapper)
        {
        }
    }
}

