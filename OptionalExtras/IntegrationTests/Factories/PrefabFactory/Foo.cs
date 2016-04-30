using System;
using Zenject;
using UnityEngine;

namespace ModestTree.Tests.Zenject.TestPrefabFactory
{
    public class Foo : MonoBehaviour
    {
        public bool WasInitialized;

        [PostInject]
        public void Init()
        {
            WasInitialized = true;
        }
    }
}
