#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModestTree.Util;
using UnityEditor;
using UnityEngine;
using ModestTree;
using Zenject.Internal;

#if UNITY_5_3
using UnityEditor.SceneManagement;
#endif

namespace Zenject
{
    public static class ZenMenuItems
    {
        [MenuItem("Edit/Zenject/Help...")]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://github.com/modesttree/zenject");
        }

        [MenuItem("Edit/Zenject/Validate All Active Scenes")]
        public static void ValidateAllActiveScenes()
        {
            ZenEditorValidator.ValidateAllActiveScenes();
        }

        [MenuItem("Edit/Zenject/Validate Current Scene #%v")]
        public static void ValidateCurrentScene()
        {
            ZenEditorValidator.ValidateCurrentScene();
        }
    }
}
#endif


