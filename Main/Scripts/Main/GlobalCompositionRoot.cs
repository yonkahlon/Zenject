using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    internal sealed class GlobalCompositionRoot : MonoBehaviour
    {
        static GlobalCompositionRoot _instance;
        DiContainer _container;
        IDependencyRoot _dependencyRoot;

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
                return _instance;
            }
        }

        static GlobalCompositionRoot()
        {
            _instance = new GameObject("Global Composition Root")
                .AddComponent<GlobalCompositionRoot>();
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            // Is this a good idea?
            //go.hideFlags = HideFlags.HideInHierarchy;

            _container = new DiContainer();
            _container.Bind<IInstaller>().ToSingle<StandardUnityInstaller>();
            _container.Bind<GameObject>().To(this.gameObject)
                .WhenInjectedInto<StandardUnityInstaller>();
            _container.InstallInstallers();
            Assert.That(!_container.HasBinding<IInstaller>());

            foreach (var installer in GlobalInstallerConfig.Instance.Installers)
            {
                FieldsInjecter.Inject(_container, installer);
                _container.Bind<IInstaller>().To(installer);
                _container.InstallInstallers();
                Assert.That(!_container.HasBinding<IInstaller>());
            }

            _dependencyRoot = _container.Resolve<IDependencyRoot>();
        }
    }
}
