using System;
using System.Collections.Generic;
using ModestTree.Util;
using UnityEngine;
using Zenject;

namespace SceneTester
{
    [CreateAssetMenu(fileName = "Untitled", menuName = "StWindowInstaller", order = 1)]
    public class StWindowInstaller : ScriptableObjectInstaller<StWindowInstaller>
    {
        [SerializeField]
        Settings _settings = null;

        public override void InstallBindings()
        {
            Container.BindAllInterfaces<StMainInitializer>().To<StMainInitializer>().AsSingle();
            Container.Bind<StListView>().AsSingle();

            Container.BindAllInterfacesAndSelf<StView>().To<StView>().AsSingle();

            Container.Bind<StModel>().AsSingle();
            Container.Bind<StListModel>().AsSingle();

            Container.BindInstance(_settings.StList);
            Container.BindInstance(_settings.StView);

            Container.BindCommand<StCommands.OpenContextMenu>()
                .To<StContextMenuHandler>(x => x.Open).AsSingle();

            Container.BindCommand<StCommands.StartSelectedScenes>()
                .To<StSceneRunner>(x => x.StartScenes).AsSingle();

            Container.BindCommand<StCommands.ValidateSelectedScenes>()
                .To<StSceneRunner>(x => x.ValidateScenes).AsSingle();

            Container.BindCommand<StCommands.SceneOpener, string>()
                .To<StSceneOpener>(x => x.OpenScene).AsSingle();

            Container.Bind<StSceneRunner>().AsSingle();
            Container.Bind<ITickable>().To<StSceneRunner>().AsSingle();

            Container.BindAllInterfaces<StListController>().To<StListController>().AsSingle();
        }

        [Serializable]
        public class Settings
        {
            public StListView.Settings StList;
            public StView.Settings StView;
        }
    }
}
