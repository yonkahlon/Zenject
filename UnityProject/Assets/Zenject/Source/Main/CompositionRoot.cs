#if !ZEN_NOT_UNITY3D

using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using UnityEngine.Serialization;
using System.Linq;
using Zenject.Internal;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Zenject
{
    public abstract class CompositionRoot : MonoBehaviour
    {
        [FormerlySerializedAs("OnlyInjectWhenActive")]
        [Tooltip("When true, inactive objects will not have their members injected")]
        [SerializeField]
        bool _onlyInjectWhenActive = false;

        [FormerlySerializedAs("Installers")]
        [SerializeField]
        MonoInstaller[] _installers = new MonoInstaller[0];

        [SerializeField]
        MonoInstaller[] _installerPrefabs = new MonoInstaller[0];

        public abstract IDependencyRoot DependencyRoot
        {
            get;
        }

        public bool OnlyInjectWhenActive
        {
            get
            {
                return _onlyInjectWhenActive;
            }
            protected set
            {
                _onlyInjectWhenActive = value;
            }
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
            binder.Install<FacadeCommonInstaller>();

            var newGameObjects = new List<GameObject>();
            var allInstallers = _installers.Cast<IInstaller>().ToList();

            try
            {
#if UNITY_EDITOR
                foreach (var installer in _installers)
                {
                    Assert.That(PrefabUtility.GetPrefabType(installer.gameObject) != PrefabType.Prefab,
                        "Found prefab with name '{0}' in the Installer property of CompositionRoot '{1}'.  You should use the property 'InstallerPrefabs' for this instead.", installer.name, this.name);
                }
#endif

                foreach (var installerPrefab in _installerPrefabs)
                {
                    Assert.IsNotNull(installerPrefab, "Found null prefab in CompositionRoot");

#if UNITY_EDITOR
                    Assert.That(PrefabUtility.GetPrefabType(installerPrefab.gameObject) == PrefabType.Prefab,
                        "Found non-prefab with name '{0}' in the InstallerPrefabs property of CompositionRoot '{1}'.  You should use the property 'Installer' for this instead",
                        installerPrefab.name, this.name);
#endif
                    var installerGameObject = GameObject.Instantiate(installerPrefab.gameObject);

                    newGameObjects.Add(installerGameObject);

                    installerGameObject.transform.SetParent(this.transform, false);
                    var installer = installerGameObject.GetComponent<MonoInstaller>();

                    Assert.IsNotNull(installer,
                        "Expected to find component with type 'MonoInstaller' on given installer prefab '{0}'", installerPrefab.name);

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

        public IEnumerable<Component> GetInjectableComponents()
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
    }
}

#endif

