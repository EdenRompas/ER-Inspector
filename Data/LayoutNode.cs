using System;
using System.Collections.Generic;
using UnityEngine;

namespace InspectorDesigner
{
    /// <summary>
    /// Base class untuk semua elemen yang muncul di Field Configuration Window
    /// sekaligus di inspector hasil render. Tambah attribute baru = tambah subclass di sini
    /// + drawer yang sesuai (lihat folder Drawers/). Tidak perlu ubah kode lain.
    /// </summary>
    [Serializable]
    public abstract class LayoutNode
    {
        [SerializeField] private string id = Guid.NewGuid().ToString("N");
        public string Id => id;

        public abstract string DisplayName { get; }
        public virtual bool IsContainer => false;
    }

    [Serializable]
    public class FieldNode : LayoutNode
    {
        [SerializeField] private string fieldName;
        public string FieldName { get => fieldName; set => fieldName = value; }
        public override string DisplayName => fieldName;

        public FieldNode() { }
        public FieldNode(string fieldName) => this.fieldName = fieldName;
    }

    [Serializable]
    public class TitleNode : LayoutNode
    {
        [SerializeField] private string text = "Title";
        [SerializeField] private bool bold = true;
        public string Text { get => text; set => text = value; }
        public bool Bold { get => bold; set => bold = value; }
        public override string DisplayName => $"Title: {text}";
    }

    [Serializable]
    public class InfoBoxNode : LayoutNode
    {
        public enum InfoBoxType { Info, Warning, Error }

        [SerializeField] private string message = "Info message";
        [SerializeField] private InfoBoxType type = InfoBoxType.Info;
        public string Message { get => message; set => message = value; }
        public InfoBoxType Type { get => type; set => type = value; }
        public override string DisplayName => $"InfoBox: {message}";
    }

    /// <summary>
    /// Base untuk node yang bisa menampung child node lain. Bentuk penyimpanan child
    /// berbeda tiap subclass (satu kolom untuk Vertical/Foldout, dua kolom untuk Horizontal),
    /// jadi tidak ada satu List&lt;LayoutNode&gt; bersama di sini.
    /// Pakai LayoutTreeUtility.GetChildLists(node) untuk mengakses child secara generik.
    /// </summary>
    [Serializable]
    public abstract class GroupNode : LayoutNode
    {
        public override bool IsContainer => true;
    }

    [Serializable]
    public class VerticalGroupNode : GroupNode
    {
        [SerializeField] private string groupName = "Vertical Group";
        [SerializeReference] private List<LayoutNode> children = new List<LayoutNode>();
        public string GroupName { get => groupName; set => groupName = value; }
        public List<LayoutNode> Children => children;
        public override string DisplayName => $"[V] {groupName}";
    }

    /// <summary>
    /// Horizontal Group dibatasi tepat 2 kolom (kiri &amp; kanan) supaya layout di inspector
    /// tetap rapi. Field/attribute di kiri dan kanan disimpan terpisah, bukan satu list gabungan.
    /// </summary>
    [Serializable]
    public class HorizontalGroupNode : GroupNode, ISerializationCallbackReceiver
    {
        [SerializeField] private string groupName = "Horizontal Group";
        [SerializeReference] private List<LayoutNode> leftChildren = new List<LayoutNode>();
        [SerializeReference] private List<LayoutNode> rightChildren = new List<LayoutNode>();

        // Nama field lama sebelum Horizontal Group dibatasi 2 kolom. Dipertahankan hanya
        // untuk migrasi data asset lama, dipindah otomatis ke leftChildren sekali saja.
        [SerializeReference] private List<LayoutNode> children = new List<LayoutNode>();

        public string GroupName { get => groupName; set => groupName = value; }
        public List<LayoutNode> LeftChildren => leftChildren;
        public List<LayoutNode> RightChildren => rightChildren;
        public override string DisplayName => $"[H] {groupName}";

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (children != null && children.Count > 0)
            {
                leftChildren.InsertRange(0, children);
                children.Clear();
            }
        }
    }

    [Serializable]
    public class FoldoutNode : GroupNode
    {
        [SerializeField] private string title = "Foldout";
        [SerializeField] private bool expandedByDefault = true;
        [SerializeReference] private List<LayoutNode> children = new List<LayoutNode>();
        public string Title { get => title; set => title = value; }
        public bool ExpandedByDefault { get => expandedByDefault; set => expandedByDefault = value; }
        public List<LayoutNode> Children => children;
        public override string DisplayName => $"[Foldout] {title}";
    }

    [Serializable]
    public class TabGroupNode : GroupNode
    {
        [Serializable]
        public class Tab
        {
            [SerializeField] private string tabName = "Tab";
            [SerializeReference] private List<LayoutNode> children = new List<LayoutNode>();

            public string TabName { get => tabName; set => tabName = value; }
            public List<LayoutNode> Children => children;

            public Tab() { }
            public Tab(string name) => tabName = name;
        }

        [SerializeField] private string groupName = "Tab Group";
        [SerializeField] private List<Tab> tabs = new List<Tab> { new Tab("Tab 1") };

        public string GroupName { get => groupName; set => groupName = value; }
        public List<Tab> Tabs => tabs;
        public override string DisplayName => $"[Tabs] {groupName}";
    }
}