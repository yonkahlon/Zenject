using System;
using UnityEngine;
using System.Collections;
using Zenject;
using Random=UnityEngine.Random;

namespace Zenject.Asteroids
{
    public class ExplosionFactory : Factory<GameObject>
    {
    }

    public class BrokenShipFactory : Factory<GameObject>
    {
    }

    public class ShipStateDead : ShipState
    {
        readonly BrokenShipFactory _brokenShipFactory;
        readonly ExplosionFactory _explosionFactory;
        readonly Settings _settings;
        readonly GameEvents _gameEvents;
        readonly Ship _ship;

        GameObject _shipBroken;
        GameObject _explosion;

        public ShipStateDead(
            Settings settings, Ship ship,
            GameEvents gameEvents,
            ExplosionFactory explosionFactory,
            BrokenShipFactory brokenShipFactory)
        {
            _brokenShipFactory = brokenShipFactory;
            _explosionFactory = explosionFactory;
            _settings = settings;
            _gameEvents = gameEvents;
            _ship = ship;
        }

        public override void Start()
        {
            _ship.MeshRenderer.enabled = false;
            _ship.ParticleEmitter.emit = false;

            _explosion = _explosionFactory.Create();
            _explosion.transform.position = _ship.Position;

            _shipBroken = _brokenShipFactory.Create();
            _shipBroken.transform.position = _ship.Position;
            _shipBroken.transform.rotation = _ship.Rotation;

            foreach (var rigidBody in _shipBroken.GetComponentsInChildren<Rigidbody>())
            {
                var randomTheta = Random.Range(0, Mathf.PI * 2.0f);
                var randomDir = new Vector3(Mathf.Cos(randomTheta), Mathf.Sin(randomTheta), 0);
                rigidBody.AddForce(randomDir * _settings.explosionForce);
            }

            _gameEvents.ShipCrashed();
        }

        public override void Dispose()
        {
            _ship.MeshRenderer.enabled = true;
            _ship.ParticleEmitter.emit = true;

            GameObject.Destroy(_explosion);
            GameObject.Destroy(_shipBroken);
        }

        public override void Update()
        {
        }

        [Serializable]
        public class Settings
        {
            public GameObject brokenTemplate;
            public GameObject explosionTemplate;
            public float explosionForce;
        }

        public class Factory : Factory<ShipStateDead>
        {
        }
    }
}
