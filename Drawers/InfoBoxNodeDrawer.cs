using System;
using UnityEditor;

namespace InspectorDesigner.Drawers
{
    [LayoutNodeDrawer(typeof(InfoBoxNode))]
    public class InfoBoxNodeDrawer : ILayoutNodeDrawer
    {
        public string MenuLabel => "Info Box";
        public void DrawInspector(SerializedObject so, LayoutNode node, Action<LayoutNode> drawChild)
        {
            var box = (InfoBoxNode)node;
            var msgType = box.Type switch
            {
                InfoBoxNode.InfoBoxType.Warning => MessageType.Warning,
                InfoBoxNode.InfoBoxType.Error => MessageType.Error,
                _ => MessageType.Info
            };
            EditorGUILayout.HelpBox(box.Message, msgType);
        }
    }
}