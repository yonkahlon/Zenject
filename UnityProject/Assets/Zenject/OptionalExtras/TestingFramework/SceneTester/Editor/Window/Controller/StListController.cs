using System;
using ModestTree.Util;
using UnityEngine;
using Zenject;
using System.Linq;

namespace SceneTester
{
    public class StListController : IInitializable, IDisposable, ITickable
    {
        readonly StModel _model;
        readonly StCommands.OpenContextMenu _openMenuCommand;
        readonly StListModel _listModel;
        readonly StListView _view;

        bool _modelChanged;

        public StListController(
            StListView view,
            StListModel listModel,
            StCommands.OpenContextMenu openMenuCommand,
            StModel model)
        {
            _model = model;
            _openMenuCommand = openMenuCommand;
            _listModel = listModel;
            _view = view;
        }

        public void Initialize()
        {
            _model.CurrentSceneChanged += OnModelChanged;
            _listModel.Changed += OnModelChanged;

            _view.SortDescendingToggled += OnViewSortDescendingToggled;
            _view.SortMethodSelected += OnViewSortMethodSelected;
            _view.SearchFilterChanged += OnViewSearchFilterChanged;
            _view.ScrollPositionChanged += OnViewScrollPositionChanged;

            _view.ContextMenuOpenRequested += OnContextMenuOpenRequested;

            _modelChanged = true;
        }

        public void Dispose()
        {
            _model.CurrentSceneChanged -= OnModelChanged;
            _listModel.Changed -= OnModelChanged;

            _view.SortDescendingToggled -= OnViewSortDescendingToggled;
            _view.SortMethodSelected -= OnViewSortMethodSelected;
            _view.SearchFilterChanged -= OnViewSearchFilterChanged;
            _view.ScrollPositionChanged -= OnViewScrollPositionChanged;

            _view.ContextMenuOpenRequested -= OnContextMenuOpenRequested;
        }

        void OnContextMenuOpenRequested()
        {
            _openMenuCommand.Execute();
        }

        public void Tick()
        {
            if (_modelChanged)
            {
                _modelChanged = false;

                _view.ActiveScene = _model.CurrentScene;

                _view.ScrollPos = _listModel.ScrollPos;
                _view.SearchFilter = _listModel.SearchFilter;
                _view.SortMethod = _listModel.SortMethod;
                _view.SortDescending = _listModel.SortDescending;

                _view.SetItems(
                    _listModel.SceneInfos.Select(x => CreateViewItem(x)).ToList());
            }
        }

        public void OnViewScrollPositionChanged(Vector2 value)
        {
            _listModel.ScrollPos = value;
        }

        public void OnViewSearchFilterChanged(string value)
        {
            _listModel.SearchFilter = value;
        }

        public void OnViewSortMethodSelected(int value)
        {
            _listModel.SortMethod = value;
        }

        public void OnViewSortDescendingToggled()
        {
            _listModel.SortDescending = !_listModel.SortDescending;
        }

        void OnModelChanged()
        {
            _modelChanged = true;
        }

        StListView.ItemDescriptor CreateViewItem(SceneData info)
        {
            return new StListView.ItemDescriptor()
            {
                Caption = info.Name,
                Model = info,
            };
        }
    }
}
