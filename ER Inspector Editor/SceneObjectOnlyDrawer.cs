using UnityEngine;
using UnityEditor;

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class SceneObjectOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(SceneObjectOnlyAttribute))]
public class SceneObjectOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
        {
            GameObject sceneObject = property.objectReferenceValue as GameObject;
            if (sceneObject != null && sceneObject.scene.isLoaded == false)
                property.objectReferenceValue = null;
        }

        EditorGUI.PropertyField(position, property, label, true);
    }
}

#endif
