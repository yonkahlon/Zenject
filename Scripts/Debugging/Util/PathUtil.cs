using System;
using System.Collections.Generic;
using System.IO;

namespace ModestTree
{
    public static class PathUtil
    {
#if UNITY_EDITOR || !UNITY_WEBPLAYER
        public static bool IsSubPath(string parent, string child)
        {
            // call Path.GetFullPath to Make sure we're using Path.DirectorySeparatorChar
            parent = Path.GetFullPath(parent);
            child = Path.GetFullPath(child);

            return child.StartsWith(parent);
        }

        public static string GetRelativePath(string fromDirectory, string toPath)
        {
            Assert.IsNotNull(toPath);
            Assert.IsNotNull(fromDirectory);

            // call Path.GetFullPath to Make sure we're using Path.DirectorySeparatorChar
            fromDirectory = Path.GetFullPath(fromDirectory);
            toPath = Path.GetFullPath(toPath);

            bool isRooted = (Path.IsPathRooted(fromDirectory) && Path.IsPathRooted(toPath));

            if (isRooted)
            {
                bool isDifferentRoot = (string.Compare(Path.GetPathRoot(fromDirectory), Path.GetPathRoot(toPath), true) != 0);

                if (isDifferentRoot)
                {
                    return toPath;
                }
            }

            List<string> relativePath = new List<string>();
            string[] fromDirectories = fromDirectory.Split(Path.DirectorySeparatorChar);

            string[] toDirectories = toPath.Split(Path.DirectorySeparatorChar);

            int length = Math.Min(fromDirectories.Length, toDirectories.Length);

            int lastCommonRoot = -1;

            // find common root
            for (int x = 0; x < length; x++)
            {
                if (string.Compare(fromDirectories [x], toDirectories [x], true) != 0)
                {
                    break;
                }

                lastCommonRoot = x;
            }

            if (lastCommonRoot == -1)
                return toPath;

            // add relative folders in from path
            for (int x = lastCommonRoot + 1; x < fromDirectories.Length; x++)
            {
                if (fromDirectories [x].Length > 0)
                {
                    relativePath.Add("..");
                }
            }

            // add to folders to path
            for (int x = lastCommonRoot + 1; x < toDirectories.Length; x++)
            {
                relativePath.Add(toDirectories [x]);
            }

            // create relative path
            string[] relativeParts = new string[relativePath.Count];
            relativePath.CopyTo(relativeParts, 0);

            return string.Join(Path.DirectorySeparatorChar.ToString(), relativeParts);
        }
#endif
    }
}
