using System.Collections.Generic;
using UnityEngine;

namespace InspectorDesigner.Drawers
{
    /// <summary>
    /// Melacak rect dari Vertical/Horizontal Group yang sedang aktif digambar, supaya
    /// drawer lain (mis. TitleNodeDrawer) bisa membatasi elemen yang digambarnya sendiri
    /// (garis bawah title) agar mengikuti lebar grup pembungkus, bukan selebar Inspector.
    /// </summary>
    internal static class GroupBoundsStack
    {
        private static readonly Stack<Rect> bounds = new Stack<Rect>();

        public static void Push(Rect rect) => bounds.Push(rect);

        public static void Pop()
        {
            if (bounds.Count > 0) bounds.Pop();
        }

        public static bool TryGetCurrent(out Rect rect)
        {
            if (bounds.Count > 0)
            {
                rect = bounds.Peek();
                return true;
            }
            rect = default;
            return false;
        }

        /// <summary>
        /// Reset paksa. Dipanggil di awal & akhir setiap render inspector sebagai jaga-jaga
        /// kalau ada Push tanpa Pop yang tertinggal akibat exception IMGUI di frame sebelumnya
        /// (mis. mismatched layout group dari Foldout yang state-nya berubah di tengah render).
        /// </summary>
        public static void Clear() => bounds.Clear();
    }
}