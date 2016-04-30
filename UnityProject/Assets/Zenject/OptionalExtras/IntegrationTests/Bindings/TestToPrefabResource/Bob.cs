using System;
using UnityEngine;
using Zenject;

namespace ModestTree.Tests.Zenject.ToPrefabResource
{
    public class Bob : MonoBehaviour
    {
        [NonSerialized]
        [Inject]
        public Jim Jim;
    }
}
