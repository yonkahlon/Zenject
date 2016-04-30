using System;
using UnityEngine;
using Zenject;

namespace ModestTree.UnityUnitTester
{
    public class MonoTestFixture : MonoBehaviour
    {
        public DiContainer Container
        {
            get;
            set;
        }
    }
}

