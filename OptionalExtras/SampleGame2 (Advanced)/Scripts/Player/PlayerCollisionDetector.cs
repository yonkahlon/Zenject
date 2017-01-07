using UnityEngine;
using Zenject;

namespace Zenject.SpaceFighter
{
    public class PlayerCollisionDetector : MonoBehaviour
    {
        GameEvents _gameEvents;

        [Inject]
        public void Construct(GameEvents gameEvents)
        {
            _gameEvents = gameEvents;
        }

        public void OnTriggerEnter(Collider other)
        {
            var bullet = other.GetComponent<Bullet>();

            if (bullet != null && bullet.Type != BulletTypes.FromPlayer)
            {
                _gameEvents.PlayerHit(bullet);
            }
        }
    }
}

