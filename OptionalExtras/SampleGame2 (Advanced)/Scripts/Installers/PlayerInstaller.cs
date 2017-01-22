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
            Container.Bind<Player>().AsSingle()
                .WithArguments(_settings.Rigidbody, _settings.MeshRenderer);

            Container.BindAllInterfaces<PlayerInputHandler>().To<PlayerInputHandler>().AsSingle();
            Container.BindAllInterfaces<PlayerMoveHandler>().To<PlayerMoveHandler>().AsSingle();
            Container.BindAllInterfacesAndSelf<PlayerDamageHandler>().To<PlayerDamageHandler>().AsSingle();
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
