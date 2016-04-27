#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEngine;
using Zenject.Internal;

#pragma warning disable 649

namespace Zenject
{
    public class GameObjectCompositionRoot : CompositionRoot
    {
        readonly List<object> _dependencyRoots = new List<object>();

        [SerializeField]
        [Tooltip("Note that this field is optional and can be ignored in most cases.  This is really only needed if you want to control the 'Script Execution Order' of your subcontainer.  In this case, define a new class that derives from MonoFacade, add it to this game object, then drag it into this field.  Then you can set a value for 'Script Execution Order' for this new class and this will control when all ITickable/IInitializable classes bound within this subcontainer get called.")]
        MonoFacade _facade;

        DiContainer _container;

        public DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        [PostInject]
        public void Construct(
            DiContainer parentContainer,
            [InjectOptional]
            InstallerExtraArgs installerExtraArgs)
        {
            Assert.IsNull(_container);

            _container = parentContainer.CreateSubContainer();

            _container.IsInstalling = true;

            try
            {
                InstallBindings(_container, installerExtraArgs);
            }
            finally
            {
                _container.IsInstalling = false;
            }

            Log.Debug("GameObjectCompositionRoot: Injecting into child components...");

            InjectComponents(_container);

            Assert.That(_dependencyRoots.IsEmpty());
            _dependencyRoots.AddRange(_container.ResolveDependencyRoots());

            Log.Debug("GameObjectCompositionRoot: Initialized successfully");
        }

        void InjectComponents(DiContainer container)
        {
            // Use ToList in case they do something weird in post inject
            foreach (var component in GetInjectableComponents().ToList())
            {
                Assert.That(!component.GetType().DerivesFrom<MonoInstaller>());

                container.Inject(component);
            }
        }

        public override IEnumerable<Component> GetInjectableComponents()
        {
            // We inject on all components on the root except ourself
            foreach (var component in GetComponents<Component>())
            {
                if (component == null)
                {
                    Log.Warn("Zenject: Found null component on game object '{0}'.  Possible missing script.", gameObject.name);
                    continue;
                }

                if (component.GetType().DerivesFrom<MonoInstaller>())
                {
                    // Do not inject on installers since these are always injected before they are installed
                    continue;
                }

                if (component == this)
                {
                    continue;
                }

                yield return component;
            }

            foreach (var gameObject in UnityUtil.GetDirectChildren(this.gameObject))
            {
                foreach (var component in GetInjectableComponents(gameObject))
                {
                    yield return component;
                }
            }
        }

        public override void InstallBindings(DiContainer container)
        {
            InstallBindings(container, null);
        }

        public void InstallBindings(
            DiContainer container, InstallerExtraArgs installerExtraArgs)
        {
            container.DefaultParent = this.transform;

            container.Bind<CompositionRoot>().FromInstance(this);

            if (_facade == null)
            {
                container.Bind<MonoFacade>()
                    .To<DefaultGameObjectFacade>().FromComponent(this.gameObject).AsSingle().NonLazy();
            }
            else
            {
                container.Bind<MonoFacade>().FromInstance(_facade).AsSingle().NonLazy();
            }

            InstallSceneBindings(container);

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
