using System;
using System.IO;
using ModestTree;
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace ModestTree.Zenject
{
    // Moq.dll has dependencies that are incompatible with webplayer (and probably android/ios/others)
    // but it is very useful to include Moq.dll on PC builds
    // However, unity does not support the ability to link in DLLs on a per-platform basis
    // So instead we are forced to turn on and off the DLL ourselves by deleting and adding back
    // the file whenever the platform changes
    // Not ideal but there doesn't appear to be another option and it works
    public class MoqToggler : AssetPostprocessor
    {
        // TODO: Do something better than this hack
        static readonly string MoqDllDirPath = Application.dataPath + @"\Plugins\ZenjectMoq\";

        static string EnabledFilePath
        {
            get
            {
                return MoqDllDirPath + "Moq.dll";
            }
        }

        static string DisabledFilePath
        {
            get
            {
                return MoqDllDirPath + "Moq.dllx";
            }
        }

        static string EnabledMetaFilePath
        {
            get
            {
                return MoqDllDirPath + "Moq.dll.meta";
            }
        }

        static string DisabledMetaFilePath
        {
            get
            {
                return MoqDllDirPath + "Moq.dll.metax";
            }
        }

        public static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebPlayer)
            {
                DisableMoq();
            }
            else
            {
                EnableMoq();
            }
        }

        static void EnableMoq()
        {
            if (!File.Exists(EnabledFilePath))
            {
                FileUtil.CopyFileOrDirectory(DisabledFilePath, EnabledFilePath);
                FileUtil.CopyFileOrDirectory(DisabledMetaFilePath, EnabledMetaFilePath);
                AssetDatabase.Refresh();
            }
        }

        static void DisableMoq()
        {
            if (File.Exists(EnabledFilePath))
            {
                FileUtil.DeleteFileOrDirectory(EnabledFilePath);
                FileUtil.DeleteFileOrDirectory(EnabledMetaFilePath);
                AssetDatabase.Refresh();
            }
        }
    }
}

