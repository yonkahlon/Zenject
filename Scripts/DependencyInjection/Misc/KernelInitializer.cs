using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    public class KernelInitializer : IEntryPoint
    {
        List<ITickable> _queuedTasks;
        IKernel _kernel;

        public KernelInitializer(
            IKernel kernel,
            [InjectOptional]
            List<ITickable> tasks)
        {
            _queuedTasks = tasks;
            _kernel = kernel;
        }

        public int InitPriority
        {
            get
            {
                // Add tasks as early as possible
                return int.MinValue;
            }
        }

        public void Initialize()
        {
            foreach (var task in _queuedTasks)
            {
                _kernel.AddTask(task);
            }
        }
    }
}

