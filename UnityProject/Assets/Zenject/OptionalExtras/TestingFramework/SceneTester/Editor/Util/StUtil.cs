using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModestTree.Util;
using ModestTree;

namespace SceneTester
{
    public static class StUtil
    {
        public static List<string> GetAllSceneNamesUnderneathFolders(List<string> folderPaths)
        {
            return folderPaths.SelectMany(
                fullPath => Directory.GetFiles(
                    fullPath, "*.unity", SearchOption.AllDirectories)
                .Select(x => Path.GetFileNameWithoutExtension(x)).ToList()).ToHashSet().ToList();
        }
    }
}
