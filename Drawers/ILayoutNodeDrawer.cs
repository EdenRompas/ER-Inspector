using System;
using UnityEditor;

namespace InspectorDesigner.Drawers
{
    /// <summary>
    /// Implement ini untuk setiap tipe LayoutNode baru: bagaimana ia digambar di Inspector asli.
    /// </summary>
    public interface ILayoutNodeDrawer
    {
        /// <summary>Label di menu "Add Attribute". Kembalikan null supaya tidak muncul di menu (contoh: FieldNode).</summary>
        string MenuLabel { get; }

        void DrawInspector(SerializedObject serializedObject, LayoutNode node, Action<LayoutNode> drawChild);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class LayoutNodeDrawerAttribute : Attribute
    {
        public Type NodeType { get; }
        public LayoutNodeDrawerAttribute(Type nodeType) => NodeType = nodeType;
    }
}