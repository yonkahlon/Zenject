using System;
namespace ModestTree.Zenject
{
    public abstract class ProviderBase
    {
        private BindingCondition _condition = delegate { return true; };

        public BindingCondition GetCondition()
        {
            return _condition;
        }

        public void SetCondition(BindingCondition condition)
        {
            _condition = condition;
        }

        public abstract object GetInstance();
        public abstract Type GetInstanceType();

        public virtual void OnRemoved()
        {
        }
    }
}
