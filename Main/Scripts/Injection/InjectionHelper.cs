using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;
using ModestTree.Util;
#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    // Helper class to manually fill in dependencies on given objects
    public static class InjectionHelper
    {
#if !ZEN_NOT_UNITY3D
        // Inject dependencies into child game objects
        public static void InjectChildGameObjects(
            DiContainer container, GameObject gameObject, bool includeInactive = false)
        {
            InjectChildGameObjects(container, gameObject, includeInactive, Enumerable.Empty<object>());
        }

        public static void InjectChildGameObjects(
            DiContainer container, GameObject gameObject, bool includeInactive, IEnumerable<object> extraArgs)
        {
            foreach (var monoBehaviour in UnityUtil.GetComponentsInChildrenDepthFirst<MonoBehaviour>(gameObject, includeInactive))
            {
                InjectMonoBehaviour(container, monoBehaviour, extraArgs);
            }
        }

        public static void InjectGameObject(DiContainer container, GameObject gameObj)
        {
            foreach (var component in gameObj.GetComponents<Component>())
            {
                InjectMonoBehaviour(container, component);
            }
        }

        public static void InjectMonoBehaviour(DiContainer container, Component component)
        {
            InjectMonoBehaviour(container, component, Enumerable.Empty<object>());
        }

        public static void InjectMonoBehaviour(
            DiContainer container, Component component, IEnumerable<object> extraArgs)
        {
            // null if monobehaviour link is broken
            if (component != null)
            {
                using (container.PushLookup(component.GetType()))
                {
                    container.Inject(component, extraArgs);
                }
            }
        }
#endif
    }
}
