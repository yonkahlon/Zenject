using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public class MethodProvider<T> : ProviderBase
    {
        readonly DiContainer _container;
        readonly Func<DiContainer, InjectContext, T> _method;

        public MethodProvider(Func<DiContainer, InjectContext, T> method, DiContainer container)
        {
            _method = method;
            _container = container;
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override bool HasInstance(Type contractType)
        {
            Assert.That(typeof(T).DerivesFromOrEqual(contractType));
            return false;
        }

        public override object GetInstance(Type contractType, InjectContext context)
        {
            Assert.That(typeof(T).DerivesFromOrEqual(contractType));
            var obj = _method(_container, context);

            Assert.That(obj != null, () =>
                "Method provider returned null when looking up type '{0}'. \nObject graph:\n{1}".Fmt(typeof(T).Name(), DiContainer.GetCurrentObjectGraph()));

            return obj;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(Type contractType, InjectContext context)
        {
            return Enumerable.Empty<ZenjectResolveException>();
        }
    }
}
