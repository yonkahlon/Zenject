using System.Collections.Generic;
using System.Linq;
using Zenject;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;
using ModestTree;

namespace Zenject
{
    public class CompositionRootEditor : UnityInspectorListEditor
    {
        SerializedProperty _includeInactiveProperty;

        protected override string[] PropertyNames
        {
            get
            {
                return new string[]
                {
                    "_installers",
                    "_installerPrefabs",
                };
            }
        }

        protected override string[] PropertyDisplayNames
        {
            get
            {
                return new string[]
                {
                    "Installers",
                    "Installer Prefabs",
                };
            }
        }

        protected override string[] PropertyDescriptions
        {
            get
            {
                return new string[]
                {
                    "Drag any MonoInstallers that you have added to your Scene Hierarchy here.",
                    "Drag any prefabs that contain a MonoInstaller on them here",
                };
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            _includeInactiveProperty = serializedObject.FindProperty("_includeInactive");
        }

        protected override void OnGui()
        {
            base.OnGui();

            EditorGUILayout.PropertyField(_includeInactiveProperty);
        }
    }

    [CustomEditor(typeof(SceneCompositionRoot))]
    public class SceneCompositionRootEditor : CompositionRootEditor
    {
        SerializedProperty _parentNewObjectsUnderRootProperty;

        public override void OnEnable()
        {
            base.OnEnable();

            _parentNewObjectsUnderRootProperty = serializedObject.FindProperty("_parentNewObjectsUnderRoot");
        }

        protected override void OnGui()
        {
            base.OnGui();

            EditorGUILayout.PropertyField(_parentNewObjectsUnderRootProperty);
        }
    }

    [CustomEditor(typeof(FacadeCompositionRoot))]
    public class FacadeCompositionRooEditor : CompositionRootEditor
    {
        SerializedProperty _facadeProperty;

        public override void OnEnable()
        {
            base.OnEnable();

            _facadeProperty = serializedObject.FindProperty("_facade");
        }

        protected override void OnGui()
        {
            base.OnGui();

            EditorGUILayout.PropertyField(_facadeProperty);
        }
    }

    [CustomEditor(typeof(GlobalCompositionRoot))]
    public class GlobalCompositionRooEditor : CompositionRootEditor
    {
    }

    [CustomEditor(typeof(EditorWindowCompositionRoot))]
    public class EditorWindowCompositionRootEditor : UnityInspectorListEditor
    {
        protected override string[] PropertyNames
        {
            get
            {
                return new string[]
                {
                    "_installers",
                };
            }
        }

        protected override string[] PropertyDisplayNames
        {
            get
            {
                return new string[]
                {
                    "Installers",
                };
            }
        }

        protected override string[] PropertyDescriptions
        {
            get
            {
                return new string[]
                {
                    "Drag any MonoEditorInstallers that you have added to your project here.",
                };
            }
        }
    }

}


