using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModestTree.Zenject
{
    // Responsibilities:
    // - Run Initialize() on all IEntryPoint's, in the order specified by InitPriority
    public class EntryPointInitializer
    {
        List<IEntryPoint> _entryPoints;
        List<IInitializable> _initializables;

        public EntryPointInitializer(
            List<IEntryPoint> entryPoints,
            [InjectOptional]
            List<IInitializable> initializables)
        {
            _entryPoints = entryPoints;
            _initializables = initializables;
        }

        public void Initialize()
        {
            InitEntryPoints();
            InitInitializables();
        }

        int SortCompare(IEntryPoint e1, IEntryPoint e2)
        {
            // Initialize entry points with null priorities last
            if (!e1.InitPriority.HasValue)
            {
                return 1;
            }

            if (!e2.InitPriority.HasValue)
            {
                return -1;
            }

            return e1.InitPriority.Value.CompareTo(e2.InitPriority.Value);
        }

        void InitEntryPoints()
        {
            _entryPoints.Sort(SortCompare);

            if (Assert.IsEnabled)
            {
                foreach (var item in FindDuplicates(_entryPoints))
                {
                    Assert.That(false, "Found duplicate IEntryPoint with type '" + item.GetType() + "'");
                }
            }

            bool foundNull = false;

            foreach (var entryPoint in _entryPoints)
            {
                if (!entryPoint.InitPriority.HasValue)
                {
                    foundNull = true;
                }
                else
                {
                    Assert.That(!foundNull);
                }

                Log.Debug("Initializing entry point with type '" + entryPoint.GetType() + "'");

                try
                {
                    entryPoint.Initialize();
                }
                catch (Exception e)
                {
                    throw new ZenjectGeneralException(
                        "Error occurred while initializing entry point with type '" + entryPoint.GetType().GetPrettyName() + "'", e);
                }
            }
        }

        void InitInitializables()
        {
            foreach (var initializable in _initializables)
            {
                initializable.Initialize();
            }
        }

        static IEnumerable<IEntryPoint> FindDuplicates(List<IEntryPoint> entryPoints)
        {
            return entryPoints.GroupBy(x => x).Where(x => x.Skip(1).Any()).Select(x => x.Key);
        }
    }
}
