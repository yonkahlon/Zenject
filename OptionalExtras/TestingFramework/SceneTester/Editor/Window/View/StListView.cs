using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEditor;
using UnityEngine;

namespace SceneTester
{
    public class StListView
    {
        public event Action SortDescendingToggled = delegate {};
        public event Action<int> SortMethodSelected = delegate {};
        public event Action<string> SearchFilterChanged = delegate {};
        public event Action<Vector2> ScrollPositionChanged = delegate {};
        public event Action ContextMenuOpenRequested = delegate {};

        readonly List<ListEntry> _entries = new List<ListEntry>();
        readonly Settings _settings;
        readonly List<string> _sortMethodCaptions = new List<string>();

        public StListView(Settings settings)
        {
            _settings = settings;

            SearchFilter = "";
        }

        public bool ShowSortPane
        {
            get;
            set;
        }

        public List<string> SortMethodCaptions
        {
            set
            {
                Assert.That(_sortMethodCaptions.IsEmpty());
                _sortMethodCaptions.AddRange(value);
            }
        }

        public int SortMethod
        {
            get;
            set;
        }

        public string ActiveScene
        {
            get;
            set;
        }

        public bool SortDescending
        {
            get;
            set;
        }

        public string SearchFilter
        {
            get;
            set;
        }

        public Vector2 ScrollPos
        {
            get;
            set;
        }

        public IEnumerable<ListEntry> Values
        {
            get
            {
                return _entries;
            }
        }

        public IEnumerable<string> DisplayValues
        {
            get
            {
                return _entries.Select(x => x.Name);
            }
        }

        public List<string> GetSelectedNames()
        {
            return _entries.Where(x => x.IsSelected).Select(x => x.Name).ToList();
        }

        public void ClearSelected()
        {
            foreach (var entry in _entries)
            {
                entry.IsSelected = false;
            }
        }

        public void SelectAll()
        {
            foreach (var entry in _entries)
            {
                entry.IsSelected = true;
            }
        }

        public void Remove(ListEntry entry)
        {
            _entries.RemoveWithConfirm(entry);
            UpdateIndices();
        }

        public void SetItems(List<ItemDescriptor> newItems)
        {
            var oldEntries = _entries.ToDictionary(x => x.Model, x => x);

            _entries.Clear();

            for (int i = 0; i < newItems.Count; i++)
            {
                var item = newItems[i];

                var entry = new ListEntry()
                {
                    Name = item.Caption,
                    Model = item.Model,
                    ListOwner = this,
                    Index = i,
                };

                var oldEntry = oldEntries.TryGetValue(item.Model);

                if (oldEntry != null)
                {
                    entry.IsSelected = oldEntry.IsSelected;
                }

                _entries.Add(entry);
            }
        }

        public ListEntry GetAtIndex(int index)
        {
            return _entries[index];
        }

        public void Remove(string name)
        {
            Remove(_entries.Where(x => x.Name == name).Single());
        }

        public void Clear()
        {
            _entries.Clear();
        }

