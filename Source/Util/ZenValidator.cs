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
        public static IEnumerable<ZenjectResolveException> ValidateCompositionRoot(
            CompositionRoot compRoot, DiContainer container)
        {
            return ValidateCompositionRoot(compRoot, container, new Type[0]);
        }

        public static IEnumerable<ZenjectResolveException> ValidateCompositionRoot(
            CompositionRoot compRoot, DiContainer container, Type[] facadeArgs)
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

            bool foundFacade = false;

            // With the scene we also need to validate all the components
            // that use [Inject] or [PostInject]
            foreach (var component in compRoot.GetInjectableComponents())
            {
                Assert.IsNotNull(component);
                Assert.That(!component.GetType().DerivesFrom<MonoInstaller>());

                if (component.GetType().DerivesFrom<MonoFacade>())
                {
                    if (foundFacade)
                    {
                        yield return new ZenjectResolveException(
                            "Found multiple components of type 'MonoFacade' while validating composition root '{0}'".Fmt(compRoot.name));
                        yield break;
                    }

                    foundFacade = true;

                    foreach (var error in container.Resolver.ValidateObjectGraph(component.GetType(), facadeArgs))
                    {
                        yield return error;
                    }
                }
                else
                {
                    foreach (var error in container.Resolver.ValidateObjectGraph(component.GetType()))
                    {
                        yield return error;
                    }
                }
            }
        }

        static IEnumerable<ZenjectResolveException> ValidateContainer(DiContainer container)
        {
            // First validate the dependency root
            foreach (var error in container.Resolver.ValidateResolve(
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

            foreach (var error in container.Resolver.ValidateValidatables())
            {
                yield return error;
            }
        }
    }
}
#endif

