#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using System.Linq;

namespace Zenject
{
    public static class PrefabValidator
    {
        public static IEnumerable<ZenjectException> ValidatePrefab(
            DiContainer container, GameObject prefab,
            List<Type> extraArgs, bool useAllArgs)
        {
            Assert.IsNotNull(prefab);

            var rootGameObject = GameObject.Instantiate(prefab);

            try
            {
                foreach (var component in CompositionRoot.GetInjectableComponents(
                    rootGameObject, container.IncludeInactiveDefault))
                {
                    Assert.IsNotNull(component);

                    Assert.That(!component.GetType().DerivesFrom<MonoInstaller>());

                    foreach (var error in container.ValidateObjectGraph(
                        component.GetType(), extraArgs, false))
                    {
                        yield return error;
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

            if (useAllArgs && !extraArgs.IsEmpty())
            {
                yield return new ZenjectException(
                    "Found unused arguments while injecting into prefab with name '{0}'",
                    prefab.name);
            }
        }

        public static IEnumerable<ZenjectException> ValidatePrefabForComponent(
            DiContainer container, GameObject prefab, InjectValidationArgs args)
        {
            Assert.IsNotNull(prefab);

            var rootGameObject = GameObject.Instantiate(prefab);

            var mainType = args.TypeInfo.Type;

            bool foundMainType = false;

            try
            {
                foreach (var component in CompositionRoot.GetInjectableComponents(
                    rootGameObject, container.IncludeInactiveDefault))
                {
                    Assert.IsNotNull(component);

                    Assert.That(!component.GetType().DerivesFrom<MonoInstaller>());

                    if (component.GetType().DerivesFromOrEqual(mainType))
                    {
                        if (foundMainType)
                        {
                            yield return new ZenjectException(
                                "Found multiple components of type '{0}' on prefab '{1}'", mainType.Name, prefab.name);
                            yield break;
                        }

                        foundMainType = true;

                        foreach (var error in container.ValidateObjectGraph(args))
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

                if (!foundMainType)
                {
                    yield return new ZenjectException(
                        "Could not find component of type '{0}' on prefab '{1}'", mainType.Name, prefab.name);
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


