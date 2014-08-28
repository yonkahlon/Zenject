using System;
using System.Collections.Generic;

namespace ModestTree.Zenject
{
    public abstract class FactoryCustom<TValue> : IValidatable
    {
        [Inject]
        readonly DiContainer _container;

        protected TValue Instantiate(params object[] constructorArgs)
        {
            return _container.Instantiate<TValue>(constructorArgs);
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(GetDynamicParams());
        }

        protected abstract Type[] GetDynamicParams();
    }
}
