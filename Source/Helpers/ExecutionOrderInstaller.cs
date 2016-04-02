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
                Container.BindPriority(type, priority);
                priority++;
            }
        }
    }
}

