using System;
using System.Collections.Generic;

namespace ModestTree.Zenject
{
    public class SingletonProviderMap
    {
        interface ISingletonLazyCreator
        {
            void IncRefCount();
            void DecRefCount();

            object GetInstance();
            Type GetInstanceType();
        }

        class SingletonLazyCreator<T> : ISingletonLazyCreator
        {
            int _referenceCount;
            T _instance;
            SingletonProviderMap _map;
            DiContainer _container;

            public SingletonLazyCreator(DiContainer container, SingletonProviderMap map)
            {
                _container = container;
                _map = map;
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
                    _map.Remove<T>();
                }
            }

            public Type GetInstanceType()
            {
                return typeof(T);
            }

            public object GetInstance()
            {
                if (_instance == null)
                {
                    _instance = Instantiator.Instantiate<T>(_container);
                    Assert.That(_instance != null);
                }

                return _instance;
            }
        }

        // NOTE: we need the provider seperate from the creator because
        // if we return the same provider multiple times then the condition 
        // will get over-written
        private class SingletonProvider : ProviderBase
        {
            ISingletonLazyCreator _creator;

            public SingletonProvider(ISingletonLazyCreator creator)
            {
                _creator = creator;
            }

            public override void OnRemoved()
            {
                _creator.DecRefCount();
            }

            public override Type GetInstanceType()
            {
                return _creator.GetInstanceType();
            }

            public override object GetInstance()
            {
                return _creator.GetInstance();
            }
        }

        private Dictionary<Type, ISingletonLazyCreator> _creators = new Dictionary<Type, ISingletonLazyCreator>();
        private DiContainer _container;

        public SingletonProviderMap(DiContainer container)
        {
            _container = container;
        }

        private void Remove<T>()
        {
            _creators.Remove(typeof(T));
        }

        public ProviderBase CreateProvider<TConcrete>()
        {
            var type = typeof (TConcrete);

            ISingletonLazyCreator creator;

            if (!_creators.TryGetValue(type, out creator))
            {
                creator = new SingletonLazyCreator<TConcrete>(_container, this);
                _creators.Add(type, creator);
            }

            creator.IncRefCount();

            return new SingletonProvider(creator);
        }
    }
}
