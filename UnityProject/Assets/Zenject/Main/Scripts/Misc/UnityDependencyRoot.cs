using System.Collections.Generic;
using UnityEngine;

namespace Zenject
{
    public sealed class UnityDependencyRoot : MonoBehaviour, IDependencyRoot
    {
        [Inject]
        TickableManager _tickableManager = null;

        [Inject]
        InitializableManager _initializableManager = null;

        [Inject]
        DisposableManager _disposablesManager = null;

        // For cases where you have game objects that aren't referenced anywhere but still want them to be
        // created on startup
        [InjectOptional]
        public List<MonoBehaviour> _initialObjects = null;

        [PostInject]
        public void Initialize()
        {
            _initializableManager.Initialize();
        }

        public void OnDestroy()
        {
            _disposablesManager.Dispose();
        }

        public void Update()
        {
            _tickableManager.Update();
        }

        public void FixedUpdate()
        {
            _tickableManager.FixedUpdate();
        }

        public void LateUpdate()
        {
            _tickableManager.LateUpdate();
        }
    }
}
