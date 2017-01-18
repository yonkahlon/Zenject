using System;
using UnityEngine;
using Zenject;

namespace Zenject.SpaceFighter
{
    public class EnemyInstaller : MonoInstaller
    {
        [SerializeField]
        Settings _settings = null;

        public override void InstallBindings()
        {
            Container.Bind<EnemyTunables>().AsSingle();

            Container.Bind<Enemy>().AsSingle()
                .WithArguments(_settings.Renderer, _settings.Collider, _settings.Rigidbody);

            Container.BindAllInterfacesAndSelf<EnemyStateManager>().To<EnemyStateManager>().AsSingle();

            Container.Bind<EnemyStateIdle>().AsSingle();
            Container.Bind<EnemyStateAttack>().AsSingle();
            Container.Bind<EnemyStateFollow>().AsSingle();

            Container.BindAllInterfacesAndSelf<EnemyDeathHandler>().To<EnemyDeathHandler>().AsSingle();
            Container.BindAllInterfaces<EnemyRotationHandler>().To<EnemyRotationHandler>().AsSingle();
        }

        [Serializable]
        public class Settings
        {
            public GameObject RootObject;
            public Rigidbody Rigidbody;
            public Collider Collider;
            public MeshRenderer Renderer;
        }
    }
}
