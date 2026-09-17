using System.IO;
using UnityEditor;
using UnityEngine;

namespace InspectorDesigner
{
    public static class ConfigAssetUtility
    {
        private const string ResourcesSubFolder = "InspectorDesigner";

        public static string GetAssetPath(System.Type targetType) =>
            $"Assets/Resources/{ResourcesSubFolder}/{targetType.Name}.asset";

        public static string GetResourcesLoadPath(System.Type targetType) =>
            $"{ResourcesSubFolder}/{targetType.Name}";

        public static InspectorLayoutConfig LoadOrCreate(System.Type targetType)
        {
            var path = GetAssetPath(targetType);
            var config = AssetDatabase.LoadAssetAtPath<InspectorLayoutConfig>(path);
            if (config != null) return config;

            EnsureFolders(path);
            config = ScriptableObject.CreateInstance<InspectorLayoutConfig>();
            config.TargetAssemblyQualifiedName = targetType.AssemblyQualifiedName;
            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.SaveAssets();
            return config;
        }

        public static InspectorLayoutConfig LoadForRuntime(System.Type targetType) =>
            Resources.Load<InspectorLayoutConfig>(GetResourcesLoadPath(targetType));

        public static void Save(InspectorLayoutConfig config)
        {
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        private static void EnsureFolders(string assetPath)
        {
            var dir = Path.GetDirectoryName(assetPath)?.Replace("\\", "/");
            if (string.IsNullOrEmpty(dir) || AssetDatabase.IsValidFolder(dir)) return;

            var parts = dir.Split('/');
            var current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}