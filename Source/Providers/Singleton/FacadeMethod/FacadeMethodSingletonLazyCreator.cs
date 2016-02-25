using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public class FacadeMethodSingletonLazyCreator
    {
        readonly Action<DiContainer> _installMethod;
        readonly SingletonId _id;
        readonly FacadeMethodSingletonProviderCreator _owner;
        readonly DiContainer _container;

        int _referenceCount;
        object _instance;

        public FacadeMethodSingletonLazyCreator(
            SingletonId id, FacadeMethodSingletonProviderCreator owner,
            Action<DiContainer> installMethod, DiContainer container)
        {
            _owner = owner;
            _installMethod = installMethod;
            _id = id;
            _container = container;
        }

        public Delegate InstallMethod
        {
            get
            {
                return _installMethod;
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
            return FacadeUtil.Validate(_id.ConcreteType, _container, _installMethod, null);
        }

        public object GetInstance(InjectContext context)
        {
            if (_instance == null)
            {
                _instance = FacadeUtil.GetFacadeInstance(
                    _container, _id.ConcreteType, _installMethod, null);
                Assert.IsNotNull(_instance);
            }

            return _instance;
        }
    }
}


