using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class PlayerFacade : MonoFacade
    {
        PlayerModel _model;

        [PostInject]
        public void Construct(PlayerModel player)
        {
            _model = player;
        }

        public bool IsDead
        {
            get
            {
                return _model.IsDead;
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
