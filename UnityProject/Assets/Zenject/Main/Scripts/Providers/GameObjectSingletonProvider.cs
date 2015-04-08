#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class GameObjectSingletonProvider : ProviderBase
    {
        object _instance;
        readonly string _name;
        GameObjectInstantiator _instantiator;
        DiContainer _container;
        Type _componentType;

        public GameObjectSingletonProvider(
            Type componentType, DiContainer container, string name)
        {
            Assert.That(componentType.DerivesFrom<Component>());
            _componentType = componentType;
            _name = name;
            _container = container;
        }

        public override Type GetInstanceType()
        {
            return _componentType;
        }

        public override object GetInstance(InjectContext context)
        {
            Assert.That(_componentType.DerivesFromOrEqual(context.MemberType));

            if (_instance == null)
            {
                if (_instantiator == null)
                {
                    _instantiator = _container.Resolve<GameObjectInstantiator>();
                }

                // This is valid sometimes
                //Assert.That(!_container.AllowNullBindings,
                    //"Tried to instantiate a MonoBehaviour with type '{0}' during validation. Object graph: {1}", _componentType, DiContainer.GetCurrentObjectGraph());

                // We don't use the generic version here to avoid duplicate generic arguments to binder
                _instance = _instantiator.Instantiate(_componentType, _name, context);
                Assert.IsNotNull(_instance);
            }

            return _instance;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return BindingValidator.ValidateObjectGraph(
                _container, _componentType, context);
        }
    }
}

#endif
