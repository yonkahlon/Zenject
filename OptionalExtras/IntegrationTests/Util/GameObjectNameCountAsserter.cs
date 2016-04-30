using System;
using Zenject;
using UnityEngine;
using System.Linq;

namespace ModestTree.Tests.Zenject
{
    public class GameObjectNameCountAsserter : IInitializable
    {
        readonly GameObject _root;
        readonly int _expectedCount;
        readonly string _name;

        public GameObjectNameCountAsserter(
            string name,
            int expectedCount,
            CompositionRoot root)
        {
            _name = name;
            _root = root.gameObject;
            _expectedCount = expectedCount;
        }

        public void Initialize()
        {
            Assert.IsEqual(
                _root.transform.Cast<Transform>()
                    .Where(x => x.name == _name).Count(),
                _expectedCount);

            Log.Info("Correctly detected '{0}' game objects with name '{1}'", _expectedCount, _name);
        }
    }
}


