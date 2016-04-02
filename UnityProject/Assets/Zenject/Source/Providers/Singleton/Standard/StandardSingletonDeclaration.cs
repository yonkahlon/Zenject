using System;
using System.Collections.Generic;
using ModestTree;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public class StandardSingletonDeclaration
    {
        public StandardSingletonDeclaration(
            SingletonId id, SingletonTypes type, object singletonSpecificId)
        {
            Id = id;
            Type = type;
            SpecificId = singletonSpecificId;
        }

        public StandardSingletonDeclaration(
            Type concreteType, string concreteIdentifier,
            SingletonTypes type, object singletonSpecificId)
            : this(
                new SingletonId(concreteType, concreteIdentifier),
                type, singletonSpecificId)
        {
        }

        public SingletonId Id
        {
            get;
            private set;
        }

        public SingletonTypes Type
        {
            get;
            private set;
        }

        public object SpecificId
        {
            get;
            private set;
        }
    }
}

