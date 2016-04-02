using System;
using ModestTree;

namespace Zenject
{
    public static class ExecutionOrderExtensions
    {
        public static void BindPriority<T>(
            this DiContainer container, int priority)
        {
            container.BindPriority(typeof(T), priority);
        }

        public static void BindPriority(
            this DiContainer container, Type type, int priority)
        {
            Assert.That(type.DerivesFrom<ITickable>() || type.DerivesFrom<IInitializable>() || type.DerivesFrom<IDisposable>() || type.DerivesFrom<IFixedTickable>() || type.DerivesFrom<ILateTickable>(),
                "Expected type '{0}' to derive from one or more of the following interfaces: ITickable, IInitializable, ILateTickable, IFixedTickable, IDisposable", type.Name());

            if (type.DerivesFrom<ITickable>())
            {
                container.BindTickablePriority(type, priority);
            }

            if (type.DerivesFrom<IInitializable>())
            {
                container.BindInitializablePriority(type, priority);
            }

            if (type.DerivesFrom<IDisposable>())
            {
                container.BindDisposablePriority(type, priority);
            }

            if (type.DerivesFrom<IFixedTickable>())
            {
                container.BindFixedTickablePriority(type, priority);
            }

            if (type.DerivesFrom<ILateTickable>())
            {
                container.BindLateTickablePriority(type, priority);
            }
        }

        public static void BindTickablePriority<T>(
            this DiContainer container, int priority)
            where T : ITickable
        {
            container.BindTickablePriority(typeof(T), priority);
        }

        public static void BindTickablePriority(
            this DiContainer container, Type type, int priority)
        {
            Assert.That(type.DerivesFrom<ITickable>(),
                "Expected type '{0}' to derive from ITickable", type.Name());

            container.BindInstance(
                ModestTree.Util.Tuple.New(type, priority)).WhenInjectedInto<TickableManager>();
        }

        public static void BindInitializablePriority<T>(
            this DiContainer container, int priority)
            where T : IInitializable
        {
            container.BindInitializablePriority(typeof(T), priority);
        }

        public static void BindInitializablePriority(
            this DiContainer container, Type type, int priority)
        {
            Assert.That(type.DerivesFrom<IInitializable>(),
                "Expected type '{0}' to derive from IInitializable", type.Name());

            container.BindInstance(
                ModestTree.Util.Tuple.New(type, priority)).WhenInjectedInto<InitializableManager>();
        }

        public static void BindDisposablePriority<T>(
            this DiContainer container, int priority)
            where T : IDisposable
        {
            container.BindDisposablePriority(typeof(T), priority);
        }

        public static void BindDisposablePriority(
            this DiContainer container, Type type, int priority)
        {
            Assert.That(type.DerivesFrom<IDisposable>(),
                "Expected type '{0}' to derive from IDisposable", type.Name());

            container.BindInstance(
                ModestTree.Util.Tuple.New(type, priority)).WhenInjectedInto<DisposableManager>();
        }

        public static void BindFixedTickablePriority<T>(
            this DiContainer container, int priority)
            where T : IFixedTickable
        {
            container.BindFixedTickablePriority(typeof(T), priority);
        }

        public static void BindFixedTickablePriority(
            this DiContainer container, Type type, int priority)
        {
            Assert.That(type.DerivesFrom<IFixedTickable>(),
                "Expected type '{0}' to derive from IFixedTickable", type.Name());

            container.BindInstance(
                "Fixed", ModestTree.Util.Tuple.New(type, priority)).WhenInjectedInto<TickableManager>();
        }

        public static void BindLateTickablePriority<T>(
            this DiContainer container, int priority)
            where T : ILateTickable
        {
            container.BindLateTickablePriority(typeof(T), priority);
        }

        public static void BindLateTickablePriority(
            this DiContainer container, Type type, int priority)
        {
            Assert.That(type.DerivesFrom<ILateTickable>(),
                "Expected type '{0}' to derive from ILateTickable", type.Name());

            container.BindInstance(
                "Late", ModestTree.Util.Tuple.New(type, priority)).WhenInjectedInto<TickableManager>();
        }
    }
}
