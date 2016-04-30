using System;
using UnityEngine;
using Zenject;

namespace ModestTree.Tests.Zenject.ToPrefabResource
{
    public class Jim : MonoBehaviour
    {
        [NonSerialized]
        [Inject]
        public Bob Bob;
    }
}
