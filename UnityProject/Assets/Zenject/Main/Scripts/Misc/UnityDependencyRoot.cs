using System.Collections.Generic;
using UnityEngine;

namespace ModestTree.Zenject
{
    public sealed class UnityDependencyRoot : MonoBehaviour, IDependencyRoot
    {
        [Inject]
        public TickableHandler _tickableHandler;

        [Inject]
        public InitializableHandler _initializableHandler;

        [Inject]
        public DisposablesHandler _disposablesHandler;

        [PostInject]
        public void Initialize()
        {
            _initializableHandler.Initialize();
        }

        public void OnDestroy()
        {
            _disposablesHandler.Dispose();
        }

        public void Update()
        {
            _tickableHandler.Update();
        }

        public void FixedUpdate()
        {
            _tickableHandler.FixedUpdate();
        }
    }
}
