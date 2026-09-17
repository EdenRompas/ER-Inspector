using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InspectorDesigner.Drawers
{
    [LayoutNodeDrawer(typeof(FoldoutNode))]
    public class FoldoutNodeDrawer : ILayoutNodeDrawer
    {
        private const float HeaderInset = 12;

        public string MenuLabel => "Foldout";
        private static readonly Dictionary<string, bool> state = new();

        public void DrawInspector(SerializedObject so, LayoutNode node, Action<LayoutNode> drawChild)
        {
            var foldout = (FoldoutNode)node;
            if (!state.TryGetValue(foldout.Id, out var expanded)) expanded = foldout.ExpandedByDefault;

            var groupRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GroupBoundsStack.Push(groupRect);
            try
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(HeaderInset);
                expanded = EditorGUILayout.Foldout(expanded, foldout.Title, true);
                EditorGUILayout.EndHorizontal();

                state[foldout.Id] = expanded;

                if (expanded) DrawChildren(foldout, drawChild, groupRect);
            }
            finally
            {
                GroupBoundsStack.Pop();
                EditorGUILayout.EndVertical();
            }
        }

        private static void DrawChildren(FoldoutNode foldout, Action<LayoutNode> drawChild, Rect groupRect)
        {
            EditorGUI.indentLevel++;
            GroupBoundsStack.Push(EditorGUI.IndentedRect(groupRect));
            try
            {
                foreach (var child in foldout.Children) drawChild(child);
            }
            finally
            {
                GroupBoundsStack.Pop();
                EditorGUI.indentLevel--;
            }
        }
    }
}