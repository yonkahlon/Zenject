using System;

namespace Zenject
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
        public InjectAttribute(string identifier)
        {
            Identifier = identifier;
        }

        public InjectAttribute()
        {
        }

        public string Identifier
        {
            get;
            private set;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class InjectOptionalAttribute : Attribute
    {
        public InjectOptionalAttribute(string identifier)
        {
            Identifier = identifier;
        }

        public InjectOptionalAttribute()
        {
        }

        public string Identifier
        {
            get;
            private set;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PostInjectAttribute : Attribute
    {
    }
}

