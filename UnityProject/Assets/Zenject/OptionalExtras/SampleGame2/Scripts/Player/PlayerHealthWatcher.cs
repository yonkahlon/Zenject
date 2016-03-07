using System;
using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class PlayerHealthWatcher : ITickable
    {
        readonly PlayerKilledSignal.Trigger _killedSignal;
        readonly Explosion.Factory _explosionFactory;
        readonly CompositionRoot _compRoot;
        readonly PlayerModel _model;

        public PlayerHealthWatcher(
            PlayerModel model,
            CompositionRoot compRoot,
            Explosion.Factory explosionFactory,
            PlayerKilledSignal.Trigger killedSignal)
        {
            _killedSignal = killedSignal;
            _explosionFactory = explosionFactory;
            _compRoot = compRoot;
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

            var explosion = _explosionFactory.Create();
            explosion.transform.position = _model.Position;

            _model.Renderer.enabled = false;

            _killedSignal.Fire();
        }
    }
}
