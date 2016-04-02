using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public interface IBindingDescriptor
    {
        bool InheritInSubContainers
        {
            get;
        }

        void Finalize(DiContainer container);
    }
}

