using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public interface IDynamicFactory : IValidatable
    {
    }

    // Dynamic factories can be used to choose a creation method in an installer, using FactoryBinder
    public abstract class DynamicFactory<TValue> : IDynamicFactory
    {
        IProvider _provider;
        InjectContext _injectContext;

        [PostInject]
        void Init(IProvider provider, InjectContext injectContext)
        {
            _provider = provider;
            _injectContext = injectContext;
        }

        protected TValue CreateInternal(List<TypeValuePair> extraArgs)
        {
            var result = _provider.GetInstance(_injectContext, extraArgs);

            Assert.That(result == null || result.GetType().DerivesFromOrEqual<TValue>());

            return (TValue)result;
        }

        public IEnumerable<ZenjectException> Validate()
        {
            return _provider.Validate(_injectContext, ParamTypes.ToList());
        }

        protected abstract IEnumerable<Type> ParamTypes
        {
            get;
        }
    }
}
