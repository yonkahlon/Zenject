using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    public interface ITickable
    {
        int TickPriority { get; }

        void Tick();
    }
}
