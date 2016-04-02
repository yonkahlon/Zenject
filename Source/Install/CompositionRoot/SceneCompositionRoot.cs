#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject.Internal;

#if UNITY_5_3
using UnityEngine.SceneManagement;
#endif

namespace Zenject
{
    public class SceneCompositionRoot : CompositionRoot
    {
        public static readonly List<Scene> DecoratedScenes = new List<Scene>();

        public static Action<DiContainer> BeforeInstallHooks;
        public static Action<DiContainer> AfterInstallHooks;

        [FormerlySerializedAs("ParentNewObjectsUnderRoot")]
        [Tooltip("When true, objects that are created at runtime will be parented to the SceneCompositionRoot")]
        [SerializeField]
        bool _parentNewObjectsUnderRoot = false;

        DiContainer _container;
        IDependencyRoot _dependencyRoot;

        bool _hasInitialized;

        static bool _autoInitialize = true;

        public bool ParentNewObjectsUnderRoot
        {
            get
            {
                return _parentNewObjectsUnderRoot;
            }
            set
            {
                _parentNewObjectsUnderRoot = value;
            }
        }

        public override IDependencyRoot DependencyRoot
        {
            get
            {
                return _dependencyRoot;
            }
        }

        public void Awake()
        {
            if (_autoInitialize)
            {
                Initialize();
            }

            // Always reset it after use to avoid affecting other SceneCompositionRoot's
            _autoInitialize = true;
        }

        public void Initialize()
        {
            Assert.That(!_hasInitialized);
            _hasInitialized = true;

            // We always want to initialize GlobalCompositionRoot as early as possible
            GlobalCompositionRoot.Instance.EnsureIsInitialized();

            Assert.IsNull(_container);

            _container = GlobalCompositionRoot.Instance.Container.CreateSubContainer();

            _container.IncludeInactiveDefault = IncludeInactiveComponents;

            // This can be valid in cases where you have everything in either facade installers
            // or global installers so just ignore
            //if (Installers.IsEmpty() && InstallerPrefabs.IsEmpty())
            //{
                //Log.Warn("No installers found while initializing CompositionRoot '{0}'", this.name);
            //}

            Log.Debug("SceneCompositionRoot: Running installers...");

            _container.IsInstalling = true;

            try
            {
                InstallBindings(_container);
            }
            finally
            {
                _container.IsInstalling = false;
            }

            Log.Debug("SceneCompositionRoot: Injecting components in the scene...");

            InjectComponents(_container);

            Log.Debug("SceneCompositionRoot: Resolving dependency root...");

            _dependencyRoot = _container.Resolve<IDependencyRoot>();

            DecoratedScenes.Clear();

            Log.Debug("SceneCompositionRoot: Initialized successfully");
        }

        // We pass in the container here instead of using our own for validation to work
        public void InstallBindings(DiContainer container)
        {
            if (_parentNewObjectsUnderRoot)
            {
                container.Bind<Transform>(DiContainer.DefaultParentId)
                    .ToInstance<Transform>(this.transform);
            }

            container.Bind(typeof(TickableManager), typeof(InitializableManager), typeof(DisposableManager)).ToSelf().AsSingle();
            container.Bind<CompositionRoot>().ToInstance(this);

            InstallSceneBindingsInternal(container);

            if (BeforeInstallHooks != null)
            {
                BeforeInstallHooks(container);
                // Reset extra bindings for next time we change scenes
                BeforeInstallHooks = null;
            }

            container.Bind<IDependencyRoot>().ToComponent<SceneFacade>(this.gameObject);

            InstallInstallers(container);

            if (AfterInstallHooks != null)
            {
                AfterInstallHooks(container);
                // Reset extra bindings for next time we change scenes
                AfterInstallHooks = null;
            }
        }

        void InstallSceneBindingsInternal(DiContainer container)
        {
            InstallSceneBindings(container);

            foreach (var autoBinding in GameObject.FindObjectsOfType<ZenjectBinding>())
            {
                if (autoBinding == null)
                {
                    continue;
                }

                if (autoBinding.ContainerType != ZenjectBinding.ContainerTypes.Scene)
                {
                    continue;
                }

                InstallAutoBinding(container, autoBinding);
            }
        }

        public override IEnumerable<Component> GetInjectableComponents()
        {
            foreach (var gameObject in GetRootGameObjects())
            {
                foreach (var component in GetInjectableComponents(gameObject, IncludeInactiveComponents))
                {
                    yield return component;
                }
            }
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

        IEnumerable<GameObject> GetRootGameObjects()
        {
            var scene = this.gameObject.scene;
            // Note: We can't use activeScene.GetRootObjects() here because that apparently fails with an exception
            // about the scene not being loaded yet when executed in Awake
            // We also can't use GameObject.FindObjectsOfType<Transform>() because that does not include inactive game objects
            // So we use Resources.FindObjectsOfTypeAll, even though that may include prefabs.  However, our assumption here
            // is that prefabs do not have their "scene" property set correctly so this should work
            //
            // It's important here that we only inject into root objects that are part of our scene
            // Otherwise, if there is an object that is marked with DontDestroyOnLoad, then it will
            // be injected multiple times when another scene is loaded
            // We also make sure not to inject into the global root objects which are injected in GlobalCompositionRoot
            return Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(x => (IncludeInactiveComponents || x.activeSelf)
                    && x.transform.parent == null
                    && x.GetComponent<GlobalCompositionRoot>() == null
                    && (x.scene == scene || DecoratedScenes.Contains(x.scene)));
        }

        // These methods can be used for cases where you need to create the SceneCompositionRoot entirely in code
        // Note that if you use these methods that you have to call Initialize() yourself
        // This is useful because it allows you to create a SceneCompositionRoot and configure it how you want
        // and add what installers you want before kicking off the Install/Resolve
        public static SceneCompositionRoot Create()
        {
            return Create(null);
        }

        public static SceneCompositionRoot Create(GameObject parent)
        {
            var gameObject = new GameObject("SceneCompositionRoot");

            if (parent != null)
            {
                gameObject.transform.SetParent(parent.transform, false);
            }

            return CreateComponent(gameObject);
        }

        public static SceneCompositionRoot CreateComponent(GameObject gameObject)
        {
            _autoInitialize = false;
            var result = gameObject.AddComponent<SceneCompositionRoot>();
            Assert.That(_autoInitialize); // Should be reset
            return result;
        }
    }
}

#endif

