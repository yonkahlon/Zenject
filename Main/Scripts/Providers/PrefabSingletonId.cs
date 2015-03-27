#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class PrefabSingletonId : IEquatable<PrefabSingletonId>
    {
        public readonly string Identifier;
        public readonly GameObject Prefab;

        public PrefabSingletonId(string identifier, GameObject prefab)
        {
            Identifier = identifier;
            Prefab = prefab;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 29 + (this.Identifier == null ? 0 : this.Identifier.GetHashCode());
                hash = hash * 29 + (ZenUtil.IsNull(this.Prefab) ? 0 : this.Prefab.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object other)
        {
            if (other is PrefabSingletonId)
            {
                PrefabSingletonId otherId = (PrefabSingletonId)other;
                return otherId == this;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(PrefabSingletonId that)
        {
            return this == that;
        }

        public static bool operator ==(PrefabSingletonId left, PrefabSingletonId right)
        {
            return object.Equals(left.Prefab, right.Prefab) && object.Equals(left.Identifier, right.Identifier);
        }

        public static bool operator !=(PrefabSingletonId left, PrefabSingletonId right)
        {
            return !left.Equals(right);
        }
    }
}

#endif
