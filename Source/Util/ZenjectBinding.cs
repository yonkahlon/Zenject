#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using ModestTree;

namespace Zenject
{
    public class ZenjectBinding : MonoBehaviour
    {
        [Tooltip("The component to add to the Zenject container")]
        [SerializeField]
        Component _component = null;

        [Tooltip("This value will determine what container the component gets added to.  If set to 'Scene', it will be as if the given component was bound inside an installer on the SceneCompositionRoot.  If set to 'Local', it will be as if it is bound inside an installer on whatever CompositionRoot it is in.  In most cases that will be the SceneCompositionRoot, but if it's inside a FacadeCompositionRoot it will be bound into that instead.  Typically you would only need to use the 'Scene' value when you want to bind something to the SceneCompositionRoot that is inside a FacadeCompositionRoot (eg. typically this would be the MonoFacade derived class)")]
        [SerializeField]
        ContainerTypes _containerType = ContainerTypes.Local;

        [Tooltip("This value is used to determine how to bind this component.  When set to 'ToInstance' is equivalent to calling Container.ToInstance inside an installer, and similarly for ToInterfaces and ToInterfacesAndSelf")]
        [SerializeField]
        BindTypes _bindType = BindTypes.ToInstance;

        public ContainerTypes ContainerType
        {
            get
            {
                return _containerType;
            }
        }

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
            ToInterfacesAndSelf,
        }

        public enum ContainerTypes
        {
            Scene,
            Local,
        }
    }
}

#endif

