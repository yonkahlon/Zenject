using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // This class can be bound to IKernel in cases where you want complete
    // control over when all tasks are updated
    // Useful in cases where you are running a sub-container
    public class KernelCustom : IKernel
    {
        public const int UnityLateUpdateTickPriority = 10000;
        public const int UnityOnGuiTickPriority = 20000;

        LinkedList<TickableInfo> _tasks = new LinkedList<TickableInfo>();
        List<ITickable> _queuedTasks = new List<ITickable>();

        public void AddTask(ITickable task)
        {
            Assert.That(!_queuedTasks.Contains(task), "Duplicate task added to kernel with name '" + task.GetType().FullName + "'");
            Assert.That(!_tasks.Any(t => ReferenceEquals(t.Tickable, task)), "Duplicate task added to kernel with name '" + task.GetType().FullName + "'");

            // Wait until next frame to add the task, otherwise whether it gets updated
            // on the current frame depends on where in the update order it was added
            // from, so you might get off by one frame issues
            _queuedTasks.Add(task);
        }

        public void RemoveTask(ITickable task)
        {
            var info = _tasks.Where(i => ReferenceEquals(i.Tickable, task)).Single();

            Assert.That(!info.IsRemoved, "Tried to remove task twice, task = " + task.GetType().Name);
            info.IsRemoved = true;
        }

        public void Update(int minPriority, int maxPriority)
        {
            AddQueuedTasks();

            foreach (var taskInfo in _tasks)
            {
                if (taskInfo.Tickable.TickPriority >= minPriority && taskInfo.Tickable.TickPriority < maxPriority)
                {
                    LogUtil.CallAndCatchExceptions(() => taskInfo.Tickable.Tick());
                }
            }

            ClearRemovedTasks();
        }

        void ClearRemovedTasks()
        {
            var node = _tasks.First;

            while (node != null)
            {
                var next = node.Next;
                var info = node.Value;

                if (info.IsRemoved)
                {
                    Log.Debug("Removed task '" + info.Tickable.GetType().ToString() + "'");
                    _tasks.Remove(node);
                }

                node = next;
            }
        }

        void AddQueuedTasks()
        {
            foreach (var task in _queuedTasks)
            {
                InsertTaskSorted(task);
            }
            _queuedTasks.Clear();
        }

        void InsertTaskSorted(ITickable task)
        {
            var newInfo = new TickableInfo(task);

            for (var current = _tasks.First; current != null; current = current.Next)
            {
                if (current.Value.Tickable.TickPriority > task.TickPriority)
                {
                    _tasks.AddBefore(current, newInfo);
                    return;
                }
            }

            _tasks.AddLast(newInfo);
        }

        class TickableInfo
        {
            public ITickable Tickable;
            public bool IsRemoved;

            public TickableInfo(ITickable tickable)
            {
                Tickable = tickable;
            }
        }
    }
}
