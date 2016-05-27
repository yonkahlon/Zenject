using System;

namespace Zenject
{
    public interface IMethodSingletonLazyCreator
    {
        void DecRefCount();
        void IncRefCount();

        object GetInstance(InjectContext context);

        Delegate CreateMethod
        {
            get;
        }
    }
}


