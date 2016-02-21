using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Zenject.Internal;

namespace Zenject
{
    public class FacadeInstallerSingletonProviderCreator
    {
        readonly Dictionary<SingletonId, FacadeInstallerSingletonLazyCreator> _creators = new Dictionary<SingletonId, FacadeInstallerSingletonLazyCreator>();
        readonly SingletonRegistry _singletonRegistry;
        readonly DiContainer _container;

        public FacadeInstallerSingletonProviderCreator(
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

        FacadeInstallerSingletonLazyCreator AddCreator(
            SingletonId id, Type installerType)
        {
            FacadeInstallerSingletonLazyCreator creator;

            if (_creators.TryGetValue(id, out creator))
            {
                if (creator.InstallerType != installerType)
                {
                    throw new ZenjectBindException(
                        "Cannot use 'ToSingleFacadeInstaller' with the same type and different installer types!");
                }
            }
            else
            {
                creator = new FacadeInstallerSingletonLazyCreator(id, this, installerType, _container);
                _creators.Add(id, creator);
            }

            return creator;
        }

        public FacadeInstallerSingletonProvider CreateProvider(
            Type concreteType, string concreteIdentifier, Type installerType)
        {
            var singletonId = new SingletonId(concreteType, concreteIdentifier);
            var creator = AddCreator(singletonId, installerType);

            return new FacadeInstallerSingletonProvider(
                creator, singletonId, _singletonRegistry);
        }
    }
}
