using UnityEngine;
using UnityEditor;
using ERInspector;

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(InfoBoxAttribute))]
    public class InfoBoxDrawerEditor : DecoratorDrawer
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