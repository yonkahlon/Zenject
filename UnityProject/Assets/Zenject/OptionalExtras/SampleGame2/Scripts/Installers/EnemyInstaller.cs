using System;
using UnityEngine;
using Zenject;
using Zenject.Commands;

namespace ModestTree
{
    public class EnemyInstaller : MonoInstaller
    {
        [SerializeField]
        Settings _settings = null;

        [InjectOptional]
        EnemyTunables _settingsOverride = null;

        public override void InstallBindings()
        {
            // Doing it this way allows us to drop enemies into the scene and set these settings
            // directly but also allow creating enemies dynamically and injecting these settings instead
            Container.BindInstance(_settingsOverride != null ? _settingsOverride : _settings.DefaultSettings);

            Container.BindInstance(_settings.Rigidbody).WhenInjectedInto<EnemyModel>();
            Container.BindInstance(_settings.Collider).WhenInjectedInto<EnemyModel>();
            Container.BindInstance(_settings.Renderer).WhenInjectedInto<EnemyModel>();

            Container.Bind<EnemyModel>().ToSingle();

            Container.BindAllInterfaces<EnemyCollisionHandler>().ToSingle<EnemyCollisionHandler>();

            Container.BindAllInterfacesAndSelf<EnemyStateManager>().ToSingle<EnemyStateManager>();

            Container.Bind<EnemyStateFactory>().ToSingle();

            Container.BindAllInterfaces<EnemyHealthWatcher>().ToSingle<EnemyHealthWatcher>();

            Container.BindSignal<EnemySignals.Hit>();
            Container.BindTrigger<EnemySignals.Hit.Trigger>();

            Container.BindAllInterfaces<EnemyStateCommon>().ToSingle<EnemyStateCommon>();
            Container.BindAllInterfaces<EnemyRotationHandler>().ToSingle<EnemyRotationHandler>();

            InstallSettings();
        }

        void InstallSettings()
        {
            Container.BindInstance(_settings.EnemyCollisionHandler);
            Container.BindInstance(_settings.EnemyStateIdle);
            Container.BindInstance(_settings.EnemyStateRunAway);
            Container.BindInstance(_settings.EnemyRotationHandler);
            Container.BindInstance(_settings.EnemyStateFollow);
            Container.BindInstance(_settings.EnemyStateAttack);
        }

        [Serializable]
        public class Settings
        {
            public GameObject RootObject;
            public Rigidbody Rigidbody;
            public Collider Collider;
            public MeshRenderer Renderer;

            public EnemyTunables DefaultSettings;
            public EnemyCollisionHandler.Settings EnemyCollisionHandler;
            public EnemyStateIdle.Settings EnemyStateIdle;
            public EnemyStateRunAway.Settings EnemyStateRunAway;
            public EnemyRotationHandler.Settings EnemyRotationHandler;
            public EnemyStateFollow.Settings EnemyStateFollow;
            public EnemyStateAttack.Settings EnemyStateAttack;
        }
    }
}
