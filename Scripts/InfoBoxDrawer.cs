using UnityEngine;
using UnityEditor;

namespace EREditor.Inspector
{
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
        private float k_Spacing = 2f;

        public override float GetHeight()
        {
            return k_DefaultBoxHeight + k_Spacing;
        }

        public override void OnGUI(Rect position)
        {
            InfoBoxAttribute typeInfoBoxAttribute = (InfoBoxAttribute)attribute;

            Rect helpBoxRect = new Rect(position.x, position.y, position.width, k_DefaultBoxHeight);
            EditorGUI.HelpBox(helpBoxRect, typeInfoBoxAttribute.message, MessageType.Info);
        }
    }

#endif
}
