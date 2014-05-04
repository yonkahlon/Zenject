using System;

namespace ModestTree.Zenject
{
    public static class KernelUtil
    {
        public static void BindTickable<TTickable>(DiContainer container, int priority) where TTickable : ITickable
        {
            container.Bind<ITickable>().AsSingle<TTickable>();
            BindTickablePriority<TTickable>(container, priority);
        }

        public static void BindTickablePriority<TTickable>(DiContainer container, int priority)
        {
            container.Bind<Tuple<Type, int>>().AsSingle(Tuple.New(typeof(TTickable), priority));
        }
    }
}
