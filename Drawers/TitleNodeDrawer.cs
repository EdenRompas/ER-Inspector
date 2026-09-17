using System;
using UnityEditor;
using UnityEngine;

namespace InspectorDesigner.Drawers
{
    [LayoutNodeDrawer(typeof(TitleNode))]
    public class TitleNodeDrawer : ILayoutNodeDrawer
    {
        private const float GroupInset = 4;

        public string MenuLabel => "Title";
        public void DrawInspector(SerializedObject so, LayoutNode node, Action<LayoutNode> drawChild)
        {
            var title = (TitleNode)node;
            EditorGUILayout.LabelField(title.Text, new GUIStyle(EditorStyles.boldLabel)
            {
                fontStyle = title.Bold ? FontStyle.Bold : FontStyle.Normal
            });

            var rect = EditorGUILayout.GetControlRect(false, 1);
            rect.y -= 2f; 
            if (GroupBoundsStack.TryGetCurrent(out var groupRect) && groupRect.width > 1f)
            {
                rect.x = groupRect.x + GroupInset;
                rect.width = Mathf.Max(0f, groupRect.width - GroupInset * 2f);
            }

            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }
    }
}