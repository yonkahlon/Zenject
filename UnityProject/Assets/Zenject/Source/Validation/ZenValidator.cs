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
        public static IEnumerable<ZenjectException> ValidateScene(
            SceneCompositionRoot sceneRoot, GlobalCompositionRoot globalRoot)
        {
            var globalContainer = new DiContainer(true);
            globalContainer.IncludeInactiveDefault = globalRoot.IncludeInactiveComponents;
            globalContainer.IsInstalling = true;
            globalRoot.InstallBindings(globalContainer);

            var sceneContainer = globalContainer.CreateSubContainer();
            sceneContainer.IncludeInactiveDefault = sceneRoot.IncludeInactiveComponents;
            sceneContainer.IsInstalling = true;
            sceneRoot.InstallBindings(sceneContainer);

            globalContainer.IsInstalling = false;
            sceneContainer.IsInstalling = false;

            return ValidateCompositionRoot(globalRoot, globalContainer)
                .Concat(ValidateCompositionRoot(sceneRoot, sceneContainer))
                .Concat(ValidateFacadeRoots(sceneContainer));
        }

        public static IEnumerable<ZenjectException> ValidateFacadeRoots(DiContainer sceneContainer)
        {
            foreach (var facadeRoot in GameObject.FindObjectsOfType<FacadeCompositionRoot>())
            {
                foreach (var error in ValidateFacadeRoot(sceneContainer, facadeRoot))
                {
                    yield return error;
                }
            }
        }

        public static IEnumerable<ZenjectException> ValidateFacadeRoot(
            DiContainer sceneContainer, FacadeCompositionRoot facadeRoot)
        {
            if (facadeRoot.Facade == null)
            {
                yield return new ZenjectException(
                    "Facade property is not set in FacadeCompositionRoot '{0}'", facadeRoot.name);
                yield break;
            }

            if (!UnityUtil.GetParentsAndSelf(facadeRoot.Facade.transform).Contains(facadeRoot.transform))
            {
                yield return new ZenjectException(
                    "The given Facade must exist on the same game object as the FacadeCompositionRoot '{0}' or a descendant!", facadeRoot.name);
                yield break;
            }

            var facadeContainer = sceneContainer.CreateSubContainer();
            facadeContainer.IncludeInactiveDefault = facadeRoot.IncludeInactiveComponents;
            facadeContainer.IsInstalling = true;
            facadeRoot.InstallBindings(facadeContainer, null);
            facadeContainer.IsInstalling = false;

            foreach (var err in ValidateCompositionRoot(facadeRoot, facadeContainer))
            {
                yield return err;
            }
        }

        public static IEnumerable<ZenjectException> ValidateCompositionRoot(
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

        public static IEnumerable<ZenjectException> ValidateChildComponents(
            DiContainer sceneContainer, GameObject gameObject, bool includeInactive)
        {
            return ValidateComponents(
                sceneContainer, CompositionRoot.GetInjectableComponents(gameObject, includeInactive));
        }

        public static IEnumerable<ZenjectException> ValidateComponents(
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

        static IEnumerable<ZenjectException> ValidateContainer(DiContainer container)
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

