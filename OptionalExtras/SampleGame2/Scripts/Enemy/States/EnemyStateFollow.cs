using System;
using UnityEngine;

namespace ModestTree
{
    public class EnemyStateFollow : IEnemyState
    {
        readonly EnemyTunables _tunables;
        readonly EnemyStateManager _stateManager;
        readonly Settings _settings;
        readonly EnemyModel _model;
        readonly PlayerFacade _player;

        public EnemyStateFollow(
            PlayerFacade player,
            EnemyModel model,
            Settings settings,
            EnemyStateManager stateManager,
            EnemyTunables tunables)
        {
            _tunables = tunables;
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
            // Always look towards the player
            _model.DesiredLookDir = (_player.Position - _model.Position).normalized;

            MoveTowardsPlayer();

            if ((_player.Position - _model.Position).magnitude < _tunables.AttackDistance)
            {
                _stateManager.ChangeState(EnemyStates.Attack);
            }
        }

        void MoveTowardsPlayer()
        {
            var playerDir = (_player.Position - _model.Position).normalized;
            _model.AddForce(playerDir * _model.MoveSpeed);
        }

        [Serializable]
        public class Settings
        {
        }
    }
}


