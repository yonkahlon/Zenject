
#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using System.Linq;

namespace Zenject
{
    // Do not inherit from this class - inherit from MonoFacadeFactory<>
    public abstract class MonoFacadeFactoryBase<TValue> : IValidatable
        where TValue : MonoFacade
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

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        protected TValue CreateInternal(List<TypeValuePair> extraArgs)
        {
            var injectContext = new InjectContext(
                _container, typeof(TValue), null);

            FacadeCompositionRoot.InstallerExtraArgs installerArgs;

            if (MainInstallerType == null)
            {
                installerArgs = null;
            }
            else
            {
                installerArgs = new FacadeCompositionRoot.InstallerExtraArgs()
                {
                    InstallerType = MainInstallerType,
                    ExtraArgs = extraArgs
                };
            }

            var facadeRoot = (FacadeCompositionRoot)Container.InstantiatePrefabForComponentExplicit(
                typeof(FacadeCompositionRoot), _prefab,
                InstantiateUtil.CreateTypeValueListExplicit(installerArgs),
                injectContext, false, _groupName);

            Assert.That(facadeRoot.Facade.GetType().DerivesFromOrEqual<TValue>(),
                "Expected facade '{0}' to derive from '{1}'", facadeRoot.Facade.GetType().Name(), typeof(TValue).Name());

            return (TValue)facadeRoot.Facade;
        }

        public IEnumerable<ZenjectResolveException> Validate()
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

                if (facadeRoot.Facade == null)
                {
                    yield return new ZenjectResolveException(
                        "'Facade' component missing on prefab '{0}' when validating '{1}'"
                        .Fmt(_prefab.name, this.GetType().Name()));
                    yield break;
                }

                if (facadeRoot.Facade.GetType() != typeof(TValue))
                {
                    yield return new ZenjectResolveException(
                        "Found unexpected MonoFacade type on prefab '{0}' when validating '{1}'.  Expected '{2}' and found '{3}'"
                        .Fmt(_prefab.name, this.GetType().Name(), typeof(TValue).Name(), facadeRoot.Facade.GetType().Name()));
                    yield break;
                }

                FacadeCompositionRoot.InstallerExtraArgs installerArgs;

                if (MainInstallerType == null)
                {
                    installerArgs = null;
                }
                else
                {
                    installerArgs = new FacadeCompositionRoot.InstallerExtraArgs()
                    {
                        InstallerType = MainInstallerType,
                        ExtraArgs = ParamTypes.Select(
                            x => new TypeValuePair(x, null)).ToList()
                    };
                }

                var facadeContainer = _container.CreateSubContainer();
                facadeRoot.InstallBindings(
                    facadeContainer, installerArgs);

                foreach (var err in ZenValidator.ValidateCompositionRoot(facadeRoot, facadeContainer))
                {
                    yield return err;
                }
            }
            finally
            {
                GameObject.DestroyImmediate(instance);
            }
        }

        protected abstract Type MainInstallerType
        {
            get;
        }

        protected abstract IEnumerable<Type> ParamTypes
        {
            get;
        }
    }

    // This is just for generic constraints in DiContainer
    public interface IMonoFacadeFactoryZeroParams
    {
    }

    // This is just for generic constraints in DiContainer
    public interface IMonoFacadeFactoryMultipleParams
    {
    }

    // Zero parameters
    public class MonoFacadeFactory<TValue> : MonoFacadeFactoryBase<TValue>, IFactory<TValue>, IMonoFacadeFactoryZeroParams
        where TValue : MonoFacade
    {
        public virtual TValue Create()
        {
            return CreateInternal(
                new List<TypeValuePair>()
                {
                });
        }

        protected override Type MainInstallerType
        {
            get
            {
                return null;
            }
        }

        protected override IEnumerable<Type> ParamTypes
        {
            get
            {
                yield break;
            }
        }
    }

    public abstract class MonoFacadeFactoryMultipleParamsBase<TValue> : MonoFacadeFactoryBase<TValue>, IMonoFacadeFactoryMultipleParams
        where TValue : MonoFacade
    {
        Type _mainInstallerType;

        [PostInject]
        public void Construct(Type mainInstallerType)
        {
            _mainInstallerType = mainInstallerType;
        }

        protected override Type MainInstallerType
        {
            get
            {
                return _mainInstallerType;
            }
        }
    }

    // One parameter
    public class MonoFacadeFactory<TParam1, TValue> : MonoFacadeFactoryMultipleParamsBase<TValue>, IFactory<TParam1, TValue>
        where TValue : MonoFacade
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
    public class MonoFacadeFactory<TParam1, TParam2, TValue> : MonoFacadeFactoryMultipleParamsBase<TValue>, IFactory<TParam1, TParam2, TValue>, IMonoFacadeFactoryMultipleParams
        where TValue : MonoFacade
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
    public class MonoFacadeFactory<TParam1, TParam2, TParam3, TValue> : MonoFacadeFactoryMultipleParamsBase<TValue>, IFactory<TParam1, TParam2, TParam3, TValue>, IMonoFacadeFactoryMultipleParams
        where TValue : MonoFacade
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
    public class MonoFacadeFactory<TParam1, TParam2, TParam3, TParam4, TValue> : MonoFacadeFactoryMultipleParamsBase<TValue>, IFactory<TParam1, TParam2, TParam3, TParam4, TValue>, IMonoFacadeFactoryMultipleParams
        where TValue : MonoFacade
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
