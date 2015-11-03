#if !ZEN_NOT_UNITY3D
using ModestTree;
using ModestTree.Util;
using UnityEngine;

namespace Zenject
{
    public abstract class CompositionRoot : MonoBehaviour
    {
        bool _isDisposed;

        public abstract DiContainer Container
        {
            get;
        }

        public abstract IDependencyRoot DependencyRoot
        {
            get;
        }

        public void Awake()
        {
            Assert.IsNull(Container);
            Assert.IsNull(DependencyRoot);

            Initialize();

            Assert.IsNotNull(Container);
            Assert.IsNotNull(DependencyRoot);
        }

        public void OnApplicationQuit()
        {
            // In some cases we have monobehaviour's that are bound to IDisposable, and who have
            // also been set with Application.DontDestroyOnLoad so that the Dispose() is always
            // called instead of OnDestroy.  This is nice because we can actually reliably predict the
            // order Dispose() is called in which is not the case for OnDestroy.
            // However, when the user quits the app, OnDestroy is called even for objects that
            // have been marked with Application.DontDestroyOnLoad, and so the destruction order
            // changes.  So to address this case, dispose before the OnDestroy event below (OnApplicationQuit
            // is always called before OnDestroy) and then don't call dispose in OnDestroy
            Assert.IsNotNull(!_isDisposed);
            DependencyRoot.Dispose();
            _isDisposed = true;
        }

        public void OnDestroy()
        {
            // See comment in OnApplicationQuit
            if (!_isDisposed)
            {
                _isDisposed = true;
                DependencyRoot.Dispose();
            }
        }

        public void Update()
        {
            DependencyRoot.Tick();
        }

        public void FixedUpdate()
        {
            DependencyRoot.FixedTick();
        }

        public void LateUpdate()
        {
            DependencyRoot.LateTick();
        }

        protected abstract void Initialize();
    }
}

#endif
