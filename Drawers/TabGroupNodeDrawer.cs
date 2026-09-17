using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace InspectorDesigner.Drawers
{
    [LayoutNodeDrawer(typeof(TabGroupNode))]
    public class TabGroupNodeDrawer : ILayoutNodeDrawer
    {
        public string MenuLabel => "Tab Group";
        private static readonly Dictionary<string, int> selectedTabByNodeId = new();

        public void DrawInspector(SerializedObject so, LayoutNode node, Action<LayoutNode> drawChild)
        {
            var tabGroup = (TabGroupNode)node;
            if (tabGroup.Tabs.Count == 0) return;

            if (!selectedTabByNodeId.TryGetValue(tabGroup.Id, out var selected) || selected >= tabGroup.Tabs.Count)
                selected = 0;

            var tabNames = tabGroup.Tabs.Select(t => t.TabName).ToArray();
            selected = GUILayout.Toolbar(selected, tabNames, GUILayout.ExpandWidth(true));
            selectedTabByNodeId[tabGroup.Id] = selected;

            EditorGUILayout.Space(2);

            var groupRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GroupBoundsStack.Push(groupRect);
            try
            {
                foreach (var child in tabGroup.Tabs[selected].Children) drawChild(child);
            }
            finally
            {
                GroupBoundsStack.Pop();
                EditorGUILayout.EndVertical();
            }
        }
    }
}