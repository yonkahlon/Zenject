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
            int priorityCount = -1 * _typeOrder.Count;

            foreach (var type in _typeOrder)
            {
                BindPriority(Binder, type, priorityCount);
                priorityCount++;
            }
        }

        public static void BindPriority<T>(
            IBinder binder, int priorityCount)
        {
            BindPriority(binder, typeof(T), priorityCount);
        }

        public static void BindPriority(
            IBinder binder, Type type, int priority)
        {
            Assert.That(type.DerivesFrom<ITickable>() || type.DerivesFrom<IInitializable>() || type.DerivesFrom<IDisposable>(),
                "Expected type '{0}' to derive from ITickable, IInitializable, or IDisposable", type.Name());

            if (type.DerivesFrom<ITickable>())
            {
                binder.Bind<ModestTree.Util.Tuple<Type, int>>().ToInstance(
                    ModestTree.Util.Tuple.New(type, priority)).WhenInjectedInto<TickableManager>();
            }

            if (type.DerivesFrom<IInitializable>())
            {
                binder.Bind<ModestTree.Util.Tuple<Type, int>>().ToInstance(
                    ModestTree.Util.Tuple.New(type, priority)).WhenInjectedInto<InitializableManager>();
            }

            if (type.DerivesFrom<IDisposable>())
            {
                binder.Bind<ModestTree.Util.Tuple<Type, int>>().ToInstance(
                    ModestTree.Util.Tuple.New(type, priority)).WhenInjectedInto<DisposableManager>();
            }
        }
    }
}

