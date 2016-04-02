#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Zenject
{
    public class EmptyGameObjectProvider : IProvider
    {
        readonly DiContainer _container;
        readonly string _gameObjectName;

        public EmptyGameObjectProvider(
            DiContainer container, string gameObjectName)
        {
            _gameObjectName = gameObjectName;
            _container = container;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return typeof(GameObject);
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEmpty(args);

            yield return new List<object>() { _container.CreateEmptyGameObject(_gameObjectName) };
        }

        public IEnumerable<ZenjectException> Validate(InjectContext context, List<Type> argTypes)
        {
            yield break;
        }
    }
}

#endif

