using System;
using UnityEngine;
using System.Collections;
using Zenject;

namespace Zenject.SpaceFighter
{
    // Main installer for our game
    public class GameInstaller : MonoInstaller
    {
        [Inject]
        Settings _settings = null;

        public override void InstallBindings()
        {
            Container.Bind<GameEvents>().AsSingle();

            Container.BindAllInterfaces<EnemySpawner>().To<EnemySpawner>().AsSingle();

            Container.BindPooledFactory<EnemyFacade, EnemyFacade.Factory>()
                .FromSubContainerResolve()
                .ByPrefab(_settings.EnemyFacadePrefab)
                .UnderTransformGroup("Enemies");

            Container.BindPooledFactory<Bullet, Bullet.Factory>().WithInitialSize(10).ExpandByDoubling()
                .FromPrefab(_settings.BulletPrefab)
                .UnderTransformGroup("Bullets");

            Container.Bind<LevelBoundary>().AsSingle();

            Container.BindPooledFactory<Explosion, Explosion.Factory>().WithInitialSize(3)
                .FromPrefab(_settings.ExplosionPrefab)
                .UnderTransformGroup("Explosions");

            Container.Bind<AudioPlayer>().AsSingle();

            Container.BindAllInterfaces<GameRestartHandler>().To<GameRestartHandler>().AsSingle();
        }

        [Serializable]
        public class Settings
        {
            public GameObject EnemyFacadePrefab;
            public GameObject BulletPrefab;
            public GameObject ExplosionPrefab;
        }
    }
}
