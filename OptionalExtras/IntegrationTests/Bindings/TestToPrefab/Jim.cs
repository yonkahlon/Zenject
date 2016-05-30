using System;
using UnityEngine;
using Zenject;

namespace Zenject.Tests.ToPrefab
{
    public class Jim : MonoBehaviour
    {
        [NonSerialized]
        [Inject]
        public Bob Bob;
    }
}
