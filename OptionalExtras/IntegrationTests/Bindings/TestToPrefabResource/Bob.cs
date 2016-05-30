using System;
using UnityEngine;
using Zenject;

namespace Zenject.Tests.ToPrefabResource
{
    public class Bob : MonoBehaviour
    {
        [NonSerialized]
        [Inject]
        public Jim Jim;
    }
}
