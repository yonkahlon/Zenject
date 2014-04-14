using System;

namespace ModestTree.Zenject
{
    public class InjectAttributeBase : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class InjectAttribute : InjectAttributeBase
    {
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class InjectOptionalAttribute : InjectAttributeBase
    {
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class InjectNamedAttribute : InjectAttributeBase
    {
        public InjectNamedAttribute(string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            private set;
        }
    }
}
