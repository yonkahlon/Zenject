using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    internal class SingletonLazyCreatorByType : SingletonLazyCreatorBase
    {
        object _instance;

        public SingletonLazyCreatorByType(
            SingletonId id, SingletonProviderMap owner)
            : base(id, owner)
        {
        }

        public override object GetInstance(InjectContext context)
        {
            if (_instance == null)
            {
                InitInstance(context);
                Assert.IsNotNull(_instance);
            }

            return _instance;
        }

        void InitInstance(InjectContext context)
        {
            var concreteType = GetTypeToInstantiate(context.MemberType);

            bool autoInject = false;

            _instance = context.Container.InstantiateExplicit(
                concreteType, new List<TypeValuePair>(), context, Id.Identifier, autoInject);

            Assert.IsNotNull(_instance);

            // Inject after we've instantiated and set _instance so that we can support circular dependencies
            // as PostInject or field parameters
            context.Container.InjectExplicit(
                _instance, Enumerable.Empty<TypeValuePair>(), true,
                TypeAnalyzer.GetInfo(_instance.GetType()), context, Id.Identifier);
        }

        Type GetTypeToInstantiate(Type contractType)
        {
            if (Id.Type.IsOpenGenericType())
            {
                Assert.That(!contractType.IsAbstract);
                Assert.That(contractType.GetGenericTypeDefinition() == Id.Type);
                return contractType;
            }

            Assert.That(Id.Type.DerivesFromOrEqual(contractType));
            return Id.Type;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return context.Container.ValidateObjectGraph(Id.Type, context, Id.Identifier);
        }
    }
}

