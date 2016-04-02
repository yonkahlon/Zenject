using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public interface IBindingFinalizer
    {
        void FinalizeBinding(
            DiContainer container, StandardBindingDescriptor binding);
    }
}
