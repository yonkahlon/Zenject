using System;
using UnityEngine;
using Zenject;

namespace Zenject.SpaceFighter
{
    // Any behaviour that is common regardless of what state the enemy is in
    // goes here
    public class EnemyStateCommon : IInitializable, IDisposable, ITickable
    {
        readonly EnemyModel _model;
        readonly EnemyStateManager _stateManager;

        GameEvents _gameEvents;

        public EnemyStateCommon(
            GameEvents gameEvents,
            EnemyStateManager stateManager,
            EnemyModel model)
        {
            _model = model;
            _stateManager = stateManager;
            _gameEvents = gameEvents;
        }

        public void Initialize()
        {
            _gameEvents.EnemyHit += OnHit;
        }

        public void Dispose()
        {
            _gameEvents.EnemyHit -= OnHit;
        }

        public void Tick()
        {
            // Always ensure we are on the main plane
            _model.Position = new Vector3(_model.Position.x, _model.Position.y, 0);
        }

        void OnHit(EnemyModel model, Bullet bullet)
        {
            if (model != _model)
            {
                return;
            }

            // Run away to fight another day
            _stateManager.ChangeState(EnemyStates.RunAway);
        }
    }
}

