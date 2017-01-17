using System;
using UnityEngine;
using Zenject;

namespace Zenject.SpaceFighter
{
    public class EnemyCollisionHandler : IInitializable, IDisposable
    {
        readonly AudioPlayer _audioPlayer;
        readonly Settings _settings;
        readonly EnemyModel _model;

        GameEvents _gameEvents;

        public EnemyCollisionHandler(
            EnemyModel model,
            Settings settings,
            AudioPlayer audioPlayer,
            GameEvents gameEvents)
        {
            _gameEvents = gameEvents;
            _audioPlayer = audioPlayer;
            _settings = settings;
            _model = model;
        }

        public void Initialize()
        {
            _gameEvents.EnemyHit += OnHit;
        }

        public void Dispose()
        {
            _gameEvents.EnemyHit -= OnHit;
        }

        void OnHit(EnemyModel enemy, Bullet bullet)
        {
            if (enemy != _model)
            {
                return;
            }

            _audioPlayer.Play(_settings.HitSound);
            _model.AddForce(-bullet.MoveDirection * _settings.HitForce);
            _model.TakeDamage(_settings.HealthLoss);

            bullet.Despawn();
        }

        [Serializable]
        public class Settings
        {
            public float HealthLoss;
            public float HitForce;
            public AudioClip HitSound;
        }
    }
}
