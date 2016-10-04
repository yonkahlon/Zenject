#if !NOT_UNITY3D

using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using ModestTree;

namespace Zenject
{
    public class ZenjectSceneLoader
    {
        public void LoadScene(string sceneName)
        {
            LoadScene(sceneName, null);
        }

        public void LoadScene(string sceneName, Action<DiContainer> preBindings)
        {
            LoadScene(sceneName, preBindings, null);
        }

        public void LoadScene(string sceneName, Action<DiContainer> preBindings, Action<DiContainer> postBindings)
        {
            LoadSceneInternal(sceneName, LoadSceneMode.Single, preBindings, postBindings);
        }

        public void LoadSceneAdditive(
            string sceneName)
        {
            LoadSceneAdditive(sceneName, null);
        }

        public void LoadSceneAdditive(
            string sceneName, Action<DiContainer> preBindings)
        {
            LoadSceneAdditive(sceneName, preBindings, null);
        }

        public void LoadSceneAdditive(
            string sceneName, Action<DiContainer> preBindings, Action<DiContainer> postBindings)
        {
            LoadSceneInternal(sceneName, LoadSceneMode.Additive, preBindings, postBindings);
        }

        void LoadSceneInternal(
            string sceneName,
            LoadSceneMode loadMode,
            Action<DiContainer> preBindings,
            Action<DiContainer> postBindings)
        {
            SceneContext.BeforeInstallHooks = preBindings;
            SceneContext.AfterInstallHooks = postBindings;

            Assert.That(Application.CanStreamedLevelBeLoaded(sceneName),
                "Unable to load scene '{0}'", sceneName);

            SceneManager.LoadScene(sceneName, loadMode);

            // It would be nice here to actually verify that the new scene has a SceneContext
            // if we have extra binding hooks, but it doesn't seem like we can do that immediately
            // after calling SceneManager.LoadScene
        }
    }
}

#endif
