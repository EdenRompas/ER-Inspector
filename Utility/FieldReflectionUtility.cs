using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace InspectorDesigner
{
    public static class FieldReflectionUtility
    {
        public static List<string> GetInspectableFieldNames(Type targetType)
        {
            var result = new List<string>();
            foreach (var f in targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (f.IsStatic) continue;
                if (f.Name.Contains("k__BackingField")) continue;
                if (f.GetCustomAttribute<HideInInspector>() != null) continue;
                if (f.IsPublic && f.GetCustomAttribute<NonSerializedAttribute>() != null) continue;
                if (!f.IsPublic && f.GetCustomAttribute<SerializeField>() == null) continue;
                result.Add(f.Name);
            }
            return result;
        }
    }

    public static class LayoutTreeUtility
    {
        /// <summary>
        /// Akses child-list dari sebuah node secara generik, tanpa peduli node itu
        /// punya 1 kolom (Vertical/Foldout) atau 2 kolom (Horizontal).
        /// </summary>
        public static IEnumerable<List<LayoutNode>> GetChildLists(LayoutNode node)
        {
            switch (node)
            {
                case HorizontalGroupNode hg:
                    yield return hg.LeftChildren;
                    yield return hg.RightChildren;
                    break;
                case VerticalGroupNode vg:
                    yield return vg.Children;
                    break;
                case FoldoutNode fo:
                    yield return fo.Children;
                    break;
                case TabGroupNode tg:
                    foreach (var tab in tg.Tabs) yield return tab.Children;
                    break;
            }
        }

        public static HashSet<string> CollectFieldNames(List<LayoutNode> nodes)
        {
            var set = new HashSet<string>();
            void Walk(List<LayoutNode> list)
            {
                foreach (var n in list)
                {
                    if (n is FieldNode fn) set.Add(fn.FieldName);
                    foreach (var childList in GetChildLists(n)) Walk(childList);
                }
            }
            Walk(nodes);
            return set;
        }

        public static bool Remove(List<LayoutNode> list, LayoutNode target)
        {
            if (list.Remove(target)) return true;
            foreach (var n in list)
                foreach (var childList in GetChildLists(n))
                    if (Remove(childList, target)) return true;
            return false;
        }

        /// <summary>
        /// True kalau "list" adalah salah satu child-list milik "node" sendiri, atau milik
        /// salah satu keturunannya. Dipakai untuk mencegah grup di-drop ke dalam dirinya
        /// sendiri / ke dalam kolom anaknya sendiri.
        /// </summary>
        public static bool IsListWithinSelfOrDescendant(LayoutNode node, List<LayoutNode> list)
        {
            foreach (var childList in GetChildLists(node))
            {
                if (ReferenceEquals(childList, list)) return true;
                foreach (var child in childList)
                    if (IsListWithinSelfOrDescendant(child, list)) return true;
            }
            return false;
        }
    }
}