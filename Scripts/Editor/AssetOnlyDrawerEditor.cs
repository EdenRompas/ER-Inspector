using UnityEngine;
using UnityEditor;
using ERInspector;

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(AssetOnlyAttribute))]
    public class AssetOnlyDrawerEditor : PropertyDrawer
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