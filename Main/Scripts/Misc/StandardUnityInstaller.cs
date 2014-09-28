using System;
using System.Linq;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class StandardUnityInstaller : Installer
    {
        [Inject]
        GameObject _root;

        // Install basic functionality for most unity apps
        public override void InstallBindings()
        {
            _container.Bind<IDependencyRoot>().ToSingleMonoBehaviour<UnityDependencyRoot>(_root);

            _container.Bind<TickableHandler>().ToSingle();
            _container.Bind<GameObjectInstantiator>().ToSingle();
            _container.Bind<Transform>().To(_root.transform)
                .WhenInjectedInto<GameObjectInstantiator>();

            _container.Bind<InitializableHandler>().ToSingle();
            _container.Bind<DisposablesHandler>().ToSingle();

            _container.Bind<UnityEventManager>().ToSingleGameObject();
            _container.Bind<ITickable>().ToLookup<UnityEventManager>();
        }
    }
}
