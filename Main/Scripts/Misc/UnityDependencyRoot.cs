using System.Collections.Generic;
using UnityEngine;

namespace Zenject
{
    public sealed class UnityDependencyRoot : MonoBehaviour, IDependencyRoot
    {
        [Inject]
        TickableManager _tickableManager;

        [Inject]
        InitializableManager _initializableManager;

        [Inject]
        DisposableManager _disposablesManager;

        // For cases where you have game objects that aren't referenced anywhere but still want them to be
        // created on startup
        [InjectOptional]
        List<MonoBehaviour> _initialObjects;

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
