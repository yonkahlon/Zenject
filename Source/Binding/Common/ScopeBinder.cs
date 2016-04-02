using System;
using ModestTree;

namespace Zenject
{
    public class ScopeBinder : ArgumentsBinder
    {
        public ScopeBinder(StandardBindingDescriptor binding)
            : base(binding)
        {
        }

        public ArgumentsBinder AsSingle()
        {
            return AsSingle(null);
        }

        public ArgumentsBinder AsSingle(string concreteIdentifier)
        {
            Assert.IsNull(Binding.ConcreteIdentifier);

            Binding.CreationType = CreationTypes.Singleton;
            Binding.ConcreteIdentifier = concreteIdentifier;

            return new ArgumentsBinder(Binding);
        }

        public ArgumentsBinder AsCached()
        {
            Binding.CreationType = CreationTypes.Cached;

            return new ArgumentsBinder(Binding);
        }

        // Note that this is the default so it's not necessary to call this
        public ArgumentsBinder AsTransient()
        {
            Binding.CreationType = CreationTypes.Transient;

            return new ArgumentsBinder(Binding);
        }
    }
}
