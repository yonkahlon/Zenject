#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using System.Linq;

namespace Zenject
{
    // This class can be used in cases where you want to instantiate a prefab but
    // do not have a MonoBehaviour class to associate with it
    // (Note: If you do have a MonoBehaviour class on your prefab that you want to
    // represent the newly instantiated prefab, you probably want to use
    // MonoBehaviourFactory instead)
    public abstract class GameObjectFactory : IValidatable
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        GameObject _prefab = null;

        [InjectOptional]
        string _groupName = null;

        protected GameObject Prefab
        {
            get
            {
                return _prefab;
            }
        }

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        public GameObject Create()
        {
            return Container.InstantiatePrefab(_groupName, _prefab);
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return PrefabBasedFactoryUtil.Validate(_container, _prefab);
        }
    }
}

#endif
