#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ModestTree;

namespace Zenject
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AutoBindAttribute : Attribute
    {
    }

    // To use this, just add [AutoBind] above the monobehaviours that you want to be automatically added to the container
    // Then also call Container.Install<AutoBindInstaller> from another installer
    public class AutoBindInstaller : Installer
    {
        public AutoBindInstaller(CompositionRoot compRoot)
        {
            Assert.That(!(compRoot is GlobalCompositionRoot),
                "You cannot use AutoBindInstaller from within a global installer");
        }

        public override void InstallBindings()
        {
            foreach (var monoBehaviour in SceneCompositionRoot.GetSceneRootObjects()
                .SelectMany(x => x.GetComponentsInChildren<MonoBehaviour>()))
            {
                if (monoBehaviour != null && monoBehaviour.GetType().HasAttribute<AutoBindAttribute>())
                {
                    Container.Bind(monoBehaviour.GetType()).ToInstance(monoBehaviour);
                }
            }
        }
    }
}

#endif
