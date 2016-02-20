using System;
using System.Collections.Generic;

// Ignore the fact that _initialObjects is not used
#pragma warning disable 414

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public class Facade : IInitializable, IDisposable, ITickable, ILateTickable, IFixedTickable, IDependencyRoot
    {
        [Inject(InjectSources.Local)]
        TickableManager _tickableManager = null;

        [Inject(InjectSources.Local)]
        InitializableManager _initializableManager = null;

        [Inject(InjectSources.Local)]
        DisposableManager _disposablesManager = null;

        // For cases where you have objects that aren't referenced anywhere but still want them to be
        // created on startup
        [InjectOptional(InjectSources.Local)]
        List<object> _initialObjects = null;

        public virtual void Initialize()
        {
            _initializableManager.Initialize();
        }

        public virtual void Dispose()
        {
            _disposablesManager.Dispose();
        }

        public virtual void Tick()
        {
            _tickableManager.Update();
        }

        public virtual void LateTick()
        {
            _tickableManager.LateUpdate();
        }

        public virtual void FixedTick()
        {
            _tickableManager.FixedUpdate();
        }
    }
}
