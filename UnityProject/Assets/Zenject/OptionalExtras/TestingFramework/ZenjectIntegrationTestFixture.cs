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
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if (scene.isLoaded)
                        yield return scene;
                }
            }
        }

        protected void UnloadOtherScenes()
        {
            LoadedScenes
                .Except(gameObject.scene)
                .ForEach(x => SceneManager.UnloadScene(x));
        }

        protected Scene GetScene(string sceneName)
        {
            return LoadedScenes.Single(x => x.name == sceneName);
        }

        protected void LoadSceneAdditive(string sceneName)
        {
            var loader = new ZenjectSceneLoader();
            loader.LoadSceneAdditive(sceneName);
        }
    }
}
