#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ModestTree;

namespace Zenject
{
    // To use this, just add [AutoBind] above the monobehaviours that you want to be automatically added to the container
    // Then also call Container.Install<AutoBindInstaller> from another installer
    public class AutoBindInstaller : Installer
    {
        readonly CompositionRoot _compRoot;

        public AutoBindInstaller(CompositionRoot compRoot)
        {
            Assert.That(!(compRoot is GlobalCompositionRoot),
                "You cannot use AutoBindInstaller from within a global installer");

            _compRoot = compRoot;
        }

        public override void InstallBindings()
        {
            foreach (var monoBehaviour in SceneCompositionRoot.GetSceneRootObjects(_compRoot.gameObject.scene)
                .SelectMany(x => x.GetComponentsInChildren<MonoBehaviour>()))
            {
                if (monoBehaviour == null)
                {
                    continue;
                }

                var autoBindAttribute = monoBehaviour.GetType().AllAttributes<AutoBindAttributeBase>().SingleOrDefault();

                if (autoBindAttribute != null)
                {
                    var bindType = autoBindAttribute.BindType;

                    if (bindType == AutoBindTypes.Self
                        || bindType == AutoBindTypes.All)
                    {
                        Container.Bind(monoBehaviour.GetType()).ToInstance(monoBehaviour);
                    }

                    if (bindType == AutoBindTypes.Interfaces
                        || bindType == AutoBindTypes.All)
                    {
                        Container.BindAllInterfacesToInstance(monoBehaviour);
                    }
                }
            }
        }
    }
}

#endif
