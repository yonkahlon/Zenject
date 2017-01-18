using System;
using UnityEngine;
using Zenject;

namespace Zenject.SpaceFighter
{
    public class EnemyFacade : MonoBehaviour, IPoolable<EnemyTunables>
    {
        Enemy _enemy;
        EnemyTunables _tunables;
        Factory _selfFactory;
        EnemyDeathHandler _deathHandler;

        // We can't use a constructor here because MonoFacade derives from
        // MonoBehaviour
        [Inject]
        public void Construct(
            Enemy enemy, EnemyTunables tunables,
            Factory selfFactory, EnemyDeathHandler deathHandler)
        {
            _enemy = enemy;
            _tunables = tunables;
            _selfFactory = selfFactory;
            _deathHandler = deathHandler;
        }

        // Here we can add some high-level methods to give some info to other
        // parts of the codebase outside of our enemy facade
        public Vector3 Position
        {
            get { return _enemy.Position; }
            set { _enemy.Position = value; }
        }

        public void OnDespawned()
        {
            gameObject.SetActive(false);
        }

        public void Update()
        {
            // Always ensure we are on the main plane
            _enemy.Position = new Vector3(_enemy.Position.x, _enemy.Position.y, 0);
        }

        public void OnSpawned(EnemyTunables tunables)
        {
            _tunables.Accuracy = tunables.Accuracy;
            _tunables.Speed = tunables.Speed;

            gameObject.SetActive(true);
        }

        public void Die()
        {
            _deathHandler.Die();
            _selfFactory.Despawn(this);
        }

        // Here we declare a parameter to our facade factory of type EnemyTunables
        // Note that unlike for normal factories, this parameter gets injected into
        // an installer instead of the EnemyFacade class itself
        // It's done this way because in some cases we want to add the arguments
        // to the container for use by other classes within the facade
        public class Factory : PooledFactory<EnemyTunables, EnemyFacade>
        {
        }
    }
}
