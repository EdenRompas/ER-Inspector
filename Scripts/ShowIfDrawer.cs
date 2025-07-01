using UnityEngine;
using UnityEditor;

namespace ERInspector
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string conditionName;

        public ShowIfAttribute(string conditionName)
        {
            this.conditionName = conditionName;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsPropertyVisible(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!IsPropertyVisible(property))
            {
                return 0f;
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private bool IsPropertyVisible(SerializedProperty property)
        {
            ShowIfAttribute showIf = (ShowIfAttribute)attribute;

            string propertyPath = property.propertyPath;
            string conditionPath = propertyPath.Replace(property.name, showIf.conditionName);

            SerializedProperty conditionProperty = property.serializedObject.FindProperty(conditionPath);

            if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean)
            {
                return conditionProperty.boolValue;
            }

            return false;
        }
    }

#endif
}