#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using System.Linq;

namespace Zenject
{
    public interface IMonoBehaviourFactory : IValidatable
    {
    }

    public abstract class MonoBehaviourFactoryBase<TValue> : IMonoBehaviourFactory
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        GameObject _prefab = null;

        [InjectOptional]
        string _groupName = null;

        public MonoBehaviourFactoryBase()
        {
            Assert.That(typeof(TValue).IsInterface || typeof(TValue).DerivesFrom<Component>());
            Assert.That(!typeof(TValue).DerivesFrom<MonoFacade>(),
                "Do not use MonoBehaviourFactory with MonoFacades - use MonoFacadeFactory instead");
        }

        protected GameObject Prefab
        {
            get
            {
                return _prefab;
            }
        }

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        protected TValue CreateInternal(List<TypeValuePair> argList)
        {
            return (TValue)Container.InstantiatePrefabForComponentExplicit(
                typeof(TValue), _prefab, argList,
                new InjectContext(_container, typeof(TValue), null), false, _groupName);
        }

        protected abstract IEnumerable<Type> ParamTypes
        {
            get;
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return PrefabBasedFactoryUtil.Validate(
                _container, _prefab, typeof(TValue), ParamTypes.ToArray());
        }
    }

    public class MonoBehaviourFactory<TValue> : MonoBehaviourFactoryBase<TValue>, IFactory<TValue>
        // We don't want to do this to allow interfaces
        //where TValue : Component
    {
        public virtual TValue Create()
        {
            return CreateInternal(new List<TypeValuePair>());
        }

        protected override IEnumerable<Type> ParamTypes
        {
            get
            {
                yield break;
            }
        }
    }

    // One parameter
    public class MonoBehaviourFactory<TParam1, TValue> : MonoBehaviourFactoryBase<TValue>, IFactory<TParam1, TValue>
        // We don't want to do this to allow interfaces
        //where TValue : Component
    {
        public virtual TValue Create(TParam1 param)
        {
            return CreateInternal(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param),
                });
        }

        protected override IEnumerable<Type> ParamTypes
        {
            get
            {
                yield return typeof(TParam1);
            }
        }
    }

    // Two parameters
    public class MonoBehaviourFactory<TParam1, TParam2, TValue> : MonoBehaviourFactoryBase<TValue>, IFactory<TParam1, TParam2, TValue>
        // We don't want to do this to allow interfaces
        //where TValue : Component
    {
        public virtual TValue Create(TParam1 param1, TParam2 param2)
        {
            return CreateInternal(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                });
        }

        protected override IEnumerable<Type> ParamTypes
        {
            get
            {
                yield return typeof(TParam1);
                yield return typeof(TParam2);
            }
        }
    }

    // Three parameters
    public class MonoBehaviourFactory<TParam1, TParam2, TParam3, TValue> : MonoBehaviourFactoryBase<TValue>, IFactory<TParam1, TParam2, TParam3, TValue>
        // We don't want to do this to allow interfaces
        //where TValue : Component
    {
        public virtual TValue Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return CreateInternal(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                    InstantiateUtil.CreateTypePair(param3),
                });
        }

        protected override IEnumerable<Type> ParamTypes
        {
            get
            {
                yield return typeof(TParam1);
                yield return typeof(TParam2);
                yield return typeof(TParam3);
            }
        }
    }

    // Four parameters
    public class MonoBehaviourFactory<TParam1, TParam2, TParam3, TParam4, TValue> : MonoBehaviourFactoryBase<TValue>, IFactory<TParam1, TParam2, TParam3, TParam4, TValue>
        // We don't want to do this to allow interfaces
        //where TValue : Component
    {
        public virtual TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return CreateInternal(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                    InstantiateUtil.CreateTypePair(param3),
                    InstantiateUtil.CreateTypePair(param4),
                });
        }

        protected override IEnumerable<Type> ParamTypes
        {
            get
            {
                yield return typeof(TParam1);
                yield return typeof(TParam2);
                yield return typeof(TParam3);
                yield return typeof(TParam4);
            }
        }
    }
}

#endif

