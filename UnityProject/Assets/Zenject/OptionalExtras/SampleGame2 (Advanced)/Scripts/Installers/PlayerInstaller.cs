using System;
using UnityEngine;
using Zenject;

namespace Zenject.SpaceFighter
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField]
        Settings _settings = null;

        public override void InstallBindings()
        {
            Container.BindInstance(_settings.Rigidbody).WhenInjectedInto<PlayerModel>();
            Container.BindInstance(_settings.MeshRenderer).WhenInjectedInto<PlayerModel>();
            Container.Bind<PlayerModel>().AsSingle();

            Container.BindAllInterfaces<PlayerInputHandler>().To<PlayerInputHandler>().AsSingle();
            Container.BindAllInterfaces<PlayerMoveHandler>().To<PlayerMoveHandler>().AsSingle();
            Container.BindAllInterfacesAndSelf<PlayerBulletHitHandler>().To<PlayerBulletHitHandler>().AsSingle();
            Container.BindAllInterfaces<PlayerDirectionHandler>().To<PlayerDirectionHandler>().AsSingle();
            Container.BindAllInterfaces<PlayerShootHandler>().To<PlayerShootHandler>().AsSingle();

            Container.Bind<PlayerInputState>().AsSingle();

            Container.BindAllInterfaces<PlayerHealthWatcher>().To<PlayerHealthWatcher>().AsSingle();
        }

        [Serializable]
        public class Settings
        {
            public Rigidbody Rigidbody;
            public MeshRenderer MeshRenderer;
        }
    }
}
