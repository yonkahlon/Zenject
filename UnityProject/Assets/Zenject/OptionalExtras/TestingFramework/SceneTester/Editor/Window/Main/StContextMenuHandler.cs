using System;
using System.Collections.Generic;
using ModestTree.Util;
using UnityEngine;
using Zenject;
using System.Linq;

namespace SceneTester
{
    public class StContextMenuHandler
    {
        readonly StListView _view;
        readonly StCommands.StartSelectedScenes _startScenesCommand;
        readonly StCommands.ValidateSelectedScenes _validateScenesCommand;
        readonly StCommands.SceneOpener _sceneOpener;
        readonly StListView _list;

        public StContextMenuHandler(
            StListView list,
            StCommands.StartSelectedScenes startScenesCommand,
            StCommands.ValidateSelectedScenes validateScenesCommand,
            StCommands.SceneOpener sceneOpener,
            StListView view)
        {
            _view = view;
            _startScenesCommand = startScenesCommand;
            _validateScenesCommand = validateScenesCommand;
            _sceneOpener = sceneOpener;
            _list = list;
        }

        public void Open()
        {
            ImguiUtil.OpenContextMenu(GetContextMenuItems());
        }

        IEnumerable<ContextMenuItem> GetContextMenuItems()
        {
            yield return new ContextMenuItem(
                true, "Run Selected Scenes", false, () => _startScenesCommand.Execute());

            yield return new ContextMenuItem(
                true, "Validate Selected Scenes", false, () => _validateScenesCommand.Execute());

            yield return new ContextMenuItem(
                _view.GetSelectedNames().Count == 1, "Open Scene", false, () => _sceneOpener.Execute(_view.GetSelectedNames().First()));
        }
    }
}

