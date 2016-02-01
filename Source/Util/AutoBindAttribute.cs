#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ModestTree;

namespace Zenject
{
    public enum AutoBindTypes
    {
        Self,
        Interfaces,
        All,
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public abstract class AutoBindAttributeBase : Attribute
    {
        public AutoBindTypes BindType
        {
            get;
            protected set;
        }
    }

    public class AutoBindAttribute : AutoBindAttributeBase
    {
        public AutoBindAttribute()
        {
            BindType = AutoBindTypes.Self;
        }
    }

    public class AutoBindAllAttribute : AutoBindAttributeBase
    {
        public AutoBindAllAttribute()
        {
            BindType = AutoBindTypes.All;
        }
    }

    public class AutoBindInterfacesAttribute : AutoBindAttributeBase
    {
        public AutoBindInterfacesAttribute()
        {
            BindType = AutoBindTypes.Interfaces;
        }
    }
}

#endif

