using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public delegate bool BindingCondition(InjectContext c);

    [System.Diagnostics.DebuggerStepThrough]
    public class BindingConditionSetter
    {
        readonly List<ProviderBase> _providers;

        public BindingConditionSetter(List<ProviderBase> providers)
        {
            _providers = providers;
        }

        public BindingConditionSetter(ProviderBase provider)
        {
            _providers = new List<ProviderBase>()
            {
                provider
            };
        }

        BindingCondition Condition
        {
            set
            {
                foreach (var provider in _providers)
                {
                    provider.Condition = value;
                }
            }
        }

        public void When(BindingCondition condition)
        {
            Condition = condition;
        }

        public void WhenInjectedIntoInstance(object instance)
        {
            Condition = r => ReferenceEquals(r.ObjectInstance, instance);
        }

        public void WhenInjectedInto(params Type[] targets)
        {
            Condition = r => targets.Where(x => r.ObjectType != null && r.ObjectType.DerivesFromOrEqual(x)).Any();
        }

        public void WhenInjectedInto<T>()
        {
            Condition = r => r.ObjectType != null && r.ObjectType.DerivesFromOrEqual(typeof(T));
        }

        public void WhenNotInjectedInto<T>()
        {
            Condition = r => r.ObjectType == null || !r.ObjectType.DerivesFromOrEqual(typeof(T));
        }
    }
}
