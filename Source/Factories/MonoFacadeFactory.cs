#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using System.Linq;

namespace Zenject
{
    public abstract class MonoFacadeFactory : IValidatable
    {
        DiContainer _container;
        GameObject _prefab;
        string _groupName;

        [PostInject]
        void Construct(
            DiContainer container, GameObject prefab,
            [InjectOptional]
            string groupName)
        {
            _container = container;
            _prefab = prefab;
            _groupName = groupName;
        }

        protected IInstantiator Instantiator
        {
            get
            {
                return _container;
            }
        }

        protected IResolver Resolver
        {
            get
            {
                return _container;
            }
        }

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        protected TValue CreateInternal<TValue>(List<TypeValuePair> argList)
            where TValue : MonoFacade
        {
            var injectContext = new InjectContext(
                _container, typeof(TValue), null);

            var facadeRoot = (FacadeCompositionRoot)Instantiator.InstantiatePrefabForComponentExplicit(
                typeof(FacadeCompositionRoot), _prefab, InstantiateUtil.CreateTypeValueList(new object[] { argList }),
                injectContext, false, _groupName);

            Assert.That(facadeRoot.Facade.GetType().DerivesFromOrEqual<TValue>(),
                "Expected facade '{0}' to derive from '{1}'", facadeRoot.Facade.GetType().Name(), typeof(TValue).Name());

            return (TValue)facadeRoot.Facade;
        }

        protected IEnumerable<ZenjectResolveException> ValidateInternal<TValue>(params Type[] paramTypes)
        {
            var instance = GameObject.Instantiate(_prefab);

            try
            {
                var rootMatches = instance.GetComponentsInChildren<FacadeCompositionRoot>().ToList();

                if (rootMatches.IsEmpty())
                {
                    yield return new ZenjectResolveException(
                        "Could not find 'FacadeCompositionRoot' on prefab '{0}' when validating '{1}'"
                        .Fmt(_prefab.name, this.GetType().Name()));
                    yield break;
                }

                if (rootMatches.Count > 1)
                {
                    yield return new ZenjectResolveException(
                        "Found multiple 'FacadeCompositionRoot' components on prefab '{0}' when validating '{1}'"
                        .Fmt(_prefab.name, this.GetType().Name()));
                    yield break;
                }

                var facadeRoot = rootMatches[0];

                var facadeContainer = _container.CreateSubContainer();
                facadeRoot.InstallBindings(facadeContainer.Binder);

                foreach (var err in ZenValidator.ValidateCompositionRoot(facadeRoot, facadeContainer, paramTypes))
                {
                    yield return err;
                }
            }
            finally
            {
                GameObject.DestroyImmediate(instance);
            }
        }

        public abstract IEnumerable<ZenjectResolveException> Validate();
    }

    public class MonoFacadeFactory<TValue> : MonoFacadeFactory, IFactory<TValue>
        where TValue : MonoFacade
    {
        public MonoFacadeFactory()
        {
            Assert.That(typeof(TValue).IsInterface || typeof(TValue).DerivesFrom<Component>());
        }

        public virtual TValue Create()
        {
            return CreateInternal<TValue>(new List<TypeValuePair>());
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return ValidateInternal<TValue>();
        }
    }

    // One parameter
    public class MonoFacadeFactory<TParam1, TValue> : MonoFacadeFactory, IFactory<TParam1, TValue>
        where TValue : MonoFacade
    {
        public MonoFacadeFactory()
        {
            Assert.That(typeof(TValue).IsInterface || typeof(TValue).DerivesFrom<Component>());
        }

        public virtual TValue Create(TParam1 param)
        {
            return CreateInternal<TValue>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param),
                });
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return ValidateInternal<TValue>(typeof(TParam1));
        }
    }

    // Two parameters
    public class MonoFacadeFactory<TParam1, TParam2, TValue> : MonoFacadeFactory, IFactory<TParam1, TParam2, TValue>
        where TValue : MonoFacade
    {
        public MonoFacadeFactory()
        {
            Assert.That(typeof(TValue).IsInterface || typeof(TValue).DerivesFrom<Component>());
        }

        public virtual TValue Create(TParam1 param1, TParam2 param2)
        {
            return CreateInternal<TValue>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                });
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return ValidateInternal<TValue>(typeof(TParam1), typeof(TParam2));
        }
    }

    // Three parameters
    public class MonoFacadeFactory<TParam1, TParam2, TParam3, TValue> : MonoFacadeFactory, IFactory<TParam1, TParam2, TParam3, TValue>
        where TValue : MonoFacade
    {
        public MonoFacadeFactory()
        {
            Assert.That(typeof(TValue).IsInterface || typeof(TValue).DerivesFrom<Component>());
        }

        public virtual TValue Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return CreateInternal<TValue>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                    InstantiateUtil.CreateTypePair(param3),
                });
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return ValidateInternal<TValue>(typeof(TParam1), typeof(TParam2), typeof(TParam3));
        }
    }

    // Four parameters
    public class MonoFacadeFactory<TParam1, TParam2, TParam3, TParam4, TValue> : MonoFacadeFactory, IFactory<TParam1, TParam2, TParam3, TParam4, TValue>
        where TValue : MonoFacade
    {
        public MonoFacadeFactory()
        {
            Assert.That(typeof(TValue).IsInterface || typeof(TValue).DerivesFrom<Component>());
        }

        public virtual TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return CreateInternal<TValue>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                    InstantiateUtil.CreateTypePair(param3),
                    InstantiateUtil.CreateTypePair(param4),
                });
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return ValidateInternal<TValue>(typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4));
        }
    }
}

#endif


