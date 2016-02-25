#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModestTree.Util;
using UnityEngine;
using ModestTree;
using Zenject.Internal;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Zenject
{
    public static class ZenValidator
    {
        public static IEnumerable<ZenjectResolveException> ValidateScene(
            SceneCompositionRoot sceneRoot, GlobalCompositionRoot globalRoot)
        {
            var globalContainer = new DiContainer(true);
            globalRoot.InstallBindings(globalContainer);

            var sceneContainer = new DiContainer(globalContainer, true);
            sceneRoot.InstallBindings(sceneContainer);

            return ValidateCompositionRoot(globalRoot, globalContainer)
                .Concat(ValidateCompositionRoot(sceneRoot, sceneContainer))
                .Concat(ValidateFacadeRoots(sceneContainer));
        }

        public static IEnumerable<ZenjectResolveException> ValidateFacadeRoots(DiContainer sceneContainer)
        {
            foreach (var facadeRoot in GameObject.FindObjectsOfType<FacadeCompositionRoot>())
            {
                foreach (var error in ValidateFacadeRoot(sceneContainer, facadeRoot))
                {
                    yield return error;
                }
            }
        }

        public static IEnumerable<ZenjectResolveException> ValidatePrefab(
            DiContainer container, GameObject prefab, Type componentType, InjectContext context)
        {
            Assert.IsNotNull(prefab);

            bool includeInactive = !container.Resolve<CompositionRoot>().OnlyInjectWhenActive;

            if (prefab.GetComponentsInChildren(componentType, includeInactive).IsEmpty())
            {
                yield return new ZenjectResolveException(
                    "Could not find component of type '{0}' in prefab '{1}' \nObject graph:\n{2}"
                    .Fmt(componentType.Name(), prefab.name, context == null ? "(unknown)" : context.GetObjectGraphString()));
                yield break;
            }

            if (componentType.IsAbstract)
            {
                // In most cases componentType will be a MonoBehaviour but we also want to allow interfaces
                // And in that case we can't validate the implementing MonoBehaviour
                foreach (var error in MonoBehaviourFactoryUtil.Validate(container, prefab))
                {
                    yield return error;
                }
            }
            else
            {
                foreach (var error in MonoBehaviourFactoryUtil.Validate(container, prefab, componentType, new Type[0]))
                {
                    yield return error;
                }
            }
        }

        public static IEnumerable<ZenjectResolveException> ValidateFacadeRoot(
            DiContainer sceneContainer, FacadeCompositionRoot facadeRoot)
        {
            if (facadeRoot.Facade == null)
            {
                yield return new ZenjectResolveException(
                    "Facade property is not set in FacadeCompositionRoot '{0}'".Fmt(facadeRoot.name));
                yield break;
            }

            if (!UnityUtil.GetParentsAndSelf(facadeRoot.Facade.transform).Contains(facadeRoot.transform))
            {
                yield return new ZenjectResolveException(
                    "The given Facade must exist on the same game object as the FacadeCompositionRoot '{0}' or a descendant!".Fmt(facadeRoot.name));
                yield break;
            }

            var facadeContainer = new DiContainer(sceneContainer, true);
            facadeRoot.InstallBindings(facadeContainer, null);

            foreach (var err in ValidateCompositionRoot(facadeRoot, facadeContainer))
            {
                yield return err;
            }
        }

        public static IEnumerable<ZenjectResolveException> ValidateCompositionRoot(
            CompositionRoot compRoot, DiContainer container)
        {
#if UNITY_EDITOR
            // We check the prefab types again here even though we did this already in ZenEditorValidator, so that
            // they are triggered for FacadeCompositionRoot validation as well
            // This only works for non-dll builds though but the best we can do right now
            foreach (var installer in compRoot.Installers)
            {
                Assert.That(PrefabUtility.GetPrefabType(installer.gameObject) != PrefabType.Prefab,
                    "Found prefab with name '{0}' in the Installer property of CompositionRoot '{1}'.  You should use the property 'InstallerPrefabs' for this instead.", installer.name, compRoot.name);
            }

            foreach (var installer in compRoot.InstallerPrefabs)
            {
                Assert.That(PrefabUtility.GetPrefabType(installer.gameObject) == PrefabType.Prefab,
                    "Found non-prefab with name '{0}' in the InstallerPrefabs property of CompositionRoot '{1}'.  You should use the property 'Installer' for this instead", installer.name, compRoot.name);
            }
#endif

            foreach (var error in ValidateContainer(container))
            {
                yield return error;
            }

            foreach (var error in ValidateComponents(container, compRoot.GetInjectableComponents()))
            {
                yield return error;
            }
        }

        public static IEnumerable<ZenjectResolveException> ValidateChildComponents(
            DiContainer sceneContainer, GameObject gameObject, bool onlyInjectWhenActive)
        {
            return ValidateComponents(
                sceneContainer, CompositionRoot.GetInjectableComponents(gameObject, onlyInjectWhenActive));
        }

        public static IEnumerable<ZenjectResolveException> ValidateComponents(
            DiContainer container, IEnumerable<Component> components)
        {
            // With the scene we also need to validate all the components
            // that use [Inject] or [PostInject]
            foreach (var component in components)
            {
                Assert.IsNotNull(component);
                Assert.That(!component.GetType().DerivesFrom<MonoInstaller>());

                foreach (var error in container.ValidateObjectGraph(component.GetType()))
                {
                    yield return error;
                }
            }
        }

        static IEnumerable<ZenjectResolveException> ValidateContainer(DiContainer container)
        {
            // First validate the dependency root
            foreach (var error in container.ValidateResolve(
                new InjectContext(container, typeof(IDependencyRoot), null)))
            {
                yield return error;
            }

            // Then validate all objects in the container that implement the IValidatable interface
            foreach (var installer in container.InstalledInstallers.OfType<IValidatable>())
            {
                foreach (var error in installer.Validate())
                {
                    yield return error;
                }
            }

            foreach (var error in container.ValidateValidatables())
            {
                yield return error;
            }
        }
    }
}
#endif

