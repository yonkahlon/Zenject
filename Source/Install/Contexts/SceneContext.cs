#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

namespace Zenject
{
    public class SceneContext : Context
    {
        public static readonly List<Scene> DecoratedScenes = new List<Scene>();

        public static Action<DiContainer> BeforeInstallHooks;
        public static Action<DiContainer> AfterInstallHooks;

        [FormerlySerializedAs("ParentNewObjectsUnderRoot")]
        [Tooltip("When true, objects that are created at runtime will be parented to the SceneContext")]
        [SerializeField]
        bool _parentNewObjectsUnderRoot = false;

        [Tooltip("Optional name of this SceneContext, allowing contexts in subsequently loaded scenes to depend on it and be parented to it")]
        [SerializeField]
        string _name;

        [Tooltip("Optional name of a SceneContext in a previously loaded scene that this context depends on and to which it must be parented")]
        [SerializeField]
        string _parentSceneContextName;

        DiContainer _container;
        readonly List<object> _dependencyRoots = new List<object>();

        bool _hasInstalled;
        bool _hasResolved;

        static bool _autoRun = true;

        public override DiContainer Container
        {
            get
            {
                return _container;
            }
        }

#if UNITY_EDITOR
        public bool IsValidating
        {
            get
            {
                return ProjectContext.Instance.Container.IsValidating;
            }
        }
#else
        public bool IsValidating
        {
            get
            {
                return false;
            }
        }
#endif

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string ParentSceneContextName
        {
            get
            {
                return _parentSceneContextName;
            }
            set
            {
                _parentSceneContextName = value;
            }
        }

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

        public void Awake()
        {
            // We always want to initialize ProjectContext as early as possible
            ProjectContext.Instance.EnsureIsInitialized();

            if (_autoRun)
            {
                Run();
            }
            else
            {
                // True should always be default
                _autoRun = true;
            }
        }

#if UNITY_EDITOR
        public void Run()
        {
            if (IsValidating)
            {
                try
                {
                    RunInternal();

                    Assert.That(_container.IsValidating);

                    _container.ValidateIValidatables();

                    Log.Info("Scene '{0}' Validated Successfully", this.gameObject.scene.name);
                }
                catch (Exception e)
                {
                    Log.ErrorException("Scene '{0}' Failed Validation!".Fmt(this.gameObject.scene.name), e);
                }
            }
            else
            {
                RunInternal();
            }
        }
#else
        public void Run()
        {
            RunInternal();
        }
#endif

        void RunInternal()
        {
            Install();
            Resolve();
        }

        IEnumerable<Scene> LoadedScenes
        {
            get
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                    yield return SceneManager.GetSceneAt(i);
            }
        }

        DiContainer GetParentContainer()
        {
            if (string.IsNullOrEmpty(_parentSceneContextName))
            {
                return ProjectContext.Instance.Container;
            }

            var sceneContexts = LoadedScenes
                .Where(scene => scene.isLoaded)
                .Except(gameObject.scene)
                .SelectMany(scene => scene.GetRootGameObjects())
                .SelectMany(root => root.GetComponentsInChildren<SceneContext>())
                .Where(sceneContext => sceneContext.Name == _parentSceneContextName)
                .ToList();

            Assert.That(sceneContexts.Any(), () => string.Format(
                "SceneContext on object {0} of scene {1} requires contract {2}, but none of the loaded SceneContexts implements that contract.",
                gameObject.name,
                gameObject.scene.name,
                _parentSceneContextName));

            Assert.That(sceneContexts.Count == 1, () => string.Format(
                "SceneContext on object {0} of scene {1} requires a single implementation of contract {2}, but multiple were found.",
                gameObject.name,
                gameObject.scene.name,
                _parentSceneContextName));

            return sceneContexts.Single().Container;
        }

