using UnityEngine;
using UnityEditor;

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
        ShowIfAttribute showIfAttribute = attribute as ShowIfAttribute;

        SerializedProperty condition = property.serializedObject.FindProperty(showIfAttribute.conditionName);
        if (condition != null && condition.propertyType == SerializedPropertyType.Boolean)
        {
            bool show = condition.boolValue;

            if (show)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
        else
        {
            EditorGUI.HelpBox(position, "ShowIf error: Condition not found or not a boolean.", MessageType.Error);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUI.GetPropertyHeight(property, label, true);

        if (!IsPropertyVisible(property))
        {
            height += EditorGUIUtility.standardVerticalSpacing;
        }

        return height;
    }

    private bool IsPropertyVisible(SerializedProperty property)
    {
        ShowIfAttribute showIfAttribute = attribute as ShowIfAttribute;
        SerializedProperty condition = property.serializedObject.FindProperty(showIfAttribute.conditionName);
        if (condition != null && condition.propertyType == SerializedPropertyType.Boolean)
        {
            return condition.boolValue;
        }
        return false;
    }
}

#endif