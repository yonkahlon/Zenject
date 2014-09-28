using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    public sealed class GlobalInstallerConfig : ScriptableObject
    {
        static GlobalInstallerConfig _instance;

        // We can refer directly to the prefabs in this case because the properties of the installers should not change
        // You could do the same for the scene composition root installers BUT this is error prone since the prefab
        // may change at run time (for eg. if another scene injects a property into it)
        public MonoInstaller[] Installers;

        static GlobalInstallerConfig()
        {
            _instance = (GlobalInstallerConfig)Resources.Load("ZenjectGlobalInstallers", typeof(GlobalInstallerConfig));
            Assert.IsNotNull(_instance);
        }

        public static GlobalInstallerConfig Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
