using System;
using UnityEngine;
using Zenject;
using Zenject.Commands;

namespace ModestTree
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField]
        Settings _settings = null;

        public override void InstallBindings()
        {
            Container.BindInstance(_settings.Rigidbody).WhenInjectedInto<PlayerModel>();
            Container.Bind<PlayerModel>().ToSingle();

            Container.BindAllInterfaces<PlayerInputHandler>().ToSingle<PlayerInputHandler>();

            Container.BindAllInterfaces<PlayerMoveHandler>().ToSingle<PlayerMoveHandler>();

            Container.BindAllInterfaces<PlayerCollisionHandler>().ToSingle<PlayerCollisionHandler>();

            Container.BindAllInterfaces<PlayerDirectionHandler>().ToSingle<PlayerDirectionHandler>();

            Container.BindAllInterfaces<PlayerShootHandler>().ToSingle<PlayerShootHandler>();

            Container.Bind<PlayerInputState>().ToSingle();

            Container.BindSignal<PlayerSignals.Hit>();
            Container.BindTrigger<PlayerSignals.Hit.Trigger>();

            InstallSettings();
        }

        void InstallSettings()
        {
            Container.BindInstance(_settings.PlayerMoveHandler);
            Container.BindInstance(_settings.PlayerShootHandler);
            Container.BindInstance(_settings.PlayerCollisionHandler);
        }

        [Serializable]
        public class Settings
        {
            public Transform ShootPosition;
            public Rigidbody Rigidbody;

            public PlayerMoveHandler.Settings PlayerMoveHandler;
            public PlayerShootHandler.Settings PlayerShootHandler;
            public PlayerCollisionHandler.Settings PlayerCollisionHandler;
        }
    }
}
