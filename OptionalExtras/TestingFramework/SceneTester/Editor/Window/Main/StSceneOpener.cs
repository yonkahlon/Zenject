using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Zenject;

namespace SceneTester
{
    public class StSceneOpener
    {
        readonly StListModel _listModel;

        public StSceneOpener(StListModel listModel)
        {
            _listModel = listModel;
        }

        public void OpenScene(string sceneName)
        {
            var data = _listModel.GetSceneInfo(sceneName);

            Log.Info("Scene Tester: Starting scene '{0}'", sceneName);

            EditorSceneManager.OpenScene(data.FullPath);
        }
    }
}
