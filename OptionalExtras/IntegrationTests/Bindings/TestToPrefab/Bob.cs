using System;
using UnityEngine;
using Zenject;

namespace Zenject.Tests.ToPrefab
{
    public class Bob : MonoBehaviour
    {
        [NonSerialized]
        [Inject]
        public Jim Jim;
    }
}
