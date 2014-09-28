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
            Container.Bind<IDependencyRoot>().ToSingleMonoBehaviour<UnityDependencyRoot>(_root);

            Container.Bind<TickableHandler>().ToSingle();
            Container.Bind<GameObjectInstantiator>().ToSingle();
            Container.Bind<Transform>().To(_root == null ? null : _root.transform)
                .WhenInjectedInto<GameObjectInstantiator>();

            Container.Bind<InitializableHandler>().ToSingle();
            Container.Bind<DisposablesHandler>().ToSingle();

            Container.Bind<UnityEventManager>().ToSingleGameObject();
            Container.Bind<ITickable>().ToLookup<UnityEventManager>();
        }
    }
}
