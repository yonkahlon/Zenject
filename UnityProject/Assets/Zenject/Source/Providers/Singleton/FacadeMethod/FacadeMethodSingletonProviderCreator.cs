using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Zenject.Internal;

namespace Zenject
{
    public class FacadeMethodSingletonProviderCreator
    {
        readonly Dictionary<SingletonId, FacadeMethodSingletonLazyCreator> _creators = new Dictionary<SingletonId, FacadeMethodSingletonLazyCreator>();
        readonly SingletonRegistry _singletonRegistry;
        readonly DiContainer _container;

        public FacadeMethodSingletonProviderCreator(
            DiContainer container,
            SingletonRegistry singletonRegistry)
        {
            _singletonRegistry = singletonRegistry;
            _container = container;
        }

        public void RemoveCreator(SingletonId id)
        {
            bool success = _creators.Remove(id);
            Assert.That(success);
        }

        FacadeMethodSingletonLazyCreator AddCreator(
            SingletonId id, Action<DiContainer> installMethod)
        {
            FacadeMethodSingletonLazyCreator creator;

            if (_creators.TryGetValue(id, out creator))
            {
                if (!ZenUtilInternal.AreFunctionsEqual(creator.InstallMethod, installMethod))
                {
                    throw new ZenjectBindException(
                        "Cannot use 'ToSingleFacadeMethod' with the same type and multiple different methods!");
                }
            }
            else
            {
                creator = new FacadeMethodSingletonLazyCreator(id, this, installMethod, _container);
                _creators.Add(id, creator);
            }

            return creator;
        }

        public FacadeMethodSingletonProvider CreateProvider(
            Type concreteType, string concreteIdentifier, Action<DiContainer> installMethod)
        {
            var singletonId = new SingletonId(concreteType, concreteIdentifier);
            var creator = AddCreator(singletonId, installMethod);

            return new FacadeMethodSingletonProvider(
                creator, singletonId, _singletonRegistry);
        }
    }
}
