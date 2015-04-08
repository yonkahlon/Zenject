#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEngine;

namespace Zenject
{
    // Helper to instantiate game objects and also inject
    // any dependencies they have
    public class GameObjectInstantiator
    {
        readonly DiContainer _container;
        readonly Transform _rootTransform;

        public GameObjectInstantiator(
            DiContainer container, Transform rootTransform)
        {
            _container = container;
            _rootTransform = rootTransform;
        }

        // Add a monobehaviour to an existing game object
        // Note: gameobject here is not a prefab prototype, it is an instance
        public TContract AddMonobehaviour<TContract>(
            GameObject gameObject, params object[] args) where TContract : Component
        {
            return (TContract)AddMonobehaviour(typeof(TContract), gameObject, args);
        }

        // Add a monobehaviour to an existing game object, using Type rather than a generic
        // Note: gameobject here is not a prefab prototype, it is an instance
        public Component AddMonobehaviour(
            Type behaviourType, GameObject gameObject, params object[] args)
        {
            Assert.That(behaviourType.DerivesFrom<Component>());
            var monoBehaviour = (Component)gameObject.AddComponent(behaviourType);
            _container.Inject(monoBehaviour, args);
            return monoBehaviour;
        }

        // Create a new game object from a given prefab
        // Without returning any particular monobehaviour
        public GameObject Instantiate(GameObject template, params object[] args)
        {
            return InstantiateWithContext(template, null, args);
        }

        public GameObject InstantiateWithContext(
            GameObject template, InjectContext context, params object[] args)
        {
            var gameObj = (GameObject)GameObject.Instantiate(template);

            // By default parent to comp root
            // This is good so that the entire object graph is
            // contained underneath it, which is useful for cases
            // where you need to delete the entire object graph
            gameObj.transform.SetParent(_rootTransform, false);

            gameObj.SetActive(true);

            _container.InjectGameObject(gameObj, true, false, args, context);

            return gameObj;
        }

        public T Instantiate<T>(
            GameObject template, params object[] args) where T : Component
        {
            return (T)Instantiate(typeof(T), template, args);
        }

        // Create from prefab
        // Return specific monobehaviour
        public object Instantiate(
            Type componentType, GameObject template, params object[] args)
        {
            Assert.That(template != null, "Null template found when instantiating game object");

            // It could be an interface so this may fail in valid cases:
            Assert.That(componentType.DerivesFrom<Component>());

            var gameObj = (GameObject)GameObject.Instantiate(template);

            // By default parent to comp root
            // This is good so that the entire object graph is
            // contained underneath it, which is useful for cases
            // where you need to delete the entire object graph
            gameObj.transform.SetParent(_rootTransform, false);

            gameObj.SetActive(true);

            Component requestedScript = null;

            // Inject on the children first since the parent objects are more likely to use them in their post inject methods
            foreach (var component in UnityUtil.GetComponentsInChildrenDepthFirst<Component>(gameObj, false))
            {
                if (component != null)
                {
                    var extraArgs = Enumerable.Empty<object>();

                    if (component.GetType().DerivesFromOrEqual(componentType))
                    {
                        Assert.IsNull(requestedScript,
                            "Found multiple matches with type '{0}' when instantiating new game object from template '{1}'", componentType, template.name);
                        requestedScript = component;
                        extraArgs = args;
                    }

                    _container.Inject(component, extraArgs);
                }
                else
                {
                    Log.Warn("Found null component while instantiating prefab '{0}'.  Possible missing script.", template.name);
                }
            }

            if (requestedScript == null)
            {
                throw new ZenjectResolveException(
                    "Could not find component with type '{0}' when instantiating new game object".Fmt(componentType));
            }

            return requestedScript;
        }

        public object Instantiate(Type type, string name)
        {
            return Instantiate(type, name, new InjectContext(_container, type));
        }

        public object Instantiate(Type type, string name, InjectContext context)
        {
            var gameObj = new GameObject(name);
            gameObj.transform.SetParent(_rootTransform, false);

            if (type == typeof(Transform))
            {
                return gameObj.transform;
            }

            var component = gameObj.AddComponent(type);

            if (type.DerivesFrom(typeof(Component)))
            {
                _container.Inject((Component)component, context);
            }

            return component;
        }

        public T Instantiate<T>(string name) where T : Component
        {
            return (T)Instantiate(typeof(T), name);
        }

        public GameObject Instantiate(string name)
        {
            var gameObj = new GameObject(name);
            gameObj.transform.SetParent(_rootTransform, false);

            return gameObj;
        }
    }
}

#endif
