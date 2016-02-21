using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public class FacadeInstallerSingletonLazyCreator
    {
        readonly Type _installerType;
        readonly SingletonId _id;
        readonly FacadeInstallerSingletonProviderCreator _owner;
        readonly DiContainer _container;

        int _referenceCount;
        object _instance;

        public FacadeInstallerSingletonLazyCreator(
            SingletonId id, FacadeInstallerSingletonProviderCreator owner,
            Type installerType, DiContainer container)
        {
            _owner = owner;
            _installerType = installerType;
            _id = id;
            _container = container;
        }

        public Type InstallerType
        {
            get
            {
                return _installerType;
            }
        }

        public void IncRefCount()
        {
            _referenceCount += 1;
        }

        public void DecRefCount()
        {
            _referenceCount -= 1;

            if (_referenceCount <= 0)
            {
                _owner.RemoveCreator(_id);
            }
        }

        public IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return FacadeUtil.Validate(_id.ConcreteType, _container, null, _installerType);
        }

        public object GetInstance(InjectContext context)
        {
            if (_instance == null)
            {
                _instance = FacadeUtil.GetFacadeInstance(
                    _container, _id.ConcreteType, null, _installerType);
                Assert.IsNotNull(_instance);
            }

            return _instance;
        }
    }
}


