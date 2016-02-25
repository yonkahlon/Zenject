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

    public static class MonoBehaviourFactoryUtil
    {
        public static IEnumerable<ZenjectResolveException> Validate(
            DiContainer container, GameObject prefab)
        {
            return Validate(container, prefab, null, null);
        }

        public static IEnumerable<ZenjectResolveException> Validate(
            DiContainer container, GameObject prefab, Type mainType, Type[] paramTypes)
        {
            Assert.IsNotNull(prefab);

            var rootGameObject = GameObject.Instantiate(prefab);

            try
            {
                var onlyInjectWhenActive = container.Resolve<CompositionRoot>().OnlyInjectWhenActive;

                foreach (var component in CompositionRoot.GetInjectableComponents(
                    rootGameObject, onlyInjectWhenActive))
                {
                    Assert.IsNotNull(component);

                    Assert.That(!component.GetType().DerivesFrom<MonoInstaller>());

                    if (mainType != null && component.GetType().DerivesFromOrEqual(mainType))
                    {
                        foreach (var error in container.ValidateObjectGraph(component.GetType(), paramTypes))
                        {
                            yield return error;
                        }
                    }
                    else
                    {
                        foreach (var error in container.ValidateObjectGraph(component.GetType()))
                        {
                            yield return error;
                        }
                    }
                }

                foreach (var facadeRoot in GetFacadeRoots(rootGameObject))
                {
                    foreach (var error in ZenValidator.ValidateFacadeRoot(container, facadeRoot))
                    {
                        yield return error;
                    }
                }
            }
            finally
            {
                GameObject.DestroyImmediate(rootGameObject);
            }
        }

        static IEnumerable<FacadeCompositionRoot> GetFacadeRoots(GameObject gameObject)
        {
            // We don't want to just use GetComponentsInChildren here because
            // we want to ignore the FacadeCompositionRoot's that are inside other
            // FacadeCompositionRoot's
            var facadeRoot = gameObject.GetComponent<FacadeCompositionRoot>();

            if (facadeRoot != null)
            {
                yield return facadeRoot;
                yield break;
            }

            foreach (Transform child in gameObject.transform)
            {
                foreach (var descendantRoot in GetFacadeRoots(child.gameObject))
                {
                    yield return descendantRoot;
                }
            }
        }
    }

    public abstract class MonoBehaviourFactoryBase<TValue> : IMonoBehaviourFactory
    {
        [Inject]
        readonly DiContainer _container;

        [Inject]
        readonly GameObject _prefab;

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
            return MonoBehaviourFactoryUtil.Validate(
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

