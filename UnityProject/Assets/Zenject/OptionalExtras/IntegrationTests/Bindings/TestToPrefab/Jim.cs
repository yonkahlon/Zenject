using System;
using UnityEngine;
using Zenject;

namespace ModestTree.Tests.Zenject.ToPrefab
{
    public class Jim : MonoBehaviour
    {
        [NonSerialized]
        [Inject]
        public Bob Bob;
    }
}
