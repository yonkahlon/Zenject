using System;
using System.Collections.Generic;

namespace Zenject
{
    public class InjectContext
    {
        // The type of the object which is having its members injected
        public readonly Type ParentType;

        // The instance which is having its members injected
        // Note that this is null when injecting into the constructor
        public readonly object ParentInstance;

        // Identifier - most of the time this is null
        // It will match 'foo' in this example: Container.Bind("foo")
        public readonly string Identifier;

        // The constructor parameter name, or field name, or property name
        public readonly string MemberName;

        // The type of the constructor parameter, field or property
        public readonly Type MemberType;

        // List of all types that are in the process of being injected
        public readonly List<Type> ParentTypes;

        // When optional, null is a valid value to be returned
        public readonly bool Optional;

        // The container used for this injection
        public readonly DiContainer Container;

        // Convenience member for use in DiContainer
        internal readonly BindingId BindingId;

        public InjectContext(
            DiContainer container, Type memberType, string identifier, bool optional,
            Type parentType, object parentInstance, string memberName, List<Type> parentTypes)
        {
            ParentType = parentType;
            ParentInstance = parentInstance;
            Identifier = identifier;
            MemberName = memberName;
            MemberType = memberType;
            ParentTypes = parentTypes;
            Optional = optional;
            Container = container;
            BindingId = new BindingId(memberType, identifier);
        }

        public InjectContext(
            DiContainer container, Type memberType, string identifier, bool optional, Type parentType, object parentInstance)
            : this(container, memberType, identifier, optional, parentType, parentInstance, "", new List<Type>())
        {
        }

        public InjectContext(
            DiContainer container, Type memberType, string identifier, bool optional)
            : this(container, memberType, identifier, optional, null, null)
        {
        }

        public InjectContext(
            DiContainer container, Type memberType, string identifier)
            : this(container, memberType, identifier, false)
        {
        }

        public InjectContext(
            DiContainer container, Type memberType)
            : this(container, memberType, null, false)
        {
        }

        public InjectContext ChangeMemberType(Type newMemberType)
        {
            return new InjectContext(
                Container, newMemberType, Identifier, Optional, ParentType, ParentInstance, MemberName, ParentTypes);
        }

        public InjectContext ChangeId(string newIdentifier)
        {
            return new InjectContext(
                Container, MemberType, newIdentifier, Optional, ParentType, ParentInstance, MemberName, ParentTypes);
        }
    }
}
