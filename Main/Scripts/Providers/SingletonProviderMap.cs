using System;
using System.Collections.Generic;
using System.Linq;
using Fasterflect;

namespace ModestTree.Zenject
{
    public class SingletonProviderMap
    {
        Dictionary<Type, SingletonLazyCreator> _creators = new Dictionary<Type, SingletonLazyCreator>();
        DiContainer _container;

        public SingletonProviderMap(DiContainer container)
        {
            _container = container;
        }

        void RemoveCreator(Type instanceType)
        {
            bool success = _creators.Remove(instanceType);
            Assert.That(success);
        }

        public ProviderBase CreateProvider<TConcrete>()
        {
            return CreateProvider(typeof(TConcrete));
        }

        public ProviderBase CreateProvider<TConcrete>(TConcrete instance)
        {
            return CreateProvider(typeof(TConcrete), instance);
        }

        public ProviderBase CreateProvider(Type concreteType)
        {
            return CreateProvider(concreteType, null);
        }

        public ProviderBase CreateProvider(Type concreteType, object instance)
        {
            Assert.That(instance == null || instance.GetType() == concreteType);

            SingletonLazyCreator creator;

            if (!_creators.TryGetValue(concreteType, out creator))
            {
                creator = new SingletonLazyCreator(_container, this, concreteType);
                _creators.Add(concreteType, creator);
            }

            if (instance != null)
            {
                if (creator.HasCreatedInstance())
                {
                    throw new ZenjectBindException("Found multiple singleton instances bound to the type '{0}'".With(concreteType.Name()));
                }

                creator.SetInstance(instance);
            }

            creator.IncRefCount();

            return new SingletonProvider(_container, creator);
        }

        ////////////////////// Internal classes

        class SingletonLazyCreator
        {
            int _referenceCount;
            object _instance;
            Type _instanceType;
            SingletonProviderMap _owner;
            DiContainer _container;
            Instantiator _instantiator;

            public SingletonLazyCreator(
                DiContainer container, SingletonProviderMap owner, Type instanceType)
            {
                _container = container;
                _owner = owner;
                _instanceType = instanceType;
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
                    _owner.RemoveCreator(_instanceType);
                }
            }

            public void SetInstance(object instance)
            {
                Assert.IsNull(_instance);
                _instance = instance;
            }

            public bool HasCreatedInstance()
            {
                return _instance != null;
            }

            public Type GetInstanceType()
            {
                return _instanceType;
            }

            public object GetInstance(Type contractType)
            {
                if (_instance == null)
                {
                    if (_instantiator == null)
                    {
                        _instantiator = _container.Resolve<Instantiator>();
                    }

                    _instance = _instantiator.Instantiate(GetTypeToInstantiate(contractType));
                    Assert.That(_instance != null);
                }

                return _instance;
            }

            Type GetTypeToInstantiate(Type contractType)
            {
                if (_instanceType.IsOpenGenericType())
                {
                    Assert.That(!contractType.IsAbstract);
                    Assert.That(contractType.GetGenericTypeDefinition() == _instanceType);
                    return contractType;
                }

                Assert.That(_instanceType.DerivesFromOrEqual(contractType));
                return _instanceType;
            }
        }

        // NOTE: we need the provider seperate from the creator because
        // if we return the same provider multiple times then the condition
        // will get over-written
        class SingletonProvider : ProviderBase
        {
            SingletonLazyCreator _creator;
            DiContainer _container;

            public SingletonProvider(
                DiContainer container, SingletonLazyCreator creator)
            {
                _creator = creator;
                _container = container;
            }

            public override void Dispose()
            {
                _creator.DecRefCount();
            }

            public override bool HasInstance(Type contractType)
            {
                return _creator.HasCreatedInstance();
            }

            public override Type GetInstanceType()
            {
                return _creator.GetInstanceType();
            }

            public override object GetInstance(Type contractType, InjectContext context)
            {
                return _creator.GetInstance(contractType);
            }

            public override IEnumerable<ZenjectResolveException> ValidateBinding(Type contractType, InjectContext context)
            {
                if (_creator.HasCreatedInstance())
                {
                    // This would be the case if given an instance at binding time with ToSingle(instance)
                    return Enumerable.Empty<ZenjectResolveException>();
                }

                return BindingValidator.ValidateObjectGraph(_container, GetInstanceType());
            }
        }
    }
}
