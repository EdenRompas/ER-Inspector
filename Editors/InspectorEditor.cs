using System.Collections.Generic;
using InspectorDesigner.Drawers;
using UnityEditor;
using UnityEngine;

namespace InspectorDesigner
{
    public static class ConfigurableInspectorRenderer
    {
        public static bool TryDrawConfigured(Editor editor, System.Type targetType)
        {
            var config = ConfigAssetUtility.LoadForRuntime(targetType);
            if (config == null || config.RootElements.Count == 0) return false;

            editor.serializedObject.Update();

            var scriptProp = editor.serializedObject.FindProperty("m_Script");
            if (scriptProp != null)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(scriptProp);
                GUI.enabled = true;
            }

            GroupBoundsStack.Clear();
            try
            {
                foreach (var node in config.RootElements) Draw(editor.serializedObject, node);
            }
            finally
            {
                GroupBoundsStack.Clear();
            }

            var placed = LayoutTreeUtility.CollectFieldNames(config.RootElements);
            DrawRemaining(editor.serializedObject, placed);
            editor.serializedObject.ApplyModifiedProperties();
            return true;
        }

        private static void Draw(SerializedObject so, LayoutNode node)
        {
            var drawer = LayoutNodeDrawerRegistry.Get(node.GetType());
            drawer?.DrawInspector(so, node, child => Draw(so, child));
        }

        private static void DrawRemaining(SerializedObject so, HashSet<string> placed)
        {
            var prop = so.GetIterator();
            var enter = true;
            while (prop.NextVisible(enter))
            {
                enter = false;
                if (prop.name == "m_Script" || placed.Contains(prop.name)) continue;
                EditorGUILayout.PropertyField(prop, true);
            }
        }
    }

    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class ConfigurableMonoBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (!ConfigurableInspectorRenderer.TryDrawConfigured(this, target.GetType()))
                DrawDefaultInspector();
        }
    }

    [CustomEditor(typeof(ScriptableObject), true)]
    [CanEditMultipleObjects]
    public class ConfigurableScriptableObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (!ConfigurableInspectorRenderer.TryDrawConfigured(this, target.GetType()))
                DrawDefaultInspector();
        }
    }
}