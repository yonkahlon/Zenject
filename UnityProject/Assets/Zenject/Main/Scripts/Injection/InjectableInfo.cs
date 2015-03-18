using System;
using System.Linq;

namespace Zenject
{
    // An injectable is a field or property with [Inject] attribute
    // Or a constructor parameter
    internal class InjectableInfo
    {
        public readonly bool Optional;
        public readonly string Identifier;

        // The field name or property name from source code
        public readonly string MemberName;
        // The field type or property type from source code
        public readonly Type MemberType;

        public readonly Type ParentType;

        // Null for constructor declared dependencies
        public readonly Action<object, object> Setter;

        public InjectableInfo(
            bool optional, string identifier, string memberName,
            Type memberType, Type parentType, Action<object, object> setter)
        {
            Optional = optional;
            Setter = setter;
            ParentType = parentType;
            MemberType = memberType;
            MemberName = memberName;
            Identifier = identifier;
        }

        public InjectContext CreateInjectContext(DiContainer container, object targetInstance)
        {
            return new InjectContext(
                container, MemberType, Identifier, Optional,
                ParentType, targetInstance, MemberName,
                DiContainer.LookupsInProgress.ToList());
        }
    }
}
