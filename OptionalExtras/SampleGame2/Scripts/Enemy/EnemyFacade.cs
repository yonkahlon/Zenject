using System;
using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class EnemyFacade : MonoFacade
    {
        EnemyModel _model;
        EnemySignals.Hit.Trigger _hitTrigger;
        EnemyRegistry _registry;
        EnemyStateManager _stateManager;

        [PostInject]
        public void Construct(
            EnemyModel model, EnemySignals.Hit.Trigger hitTrigger,
            EnemyRegistry registry, EnemyStateManager stateManager)
        {
            _model = model;
            _hitTrigger = hitTrigger;
            _registry = registry;
            _stateManager = stateManager;

            registry.AddEnemy(this);
        }

        public bool IsAttackingOrChasing
        {
            get
            {
                return _stateManager.CurrentState == EnemyStates.Attack || _stateManager.CurrentState == EnemyStates.Follow;
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            var bullet = other.GetComponent<Bullet>();

            if (bullet != null && bullet.Type != BulletTypes.FromEnemy)
            {
                _hitTrigger.Fire(bullet);
            }
        }

        public override void Update()
        {
            base.Update();

            // Always ensure we are on the main plane
            _model.Position = new Vector3(_model.Position.x, _model.Position.y, 0);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            _registry.RemoveEnemy(this);
        }

        public Vector3 Position
        {
            get
            {
                return _model.Position;
            }
            set
            {
                _model.Position = value;
            }
        }

        public class Factory : MonoFacadeFactory<EnemyTunables, EnemyFacade>
        {
        }
    }
}
