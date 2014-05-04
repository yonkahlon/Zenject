using System;

namespace ModestTree.Zenject
{
    public static class KernelUtil
    {
        public static void BindTickable<TTickable>(DiContainer container, int priority) where TTickable : ITickable
        {
            container.Bind<ITickable>().AsSingle<TTickable>();
            container.Bind<Tuple<Type, int>>().AsSingle(Tuple.New(typeof(TTickable), priority));
        }
    }
}
