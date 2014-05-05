using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModestTree.Zenject
{
    // Helper to instantiate game objects and also inject
    // any dependencies they have
    public class GameObjectInstantiator
    {
        public event Action<GameObject> GameObjectInstantiated = delegate { };

        readonly DiContainer _container;
        readonly CompositionRoot _compRoot;

        public GameObjectInstantiator(DiContainer container)
        {
            _container = container;
            _compRoot = container.Resolve<CompositionRoot>();
        }

        public Transform RootParent
        {
            get
            {
                return _compRoot.transform;
            }
        }

        // Add a monobehaviour to an existing game object
        // Note: gameobject here is not a prefab prototype, it is an instance
        public TContract AddMonobehaviour<TContract>(GameObject gameObject, params object[] args) where TContract : Component
        {
            return (TContract)AddMonobehaviour(typeof(TContract), gameObject, args);
        }

        // Add a monobehaviour to an existing game object, using Type rather than a generic
        // Note: gameobject here is not a prefab prototype, it is an instance
        public Component AddMonobehaviour(Type behaviourType, GameObject gameObject, params object[] args)
        {
            var component = gameObject.AddComponent(behaviourType);

            using (_container.PushLookup(behaviourType))
            {
                List<object> additional = new List<object>();
                if (args != null)
                {
                    additional.AddRange(args);
                }

                FieldsInjecter.Inject(_container, component, additional);

                return component;
            }
        }

        // Create a new game object from a given prefab
        // Without returning any particular monobehaviour
        public GameObject Instantiate(GameObject template, params object[] args)
        {
            var gameObj = (GameObject)GameObject.Instantiate(template);

            // By default parent to comp root
            // This is good so that the entire object graph is
            // contained underneath it, which is useful for cases
            // where you need to delete the entire object graph
            gameObj.transform.parent = _compRoot.transform;

            gameObj.SetActive(true);

            List<object> additional = new List<object>();
            if (args != null)
            {
                additional.AddRange(args);
            }

            var components = gameObj.GetComponentsInChildren<MonoBehaviour>();

            foreach (var component in components)
            {
                if (component == null)
                {
                    throw new ZenjectGeneralException(
                        "Undefined monobehaviour in template '" + template.name + "'");
                }

                FieldsInjecter.Inject(_container, component, additional);
            }

            GameObjectInstantiated(gameObj);

            return gameObj;
        }

        // Create from prefab and customize name
        // Return specific monobehaviour
        public T Instantiate<T>(GameObject template, string name) where T : MonoBehaviour
        {
            var component = Instantiate<T>(template);
            component.gameObject.name = name;
            return component;
        }

        // Create from prefab
        // Return specific monobehaviour
        public T Instantiate<T>(GameObject template) where T : MonoBehaviour
        {
            Assert.That(template != null, "Null template found when instantiating game object");

            using (_container.PushLookup(typeof(T)))
            {
                var obj = Instantiate(template);

                var component = obj.GetComponentInChildren<T>();

                if (component == null)
                {
                    throw new ZenjectResolveException(
                        "Could not find component with type '{0}' when instantiating template", typeof(T));
                }

                return component;
            }
        }

        public object Instantiate(Type type, string name)
        {
            Assert.That(typeof(Component).IsAssignableFrom(type));

            var gameObj = new GameObject(name);
            gameObj.transform.parent = _compRoot.transform;

            var component = gameObj.AddComponent(type);

            using (_container.PushLookup(type))
            {
                FieldsInjecter.Inject(_container, component);
            }

            GameObjectInstantiated(gameObj);

            return component;
        }

        public T Instantiate<T>(string name) where T : Component
        {
            return (T)Instantiate(typeof(T), name);
        }

        public GameObject Instantiate(string name)
        {
            var gameObj = new GameObject(name);
            gameObj.transform.parent = _compRoot.transform;

            GameObjectInstantiated(gameObj);

            return gameObj;
        }
    }
}
