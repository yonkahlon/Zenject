using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public class PoolExceededFixedSizeException : Exception
    {
        public PoolExceededFixedSizeException(string errorMessage)
            : base(errorMessage)
        {
        }
    }

    public interface IDynamicPooledFactory : IValidatable
    {
        int NumCreated
        {
            get;
        }

        int NumActive
        {
            get;
        }

        int NumInactive
        {
            get;
        }

        Type ContractType
        {
            get;
        }

        Type ConcreteType
        {
            get;
        }
    }

    public abstract class DynamicPooledFactory<TContract> : IDynamicPooledFactory
        // don't add generic constraint 'where TContract : IPoolable' since
        // we really only require that the concrete type implement that
        where TContract : IPoolableBase
    {
        Stack<TContract> _pool;
        Type _concreteType;
        InjectContext _injectContext;
        IProvider _provider;
        int _numCreated;
        int _numActive;
        PoolExpandMethods _expandMethod;

        [Inject]
        void Construct(
            Type concreteType,
            IProvider provider,
            DiContainer container,
            int initialSize,
            PoolExpandMethods expandMethod)
        {
            Assert.That(concreteType.DerivesFromOrEqual<TContract>());

            _expandMethod = expandMethod;
            _provider = provider;
            _concreteType = concreteType;
            _injectContext = new InjectContext(container, concreteType);

            _pool = new Stack<TContract>(initialSize);

            for (int i = 0; i < initialSize; i++)
            {
                _pool.Push(AllocNew());
            }
        }

        public IEnumerable<TContract> InactiveItems
        {
            get { return _pool; }
        }

        public int NumCreated
        {
            get { return _numCreated; }
        }

        public int NumActive
        {
            get { return _numActive; }
        }

        public int NumInactive
        {
            get { return _pool.Count; }
        }

        public Type ContractType
        {
            get { return typeof(TContract); }
        }

        public Type ConcreteType
        {
            get { return _concreteType; }
        }

        TContract AllocNew()
        {
            try
            {
                var resultObj = _provider.GetInstance(_injectContext);

                Assert.IsNotNull(resultObj);
                Assert.That(resultObj.GetType().DerivesFromOrEqual(_concreteType));

                _numCreated++;

                var result = (TContract)resultObj;

                // While it might seem a bit weird to call OnDespawned before OnSpawned,
                // this is necessary to ensure that MonoBehaviour's get a chance to deactive
                // their game object
                result.OnDespawned();

                return result;
            }
            catch (Exception e)
            {
                throw new ZenjectException(
                    "Error during construction of type '{0}' via {1}.Create method!".Fmt(
                        _concreteType, this.GetType().Name()), e);
            }
        }

        public virtual void Validate()
        {
            try
            {
                _provider.GetInstance(_injectContext);
            }
            catch (Exception e)
            {
                throw new ZenjectException(
                    "Validation for factory '{0}' failed".Fmt(this.GetType()), e);
            }
        }

        protected TContract GetInternal()
        {
            TContract item;

            if (_pool.IsEmpty())
            {
                ExpandPool();
                Assert.That(!_pool.IsEmpty());
            }

            item = _pool.Pop();

            _numActive++;
            return item;
        }

        void ExpandPool()
        {
            switch (_expandMethod)
            {
                case PoolExpandMethods.Fixed:
                {
                    throw new PoolExceededFixedSizeException(
                        "Pool factory '{0}' exceeded its max size of '{1}'!"
                        .Fmt(this.GetType(), _numCreated));
                }
                case PoolExpandMethods.OneAtATime:
                {
                    _pool.Push(AllocNew());
                    break;
                }
                case PoolExpandMethods.Double:
                {
                    if (_numCreated == 0)
                    {
                        _pool.Push(AllocNew());
                    }
                    else
                    {
                        var oldSize = _numCreated;

                        for (int i = 0; i < oldSize; i++)
                        {
                            _pool.Push(AllocNew());
                        }
                    }
                    break;
                }
                default:
                {
                    throw Assert.CreateException();
                }
            }
        }

        public void Despawn(TContract item)
        {
            Assert.That(!_pool.Contains(item),
                "Tried to return an item to pool {0} twice", this.GetType());

            _numActive--;
            _pool.Push(item);

            Assert.That(_numActive >= 0,
                "Tried to return an item to the pool that was not originally created in pool");

            item.OnDespawned();
        }
    }
}
