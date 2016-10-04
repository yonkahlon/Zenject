using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEditor;
using UnityEngine;

namespace SceneTester
{
    [Serializable]
    public class SceneData
    {
        public string Name;
        public string FullPath;
    }

    // View data that needs to be saved and restored
    public class StListModel
    {
        public event Action Changed = delegate {};

        readonly Data _data;

        public StListModel(Data data)
        {
            _data = data;
        }

        public Vector2 ScrollPos
        {
            get
            {
                return _data.ScrollPos;
            }
            set
            {
                if (_data.ScrollPos != value)
                {
                    _data.ScrollPos = value;
                    Changed();
                }
            }
        }

        public string SearchFilter
        {
            get
            {
                return _data.SearchFilter;
            }
            set
            {
                if (_data.SearchFilter != value)
                {
                    _data.SearchFilter = value;
                    Changed();
                }
            }
        }

        public int SortMethod
        {
            get
            {
                return _data.SortMethod;
            }
            set
            {
                if (_data.SortMethod != value)
                {
                    _data.SortMethod = value;
                    Changed();
                }
            }
        }

        public bool SortDescending
        {
            get
            {
                return _data.SortDescending;
            }
            set
            {
                if (_data.SortDescending != value)
                {
                    _data.SortDescending = value;
                    Changed();
                }
            }
        }

        public List<SceneData> SceneInfos
        {
            get
            {
                return _data.SceneInfos;
            }
            set
            {
                Assert.IsNotNull(value);

                _data.SceneInfos = value;
                Changed();
            }
        }

        public SceneData GetSceneInfo(string name)
        {
            var matches = _data.SceneInfos.Where(x => x.Name == name).ToList();

            Assert.That(matches.Count == 1, "Could not find unique scene with name '{0}'", name);

            return matches[0];
        }

        [Serializable]
        public class Data
        {
            public Vector2 ScrollPos;
            public string SearchFilter = "";

            public int SortMethod;
            public bool SortDescending;

            public List<SceneData> SceneInfos;
        }
    }
}
