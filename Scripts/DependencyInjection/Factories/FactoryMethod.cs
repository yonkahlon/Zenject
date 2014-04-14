using System;

namespace ModestTree.Zenject
{
    public class FactoryMethod<TContract> : IFactory<TContract>
    {
        readonly DiContainer _container;
        readonly Func<DiContainer, TContract> _factoryMethod;

        public FactoryMethod(DiContainer container, Func<DiContainer, TContract> factoryMethod)
        {
            _container = container;
            _factoryMethod = factoryMethod;
        }

        public virtual TContract Create(params object[] constructorArgs)
        {
            return _factoryMethod(_container);
        }
    }
}
