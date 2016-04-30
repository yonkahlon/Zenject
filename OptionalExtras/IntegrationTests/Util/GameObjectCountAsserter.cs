using System;
using Zenject;
using UnityEngine;

namespace ModestTree.Tests.Zenject
{
    public class GameObjectCountAsserter : IInitializable
    {
        readonly GameObject _root;
        readonly int _expectedCount;

        public GameObjectCountAsserter(
            int expectedCount,
            CompositionRoot root)
        {
            _root = root.gameObject;
            _expectedCount = expectedCount;
        }

        public void Initialize()
        {
            Assert.IsEqual(_root.transform.childCount, _expectedCount);

            Log.Info("Correctly detected '{0}' game objects", _expectedCount);
        }
    }
}

