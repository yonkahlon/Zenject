﻿using System;
using UnityEngine;
using System.Collections;
using Zenject;
using Zenject.Commands;

namespace ModestTree
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField]
        Settings _settings = null;

        public override void InstallBindings()
        {
            Container.BindInstance(_settings.MainCamera).WhenInjectedInto<CameraHandler>();
            Container.BindAllInterfaces<CameraHandler>().ToSingle<CameraHandler>();

            Container.BindSignal<PlayerKilledSignal>();
            Container.BindTrigger<PlayerKilledSignal.Trigger>().WhenInjectedInto<PlayerCollisionHandler>();

            Container.BindAllInterfaces<RestartHandler>().ToSingle<RestartHandler>();

            Container.BindAllInterfaces<EnemySpawner>().ToSingle<EnemySpawner>();

            Container.BindMonoFacadeFactory<EnemyInstaller, EnemyFacade.Factory>(_settings.EnemyFacadePrefab, "Enemies");

            Container.BindAllInterfaces<GameDifficultyHandler>().ToSingle<GameDifficultyHandler>();

            Container.Bind<EnemyRegistry>().ToSingle();

            Container.BindMonoBehaviourFactory<Bullet.Factory>(
                _settings.BulletPrefab, "Bullets",
                // We use FactoryCreateContainers.RuntimeContainer instead of the default
                // (FactoryCreateContainers.BindContainer) so that the bullets get created as root game
                // objects.  Otherwise, our bullets would be created inside the enemy and player facade
                // composition roots.  In that case, if an enemy was destroyed then its bullets would
                // also be destroyed which is not what we want
                ContainerTypes.BindContainer);

            Container.Bind<AudioPlayer>().ToSingle();

            InstallSettings();
        }

        void InstallSettings()
        {
            Container.BindInstance(_settings.CameraHandler);
            Container.BindInstance(_settings.EnemySpawner);
            Container.BindInstance(_settings.GameDifficultyHandler);
            Container.BindInstance(_settings.EnemyGlobalTunables);
        }

        [Serializable]
        public class Settings
        {
            public GameObject EnemyFacadePrefab;
            public GameObject BulletPrefab;

            public Camera MainCamera;

            public CameraHandler.Settings CameraHandler;
            public EnemySpawner.Settings EnemySpawner;
            public GameDifficultyHandler.Settings GameDifficultyHandler;

            public EnemyGlobalTunables EnemyGlobalTunables;
        }
    }
}
