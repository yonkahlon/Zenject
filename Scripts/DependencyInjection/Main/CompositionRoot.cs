using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // Define this class as a component of a top-level game object of your scene heirarchy
    // Then any children will get injected during resolve stage
    public class CompositionRoot : MonoBehaviour
    {
        DiContainer _container;
        IDependencyRoot _dependencyRoot;

        static Action<DiContainer> _extraBindingLookup;

        public static Action<DiContainer> ExtraBindingsLookup
        {
            set
            {
                Assert.IsNull(_extraBindingLookup);
                _extraBindingLookup = value;
            }
        }

        public DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        void Register()
        {
            // call RegisterBindings on any installers on our game object or somewhere below in the scene heirarchy
            BroadcastMessage("RegisterBindings", _container, SendMessageOptions.RequireReceiver);
        }

        void InitContainer()
        {
            _container = new DiContainer();

            // Note: This has to go first
            _container.Bind<CompositionRoot>().ToSingle(this);

            // Init default dependencies
            _container.Bind<UnityEventManager>().ToSingleGameObject();
            _container.Bind<GameObjectInstantiator>().ToSingle();

            if (_extraBindingLookup != null)
            {
                _extraBindingLookup(_container);
                _extraBindingLookup = null;
            }
        }

        void Awake()
        {
            // Note: This log statement is important because it is often what triggers the logging system
            // to initialize.  So call as soon as possible
            Log.Debug("Started Composition Root");

            InitContainer();
            Register();
            Resolve();
        }

        void Resolve()
        {
            InjectionHelper.InjectChildGameObjects(_container, gameObject);

            if (_container.HasBinding<IDependencyRoot>())
            {
                _dependencyRoot = _container.Resolve<IDependencyRoot>();
                _dependencyRoot.Start();
            }
            else
            {
                Log.Warn("No dependency root found");
            }
        }
    }
}
