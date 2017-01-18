using UnityEngine;
using Zenject;

namespace Zenject.SpaceFighter
{
    public class PlayerFacade : MonoBehaviour
    {
        PlayerModel _model;
        PlayerBulletHitHandler _hitHandler;

        [Inject]
        public void Construct(PlayerModel player, PlayerBulletHitHandler hitHandler)
        {
            _model = player;
            _hitHandler = hitHandler;
        }

        public bool IsDead
        {
            get { return _model.IsDead; }
        }

        public Vector3 Position
        {
            get { return _model.Position; }
        }

        public Quaternion Rotation
        {
            get { return _model.Rotation; }
        }

        public void TakeDamage(Vector3 moveDirection)
        {
            _hitHandler.TakeDamage(moveDirection);
        }
    }
}
