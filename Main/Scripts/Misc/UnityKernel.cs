using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    public class UnityKernel : MonoBehaviour
    {
        public const int LateUpdateTickPriority = 10000;
        public const int OnGuiTickPriority = 20000;

        [Inject]
        [InjectOptional]
        readonly List<ITickable> _tickables;

        [Inject]
        [InjectOptional]
        readonly List<IFixedTickable> _fixedTickables;

        [Inject]
        readonly List<Tuple<Type, int>> _priorities;

        TaskUpdater<ITickable> _updater;
        TaskUpdater<IFixedTickable> _fixedUpdater;

        [PostInject]
        public void Initialize()
        {
            _updater = new TaskUpdater<ITickable>(UpdateTickable);
            _fixedUpdater = new TaskUpdater<IFixedTickable>(UpdateFixedTickable);

            var priorityMap = _priorities.ToDictionary(x => x.First, x => x.Second);

            foreach (var tickable in _tickables)
            {
                int priority;

                if (priorityMap.TryGetValue(tickable.GetType(), out priority))
                {
                    _updater.AddTask(tickable, priority);
                }
                else
                {
                    _updater.AddTask(tickable);
                }
            }

            foreach (var tickable in _fixedTickables)
            {
                int priority;

                if (priorityMap.TryGetValue(tickable.GetType(), out priority))
                {
                    _fixedUpdater.AddTask(tickable, priority);
                }
                else
                {
                    _fixedUpdater.AddTask(tickable);
                }
            }
        }

        void UpdateFixedTickable(IFixedTickable tickable)
        {
            using (ProfileBlock.Start("{0}.FixedTick()".With(tickable.GetType().Name())))
            {
                tickable.FixedTick();
            }
        }

        void UpdateTickable(ITickable tickable)
        {
            using (ProfileBlock.Start("{0}.Tick()".With(tickable.GetType().Name())))
            {
                tickable.Tick();
            }
        }

        public void Add(ITickable tickable)
        {
            _updater.AddTask(tickable);
        }

        public void Add(ITickable tickable, int priority)
        {
            _updater.AddTask(tickable, priority);
        }

        public void AddFixed(IFixedTickable tickable)
        {
            _fixedUpdater.AddTask(tickable);
        }

        public void AddFixed(IFixedTickable tickable, int priority)
        {
            _fixedUpdater.AddTask(tickable, priority);
        }

        public void Remove(ITickable tickable)
        {
            _updater.RemoveTask(tickable);
        }

        public void RemoveFixed(IFixedTickable tickable)
        {
            _fixedUpdater.RemoveTask(tickable);
        }

        public void Update()
        {
            _updater.OnFrameStart();
            _updater.UpdateRange(int.MinValue, LateUpdateTickPriority);

            // Put Tickables with unspecified priority after Update() and before LateUpdate()
            _updater.UpdateUnsorted();
        }

        public void LateUpdate()
        {
            _updater.UpdateRange(LateUpdateTickPriority, OnGuiTickPriority);
        }

        public void OnGUI()
        {
            _updater.UpdateRange(OnGuiTickPriority, int.MaxValue);
        }

        public void FixedUpdate()
        {
            _fixedUpdater.OnFrameStart();
            _fixedUpdater.UpdateAll();
        }
    }
}
