using UnityEngine;
using UnityEditor;

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class InfoBoxAttribute : PropertyAttribute
{
    public string message;
    public float viewWidth;

    public InfoBoxAttribute(string message)
    {
        this.message = message;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(InfoBoxAttribute))]
public class InfoBoxDrawer : DecoratorDrawer
{
    private float k_DefaultBoxHeight = 26f;

    public override float GetHeight()
    {
        return k_DefaultBoxHeight;
    }

    public override void OnGUI(Rect position)
    {
        InfoBoxAttribute typeInfoBoxAttribute = (InfoBoxAttribute)attribute;

        EditorGUI.HelpBox(position, typeInfoBoxAttribute.message, MessageType.Info);
    }
}

#endif
