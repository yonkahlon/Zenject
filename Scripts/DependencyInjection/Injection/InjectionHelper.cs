using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    // Helper class to manually fill in dependencies on given objects
    public static class InjectionHelper
    {
        public static void InjectChildGameObjects(DiContainer container, Transform transform)
        {
            // Inject dependencies into child game objects
            foreach (var childTransform in transform.GetComponentsInChildren<Transform>())
            {
                InjectGameObject(container, childTransform.gameObject);
            }
        }

        public static void InjectGameObject(DiContainer container, GameObject gameObj)
        {
            foreach (var component in gameObj.GetComponents<MonoBehaviour>())
            {
                InjectMonoBehaviour(container, component);
            }
        }

        public static void InjectMonoBehaviour(DiContainer container, MonoBehaviour component)
        {
            // null if monobehaviour link is broken
            if (component != null && component.enabled)
            {
                using (container.PushLookup(component.GetType()))
                {
                    FieldsInjecter.Inject(container, component);
                }
            }
        }
    }
}
