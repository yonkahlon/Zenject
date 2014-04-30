using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // Define this class as a component of a top-level game object, and define
    // any editor-defined components as a child.  This is necessary to guarantee that
    // UnityContext gets created first before anything that uses IoC
    public class CompositionRoot : MonoBehaviour
    {
        DiContainer _container;
        IDependencyRoot _dependencyRoot;
        bool _hasStarted = false;

        public DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        void Register()
        {
            // call RegisterBindings on any installers on our game object or somewhere below in the scene heirarhcy
            BroadcastMessage("RegisterBindings", _container, SendMessageOptions.RequireReceiver);
        }

        void InitContainer()
        {
            _container = new DiContainer();
            // Note: This has to go first
            _container.Bind<CompositionRoot>().AsSingle(this);

            // Init default dependencies
            _container.Bind<MonoBehaviourFactory>().AsSingle();

            _container.Bind<UnityEventManager>().AsSingleGameObject();
        }

        void Awake()
        {
            // Note: This log statement is important because it is often what triggers the logging system
            // to initialize
            Log.Debug("Initializing Container");

            InitContainer();
            Register();
        }

        public void Start()
        {
            if (!_hasStarted)
            {
                _hasStarted = true;
                Resolve();
            }
        }

        void Inject(GameObject gameObj)
        {
            var injecter = new PropertiesInjecter(_container);

            foreach (var component in gameObj.GetComponents<MonoBehaviour>())
            {
                if (component != null && component.enabled) // null if monobehaviour link is broken
                {
                    using (new LookupInProgressAdder(_container, component.GetType()))
                    {
                        injecter.Inject(component);
                    }
                }
            }
        }

        void InjectStartingMonoBehaviours()
        {
            // Inject dependencies into child game objects
            foreach (var childTransform in GetComponentsInChildren<Transform>())
            {
                Inject(childTransform.gameObject);
            }

            Log.Debug("CompositionRoot: Finished Injecting Dependencies");
        }

        void Resolve()
        {
            InjectStartingMonoBehaviours();

            _dependencyRoot = _container.TryResolve<IDependencyRoot>();

            if (_dependencyRoot == null)
            {
                Log.Warn("No dependency root found");
            }
            else
            {
                _dependencyRoot.Start();
            }
        }
    }
}
