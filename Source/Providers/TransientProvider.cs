using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public class TransientProvider : ProviderBase
    {
        readonly Type _concreteType;
        readonly DiContainer _container;
        readonly ContainerTypes _containerType;

        public TransientProvider(
            Type concreteType, DiContainer container, ContainerTypes containerType)
        {
            _container = container;
            _containerType = containerType;
            _concreteType = concreteType;

            var singletonMark = container.SingletonRegistry.TryGetSingletonType(concreteType);

            if (singletonMark.HasValue)
            {
                throw new ZenjectBindException(
                    "Attempted to use 'ToTransient' with the same type ('{0}') that is already marked with '{1}'".Fmt(concreteType.Name(), singletonMark.Value));
            }
        }

        public override Type GetInstanceType()
        {
            return _concreteType;
        }

        public override object GetInstance(InjectContext context)
        {
            var container = _containerType == ContainerTypes.RuntimeContainer ? context.Container : _container;

            var obj = container.InstantiateExplicit(
                GetTypeToInstantiate(context.MemberType), new List<TypeValuePair>(), context, null, true);
            Assert.That(obj != null);
            return obj;
        }

        Type GetTypeToInstantiate(Type contractType)
        {
            if (_concreteType.IsOpenGenericType())
            {
                Assert.That(!contractType.IsAbstract);
                Assert.That(contractType.GetGenericTypeDefinition() == _concreteType);
                return contractType;
            }

            Assert.That(_concreteType.DerivesFromOrEqual(contractType));
            return _concreteType;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            var container = _containerType == ContainerTypes.RuntimeContainer ? context.Container : _container;

            return container.ValidateObjectGraph(_concreteType, context);
        }
    }
}
