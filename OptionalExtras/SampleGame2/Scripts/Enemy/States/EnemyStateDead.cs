using System;
using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class EnemyStateDead : IEnemyState
    {
        readonly EnemyModel _model;
        readonly CompositionRoot _compRoot;
        readonly AudioPlayer _audioPlayer;
        readonly Settings _settings;

        float _startTime;

        public EnemyStateDead(
            Settings settings,
            AudioPlayer audioPlayer,
            CompositionRoot compRoot,
            EnemyModel model)
        {
            _model = model;
            _compRoot = compRoot;
            _audioPlayer = audioPlayer;
            _settings = settings;
        }

        public void Initialize()
        {
            _audioPlayer.Play(_settings.DeathSound, _settings.DeathSoundVolume);
            _startTime = Time.realtimeSinceStartup;

            var explosion = (GameObject)GameObject.Instantiate(_settings.ExplosionPrefab);
            explosion.transform.SetParent(_compRoot.transform, true);
            explosion.transform.position = _model.Position;

            _model.Renderer.enabled = false;
        }

        public void Dispose()
        {
        }

        public void Update()
        {
            if (Time.realtimeSinceStartup - _startTime > _settings.Delay)
            {
                GameObject.Destroy(_compRoot.gameObject);
            }
        }

        [Serializable]
        public class Settings
        {
            public AudioClip DeathSound;
            public float DeathSoundVolume;
            public float Delay;
            public GameObject ExplosionPrefab;
        }
    }
}
