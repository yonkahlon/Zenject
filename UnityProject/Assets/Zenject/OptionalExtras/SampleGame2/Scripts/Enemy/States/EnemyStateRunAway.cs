using System;
using UnityEngine;

namespace ModestTree
{
    public class EnemyStateRunAway : IEnemyState
    {
        readonly EnemyStateManager _stateManager;
        readonly Settings _settings;
        readonly EnemyModel _model;
        readonly PlayerFacade _player;

        public EnemyStateRunAway(
            PlayerFacade player,
            EnemyModel model,
            Settings settings,
            EnemyStateManager stateManager)
        {
            _stateManager = stateManager;
            _settings = settings;
            _model = model;
            _player = player;
        }

        public void Initialize()
        {
        }

        public void Dispose()
        {
        }

        public void Update()
        {
            // look away from player
            _model.DesiredLookDir = -(_player.Position - _model.Position).normalized;

            MoveAwayFromPlayer();

            if ((_player.Position - _model.Position).magnitude > _settings.SafeDistance)
            {
                _stateManager.ChangeState(EnemyStates.Idle);
            }
        }

        void MoveAwayFromPlayer()
        {
            _model.AddForce(_model.LookDir * _model.MoveSpeed);
        }

        [Serializable]
        public class Settings
        {
            public float SafeDistance;
        }
    }
}
