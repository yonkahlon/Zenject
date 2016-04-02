using System;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public class InheritInSubContainersBinder
    {
        readonly StandardBindingDescriptor _binding;

        public InheritInSubContainersBinder(StandardBindingDescriptor binding)
        {
            _binding = binding;
        }

        public StandardBindingDescriptor Binding
        {
            get
            {
                return _binding;
            }
        }

        public void InheritInSubContainers()
        {
            Binding.InheritInSubContainers = true;
        }
    }
}

