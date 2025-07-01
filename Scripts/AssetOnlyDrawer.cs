using UnityEngine;
using UnityEditor;

namespace ERInspector
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class AssetOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(AssetOnlyAttribute))]
    public class AssetOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(property.objectReferenceValue);
                if (string.IsNullOrEmpty(assetPath))
                    property.objectReferenceValue = null;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }
    }

#endif
}