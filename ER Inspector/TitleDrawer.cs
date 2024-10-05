using UnityEngine;
using UnityEditor;

namespace EREditor.Inspector
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class TitleAttribute : PropertyAttribute
    {
        public string title;

        public TitleAttribute(string title)
        {
            this.title = title;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitleDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            TitleAttribute titleAttribute = attribute as TitleAttribute;
            if (titleAttribute != null)
            {
                GUIStyle titleStyle = new GUIStyle(EditorStyles.label);
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.alignment = TextAnchor.UpperLeft;

                Rect textRect = position;
                textRect.y += EditorGUIUtility.standardVerticalSpacing + 2;

                EditorGUI.LabelField(textRect, titleAttribute.title, titleStyle);

                Rect lineRect = position;
                lineRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                lineRect.height = 1f;
                EditorGUI.DrawRect(lineRect, Color.gray);
            }
        }

        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + 5f;
        }
    }

#endif
}
