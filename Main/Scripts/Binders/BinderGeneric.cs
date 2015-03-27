using System;

namespace Zenject
{
    public class BinderGeneric<TContract> : Binder
    {
        public BinderGeneric(DiContainer container, string identifier)
            : base(container, typeof(TContract), identifier)
        {
        }

        public BindingConditionSetter ToLookup<TConcrete>() where TConcrete : TContract
        {
            return ToLookup<TConcrete>(null);
        }

        public BindingConditionSetter ToLookup<TConcrete>(string identifier) where TConcrete : TContract
        {
            return ToMethod((ctx) => ctx.Container.Resolve<TConcrete>(ctx.ChangeMemberType(typeof(TConcrete)).ChangeId(identifier)));
        }

        public BindingConditionSetter ToMethod(Func<InjectContext, TContract> method)
        {
            return ToProvider(new MethodProvider<TContract>(method));
        }

        public BindingConditionSetter ToGetter<TObj>(Func<TObj, TContract> method)
        {
            return ToMethod((ctx) => method(ctx.Container.Resolve<TObj>()));
        }

        public BindingConditionSetter ToGetter<TObj>(string identifier, Func<TObj, TContract> method)
        {
            return ToMethod((ctx) => method(ctx.Container.Resolve<TObj>(identifier)));
        }
    }
}
