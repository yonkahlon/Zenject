using System.Collections.Generic;

namespace Zenject
{
    public interface IFactorySingletonLazyCreator
    {
        IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context);
        object GetInstance(InjectContext context);
        void DecRefCount();
        void IncRefCount();
    }
}