        public void UpdateIndices()
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                _entries[i].Index = i;
            }
        }

        void ClickSelect(ListEntry newEntry)
        {
            if (newEntry.IsSelected)
            {
                if (Event.current.control)
                {
                    newEntry.IsSelected = false;
                }

                return;
            }

            if (!Event.current.control && !Event.current.shift)
            {
                ClearSelected();
            }

            var selected = GetSelected();

            if (Event.current.shift && !selected.IsEmpty())
            {
                var closestEntry = selected
                    .Select(x => new { Distance = Mathf.Abs(x.Index - newEntry.Index), Entry = x })
                    .OrderBy(x => x.Distance)
                    .Select(x => x.Entry).First();

                int startIndex;
                int endIndex;

                if (closestEntry.Index > newEntry.Index)
                {
                    startIndex = newEntry.Index + 1;
                    endIndex = closestEntry.Index - 1;
                }
                else
                {
                    startIndex = closestEntry.Index + 1;
                    endIndex = newEntry.Index - 1;
                }

                for (int i = startIndex; i <= endIndex; i++)
                {
                    var inBetweenEntry = closestEntry.ListOwner.GetAtIndex(i);

                    inBetweenEntry.IsSelected = true;
                }
            }

            newEntry.IsSelected = true;
        }

        public List<ListEntry> GetSelected()
        {
            return _entries.Where(x => x.IsSelected).ToList();
        }

        public List<object> GetSelectedModels()
        {
            return _entries.Where(x => x.IsSelected).Select(x => x.Model).ToList();
        }

        void DrawSearchPane(Rect rect)
        {
            Assert.That(ShowSortPane);

            var startX = rect.xMin;
            var endX = rect.xMax;
            var startY = rect.yMin;
            var endY = rect.yMax;

            ImguiUtil.DrawColoredQuad(rect, _settings.IconRowBackgroundColor);

            endX = rect.xMax - 2 * _settings.ButtonWidth;

            var searchBarRect = Rect.MinMaxRect(startX, startY, endX, endY);
            if (GUI.enabled && searchBarRect.Contains(Event.current.mousePosition))
            {
                ImguiUtil.DrawColoredQuad(searchBarRect, _settings.MouseOverBackgroundColor);
            }

            GUI.Label(new Rect(startX + _settings.SearchIconOffset.x, startY + _settings.SearchIconOffset.y, _settings.SearchIconSize.x, _settings.SearchIconSize.y), _settings.SearchIcon);

            var newSearchFilter = GUI.TextField(
                searchBarRect, this.SearchFilter, _settings.SearchTextStyle);

            if (newSearchFilter != this.SearchFilter)
            {
                SearchFilterChanged(newSearchFilter);
            }

            startX = endX;
            endX = startX + _settings.ButtonWidth;

            Rect buttonRect;

            buttonRect = Rect.MinMaxRect(startX, startY, endX, endY);
            if (buttonRect.Contains(Event.current.mousePosition))
            {
                ImguiUtil.DrawColoredQuad(buttonRect, _settings.MouseOverBackgroundColor);

                if (Event.current.type == EventType.MouseDown)
                {
                    SortDescendingToggled();
                    //SortDescending = !SortDescending;
                    this.UpdateIndices();
                }
            }
            GUI.DrawTexture(buttonRect, SortDescending ? _settings.SortDirUpIcon : _settings.SortDirDownIcon);

            startX = endX;
            endX = startX + _settings.ButtonWidth;

            buttonRect = Rect.MinMaxRect(startX, startY, endX, endY);
            if (buttonRect.Contains(Event.current.mousePosition))
            {
                ImguiUtil.DrawColoredQuad(buttonRect, _settings.MouseOverBackgroundColor);

                if (Event.current.type == EventType.MouseDown && !_sortMethodCaptions.IsEmpty())
                {
                    var startPos = new Vector2(buttonRect.xMin, buttonRect.yMax);
                    ImguiUtil.OpenContextMenu(startPos, CreateSortMethodContextMenuItems());
                }
            }
            GUI.DrawTexture(buttonRect, _settings.SortIcon);
        }

        List<ContextMenuItem> CreateSortMethodContextMenuItems()
        {
            var result = new List<ContextMenuItem>();

            for (int i = 0; i < _sortMethodCaptions.Count; i++)
            {
                var closedI = i;
                result.Add(new ContextMenuItem(
                    true, _sortMethodCaptions[i],
                    SortMethod == i, () => SortMethodSelected(closedI)));
                    //SortMethod == i, () => SortMethod = closedI));
            }

            return result;
        }

        public void Draw(Rect fullRect)
        {
            Rect listRect;
            if (ShowSortPane)
            {
                var searchRect = new Rect(fullRect.xMin, fullRect.yMin, fullRect.width, _settings.IconRowHeight);
                DrawSearchPane(searchRect);

                listRect = Rect.MinMaxRect(
                    fullRect.xMin, fullRect.yMin + _settings.IconRowHeight, fullRect.xMax, fullRect.yMax);
            }
            else
            {
                listRect = fullRect;
            }

            var searchFilter = SearchFilter.Trim().ToLowerInvariant();
            var visibleEntries = _entries.Where(x => x.Name.ToLowerInvariant().Contains(searchFilter)).ToList();

            var viewRect = new Rect(0, 0, listRect.width - 30.0f, visibleEntries.Count * _settings.ItemHeight);

            var isListUnderMouse = listRect.Contains(Event.current.mousePosition);

            ImguiUtil.DrawColoredQuad(listRect, GetListBackgroundColor(isListUnderMouse));

            switch (Event.current.type)
            {
                case EventType.MouseUp:
                {
                    break;
                }
                case EventType.ContextClick:
                {
                    if (isListUnderMouse)
                    {
                        ContextMenuOpenRequested();
                        Event.current.Use();
                    }

                    break;
                }
            }

            bool clickedItem = false;

            float yPos = 0;
            var newScrollPos = GUI.BeginScrollView(listRect, ScrollPos, viewRect);
            {
                foreach (var entry in visibleEntries)
                {
                    var labelRect = new Rect(0, yPos, listRect.width, _settings.ItemHeight);

                    bool isItemUnderMouse = labelRect.Contains(Event.current.mousePosition);

                    Color itemColor;

                    if (entry.IsSelected)
                    {
                        itemColor = _settings.Theme.ListItemSelectedColor;
                    }
                    else if (GUI.enabled && isItemUnderMouse)
                    {
                        itemColor = _settings.Theme.ListItemHoverColor;
                    }
                    else if (entry.Name == ActiveScene)
                    {
                        itemColor = _settings.Theme.ActiveSceneSelectedColor;
                    }
                    else
                    {
                        itemColor = _settings.Theme.ListItemColor;
                    }

                    ImguiUtil.DrawColoredQuad(labelRect, itemColor);

                    switch (Event.current.type)
                    {
                        case EventType.MouseUp:
                        {
                            if (isItemUnderMouse && Event.current.button == 0)
                            {
                                if (!Event.current.shift && !Event.current.control)
                                {
                                    ClearSelected();
                                    ClickSelect(entry);
                                }
                            }

                            break;
                        }
                        case EventType.MouseDown:
                        {
                            if (isItemUnderMouse)
                            {
                                // Unfocus on text field
                                GUI.FocusControl(null);

                                clickedItem = true;
                                ClickSelect(entry);

                                Event.current.Use();
                            }
                            break;
                        }
                    }

                    GUI.Label(labelRect, entry.Name, _settings.ItemTextStyle);

                    yPos += _settings.ItemHeight;
                }
            }
            GUI.EndScrollView();

            if (newScrollPos != ScrollPos)
            {
                ScrollPositionChanged(newScrollPos);
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !clickedItem &&  isListUnderMouse)
            {
                // Unfocus on text field
                GUI.FocusControl(null);

                ClearSelected();
            }
        }

        Color GetListBackgroundColor(bool isHover)
        {
            if (!GUI.enabled)
            {
                return _settings.Theme.ListColor;
            }

            if (SearchFilter.Trim().Count() > 0)
            {
                return isHover ? _settings.Theme.FilteredListHoverColor : _settings.Theme.FilteredListColor;
            }

            return isHover ? _settings.Theme.ListHoverColor : _settings.Theme.ListColor;
        }

        public class ItemDescriptor
        {
            public string Caption;
            public object Model;
        }

        [Serializable]
        public class Settings
        {
            public float ButtonWidth;

            public Color MouseOverBackgroundColor;
            public Color IconRowBackgroundColor;

            public float IconRowHeight;

            public Vector2 SearchIconOffset;

            public Texture2D SortIcon;
            public Texture2D SortDirDownIcon;
            public Texture2D SortDirUpIcon;
            public Texture2D SearchIcon;

            public Vector2 SearchIconSize;

            public float ItemHeight;

            public ThemeProperties Light;
            public ThemeProperties Dark;

            public ThemeProperties Theme
            {
                get
                {
                    return EditorGUIUtility.isProSkin ? Dark : Light;
                }
            }

            public GUIStyle ItemTextStyle
            {
                get
                {
                    return GUI.skin.GetStyle("StListItemStyle");
                }
            }

            public GUIStyle SearchTextStyle
            {
                get
                {
                    return GUI.skin.GetStyle("StSearchPaneTextStyle");
                }
            }

            [Serializable]
            public class ThemeProperties
            {
                public Color ListColor;
                public Color ListHoverColor;

                public Color FilteredListColor;
                public Color FilteredListHoverColor;

                public Color ListItemColor;
                public Color ListItemHoverColor;
                public Color ListItemSelectedColor;

                public Color ActiveSceneSelectedColor;
            }
        }

        public class ListEntry
        {
            public string Name;
            public bool IsSelected;
            public object Model;
            public int Index;
            public StListView ListOwner;
        }
    }
}
