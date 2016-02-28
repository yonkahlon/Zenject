using UnityEngine;

namespace ModestTree
{
    public class PlayerModel
    {
        readonly Rigidbody _rigidBody;

        float _health = 100.0f;

        public PlayerModel(
            Rigidbody rigidBody)
        {
            _rigidBody = rigidBody;
        }

        public Vector3 LookDir
        {
            get
            {
                return -_rigidBody.transform.right;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return _rigidBody.rotation;
            }
            set
            {
                _rigidBody.rotation = value;
            }
        }

        public Vector3 Position
        {
            get
            {
                return _rigidBody.position;
            }
            set
            {
                _rigidBody.position = value;
            }
        }

        public Vector3 Velocity
        {
            get
            {
                return _rigidBody.velocity;
            }
        }

        public void TakeDamage(float healthLoss)
        {
            _health -= healthLoss;
        }

        public void AddForce(Vector3 force)
        {
            _rigidBody.AddForce(force);
        }
    }
}
