﻿using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public delegate bool BindingCondition(InjectContext c);

    public class BindingConditionSetter
    {
        readonly ProviderBase _provider;

        public BindingConditionSetter(ProviderBase provider)
        {
            _provider = provider;
        }

        public void When(BindingCondition condition)
        {
            _provider.Condition = condition;
        }

        public void WhenInjectedIntoInstance(object instance)
        {
            _provider.Condition = r => ReferenceEquals(r.ParentInstance, instance);
        }

        public void WhenInjectedInto(params Type[] targets)
        {
            _provider.Condition = r => targets.Where(x => r.ParentType != null && r.ParentType.DerivesFromOrEqual(x)).Any();
        }

        public void WhenInjectedInto<T>()
        {
            _provider.Condition = r => r.ParentType != null && r.ParentType.DerivesFromOrEqual(typeof(T));
        }

        public void WhenNotInjectedInto<T>()
        {
            _provider.Condition = r => r.ParentType == null || !r.ParentType.DerivesFromOrEqual(typeof(T));
        }
    }
}
