using System;
using UnityEditor;
using UnityEngine;

namespace InspectorDesigner.Drawers
{
    [LayoutNodeDrawer(typeof(FieldNode))]
    public class FieldNodeDrawer : ILayoutNodeDrawer
    {
        public string MenuLabel => null;

        private const float ArrayLeftPadding = 15f;
        private const float ClassLeftPadding = 14f;
        private const float RightPadding = 5f;

        public void DrawInspector(SerializedObject so, LayoutNode node, Action<LayoutNode> drawChild)
        {
            var prop = so.FindProperty(((FieldNode)node).FieldName);
            if (prop != null)
            {
                bool isArray = prop.isArray;

                bool isClass =  prop.propertyType == SerializedPropertyType.Generic || prop.propertyType == SerializedPropertyType.ManagedReference;

                if (isArray)
                {
                    GUILayout.BeginHorizontal();
                    
                    GUILayout.Space(ArrayLeftPadding);

                    GUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(prop, true);
                    GUILayout.EndVertical();

                    GUILayout.Space(RightPadding);

                    GUILayout.EndHorizontal();
                }
                else if (isClass)
                {
                    GUILayout.BeginHorizontal();
                    
                    GUILayout.Space(ClassLeftPadding);

                    GUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(prop, true);
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.PropertyField(prop, true);
                }
            }
        }
    }
}