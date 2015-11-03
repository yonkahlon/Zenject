using System;
using System.Collections.Generic;

namespace Zenject
{
    public interface IDependencyRoot : IDisposable
    {
        void Initialize();
        void Tick();
        void LateTick();
        void FixedTick();
    }
}
