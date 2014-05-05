using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class MonoBehaviourFactory
    {
        readonly DiContainer _container;

        public MonoBehaviourFactory(DiContainer container)
        {
            _container = container;
        }

        public TContract Create<TContract>(GameObject gameObject) where TContract : Component
        {
            using (_container.PushLookup(typeof(TContract)))
            {
                var injecter = new PropertiesInjecter(_container);

                var component = gameObject.AddComponent<TContract>();
                injecter.Inject(component);

                return component;
            }
        }
    }
}

