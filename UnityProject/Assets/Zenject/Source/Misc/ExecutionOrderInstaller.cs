using System;
using System.Collections.Generic;
using Zenject;
using ModestTree;
using ModestTree.Util;

namespace Zenject
{
    public class ExecutionOrderInstaller : Installer
    {
        List<Type> _typeOrder;

        public ExecutionOrderInstaller(List<Type> typeOrder)
        {
            _typeOrder = typeOrder;
        }

        public override void InstallBindings()
        {
            // All tickables without explicit priorities assigned are given priority of zero,
            // so put all of these before that (ie. negative)
            int priority = -1 * _typeOrder.Count;

            foreach (var type in _typeOrder)
            {
                BindPriority(Container, type, priority);
                priority++;
            }
        }

        public static void BindPriority<T>(
            DiContainer container, int priority)
        {
            BindPriority(container, typeof(T), priority);
        }

        public static void BindPriority(
            DiContainer container, Type type, int priority)
        {
            Assert.That(type.DerivesFrom<ITickable>() || type.DerivesFrom<IInitializable>() || type.DerivesFrom<IDisposable>() || type.DerivesFrom<IFixedTickable>() || type.DerivesFrom<ILateTickable>(),
                "Expected type '{0}' to derive from one or more of the following interfaces: ITickable, IInitializable, ILateTickable, IFixedTickable, IDisposable", type.Name());

            if (type.DerivesFrom<ITickable>())
            {
                BindTickablePriority(container, type, priority);
            }

            if (type.DerivesFrom<IInitializable>())
            {
                BindInitializablePriority(container, type, priority);
            }

            if (type.DerivesFrom<IDisposable>())
            {
                BindDisposablePriority(container, type, priority);
            }

            if (type.DerivesFrom<IFixedTickable>())
            {
                BindFixedTickablePriority(container, type, priority);
            }

            if (type.DerivesFrom<ILateTickable>())
            {
                BindLateTickablePriority(container, type, priority);
            }
        }

        public static void BindTickablePriority<T>(
            DiContainer container, int priority)
            where T : ITickable
        {
            BindTickablePriority(container, typeof(T), priority);
        }

        public static void BindTickablePriority(
            DiContainer container, Type type, int priority)
        {
            Assert.That(type.DerivesFrom<ITickable>(),
                "Expected type '{0}' to derive from ITickable", type.Name());

            container.BindInstance(
                ModestTree.Util.Tuple.New(type, priority)).WhenInjectedInto<TickableManager>();
        }

        public static void BindInitializablePriority<T>(
            DiContainer container, int priority)
            where T : IInitializable
        {
            BindInitializablePriority(container, typeof(T), priority);
        }

        public static void BindInitializablePriority(
            DiContainer container, Type type, int priority)
        {
            Assert.That(type.DerivesFrom<IInitializable>(),
                "Expected type '{0}' to derive from IInitializable", type.Name());

            container.BindInstance(
                ModestTree.Util.Tuple.New(type, priority)).WhenInjectedInto<InitializableManager>();
        }

        public static void BindDisposablePriority<T>(
            DiContainer container, int priority)
            where T : IDisposable
        {
            BindDisposablePriority(container, typeof(T), priority);
        }

        public static void BindDisposablePriority(
            DiContainer container, Type type, int priority)
        {
            Assert.That(type.DerivesFrom<IDisposable>(),
                "Expected type '{0}' to derive from IDisposable", type.Name());

            container.BindInstance(
                ModestTree.Util.Tuple.New(type, priority)).WhenInjectedInto<DisposableManager>();
        }

        public static void BindFixedTickablePriority<T>(
            DiContainer container, int priority)
            where T : IFixedTickable
        {
            BindFixedTickablePriority(container, typeof(T), priority);
        }

        public static void BindFixedTickablePriority(
            DiContainer container, Type type, int priority)
        {
            Assert.That(type.DerivesFrom<IFixedTickable>(),
                "Expected type '{0}' to derive from IFixedTickable", type.Name());

            container.BindInstance(
                "Fixed", ModestTree.Util.Tuple.New(type, priority)).WhenInjectedInto<TickableManager>();
        }

        public static void BindLateTickablePriority<T>(
            DiContainer container, int priority)
            where T : ILateTickable
        {
            BindLateTickablePriority(container, typeof(T), priority);
        }

        public static void BindLateTickablePriority(
            DiContainer container, Type type, int priority)
        {
            Assert.That(type.DerivesFrom<ILateTickable>(),
                "Expected type '{0}' to derive from ILateTickable", type.Name());

            container.BindInstance(
                "Late", ModestTree.Util.Tuple.New(type, priority)).WhenInjectedInto<TickableManager>();
        }
    }
}

