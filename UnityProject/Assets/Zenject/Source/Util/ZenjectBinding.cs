#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using ModestTree;

namespace Zenject
{
    // We include Zenject as a prefix to make it obvious to whoever sees it in the scene what it is, since
    // it will likely be spread out throughout the scene
    [ExecuteInEditMode]
    public class ZenjectBinding : MonoBehaviour
    {
        [SerializeField]
        Component _component = null;

        [SerializeField]
        BindTypes _bindType = BindTypes.ToInstance;

        public Component Component
        {
            get
            {
                return _component;
            }
        }

        public BindTypes BindType
        {
            get
            {
                return _bindType;
            }
        }

        public enum BindTypes
        {
            ToInstance,
            ToInterfaces,
            ToInstanceAndInterfaces,
        }
    }
}

#endif

