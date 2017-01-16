using System;
using UnityEngine;
using Zenject;

namespace Zenject.SpaceFighter
{
    public class PlayerHealthWatcher : ITickable
    {
        readonly GameEvents _gameEvents;
        readonly Explosion.Factory _explosionFactory;
        readonly PlayerModel _model;

        public PlayerHealthWatcher(
            PlayerModel model,
            Explosion.Factory explosionFactory,
            GameEvents gameEvents)
        {
            _gameEvents = gameEvents;
            _explosionFactory = explosionFactory;
            _model = model;
        }

        public void Tick()
        {
            if (_model.Health <= 0 && !_model.IsDead)
            {
                Die();
            }
        }

        void Die()
        {
            _model.IsDead = true;

            var explosion = _explosionFactory.Spawn();
            explosion.transform.position = _model.Position;

            _model.Renderer.enabled = false;

            _gameEvents.PlayerKilled();
        }
    }
}
