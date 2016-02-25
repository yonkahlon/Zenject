#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEngine;
using Zenject.Internal;

namespace Zenject
{
    public class FacadeCompositionRoot : CompositionRoot
    {
        [SerializeField]
        MonoFacade _facade = null;

        DiContainer _container;

        public override IDependencyRoot DependencyRoot
        {
            get
            {
                return _facade;
            }
        }

        public MonoFacade Facade
        {
            get
            {
                return _facade;
            }
        }

        [PostInject]
        public void Construct(
            DiContainer parentContainer,
            [InjectOptional]
            InstallerExtraArgs installerExtraArgs)
        {
            Assert.IsNull(_container);

            if (_facade == null)
            {
                throw new ZenjectBindException(
                    "Facade property is not set in FacadeCompositionRoot '{0}'".Fmt(this.gameObject.name));
            }

            if (!UnityUtil.GetParentsAndSelf(_facade.transform).Contains(this.transform))
            {
                throw new ZenjectBindException(
                    "The given Facade must exist on the same game object as the FacadeCompositionRoot '{0}' or a descendant!".Fmt(this.name));
            }

            Log.Debug("FacadeCompositionRoot: Running installers...");

            _container = parentContainer.CreateSubContainer();
            InstallBindings(_container, installerExtraArgs);

            Log.Debug("FacadeCompositionRoot: Injecting into child components...");

            InjectComponents(_container);

            Log.Debug("FacadeCompositionRoot: Initialized successfully");
        }

        public override IEnumerable<GameObject> GetRootGameObjects()
        {
            return UnityUtil.GetDirectChildren(this.gameObject);
        }

        public void InstallBindings(
            DiContainer container, InstallerExtraArgs installerExtraArgs)
        {
            container.Bind<CompositionRoot>().ToInstance(this);
            container.Bind<Transform>(DiContainer.DefaultParentId)
                .ToInstance<Transform>(this.transform);

            container.Bind<IDependencyRoot>().ToInstance(_facade);

            var extraArgsMap = new Dictionary<Type, List<TypeValuePair>>();

            if (installerExtraArgs != null)
            {
                extraArgsMap.Add(
                    installerExtraArgs.InstallerType, installerExtraArgs.ExtraArgs);
            }

            InstallInstallers(container, extraArgsMap);
        }

        public class InstallerExtraArgs
        {
            public Type InstallerType;
            public List<TypeValuePair> ExtraArgs;
        }
    }
}

#endif
