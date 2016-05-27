using UnityEditor;

namespace Zenject
{
    [CustomEditor(typeof(SceneCompositionRoot))]
    public class SceneCompositionRootEditor : UnityInspectorListEditor
    {
        protected override string[] PropertyNames
        {
            get
            {
                return new string[]
                {
                    "Installers"
                };
            }
        }

        protected override string[] PropertyDescriptions
        {
            get
            {
                return new string[]
                {
                    ""
                };
            }
        }
    }
}


