using System;
using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField]
        float _lifeTime;

        [SerializeField]
        AudioClip _sound;

        [SerializeField]
        float _soundVolume;

        float _startTime;

        [PostInject]
        public void Construct(AudioPlayer audioPlayer)
        {
            _startTime = Time.realtimeSinceStartup;

            audioPlayer.Play(_sound, _soundVolume);
        }

        public void Update()
        {
            if (Time.realtimeSinceStartup - _startTime > _lifeTime)
            {
                GameObject.Destroy(this.gameObject);
            }
        }

        public class Factory : MonoBehaviourFactory<Explosion>
        {
        }
    }
}

