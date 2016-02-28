using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ModestTree
{
    public class EnemyStateAttack : IEnemyState
    {
        readonly EnemyRegistry _registry;
        readonly EnemyTunables _tunables;
        readonly EnemySignals.Hit _hitSignal;
        readonly EnemyStateManager _stateManager;
        readonly PlayerFacade _player;
        readonly Settings _settings;
        readonly EnemyModel _model;
        readonly Bullet.Factory _bulletFactory;

        float _lastShootTime;
        bool _strafeRight;
        float _lastStrafeChangeTime;

        public EnemyStateAttack(
            Bullet.Factory bulletFactory,
            EnemyModel model,
            Settings settings,
            PlayerFacade player,
            EnemyStateManager stateManager,
            EnemySignals.Hit hitSignal,
            EnemyTunables tunables,
            EnemyRegistry registry)
        {
            _registry = registry;
            _tunables = tunables;
            _hitSignal = hitSignal;
            _stateManager = stateManager;
            _player = player;
            _settings = settings;
            _model = model;
            _bulletFactory = bulletFactory;
            _strafeRight = Random.Range(0.0f, 1.0f) < 0.5f;
        }

        void Fire()
        {
            var bullet = _bulletFactory.Create(
                _settings.BulletSpeed, _settings.BulletLifetime, BulletTypes.FromEnemy);

            var accuracy = Mathf.Clamp(_tunables.Accuracy, 0, 1);
            var maxError = 1.0f - accuracy;
            var error = Random.Range(0, maxError);

            if (Random.Range(0.0f, 1.0f) < 0.5f)
            {
                error *= -1;
            }

            var thetaError = error * _settings.ErrorRangeTheta;

            bullet.transform.position = _model.Position + _model.LookDir * _settings.BulletOffsetDistance;
            bullet.transform.rotation = Quaternion.AngleAxis(thetaError, Vector3.forward) * _model.Rotation;
        }

        public void Initialize()
        {
            _hitSignal.Event += OnHit;
        }

        public void Dispose()
        {
            _hitSignal.Event -= OnHit;
        }

        void OnHit(Bullet bullet)
        {
            if (_model.Health < _settings.HealthToRunAt)
            {
                _stateManager.ChangeState(EnemyStates.RunAway);
            }
        }

        public void Update()
        {
            _model.DesiredLookDir = (_player.Position - _model.Position).normalized;

            if (Time.realtimeSinceStartup - _lastStrafeChangeTime > _settings.StrafeChangeInterval)
            {
                _lastStrafeChangeTime = Time.realtimeSinceStartup;
                _strafeRight = !_strafeRight;
            }

            Strafe();

            if (Time.realtimeSinceStartup - _lastShootTime > _settings.ShootInterval)
            {
                _lastShootTime = Time.realtimeSinceStartup;
                Fire();
            }

            if ((_player.Position - _model.Position).magnitude > _tunables.AttackDistance + _settings.AttackRangeBuffer)
            {
                _stateManager.ChangeState(EnemyStates.Follow);
            }
        }

        void Strafe()
        {
            if (_strafeRight)
            {
                _model.AddForce(_model.RightDir * _settings.StrafeMultiplier * _model.MoveSpeed);
            }
            else
            {
                _model.AddForce(-_model.RightDir * _settings.StrafeMultiplier * _model.MoveSpeed);
            }
        }

        [Serializable]
        public class Settings
        {
            public float BulletLifetime;
            public float BulletSpeed;
            public float BulletOffsetDistance;
            public float ShootInterval;
            public float ErrorRangeTheta;
            public float HealthToRunAt;
            public float AttackRangeBuffer;
            public float StrafeMultiplier;
            public float StrafeChangeInterval;
        }
    }
}
