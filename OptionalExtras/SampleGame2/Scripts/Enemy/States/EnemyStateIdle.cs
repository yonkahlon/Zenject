using System;
using UnityEngine;
using System.Linq;

namespace ModestTree
{
    public class EnemyStateIdle : IEnemyState
    {
        readonly EnemyGlobalTunables _globalTunables;
        readonly PlayerFacade _player;
        readonly EnemyRegistry _registry;
        readonly EnemyStateManager _stateManager;
        readonly EnemySignals.Hit _hitSignal;
        readonly Settings _settings;
        readonly EnemyModel _model;

        Vector3 _startPos;
        float _theta;
        float _startTime;

        public EnemyStateIdle(
            EnemyModel model, Settings settings,
            EnemySignals.Hit hitSignal,
            EnemyStateManager stateManager,
            EnemyRegistry registry,
            PlayerFacade player,
            EnemyGlobalTunables globalTunables)
        {
            _globalTunables = globalTunables;
            _player = player;
            _registry = registry;
            _stateManager = stateManager;
            _hitSignal = hitSignal;
            _settings = settings;
            _model = model;
        }

        public void Initialize()
        {
            _startPos = _model.Position;
            _theta = 0;
            _startTime = Time.realtimeSinceStartup;

            _hitSignal.Event += OnHit;
        }

        public void Dispose()
        {
            _hitSignal.Event -= OnHit;
        }

        void OnHit(Bullet bullet)
        {
            if (_model.Health < 50.0f)
            {
                _stateManager.ChangeState(EnemyStates.RunAway);
            }
            else
            {
                _stateManager.ChangeState(EnemyStates.Follow);
            }
        }

        // Just add a bit of subtle movement
        public void Update()
        {
            _theta += Time.deltaTime * _settings.Frequency;

            _model.Position = _startPos + _model.RightDir * _settings.Amplitude * Mathf.Sin(_theta);

            // look away from player
            _model.DesiredLookDir = -(_player.Position - _model.Position).normalized;

            var elapsed = Time.realtimeSinceStartup - _startTime;

            if (_registry.Enemies.Where(x => x.IsAttackingOrChasing).Count() < _globalTunables.NumAttacking)
            {
                _stateManager.ChangeState(EnemyStates.Follow);
            }
            else if ((_player.Position - _model.Position).magnitude < _settings.AttackDistance)
            {
                _stateManager.ChangeState(EnemyStates.Attack);
            }
        }

        [Serializable]
        public class Settings
        {
            public float Amplitude;
            public float Frequency;
            public float AttackDistance;
            public float WaitTime;
        }
    }
}
