using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class CustomWindow : EditorWindow
{
    private Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Tools/ER Inspector")]
    public static void OpenWindow()
    {
        GetWindow<CustomWindow>("ER Inspector");
    }

    private void OnGUI()
    {
        GUILayout.Label("How to use ER Attribute");

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Button", EditorStyles.boldLabel);
        GUILayout.Label("[Button]");
        GUILayout.Label("public void MethodName() {}");
        EditorGUILayout.EndVertical();

        DrawSeparator();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Title", EditorStyles.boldLabel);
        GUILayout.Label("[Title(\"Title Name\")]");
        GUILayout.Label("[SerializeField] private int _variable;");
        EditorGUILayout.EndVertical();

        DrawSeparator();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Info Box", EditorStyles.boldLabel);
        GUILayout.Label("[InfoBox(\"This Is Info\")]");
        GUILayout.Label("[SerializeField] private int _variable;");
        EditorGUILayout.EndVertical();

        DrawSeparator();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Show If", EditorStyles.boldLabel);
        GUILayout.Label("[SerializeField] private bool _boolVariable;");
        GUILayout.Label("[ShowIf(\"_boolVariable\")]");
        GUILayout.Label("[SerializeField] private int _variable;");
        EditorGUILayout.EndVertical();

        DrawSeparator();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Read Only", EditorStyles.boldLabel);
        GUILayout.Label("[ReadOnly(\"Read Only Name\")]");
        GUILayout.Label("[SerializeField] private int _variable;");
        EditorGUILayout.EndVertical();

        DrawSeparator();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Asset Only", EditorStyles.boldLabel);
        GUILayout.Label("[AssetOnly(\"Asset Only Name\")]");
        GUILayout.Label("[SerializeField] private int _variable;");
        EditorGUILayout.EndVertical();

        DrawSeparator();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Scene Object Only", EditorStyles.boldLabel);
        GUILayout.Label("[SceneObjectOnly(\"Scene Object Only Name\")]");
        GUILayout.Label("[SerializeField] private int _variable;");
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
    }

    private void DrawSeparator()
    {
        var rect = EditorGUILayout.GetControlRect(false, 1);
        rect.height = 1;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}

#endif
