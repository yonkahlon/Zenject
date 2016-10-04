using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace SceneTester
{
    public class StModel
    {
        public event Action CurrentSceneChanged = delegate {};

        readonly StListModel _list;
        readonly Data _data;

        public StModel(
            Data data,
            StListModel list)
        {
            _list = list;
            _data = data;
        }

        public bool ExitAfter
        {
            get
            {
                return _data.ExitAfter;
            }
            set
            {
                _data.ExitAfter = value;
            }
        }

        public StListModel List
        {
            get
            {
                return _list;
            }
        }

        public bool IsRunningTest
        {
            get
            {
                return _data.IsRunningTest;
            }
            set
            {
                _data.IsRunningTest = value;
            }
        }

        public bool IsValidating
        {
            get
            {
                return _data.IsValidating;
            }
            set
            {
                _data.IsValidating = value;
            }
        }

        public string CurrentScene
        {
            get
            {
                return _data.CurrentScene;
            }
            set
            {
                if (_data.CurrentScene != value)
                {
                    _data.CurrentScene = value;
                    CurrentSceneChanged();
                }
            }
        }

        public IEnumerable<string> QueuedScenes
        {
            get
            {
                return _data.QueuedScenes;
            }
            set
            {
                _data.QueuedScenes = value.ToList();
            }
        }

        public string DequeueScene()
        {
            Assert.That(!_data.QueuedScenes.IsEmpty());

            var first = _data.QueuedScenes.First();
            _data.QueuedScenes.RemoveAt(0);
            return first;
        }

        [Serializable]
        public class Data
        {
            public bool IsValidating;
            public bool IsRunningTest;
            public bool ExitAfter;
            public string CurrentScene;
            public List<string> QueuedScenes;

            public StListModel.Data List;
        }
    }
}
