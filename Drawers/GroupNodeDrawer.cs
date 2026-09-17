using System;
using UnityEditor;
using UnityEngine;

namespace InspectorDesigner.Drawers
{
    [LayoutNodeDrawer(typeof(VerticalGroupNode))]
    public class VerticalGroupNodeDrawer : ILayoutNodeDrawer
    {
        public string MenuLabel => "Vertical Group";
        public void DrawInspector(SerializedObject so, LayoutNode node, Action<LayoutNode> drawChild)
        {
            var groupRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GroupBoundsStack.Push(groupRect);
            try
            {
                foreach (var child in ((VerticalGroupNode)node).Children) drawChild(child);
            }
            finally
            {
                GroupBoundsStack.Pop();
                EditorGUILayout.EndVertical();
            }
        }
    }

    [LayoutNodeDrawer(typeof(HorizontalGroupNode))]
    public class HorizontalGroupNodeDrawer : ILayoutNodeDrawer
    {
        private const float ColumnGap = 6f;

        public string MenuLabel => "Horizontal Group";
        public void DrawInspector(SerializedObject so, LayoutNode node, Action<LayoutNode> drawChild)
        {
            var hg = (HorizontalGroupNode)node;

            var rowRect = EditorGUILayout.BeginHorizontal();
            try
            {
                // Lebar baris dari BeginHorizontal kadang belum valid di event Layout (masih 1px).
                // Kalau itu terjadi, pakai lebar Inspector saat ini sebagai perkiraan supaya
                // kedua kolom tetap terisi maksimal & seimbang, bukan menyusut mengikuti konten.
                var availableWidth = rowRect.width > 1f
                    ? rowRect.width
                    : Mathf.Max(0f, EditorGUIUtility.currentViewWidth - 40f);

                var columnWidth = Mathf.Max(0f, (availableWidth - ColumnGap) / 2f);
                var columnOptions = columnWidth > 1f
                    ? new[] { GUILayout.Width(columnWidth) }
                    : new[] { GUILayout.ExpandWidth(true) };

                var leftRect = EditorGUILayout.BeginVertical(columnOptions);
                GroupBoundsStack.Push(leftRect);
                try { foreach (var child in hg.LeftChildren) drawChild(child); }
                finally { GroupBoundsStack.Pop(); EditorGUILayout.EndVertical(); }

                GUILayout.Space(ColumnGap);

                var rightRect = EditorGUILayout.BeginVertical(columnOptions);
                GroupBoundsStack.Push(rightRect);
                try { foreach (var child in hg.RightChildren) drawChild(child); }
                finally { GroupBoundsStack.Pop(); EditorGUILayout.EndVertical(); }
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}