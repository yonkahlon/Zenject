using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModestTree.Zenject
{
    // Responsibilities:
    // - Run Initialize() on all IEntryPoint's, in the order specified by InitPriority
    public class EntryPointInitializer : ITickable
    {
        public const int UnityStartInitPriority = 10000;

        [Inject]
        public List<IEntryPoint> _entryPoints;

        IKernel _kernel;

        List<IEntryPoint> _entryPointsEarly = new List<IEntryPoint>();
        List<IEntryPoint> _entryPointsLate = new List<IEntryPoint>();
        List<IInitializable> _initializables;

        public EntryPointInitializer(
            List<IEntryPoint> entryPoints,
            [InjectOptional]
            List<IInitializable> initializables,
            IKernel kernel)
        {
            _kernel = kernel;
            _entryPoints = entryPoints;
            _initializables = initializables ?? new List<IInitializable>();
        }

        public int? TickPriority
        {
            // We need to be the first tickable that's run, since
            // initialization should always occur before any update call
            // so use the minimum priority to ensure this is the case
            get {  return int.MinValue; }
        }

        void TriggerEntryPoints()
        {
            foreach (var entryPoint in _entryPointsEarly)
            {
                Log.Debug("Initializing entry point with type '" + entryPoint.GetType() + "'");

                try
                {
                    entryPoint.Initialize();
                }
                catch (Exception e)
                {
                    throw new Exception(
                        "Error occurred while initializing entry point with type '" + entryPoint.GetType().GetPrettyName() + "'", e);
                }
            }
        }

        public void Initialize()
        {
            InitEntryPoints();
            TriggerEntryPoints();

            _kernel.AddTask(this);
        }

        void InitEntryPoints()
        {
            _entryPoints.Sort(delegate (IEntryPoint e1, IEntryPoint e2)
                    {
                        return (e1.InitPriority.CompareTo(e2.InitPriority));
                    });

            foreach (var item in FindDuplicates(_entryPoints))
            {
                Assert.That(false, "Found duplicate IEntryPoint with type '" + item.GetType() + "'");
            }

            foreach (var entryPoint in _entryPoints)
            {
                if (entryPoint.InitPriority < UnityStartInitPriority)
                {
                    _entryPointsEarly.Add(entryPoint);
                }
                else
                {
                    _entryPointsLate.Add(entryPoint);
                }
            }
        }

        static IEnumerable<IEntryPoint> FindDuplicates(List<IEntryPoint> entryPoints)
        {
            return entryPoints.GroupBy(x => x).Where(x => x.Skip(1).Any()).Select(x => x.Key);
        }

        // Do late initialize in Update() to ensure that all monobehavior Start() methods have
        // been called
        public void Tick()
        {
            foreach (var entryPoint in _entryPointsLate)
            {
                Log.Debug("Initializing entry point with type '" + entryPoint.GetType() + "'");
                entryPoint.Initialize();
            }

            foreach (var initializable in _initializables)
            {
                initializable.Initialize();
            }

            _kernel.RemoveTask(this);
        }
    }
}
