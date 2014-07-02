#if !UNITY_WEBPLAYER && (NOT_UNITY || UNITY_EDITOR)

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Fasterflect;

namespace ModestTree.Zenject
{
    public class TransientMockProvider : ProviderBase
    {
        readonly DiContainer _container;

        Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        public TransientMockProvider(DiContainer container)
        {
            _container = container;
        }

        public override Type GetInstanceType()
        {
            return null;
        }

        public override bool HasInstance(Type contractType)
        {
            return false;
        }

        public override object GetInstance(Type contractType, InjectContext context)
        {
            object instance;

            if (!_instances.TryGetValue(contractType, out instance))
            {
                instance = typeof(Mock).Method(new Type[] { contractType }, "Of", Flags.StaticAnyVisibility).Invoke(null, null);
                _instances.Add(contractType, instance);
            }

            return instance;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(Type contractType, InjectContext context)
        {
            // Always succeeds
            return Enumerable.Empty<ZenjectResolveException>();
        }
    }
}


#endif
