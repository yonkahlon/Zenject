using System;
using UnityEngine;
using Zenject;

namespace Zenject.SpaceFighter
{
    public class EnemyDeathHandler
    {
        readonly GameEvents _gameEvents;
        readonly Settings _settings;
        readonly Explosion.Factory _explosionFactory;
        readonly AudioPlayer _audioPlayer;
        readonly Enemy _enemy;

        public EnemyDeathHandler(
            Enemy enemy,
            AudioPlayer audioPlayer,
            Explosion.Factory explosionFactory,
            Settings settings,
            GameEvents gameEvents)
        {
            _gameEvents = gameEvents;
            _settings = settings;
            _explosionFactory = explosionFactory;
            _audioPlayer = audioPlayer;
            _enemy = enemy;
        }

        public void Die()
        {
            var explosion = _explosionFactory.Spawn();
            explosion.transform.position = _enemy.Position;

            _audioPlayer.Play(_settings.DeathSound, _settings.DeathSoundVolume);

            _gameEvents.EnemyKilled();
        }

        [Serializable]
        public class Settings
        {
            public AudioClip DeathSound;
            public float DeathSoundVolume = 1.0f;
        }
    }
}

