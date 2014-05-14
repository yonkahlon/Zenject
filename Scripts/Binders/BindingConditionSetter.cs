using System;
using System.Collections.Generic;
using System.Linq;

namespace ModestTree.Zenject
{
    public class ResolveContext
    {
        public Type Target;
        // note this is null for constructor params
        public object TargetInstance;
        public string FieldName;
        public object Identifier;
        public List<Type> Parents;

        internal ResolveContext(
            InjectInfo injectInfo, List<Type> parents, object targetInstance)
        {
            Identifier = injectInfo.Identifier;
            FieldName = injectInfo.Name;
            Target = injectInfo.ContainedType;
            TargetInstance = targetInstance;
            Parents = parents;
        }

        internal ResolveContext(Type targetType)
        {
            Parents = new List<Type>();
            Target = targetType;
        }
    }

    public delegate bool BindingCondition(ResolveContext c);

    public class BindingConditionSetter
    {
        readonly ProviderBase _provider;

        public BindingConditionSetter(ProviderBase provider)
        {
            _provider = provider;
        }

        public void When(BindingCondition condition)
        {
            _provider.SetCondition(condition);
        }

        public void WhenInjectedIntoInstance(object instance)
        {
            _provider.SetCondition(r => ReferenceEquals(r.TargetInstance, instance));
        }

        public void WhenInjectedInto(params Type[] targets)
        {
            _provider.SetCondition(r => targets.Contains(r.Target));
        }

        public void WhenInjectedInto<T>()
        {
            _provider.SetCondition(r => r.Target == typeof(T));
        }

        public void WhenInjectedInto<T>(object identifier)
        {
            Assert.IsNotNull(identifier);
            _provider.SetCondition(r => r.Target == typeof(T) && identifier.Equals(r.Identifier));
        }

        public void WhenInjectedInto(object identifier)
        {
            Assert.IsNotNull(identifier);
            _provider.SetCondition(r => identifier.Equals(r.Identifier));
        }
    }
}
