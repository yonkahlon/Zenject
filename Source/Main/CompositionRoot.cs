#if !ZEN_NOT_UNITY3D

using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using UnityEngine.Serialization;
using System.Linq;
using Zenject.Internal;

namespace Zenject
{
    public abstract class CompositionRoot : MonoBehaviour
    {
        [Tooltip("When true, inactive objects will not have their members injected")]
        public bool OnlyInjectWhenActive = false;

        [FormerlySerializedAs("Installers")]
        [SerializeField]
        MonoInstaller[] _installers = new MonoInstaller[0];

        [SerializeField]
        MonoInstaller[] _installerPrefabs = new MonoInstaller[0];

        public abstract IDependencyRoot DependencyRoot
        {
            get;
        }

        public IEnumerable<MonoInstaller> Installers
        {
            get
            {
                return _installers;
            }
        }

        public IEnumerable<MonoInstaller> InstallerPrefabs
        {
            get
            {
                return _installerPrefabs;
            }
        }

        protected void SetInstallers(MonoInstaller[] installers)
        {
            _installers = installers;
        }

        public abstract void InstallBindings(IBinder binder);

        // We pass in the binder here instead of using our own for validation to work
        protected void InstallInstallers(IBinder binder)
        {
            binder.Install<StandardInstaller>();

            if (_installers.IsEmpty() && _installerPrefabs.IsEmpty())
            {
                Log.Warn("No installers found while initializing CompositionRoot '{0}'", this.name);
            }
            else
            {
                var newGameObjects = new List<GameObject>();
                var allInstallers = _installers.Cast<IInstaller>().ToList();

                try
                {
                    foreach (var prefab in _installerPrefabs)
                    {
                        Assert.IsNotNull(prefab, "Found null prefab in CompositionRoot");

                        var installerGameObject = GameObject.Instantiate(prefab.gameObject);

                        newGameObjects.Add(installerGameObject);

                        installerGameObject.transform.SetParent(this.transform, false);
                        var installer = installerGameObject.GetComponent<MonoInstaller>();

                        Assert.IsNotNull(installer,
                            "Expected to find component with type 'MonoInstaller' on given installer prefab '{0}'", prefab.name);

                        allInstallers.Add(installer);
                    }

                    binder.Install(allInstallers);
                }
                finally
                {
                    if (binder.IsValidating)
                    {
                        foreach (var gameObject in newGameObjects)
                        {
                            GameObject.DestroyImmediate(gameObject);
                        }
                    }
                }
            }
        }

        protected IEnumerable<Component> GetRootObjectsInjectableComponents()
        {
            foreach (var gameObject in GetRootGameObjects())
            {
                foreach (var component in ZenUtilInternal.GetInjectableComponentsBottomUp(
                    gameObject, true, !OnlyInjectWhenActive))
                {
                    if (component == null)
                    {
                        // This warning about fiBackupSceneStorage appears in normal cases so just ignore
                        // Not sure what it is
                        if (gameObject.name != "fiBackupSceneStorage")
                        {
                            Log.Warn("Zenject: Found null component on game object '{0}'.  Possible missing script.", gameObject.name);
                        }
                        continue;
                    }

                    if (component.GetType().DerivesFrom<MonoInstaller>())
                    {
                        // Do not inject on installers since these are always injected before they are installed
                        continue;
                    }

                    yield return component;
                }
            }
        }

        protected void InstallSceneBindings(IBinder binder)
        {
            foreach (var autoBinding in GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<ZenjectBinding>()))
            {
                if (autoBinding == null)
                {
                    continue;
                }

                var component = autoBinding.Component;
                var bindType = autoBinding.BindType;

                if (component == null)
                {
                    continue;
                }

                if (bindType == ZenjectBinding.BindTypes.ToInstance
                        || bindType == ZenjectBinding.BindTypes.ToInstanceAndInterfaces)
                {
                    binder.Bind(component.GetType()).ToInstance(component);
                }

                if (bindType == ZenjectBinding.BindTypes.ToInterfaces
                        || bindType == ZenjectBinding.BindTypes.ToInstanceAndInterfaces)
                {
                    binder.BindAllInterfacesToInstance(component);
                }
            }
        }

        protected void InjectComponents(IResolver resolver)
        {
            // Use ToList in case they do something weird in post inject
            foreach (var component in GetInjectableComponents().ToList())
            {
                Assert.That(!component.GetType().DerivesFrom<MonoInstaller>());

                resolver.Inject(component);
            }
        }

        public abstract IEnumerable<GameObject> GetRootGameObjects();
        public abstract IEnumerable<Component> GetInjectableComponents();
    }
}

#endif
