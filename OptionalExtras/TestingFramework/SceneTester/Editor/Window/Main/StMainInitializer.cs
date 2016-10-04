using System.IO;
using Zenject;
using UnityEngine;
using ModestTree;
using System.Linq;

namespace SceneTester
{
    public class StMainInitializer : IInitializable
    {
        const string SceneFileExtension = ".unity";

        readonly StModel _model;

        public StMainInitializer(StModel model)
        {
            _model = model;
        }

        public void Initialize()
        {
            string[] allSceneFiles = Directory.GetFiles(
                Path.GetFullPath(Application.dataPath), ("*" + SceneFileExtension), SearchOption.AllDirectories);

            Log.Debug("Scene Runner: Found {0} scenes! {1}",
                allSceneFiles.Length, allSceneFiles.Select(x => x).Join("\n"));

            _model.List.SceneInfos = allSceneFiles.Select(x => CreateItem(x)).ToList();
        }

        SceneData CreateItem(string fullPath)
        {
            return new SceneData()
            {
                Name = Path.GetFileNameWithoutExtension(fullPath),
                FullPath = fullPath
            };
        }
    }
}
