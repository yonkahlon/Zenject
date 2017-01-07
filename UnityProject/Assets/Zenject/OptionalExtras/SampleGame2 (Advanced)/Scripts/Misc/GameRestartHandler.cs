using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Zenject.SpaceFighter
{
    public class GameRestartHandler : IInitializable, IDisposable, ITickable
    {
        readonly Settings _settings;

        GameEvents _gameEvents;
        bool _isDelaying;
        float _delayStartTime;

        public GameRestartHandler(
            Settings settings,
            GameEvents gameEvents)
        {
            _gameEvents = gameEvents;
            _settings = settings;
        }

        public void Initialize()
        {
            _gameEvents.PlayerKilled += OnPlayerKilled;
        }

        public void Dispose()
        {
            _gameEvents.PlayerKilled -= OnPlayerKilled;
        }

        public void Tick()
        {
            if (_isDelaying)
            {
                if (Time.realtimeSinceStartup - _delayStartTime > _settings.RestartDelay)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }

        void OnPlayerKilled()
        {
            // Wait a bit before restarting the scene
            _delayStartTime = Time.realtimeSinceStartup;
            _isDelaying = true;
        }

        [Serializable]
        public class Settings
        {
            public float RestartDelay;
        }
    }
}
