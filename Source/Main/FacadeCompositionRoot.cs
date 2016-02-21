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
            List<TypeValuePair> facadeExtraArgs)
        {
            Assert.IsNull(_container);

            if (facadeExtraArgs == null)
            {
                facadeExtraArgs = new List<TypeValuePair>();
            }

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

            _container = parentContainer.CreateSubContainer();

            InstallBindings(_container.Binder);

            InjectComponents(_container.Resolver, facadeExtraArgs);
        }

        void InjectComponents(IResolver resolver, List<TypeValuePair> facadeExtraArgs)
        {
            bool foundFacade = false;

            // Use ToList in case they do something weird in post inject
            foreach (var component in GetInjectableComponents().ToList())
            {
                Assert.That(!component.GetType().DerivesFrom<MonoInstaller>());

                if (component == _facade)
                {
                    Assert.That(!foundFacade);
                    foundFacade = true;
                    resolver.InjectExplicit(component, facadeExtraArgs);
                }
                else
                {
                    resolver.Inject(component);
                }
            }

            Assert.That(foundFacade);
        }

        public override IEnumerable<GameObject> GetRootGameObjects()
        {
            return UnityUtil.GetDirectChildren(this.gameObject);
        }

        // We pass in the binder here instead of using our own for validation to work
        public override void InstallBindings(IBinder binder)
        {
            binder.Bind<CompositionRoot>().ToInstance(this);
            binder.Bind<Transform>(DiContainer.DefaultParentId)
                .ToInstance<Transform>(this.transform);

            binder.Bind<IDependencyRoot>().ToInstance(_facade);

            InstallInstallers(binder);
        }
    }
}

#endif
