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
        public void Construct(DiContainer parentContainer)
        {
            Assert.IsNull(_container);

            if (_facade == null)
            {
                throw new ZenjectBindException(
                    "Facade property is not set in FacadeCompositionRoot '{0}'".Fmt(this.gameObject.name));
            }

            if (!UnityUtil.GetParents(_facade.transform).Contains(this.transform))
            {
                throw new ZenjectBindException(
                    "The given Facade must exist on the same game object as the FacadeCompositionRoot '{0}' or a descendant!".Fmt(this.name));
            }

            _container = new DiContainer(parentContainer);

            InstallBindings(_container.Binder);

            InjectComponents(_container.Resolver);
        }

        public override IEnumerable<Component> GetInjectableComponents()
        {
            // Note: We intentionally do not inject into the components on our own game object
            // This is nice so you can add zenject bindings on it
            // Installers are also fine to add since these are only injected when they are installed

            foreach (var component in GetRootObjectsInjectableComponents())
            {
                yield return component;
            }

            yield return _facade;
        }

        public override IEnumerable<GameObject> GetRootGameObjects()
        {
            return UnityUtil.GetDirectChildrenAndSelf(this.gameObject);
        }

        // We pass in the binder here instead of using our own for validation to work
        public override void InstallBindings(IBinder binder)
        {
            binder.Bind<CompositionRoot>().ToInstance(this);
            binder.Bind<Transform>(ZenConstants.DefaultParentId)
                .ToInstance<Transform>(this.transform);

            binder.Bind<IDependencyRoot>().ToInstance(_facade);

            InstallInstallers(binder);
        }
    }
}

#endif
