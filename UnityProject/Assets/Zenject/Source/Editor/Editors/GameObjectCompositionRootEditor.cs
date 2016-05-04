using System;
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
    [CustomEditor(typeof(GameObjectCompositionRoot))]
    public class GameObjectCompositionRootEditor : CompositionRootEditor
    {
        SerializedProperty _facade;

        public override void OnEnable()
        {
            base.OnEnable();

            _facade = serializedObject.FindProperty("_facade");
        }

        protected override void OnGui()
        {
            base.OnGui();

            EditorGUILayout.PropertyField(_facade);
        }
    }
}
