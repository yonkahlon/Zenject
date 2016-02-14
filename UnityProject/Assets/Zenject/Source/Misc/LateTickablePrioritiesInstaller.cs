using System;
using System.Collections.Generic;
using Zenject;
using ModestTree;
using ModestTree.Util;

namespace Zenject
{
    public class LateTickablePrioritiesInstaller : Installer
    {
        List<Type> _tickables;

        public LateTickablePrioritiesInstaller(List<Type> tickables)
        {
            _tickables = tickables;
        }

        public override void InstallBindings()
        {
            // All tickables without explicit priorities assigned are given priority of zero,
            // so put all of these before that (ie. negative)
            int priorityCount = -1 * _tickables.Count;

            foreach (var tickableType in _tickables)
            {
                BindPriority(Binder, tickableType, priorityCount);
                priorityCount++;
            }
        }

        public static void BindPriority<T>(
            IBinder binder, int priorityCount)
            where T : ILateTickable
        {
            BindPriority(binder, typeof(T), priorityCount);
        }

        public static void BindPriority(
            IBinder binder, Type tickableType, int priorityCount)
        {
            Assert.That(tickableType.DerivesFrom<ILateTickable>(),
                "Expected type '{0}' to derive from ILateTickable", tickableType.Name());

            binder.Bind<ModestTree.Util.Tuple<Type, int>>("Late").ToInstance(
                ModestTree.Util.Tuple.New(tickableType, priorityCount)).WhenInjectedInto<TickableManager>();
        }
    }
}
