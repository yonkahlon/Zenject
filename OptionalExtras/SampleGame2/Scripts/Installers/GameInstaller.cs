using System;
using UnityEngine;
using System.Collections;
using Zenject;
using Zenject.Commands;

namespace ModestTree
{
    // Main installer for our game
    public class GameInstaller : MonoInstaller
    {
        [SerializeField]
        Settings _settings = null;

        public override void InstallBindings()
        {
            Container.BindInstance(_settings.MainCamera).WhenInjectedInto<CameraHandler>();
            Container.BindAllInterfaces<CameraHandler>().To<CameraHandler>().AsSingle();

            Container.BindSignal<PlayerKilledSignal>();
            Container.BindTrigger<PlayerKilledSignal.Trigger>();

            Container.BindSignal<EnemyKilledSignal>();
            Container.BindTrigger<EnemyKilledSignal.Trigger>();

            Container.BindAllInterfaces<EnemySpawner>().To<EnemySpawner>().AsSingle();

            // We provide the installer here so that Zenject knows where to inject the arguments into
            throw Assert.CreateException("TODO");
            //Container.BindFacadeFactory<EnemyInstaller, EnemyFacade.Factory>(_settings.EnemyFacadePrefab, "Enemies");

            Container.BindAllInterfaces<GameDifficultyHandler>().To<GameDifficultyHandler>().AsSingle();

            Container.Bind<EnemyRegistry>().ToSelf().AsSingle();

            Container.BindFactory<float, float, BulletTypes, Bullet, Bullet.Factory>()
                .ToPrefabSelf(_settings.BulletPrefab, "Bullets");

            Container.BindFactory<Explosion, Explosion.Factory>()
                .ToPrefabSelf(_settings.ExplosionPrefab, "Explosions");

            Container.Bind<AudioPlayer>().ToSelf().AsSingle();

            Container.BindAllInterfaces<GameRestartHandler>().To<GameRestartHandler>().AsSingle();

            InstallSettings();
        }

        void InstallSettings()
        {
            Container.BindInstance(_settings.CameraHandler);
            Container.BindInstance(_settings.EnemySpawner);
            Container.BindInstance(_settings.GameDifficultyHandler);
            Container.BindInstance(_settings.GameRestartHandler);
            Container.BindInstance(_settings.EnemyGlobalTunables);
        }

        [Serializable]
        public class Settings
        {
            public GameObject EnemyFacadePrefab;

            public GameObject BulletPrefab;
            public GameObject ExplosionPrefab;

            public Camera MainCamera;

            public CameraHandler.Settings CameraHandler;
            public EnemySpawner.Settings EnemySpawner;
            public GameDifficultyHandler.Settings GameDifficultyHandler;
            public GameRestartHandler.Settings GameRestartHandler;

            public EnemyGlobalTunables EnemyGlobalTunables;
        }
    }
}
