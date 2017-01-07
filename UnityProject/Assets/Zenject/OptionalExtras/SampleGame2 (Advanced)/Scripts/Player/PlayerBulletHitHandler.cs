using System;
using UnityEngine;
using Zenject;

namespace Zenject.SpaceFighter
{
    public class PlayerBulletHitHandler : IInitializable, IDisposable
    {
        readonly AudioPlayer _audioPlayer;
        readonly Settings _settings;
        readonly PlayerModel _model;

        GameEvents _gameEvents;

        public PlayerBulletHitHandler(
            PlayerModel model,
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
            _gameEvents.PlayerHit += OnHit;
        }

        public void Dispose()
        {
            _gameEvents.PlayerHit -= OnHit;
        }

        void OnHit(Bullet bullet)
        {
            _audioPlayer.Play(_settings.HitSound, _settings.HitVolume);
            _model.AddForce(-bullet.MoveDirection * _settings.HitForce);

            _model.TakeDamage(_settings.HealthLoss);

            GameObject.Destroy(bullet.gameObject);
        }

        [Serializable]
        public class Settings
        {
            public float HealthLoss;
            public float HitForce;
            public AudioClip HitSound;
            public float HitVolume;
        }
    }
}
