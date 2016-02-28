using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace ModestTree
{
    public class RestartHandler : IInitializable
    {
        readonly PlayerKilledSignal _killSignal;

        public RestartHandler(PlayerKilledSignal killSignal)
        {
            _killSignal = killSignal;
        }

        public void Initialize()
        {
            _killSignal.Event += OnPlayerKilled;
        }

        void OnPlayerKilled()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
