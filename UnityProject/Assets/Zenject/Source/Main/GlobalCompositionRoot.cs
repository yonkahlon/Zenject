#if !ZEN_NOT_UNITY3D

#pragma warning disable 414
using ModestTree;

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree.Util;
using UnityEngine;
using Zenject.Internal;

namespace Zenject
{
    public class GlobalCompositionRoot : CompositionRoot
    {
        public const string GlobalCompRootResourcePath = "GlobalCompositionRoot";

        static GlobalCompositionRoot _instance;

        DiContainer _container;
        IDependencyRoot _dependencyRoot;

        public override IDependencyRoot DependencyRoot
        {
            get
            {
                return _dependencyRoot;
            }
        }

        public DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        public static GlobalCompositionRoot Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = InstantiateNewRoot();

                    // Note: We use Initialize instead of awake here in case someone calls
                    // GlobalCompositionRoot.Instance while GlobalCompositionRoot is initializing
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        public static GameObject TryGetPrefab()
        {
            return (GameObject)Resources.Load(GlobalCompRootResourcePath);
        }

        public static GlobalCompositionRoot InstantiateNewRoot()
        {
            Assert.That(GameObject.FindObjectsOfType<GlobalCompositionRoot>().IsEmpty(),
                "Tried to create multiple instances of GlobalCompositionRoot!");

            GlobalCompositionRoot instance;

            var prefab = TryGetPrefab();

            if (prefab == null)
            {
                instance = new GameObject("GlobalCompositionRoot").AddComponent<GlobalCompositionRoot>();
            }
            else
            {
                instance = GameObject.Instantiate(prefab).GetComponent<GlobalCompositionRoot>();

                Assert.IsNotNull(instance,
                    "Could not find GlobalCompositionRoot component on prefab 'Resources/{0}.prefab'", GlobalCompRootResourcePath);
            }

            return instance;
        }

        public void EnsureIsInitialized()
        {
            // Do nothing - Initialize occurs in Instance property
        }

        void Initialize()
        {
            Log.Debug("Initializing GlobalCompositionRoot");

            Assert.IsNull(_container);

            DontDestroyOnLoad(gameObject);

            _container = new DiContainer();

            InstallBindings(_container.Binder);
            InjectComponents(_container.Resolver);

            _dependencyRoot = _container.Resolver.Resolve<IDependencyRoot>();
        }

        public override IEnumerable<Component> GetInjectableComponents()
        {
            return GetRootObjectsInjectableComponents();
        }

        public override IEnumerable<GameObject> GetRootGameObjects()
        {
            return UnityUtil.GetDirectChildrenAndSelf(this.gameObject);
        }

        // We pass in the binder here instead of using our own for validation to work
        public override void InstallBindings(IBinder binder)
        {
            binder.Bind<CompositionRoot>().ToInstance(this);
            binder.Bind<IDependencyRoot>().ToSingleMonoBehaviour<GlobalFacade>(this.gameObject);

            binder.Bind<Transform>(ZenConstants.DefaultParentId)
                .ToInstance<Transform>(this.gameObject.transform);

            InstallSceneBindings(binder);

            InstallInstallers(binder);
        }
    }
}

#endif