        public void Install()
        {
#if !UNITY_EDITOR
            Assert.That(!IsValidating);
#endif

            Assert.That(!_hasInstalled);
            _hasInstalled = true;

            Assert.IsNull(_container);

            _container = GetParentContainer().CreateSubContainer();

            // This can happen if you run a decorated scene with immediately running a normal scene afterwards
            foreach (var decoratedScene in DecoratedScenes)
            {
                Assert.That(decoratedScene.isLoaded,
                    "Unexpected state in SceneContext - found unloaded decorated scene");
            }

            // Record all the injectable components in the scene BEFORE installing the installers
            // This is nice for cases where the user calls InstantiatePrefab<>, etc. in their installer
            // so that it doesn't inject on the game object twice
            // InitialComponentsInjecter will also guarantee that any component that is injected into
            // another component has itself been injected
            _container.LazyInstanceInjector
                .AddInstances(GetInjectableComponents().Cast<object>());

            Log.Debug("SceneContext: Running installers...");

            _container.IsInstalling = true;

            try
            {
                InstallBindings();
            }
            finally
            {
                _container.IsInstalling = false;
            }

            DecoratedScenes.Clear();
        }

        public void Resolve()
        {
            Log.Debug("SceneContext: Injecting components in the scene...");

            Assert.That(_hasInstalled);
            Assert.That(!_hasResolved);
            _hasResolved = true;

            _container.LazyInstanceInjector.LazyInjectAll();

            Log.Debug("SceneContext: Resolving dependency roots...");

            Assert.That(_dependencyRoots.IsEmpty());
            _dependencyRoots.AddRange(_container.ResolveDependencyRoots());

            Log.Debug("SceneContext: Initialized successfully");
        }

        void InstallBindings()
        {
            if (_parentNewObjectsUnderRoot)
            {
                _container.DefaultParent = this.transform;
            }
            else
            {
                // This is necessary otherwise we inherit the project root DefaultParent
                _container.DefaultParent = null;
            }

            _container.Bind<Context>().FromInstance(this);
            _container.Bind<SceneContext>().FromInstance(this);

            InstallSceneBindings();

            if (BeforeInstallHooks != null)
            {
                BeforeInstallHooks(_container);
                // Reset extra bindings for next time we change scenes
                BeforeInstallHooks = null;
            }

            _container.Bind<SceneKernel>().FromComponent(this.gameObject).AsSingle().NonLazy();

            _container.Bind<ZenjectSceneLoader>().AsSingle();

            InstallInstallers();

            if (AfterInstallHooks != null)
            {
                AfterInstallHooks(_container);
                // Reset extra bindings for next time we change scenes
                AfterInstallHooks = null;
            }
        }

        protected override IEnumerable<Component> GetInjectableComponents()
        {
            foreach (var gameObject in GetRootGameObjects())
            {
                foreach (var component in GetInjectableComponents(gameObject))
                {
                    yield return component;
                }
            }

            yield break;
        }

        public IEnumerable<GameObject> GetRootGameObjects()
        {
            var scene = this.gameObject.scene;

            // Note: We can't use activeScene.GetRootObjects() here because that apparently fails with an exception
            // about the scene not being loaded yet when executed in Awake
            // We also can't use GameObject.FindObjectsOfType<Transform>() because that does not include inactive game objects
            // So we use Resources.FindObjectsOfTypeAll, even though that may include prefabs.  However, our assumption here
            // is that prefabs do not have their "scene" property set correctly so this should work
            //
            // It's important here that we only inject into root objects that are part of our scene, to properly support
            // multi-scene editing features of Unity 5.x
            //
            // Also, even with older Unity versions, if there is an object that is marked with DontDestroyOnLoad, then it will
            // be injected multiple times when another scene is loaded
            //
            // We also make sure not to inject into the project root objects which are injected by ProjectContext.
            return Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(x => x.transform.parent == null
                    && x.GetComponent<ProjectContext>() == null
                    && (x.scene == scene || DecoratedScenes.Contains(x.scene)));
        }

        // These methods can be used for cases where you need to create the SceneContext entirely in code
        // Note that if you use these methods that you have to call Run() yourself
        // This is useful because it allows you to create a SceneContext and configure it how you want
        // and add what installers you want before kicking off the Install/Resolve
        public static SceneContext Create()
        {
            return CreateComponent(
                new GameObject("SceneContext"));
        }

        public static SceneContext CreateComponent(GameObject gameObject)
        {
            _autoRun = false;
            var result = gameObject.AddComponent<SceneContext>();
            Assert.That(_autoRun); // Should be reset
            return result;
        }
    }
}

#endif
