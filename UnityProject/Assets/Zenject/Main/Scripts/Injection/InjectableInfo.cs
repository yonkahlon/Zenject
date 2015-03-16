using System;
using System.Linq;

namespace Zenject
{
    // An injectable is a field or property with [Inject] attribute
    // Or a constructor parameter
    internal class InjectableInfo
    {
        public bool Optional;
        public string Identifier;

        // The field name or property name from source code
        public string MemberName;
        // The field type or property type from source code
        public Type MemberType;

        public Type ParentType;

        // Null for constructor declared dependencies
        public Action<object, object> Setter;

        public InjectContext CreateInjectContext(DiContainer container, object targetInstance)
        {
            return new InjectContext(
                container, MemberType, Identifier, Optional,
                ParentType, targetInstance, MemberName,
                DiContainer.LookupsInProgress.ToList());
        }
    }
}
