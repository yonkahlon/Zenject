using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using UnityEngine;
using Fasterflect;

namespace ModestTree.Zenject
{
    public class ZenUtil
    {
        public static void LoadScene(string levelName)
        {
            ZenUtil.LoadScene(levelName, null);
        }

        public static void LoadScene(
            string levelName, Action<DiContainer> extraBindings)
        {
            CompositionRoot.ExtraBindingsLookup = extraBindings;
            Application.LoadLevel(levelName);
        }

        public static void LoadSceneAdditive(string levelName)
        {
            LoadSceneAdditive(levelName, null);
        }

        public static void LoadSceneAdditive(
            string levelName, Action<DiContainer> extraBindings)
        {
            CompositionRoot.ExtraBindingsLookup = extraBindings;
            Application.LoadLevelAdditive(levelName);
        }

        public static IEnumerable<ZenjectResolveException> ValidateInstallers(DiContainer container)
        {
            var uninstalled = container.ResolveMany<IInstaller>();
            var allInstallers = new List<IInstaller>();

            while (!uninstalled.IsEmpty())
            {
                allInstallers.AddRange(uninstalled);

                container.ReleaseBindings<IInstaller>();

                foreach (var installer in uninstalled)
                {
                    installer.InstallBindings();
                }

                uninstalled = container.ResolveMany<IInstaller>();
            }

            foreach (var error in container.ValidateResolve<IDependencyRoot>())
            {
                yield return error;
            }

            foreach (var installer in allInstallers)
            {
                foreach (var error in installer.ValidateSubGraphs())
                {
                    yield return error;
                }
            }
        }
    }
}
