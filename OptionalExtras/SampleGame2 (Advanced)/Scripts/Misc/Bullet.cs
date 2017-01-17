using System;
using UnityEngine;
using Zenject;

namespace Zenject.SpaceFighter
{
    public enum BulletTypes
    {
        FromEnemy,
        FromPlayer,
    }

    public class Bullet : MonoBehaviour, IPoolable<float, float, BulletTypes>
    {
        float _startTime;
        BulletTypes _type;
        float _speed;
        float _lifeTime;

        [SerializeField]
        MeshRenderer _renderer = null;

        [SerializeField]
        Material _playerMaterial = null;

        [SerializeField]
        Material _enemyMaterial = null;

        Factory _selfFactory;

        [Inject]
        void Construct(Factory selfFactory)
        {
            _selfFactory = selfFactory;
        }

        public void OnSpawned(float speed, float lifeTime, BulletTypes type)
        {
            _type = type;
            _speed = speed;
            _lifeTime = lifeTime;

            _renderer.material = type == BulletTypes.FromEnemy ? _enemyMaterial : _playerMaterial;

            _startTime = Time.realtimeSinceStartup;

            this.gameObject.SetActive(true);
        }

        public void OnDespawned()
        {
            this.gameObject.SetActive(false);
        }

        public BulletTypes Type
        {
            get
            {
                return _type;
            }
        }

        public Vector3 MoveDirection
        {
            get
            {
                return transform.right;
            }
        }

        public void Update()
        {
            transform.position -= transform.right * _speed * Time.deltaTime;

            if (Time.realtimeSinceStartup - _startTime > _lifeTime)
            {
                Despawn();
            }
        }

        public void Despawn()
        {
            _selfFactory.Despawn(this);
        }

        public class Factory : PooledFactory<float, float, BulletTypes, Bullet>
        {
        }
    }
}
