using UnityEngine;
using Zenject;

namespace Zenject.SpaceFighter
{
    public class EnemyCollisionDetector : MonoBehaviour
    {
        GameEvents _gameEvents;
        EnemyModel _enemy;

        [Inject]
        public void Construct(GameEvents gameEvents, EnemyModel enemy)
        {
            _gameEvents = gameEvents;
            _enemy = enemy;
        }

        public void OnTriggerEnter(Collider other)
        {
            var bullet = other.GetComponent<Bullet>();

            if (bullet != null && bullet.Type != BulletTypes.FromEnemy)
            {
                _gameEvents.EnemyHit(_enemy, bullet);
            }
        }
    }
}


