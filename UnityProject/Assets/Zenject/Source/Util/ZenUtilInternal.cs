using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;

#if !NOT_UNITY3D
using UnityEngine.SceneManagement;
using UnityEngine;
#endif

namespace Zenject.Internal
{
    public class ZenUtilInternal
    {
        // Due to the way that Unity overrides the Equals operator,
        // normal null checks such as (x == null) do not always work as
        // expected
        // In those cases you can use this function which will also
        // work with non-unity objects
        public static bool IsNull(System.Object obj)
        {
            return obj == null || obj.Equals(null);
        }

        public static bool AreFunctionsEqual(Delegate left, Delegate right)
        {
            return left.Target == right.Target && left.Method() == right.Method();
        }

#if !NOT_UNITY3D
        public static IEnumerable<SceneContext> GetAllSceneContexts()
        {
            foreach (var scene in UnityUtil.AllLoadedScenes)
            {
                var contexts = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<SceneContext>()).ToList();

                if (contexts.IsEmpty())
                {
                    continue;
                }

                Assert.That(contexts.Count == 1,
                    "Found multiple scene contexts in scene '{0}'", scene.name);

                yield return contexts[0];
            }
        }

        // NOTE: This method will not return components that are within a GameObjectContext
        public static List<MonoBehaviour> GetInjectableComponents(GameObject gameObject)
        {
            var childMonoBehaviours = gameObject.GetComponentsInChildren<MonoBehaviour>();

            var subContexts = childMonoBehaviours.OfType<GameObjectContext>().Select(x => x.transform).ToList();

            // Need to make sure we don't inject on any MonoBehaviour's that are below a GameObjectContext
            // Since that is the responsibility of the GameObjectContext
            // BUT we do want to inject on the GameObjectContext itself
            return childMonoBehaviours.Where(x => x != null && x.transform.GetParents().Intersect(subContexts).IsEmpty()
                    && (x.GetComponent<GameObjectContext>() == null || x is GameObjectContext))
                .ToList();
        }
#endif
    }
}
