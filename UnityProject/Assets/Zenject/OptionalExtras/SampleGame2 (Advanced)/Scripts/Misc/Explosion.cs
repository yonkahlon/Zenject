using System;
using UnityEngine;
using Zenject;

#pragma warning disable 649

namespace Zenject.SpaceFighter
{
    public class Explosion : MonoBehaviour, IPoolable
    {
        [SerializeField]
        float _lifeTime;

        [SerializeField]
        ParticleSystem _particleSystem;

        [SerializeField]
        AudioClip _sound;

        [SerializeField]
        float _soundVolume;

        float _startTime;

        [Inject]
        Factory _selfFactory;

        public void OnSpawned()
        {
            gameObject.SetActive(true);

            _particleSystem.Clear();
            _particleSystem.Play();

            _startTime = Time.realtimeSinceStartup;
        }

        public void OnDespawned()
        {
            gameObject.SetActive(false);
        }

        public void Update()
        {
            if (Time.realtimeSinceStartup - _startTime > _lifeTime)
            {
                _selfFactory.Despawn(this);
            }
        }

        public class Factory : PooledFactory<Explosion>
        {
        }
    }
}

