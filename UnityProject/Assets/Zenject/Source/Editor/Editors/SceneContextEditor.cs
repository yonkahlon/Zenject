using UnityEditor;

namespace Zenject
{
    [CustomEditor(typeof(SceneContext))]
    public class SceneContextEditor : ContextEditor
    {
        SerializedProperty _nameProperty;
        SerializedProperty _parentSceneContextNameProperty;
        SerializedProperty _parentNewObjectsUnderRootProperty;

        public override void OnEnable()
        {
            base.OnEnable();

            _nameProperty = serializedObject.FindProperty("_name");
            _parentSceneContextNameProperty = serializedObject.FindProperty("_parentSceneContextName");
            _parentNewObjectsUnderRootProperty = serializedObject.FindProperty("_parentNewObjectsUnderRoot");
        }

        protected override void OnGui()
        {
            base.OnGui();

            EditorGUILayout.PropertyField(_nameProperty);
            EditorGUILayout.PropertyField(_parentSceneContextNameProperty);
            EditorGUILayout.PropertyField(_parentNewObjectsUnderRootProperty);
        }
    }
}



