#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using System.Linq;

namespace Zenject
{
    public static class PrefabBasedFactoryUtil
    {
        public static IEnumerable<ZenjectResolveException> Validate(
            DiContainer container, GameObject prefab)
        {
            return Validate(container, prefab, null, null);
        }

        public static IEnumerable<ZenjectResolveException> Validate(
            DiContainer container, GameObject prefab, Type mainType, Type[] paramTypes)
        {
            Assert.IsNotNull(prefab);

            var rootGameObject = GameObject.Instantiate(prefab);

            try
            {
                var onlyInjectWhenActive = container.Resolve<CompositionRoot>().OnlyInjectWhenActive;

                foreach (var component in CompositionRoot.GetInjectableComponents(
                    rootGameObject, onlyInjectWhenActive))
                {
                    Assert.IsNotNull(component);

                    Assert.That(!component.GetType().DerivesFrom<MonoInstaller>());

                    if (mainType != null && component.GetType().DerivesFromOrEqual(mainType))
                    {
                        foreach (var error in container.ValidateObjectGraph(component.GetType(), paramTypes))
                        {
                            yield return error;
                        }
                    }
                    else
                    {
                        foreach (var error in container.ValidateObjectGraph(component.GetType()))
                        {
                            yield return error;
                        }
                    }
                }

                foreach (var facadeRoot in GetFacadeRoots(rootGameObject))
                {
                    foreach (var error in ZenValidator.ValidateFacadeRoot(container, facadeRoot))
                    {
                        yield return error;
                    }
                }
            }
            finally
            {
                GameObject.DestroyImmediate(rootGameObject);
            }
        }

        static IEnumerable<FacadeCompositionRoot> GetFacadeRoots(GameObject gameObject)
        {
            // We don't want to just use GetComponentsInChildren here because
            // we want to ignore the FacadeCompositionRoot's that are inside other
            // FacadeCompositionRoot's
            var facadeRoot = gameObject.GetComponent<FacadeCompositionRoot>();

            if (facadeRoot != null)
            {
                yield return facadeRoot;
                yield break;
            }

            foreach (Transform child in gameObject.transform)
            {
                foreach (var descendantRoot in GetFacadeRoots(child.gameObject))
                {
                    yield return descendantRoot;
                }
            }
        }
    }
}

#endif


