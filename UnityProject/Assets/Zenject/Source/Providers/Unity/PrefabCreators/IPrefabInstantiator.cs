#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zenject
{
    public interface IPrefabInstantiator
    {
        List<TypeValuePair> ExtraArguments
        {
            get;
        }

        string GameObjectName
        {
            get;
        }

        IEnumerator<GameObject> Instantiate(List<TypeValuePair> args);

        IEnumerable<ZenjectException> Validate(List<Type> args);

        GameObject GetPrefab();
    }
}

#endif
