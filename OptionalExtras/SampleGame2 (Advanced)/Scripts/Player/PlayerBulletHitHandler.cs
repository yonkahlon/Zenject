using System;
using UnityEngine;
using Zenject;

namespace Zenject.SpaceFighter
{
    public class PlayerBulletHitHandler
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

        public void TakeDamage(Vector3 moveDirection)
        {
            _audioPlayer.Play(_settings.HitSound, _settings.HitSoundVolume);

            _model.AddForce(-moveDirection * _settings.HitForce);

            _model.TakeDamage(_settings.HealthLoss);
        }

        [Serializable]
        public class Settings
        {
            public float HealthLoss;
            public float HitForce;

            public AudioClip HitSound;
            public float HitSoundVolume = 1.0f;
        }
    }
}
