using System;
using Zenject;

namespace SceneTester
{
    public class StCommands
    {
        public class OpenContextMenu : Command
        {
        }

        public class ValidateSelectedScenes : Command
        {
        }

        public class StartSelectedScenes : Command
        {
        }

        public class SceneOpener : Command<string>
        {
        }
    }
}
