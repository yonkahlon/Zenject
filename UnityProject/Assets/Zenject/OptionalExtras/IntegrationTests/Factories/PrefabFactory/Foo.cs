using System;
using Zenject;
using UnityEngine;

namespace Zenject.Tests.TestPrefabFactory
{
    public class Foo : MonoBehaviour
    {
        public bool WasInitialized;

        [Inject]
        public void Init()
        {
            WasInitialized = true;
        }
    }
}
