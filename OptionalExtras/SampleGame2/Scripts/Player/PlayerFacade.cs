using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class PlayerFacade : MonoFacade
    {
        PlayerModel _model;
        PlayerSignals.Hit.Trigger _hitTrigger;

        [PostInject]
        public void Construct(PlayerModel player, PlayerSignals.Hit.Trigger hitTrigger)
        {
            _model = player;
            _hitTrigger = hitTrigger;
        }

        public void OnTriggerEnter(Collider other)
        {
            var bullet = other.GetComponent<Bullet>();

            if (bullet != null && bullet.Type != BulletTypes.FromPlayer)
            {
                _hitTrigger.Fire(bullet);
            }
        }

        public Vector3 Position
        {
            get
            {
                return _model.Position;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return _model.Rotation;
            }
        }
    }
}
