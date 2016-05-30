using System;
using UnityEngine;
using Zenject;

namespace Zenject.Tests.ToPrefabResource
{
    public class Jim : MonoBehaviour
    {
        [NonSerialized]
        [Inject]
        public Bob Bob;
    }
}
