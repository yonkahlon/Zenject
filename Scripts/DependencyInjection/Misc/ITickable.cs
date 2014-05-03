using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    public interface ITickable
    {
        // Return null if you don't care when your tick gets called
        int? TickPriority { get; }

        void Tick();
    }
}
