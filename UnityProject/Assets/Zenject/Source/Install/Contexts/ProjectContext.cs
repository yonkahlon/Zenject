#if !NOT_UNITY3D

using ModestTree;

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree.Util;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Zenject.Internal;

namespace Zenject
{
    public class ProjectContext : Context
    {
#if UNITY_EDITOR
        public const string IsValidatingEditorPrefsKey = "Zenject.IsValidating";
#endif

        public const string ProjectContextResourcePath = "ProjectContext";
        public const string ProjectContextResourcePathOld = "ProjectCompositionRoot";

        static ProjectContext _instance;

        DiContainer _container;

        readonly List<object> _dependencyRoots = new List<object>();

        public override DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        public static ProjectContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = InstantiateNewRoot();

                    // Note: We use Initialize instead of awake here in case someone calls
                    // ProjectContext.Instance while ProjectContext is initializing
                    _instance.Initialize();
                }

#if UNITY_EDITOR
                if (_instance.Container.IsValidating)
                {
                    // ProjectContext.Instance is called as the first thing in every
                    // Zenject scene (including decorator scenes)
                    // During validation, we want to ensure that no Awake() gets called except
                    // for SceneContext.Awake
                    // Need to call DisableEverything() again here for any new scenes that may
                    // have been loaded
                    ValidationSceneDisabler.Instance.DisableEverything();
                }
#endif

                return _instance;
            }
        }

#if UNITY_EDITOR
        public static bool PersistentIsValidating
        {
            get
            {
                return EditorPrefs.GetInt(
                    IsValidatingEditorPrefsKey, 0) != 0;
            }
            set
            {
                EditorPrefs.SetInt(
                    IsValidatingEditorPrefsKey, value ? 1 : 0);
            }
        }
#endif

        public static GameObject TryGetPrefab()
        {
            var prefab = (GameObject)Resources.Load(ProjectContextResourcePath);

            if (prefab == null)
            {
                prefab = (GameObject)Resources.Load(ProjectContextResourcePathOld);
            }

            return prefab;
        }

        public static ProjectContext InstantiateNewRoot()
        {
            Assert.That(GameObject.FindObjectsOfType<ProjectContext>().IsEmpty(),
                "Tried to create multiple instances of ProjectContext!");

            ProjectContext instance;

            var prefab = TryGetPrefab();

            if (prefab == null)
            {
                instance = new GameObject("ProjectContext")
                    .AddComponent<ProjectContext>();
            }
            else
            {
                instance = GameObject.Instantiate(prefab).GetComponent<ProjectContext>();

                Assert.IsNotNull(instance,
                    "Could not find ProjectContext component on prefab 'Resources/{0}.prefab'", ProjectContextResourcePath);
            }

            return instance;
        }

        public void EnsureIsInitialized()
        {
            // Do nothing - Initialize occurs in Instance property
        }

        void Initialize()
        {
            Log.Debug("Initializing ProjectContext");

            Assert.IsNull(_container);

            DontDestroyOnLoad(gameObject);

            bool isValidating = false;

#if UNITY_EDITOR
            isValidating = PersistentIsValidating;

            if (isValidating)
            {
                ValidationSceneDisabler.Instance.DisableEverything();
            }

            // Always default to false to avoid validating next time play is hit
            PersistentIsValidating = false;
#endif

            _container = new DiContainer(
                StaticContext.Container, isValidating);

            _container.IsInstalling = true;

            try
            {
                InstallBindings();
            }
            finally
            {
                _container.IsInstalling = false;
            }

            InjectComponents();

            Assert.That(_dependencyRoots.IsEmpty());

            _dependencyRoots.AddRange(_container.ResolveDependencyRoots());
        }

        protected override IEnumerable<Component> GetInjectableComponents()
        {
            return GetInjectableComponents(this.gameObject);
        }

        void InjectComponents()
        {
            // Use ToList in case they do something weird in post inject
            foreach (var component in GetInjectableComponents().ToList())
            {
                Assert.That(!component.GetType().DerivesFrom<MonoInstaller>());

                _container.Inject(component);
            }
        }

        void InstallBindings()
        {
            _container.DefaultParent = this.transform;

            _container.Bind(typeof(TickableManager), typeof(InitializableManager), typeof(DisposableManager))
                .ToSelf().AsSingle().InheritInSubContainers();

            _container.Bind<Context>().FromInstance(this);

            _container.Bind<ProjectKernel>().FromComponent(this.gameObject).AsSingle().NonLazy();

            InstallSceneBindings();

            InstallInstallers();
        }
    }
}

#endif
