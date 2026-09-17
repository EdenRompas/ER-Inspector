using System.Collections.Generic;
using UnityEngine;

namespace InspectorDesigner
{
    /// <summary>
    /// Asset yang menyimpan hasil desain layout untuk satu tipe script.
    /// Otomatis disimpan di Resources/InspectorDesigner/<NamaScript>.asset
    /// supaya bisa dibaca via Resources.Load saat inspector digambar.
    /// </summary>
    public class InspectorLayoutConfig : ScriptableObject
    {
        [SerializeField] private string targetAssemblyQualifiedName;
        [SerializeReference] private List<LayoutNode> rootElements = new List<LayoutNode>();

        public string TargetAssemblyQualifiedName
        {
            get => targetAssemblyQualifiedName;
            set => targetAssemblyQualifiedName = value;
        }

        public List<LayoutNode> RootElements => rootElements;
    }
}