using UnityEditor;

namespace Zenject
{
    [CustomEditor(typeof(SceneContext))]
    public class SceneContextEditor : ContextEditor
    {
        SerializedProperty _contractNameProperty;
        SerializedProperty _parentContractNameProperty;
        SerializedProperty _parentNewObjectsUnderRootProperty;
        SerializedProperty _autoRun;

        public override void OnEnable()
        {
            base.OnEnable();

            _contractNameProperty = serializedObject.FindProperty("_contractName");
            _parentContractNameProperty = serializedObject.FindProperty("_parentContractName");
            _parentNewObjectsUnderRootProperty = serializedObject.FindProperty("_parentNewObjectsUnderRoot");
            _autoRun = serializedObject.FindProperty("_autoRun");
        }

        protected override void OnGui()
        {
            base.OnGui();

            EditorGUILayout.PropertyField(_contractNameProperty);
            EditorGUILayout.PropertyField(_parentContractNameProperty);
            EditorGUILayout.PropertyField(_parentNewObjectsUnderRootProperty);
            EditorGUILayout.PropertyField(_autoRun);
        }
    }
}

