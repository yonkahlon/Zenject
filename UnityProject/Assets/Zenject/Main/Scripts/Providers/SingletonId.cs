using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class SingletonId : IEquatable<SingletonId>
    {
        public readonly Type Type;
        public readonly string Identifier;
        public readonly GameObject Prefab;

        public SingletonId(string identifier, Type type, GameObject prefab)
        {
            Identifier = identifier;
            Type = type;
            Prefab = prefab;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 29 + this.Type.GetHashCode();
                hash = hash * 29 + (this.Identifier == null ? 0 : this.Identifier.GetHashCode());
                hash = hash * 29 + (UnityUtil.IsNull(this.Prefab) ? 0 : this.Prefab.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object other)
        {
            if (other is SingletonId)
            {
                SingletonId otherId = (SingletonId)other;
                return otherId == this;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(SingletonId that)
        {
            return this == that;
        }

        public static bool operator ==(SingletonId left, SingletonId right)
        {
            return left.Type == right.Type && object.Equals(left.Prefab, right.Prefab) && object.Equals(left.Identifier, right.Identifier);
        }

        public static bool operator !=(SingletonId left, SingletonId right)
        {
            return !left.Equals(right);
        }
    }
}
