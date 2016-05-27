using System;

namespace Zenject
{
    public interface IFacade : IInitializable, IDisposable, ITickable, ILateTickable, IFixedTickable
    {
    }
}
