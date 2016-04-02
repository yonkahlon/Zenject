#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Zenject
{
    public abstract class AddToGameObjectComponentProviderBase : IProvider
    {
        readonly string _concreteIdentifier;
        readonly Type _componentType;
        readonly DiContainer _container;
        readonly List<TypeValuePair> _extraArguments;
        readonly List<Type> _extraArgumentTypes;

        public AddToGameObjectComponentProviderBase(
            DiContainer container, Type componentType,
            string concreteIdentifier, List<TypeValuePair> extraArguments)
        {
            Assert.That(componentType.DerivesFrom<Component>());

            _concreteIdentifier = concreteIdentifier;
            _extraArguments = extraArguments;
            _componentType = componentType;
            _container = container;

            // Cache this since _extraArguments changes
            _extraArgumentTypes = extraArguments.Select(x => x.Type).ToList();
        }

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        protected Type ComponentType
        {
            get
            {
                return _componentType;
            }
        }

        protected string ConcreteIdentifier
        {
            get
            {
                return _concreteIdentifier;
            }
        }

        public Type GetInstanceType(InjectContext context)
        {
            return _componentType;
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsNotNull(context);

            var gameObj = GetGameObject(context);

            var instance = gameObj.AddComponent(_componentType);

            // Note that we don't just use InstantiateComponentOnNewGameObjectExplicit here
            // because then circular references don't work
            yield return new List<object>() { instance };

            var injectArgs = new InjectArgs()
            {
                ExtraArgs = _extraArguments.Concat(args).ToList(),
                UseAllArgs = true,
                TypeInfo = TypeAnalyzer.GetInfo(_componentType),
                Context = context,
                ConcreteIdentifier = _concreteIdentifier,
            };

            _container.InjectExplicit(instance, injectArgs);

            Assert.That(injectArgs.ExtraArgs.IsEmpty());
        }

        public IEnumerable<ZenjectException> Validate(InjectContext context, List<Type> argTypes)
        {
            var validateArgs = new InjectValidationArgs()
            {
                ArgTypes = _extraArgumentTypes.Concat(argTypes).ToList(),
                TypeInfo = TypeAnalyzer.GetInfo(_componentType),
                Context = context,
                ConcreteIdentifier = _concreteIdentifier,
                UseAllArgs = true,
            };

            return _container.ValidateObjectGraph(validateArgs);
        }

        protected abstract GameObject GetGameObject(InjectContext context);
    }
}

#endif
