using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using ModestTree;

namespace Zenject.TestFramework
{
    public abstract class ZenjectIntegrationTestFixture : MonoBehaviour
    {
        public DiContainer Container
        {
            get;
            set;
        }

        protected IEnumerable<Scene> LoadedScenes
        {
            get
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                    yield return SceneManager.GetSceneAt(i);
            }
        }

        protected void UnloadOtherScenes()
        {
            LoadedScenes
                .Except(gameObject.scene)
				.Where(x => x.isLoaded)
                .ForEach(x => SceneManager.UnloadScene(x));
        }

        protected void LoadSceneAdditive(string name)
        {
            var loader = new ZenjectSceneLoader();
            loader.LoadSceneAdditive(name);
        }
    }
}
