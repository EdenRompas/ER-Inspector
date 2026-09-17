using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace InspectorDesigner.Drawers
{
    /// <summary>
    /// Auto-discover semua drawer lewat reflection. Ini kunci ekstensibilitas:
    /// tambah class baru + [LayoutNodeDrawer(typeof(X))] = otomatis terdaftar,
    /// tanpa menyentuh window atau editor sama sekali.
    /// </summary>
    [InitializeOnLoad]
    public static class LayoutNodeDrawerRegistry
    {
        private static readonly Dictionary<Type, ILayoutNodeDrawer> drawersByNodeType = new();

        static LayoutNodeDrawerRegistry() => Rescan();

        public static void Rescan()
        {
            drawersByNodeType.Clear();
            var drawerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(SafeGetTypes)
                .Where(t => typeof(ILayoutNodeDrawer).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

            foreach (var drawerType in drawerTypes)
            {
                var attr = drawerType.GetCustomAttribute<LayoutNodeDrawerAttribute>();
                if (attr == null) continue;
                drawersByNodeType[attr.NodeType] = (ILayoutNodeDrawer)Activator.CreateInstance(drawerType);
            }
        }

        private static IEnumerable<Type> SafeGetTypes(Assembly a)
        {
            try { return a.GetTypes(); } catch { return Array.Empty<Type>(); }
        }

        public static ILayoutNodeDrawer Get(Type nodeType) =>
            drawersByNodeType.TryGetValue(nodeType, out var d) ? d : null;

        public static IEnumerable<(Type nodeType, ILayoutNodeDrawer drawer)> AllAddable() =>
            drawersByNodeType.Where(kv => kv.Value.MenuLabel != null).Select(kv => (kv.Key, kv.Value));
    }
}